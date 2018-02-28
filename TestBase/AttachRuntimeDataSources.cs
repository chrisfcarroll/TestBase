#if MSTEST
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace TestBase
{
    public static class Extensions
    {
        public static T GetRuntimeDataSourceObject<T>(this TestContext testContext)
        {
            return JsonConvert.DeserializeObject<T>(testContext.DataRow["Payload"].ToString());
        }
    }

    /// <summary>
    /// Allows you to define DataSources in your Test Class instead of an external datasource, so that you can define multiple TestCases inline with the code.
    /// Works (I think) by creating XML files on the fly from the DataSource that you specify.
    /// 
    /// Of course, you could have just used NUnit instead.
    /// 
    /// Taken from https://raw.githubusercontent.com/Thwaitesy/MSTestHacks/master/MSTestHacks/RuntimeDataSource/AttachRuntimeDataSources.cs
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    sealed class AttachRuntimeDataSources : ContextAttribute
    {
        static string DATASOURCES_PATH = Directory.GetCurrentDirectory();
        static List<string> dataSourcesInitalized = new List<string>();

        internal AttachRuntimeDataSources() : base("AttachRuntimeDataSources") { }

        static AttachRuntimeDataSources()
        {
            var appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //If the connectionStrings section doesn't exist, add it.
            if (appConfig.Sections.Cast<ConfigurationSection>().All(x => x.SectionInformation.Name != "connectionStrings"))
                appConfig.Sections.Add("connectionStrings", new ConnectionStringsSection());

            //If the test tools section doesn't exist, add it.
            if (appConfig.Sections.Cast<ConfigurationSection>().All(x => x.SectionInformation.Name != "microsoft.visualstudio.testtools"))
                appConfig.Sections.Add("microsoft.visualstudio.testtools", new TestConfigurationSection());

            var connectionStringsSection = (ConnectionStringsSection)appConfig.Sections["connectionStrings"];
            var testConfigurationSection = (TestConfigurationSection)appConfig.Sections["microsoft.visualstudio.testtools"];

            //Remove all connection strings that have the "_RuntimeDataSources" in the name.
            var connectionsToRemove = connectionStringsSection.ConnectionStrings.Cast<ConnectionStringSettings>().Where(x => x.Name.Contains("RuntimeDataSource")).ToList();
            foreach (var con in connectionsToRemove)
            {
                connectionStringsSection.ConnectionStrings.Remove(con);
            }

            //Make sure dir exists
            if (!Directory.Exists(DATASOURCES_PATH)){Directory.CreateDirectory(DATASOURCES_PATH);}

            var testClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t=>t.GetCustomAttributes<AttachRuntimeDataSources>().Any());
            var dataSourcesRequired = testClasses.Select(
                        t => new
                        {
                            Type = t,
                            Names = t.GetMethods().Where(m => m.GetCustomAttributes<DataSourceAttribute>(false).Any())
                                        .Select(m => m.GetCustomAttribute<DataSourceAttribute>().DataSourceSettingName)
                        });

            // read the exclusions from the app.config file
            var exclusionRegEx = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ExclusionRegEx"]) ? ConfigurationManager.AppSettings["ExclusionRegEx"] : "";

            var configChanged = false;
            foreach (var dataSourceR in dataSourcesRequired)
            foreach (var dataSourceName in dataSourceR.Names )
            {
                if (!string.IsNullOrEmpty(exclusionRegEx) && Regex.IsMatch(dataSourceName, exclusionRegEx)) continue;

                var testData = new ProviderReference(dataSourceR.Type, dataSourceName).GetInstance().Cast<object>().ToList();

                var connectionStringName = dataSourceName + "_RuntimeDataSource";
                var dataSourceFilePath = Path.Combine(DATASOURCES_PATH, Guid.NewGuid().ToString("N") + ".xml");
                connectionStringsSection.ConnectionStrings.Add(new ConnectionStringSettings(connectionStringName, dataSourceFilePath, "Microsoft.VisualStudio.TestTools.DataSource.XML"));

                var dataSource = new DataSourceElement()
                {
                    Name = dataSourceName,
                    ConnectionString = connectionStringName,
                    DataTableName = "Row",
                    DataAccessMethod = "Sequential"
                };

                testConfigurationSection.DataSources.Add(dataSource);
                configChanged = true;

                //Create the iterations element and set the dataSourceName as an attribute for debugging.
                var iterationsElement = new XElement("Iterations");
                iterationsElement.SetAttributeValue("DataSourceName", dataSource);

                //Create the file
                File.WriteAllText(dataSourceFilePath, new XDocument(new XDeclaration("1.0", "utf-8", "true"), iterationsElement).ToString());

                //Load the file
                var doc = XDocument.Load(dataSourceFilePath);

                //Add the iterations
                doc.Element("Iterations").Add(

                            from iteration in testData
                            select new XElement(dataSource.DataTableName,
                                        new XElement("Payload", JsonConvert.SerializeObject(iteration))));

                //Save the file
                doc.Save(dataSourceFilePath);
            }

            if (configChanged)
            {
                appConfig.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
                ConfigurationManager.RefreshSection("microsoft.visualstudio.testtools");
            }
        }
    }

    class ProviderReference
    {
        readonly Type providerType;

        internal ProviderReference(Type providerType, string providerName)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");
            if (providerName == null)
                throw new ArgumentNullException("providerName");

            this.providerType = providerType;
            this.Name = providerName;
        }

        internal string Name { get; set; }

        internal IEnumerable GetInstance()
        {
            MemberInfo[] members = providerType.GetMember(Name,
                                                          MemberTypes.Field | MemberTypes.Method | MemberTypes.Property,
                                                          BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (members.Length == 0)
                throw new Exception(string.Format(
                    "Unable to locate {0}.{1}", providerType.FullName, Name));

            return (IEnumerable)GetProviderObjectFromMember(members[0]);
        }

        internal object Construct(Type type)
        {
            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
                throw new Exception(type.FullName + " does not have a default constructor");

            return ctor.Invoke(null);
        }

        private object GetProviderObjectFromMember(MemberInfo member)
        {
            object providerObject = null;
            object instance = null;

            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo providerProperty = member as PropertyInfo;
                    MethodInfo getMethod = providerProperty.GetGetMethod(true);
                    if (!getMethod.IsStatic)
                        instance = Construct(providerType);
                    providerObject = providerProperty.GetValue(instance, null);
                    break;

                case MemberTypes.Method:
                    MethodInfo providerMethod = member as MethodInfo;
                    if (!providerMethod.IsStatic)
                        instance = Construct(providerType);
                    providerObject = providerMethod.Invoke(instance, null);
                    break;

                case MemberTypes.Field:
                    FieldInfo providerField = member as FieldInfo;
                    if (!providerField.IsStatic)
                        instance = Construct(providerType);
                    providerObject = providerField.GetValue(instance);
                    break;
            }

            return providerObject;
        }
    }
}
#endif  
