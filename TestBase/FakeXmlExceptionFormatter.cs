﻿//===============================================================================
//
//
// Source code copied & reduced from MS EntLib in preference to having a test dependency on the entlib
//
 //
// Microsoft patterns & practices Enterprise Library
// Exception Handling Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Xml;
using System.Collections.Specialized;
#if !SILVERLIGHT

#else
using NameValueCollection = System.Collections.Generic.Dictionary<string, string>;
#endif


namespace TestBase
{

    public abstract class ExceptionFormatter
    {
        private static readonly List<string> IgnoredProperties = new List<string>(
            new String[] { "Source", "Message", "HelpLink", "InnerException", "StackTrace" });

        private readonly Guid handlingInstanceId;
        private readonly Exception exception;
        private NameValueCollection additionalInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionFormatter"/> class with an <see cref="Exception"/> to format.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> object to format.</param>
        /// <param name="handlingInstanceId">The id of the handling chain.</param>
        protected ExceptionFormatter(Exception exception, Guid handlingInstanceId)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            this.exception = exception;
            this.handlingInstanceId = handlingInstanceId;
        }

        /// <summary>
        /// Gets the <see cref="Exception"/> to format.
        /// </summary>
        /// <value>
        /// The <see cref="Exception"/> to format.
        /// </value>
        public Exception Exception
        {
            get { return this.exception; }
        }

        /// <summary>
        /// Gets the id of the handling chain requesting a formatting.
        /// </summary>
        /// <value>
        /// The id of the handling chain requesting a formatting, or <see cref="Guid.Empty"/> if no such id is available.
        /// </value>
        public Guid HandlingInstanceId
        {
            get { return this.handlingInstanceId; }
        }

        /// <summary>
        /// Gets additional information related to the <see cref="Exception"/> but not
        /// stored in the exception (for example, the time in which the <see cref="Exception"/> was 
        /// thrown).
        /// </summary>
        /// <value>
        /// Additional information related to the <see cref="Exception"/> but not
        /// stored in the exception (for example, the time when the <see cref="Exception"/> was 
        /// thrown).
        /// </value>
        public NameValueCollection AdditionalInfo
        {
            get
            {
                if (this.additionalInfo == null)
                {
                    this.additionalInfo = new NameValueCollection();
#if !SILVERLIGHT
                    this.additionalInfo.Add("MachineName", GetMachineName());
#endif
                    this.additionalInfo.Add("TimeStamp", DateTime.UtcNow.ToString(CultureInfo.CurrentCulture));
                    this.additionalInfo.Add("FullName", Assembly.GetExecutingAssembly().FullName);
                    this.additionalInfo.Add("AppDomainName", AppDomain.CurrentDomain.FriendlyName);
#if !SILVERLIGHT
                    this.additionalInfo.Add("ThreadIdentity", Thread.CurrentPrincipal.Identity.Name);
                    this.additionalInfo.Add("WindowsIdentity", GetWindowsIdentity());
#endif
                }

                return this.additionalInfo;
            }
        }

        /// <summary>
        /// Formats the <see cref="Exception"/> into the underlying stream.
        /// </summary>
        public virtual void Format()
        {
            WriteDescription();
            WriteDateTime(DateTime.UtcNow);
            WriteException(this.exception, null);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Formats the exception and all nested inner exceptions.
        /// </summary>
        /// <param name="exceptionToFormat">The exception to format.</param>
        /// <param name="outerException">The outer exception. This 
        /// value will be null when writing the outer-most exception.</param>
        /// <remarks>
        /// <para>This method calls itself recursively until it reaches
        /// an exception that does not have an inner exception.</para>
        /// <para>
        /// This is a template method which calls the following
        /// methods in order
        /// <list type="number">
        /// <item>
        /// <description><see cref="WriteExceptionType"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WriteMessage"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WriteSource"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WriteHelpLink"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WriteReflectionInfo"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WriteStackTrace"/></description>
        /// </item>
        /// <item>
        /// <description>If the specified exception has an inner exception
        /// then it makes a recursive call. <see cref="WriteException"/></description>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
#else
        /// <summary>
        /// Formats the exception and all nested inner exceptions.
        /// </summary>
        /// <param name="exceptionToFormat">The exception to format.</param>
        /// <param name="outerException">The outer exception. This 
        /// value will be null when writing the outer-most exception.</param>
        /// <remarks>
        /// <para>This method calls itself recursively until it reaches
        /// an exception that does not have an inner exception.</para>
        /// <para>
        /// This is a template method which calls the following
        /// methods in order
        /// <list type="number">
        /// <item>
        /// <description><see cref="WriteExceptionType"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WriteMessage"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WriteReflectionInfo"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WriteStackTrace"/></description>
        /// </item>
        /// <item>
        /// <description>If the specified exception has an inner exception
        /// then it makes a recursive call. <see cref="WriteException"/></description>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
#endif
        protected virtual void WriteException(Exception exceptionToFormat, Exception outerException)
        {
            if (exceptionToFormat == null) throw new ArgumentNullException("exceptionToFormat");

            this.WriteExceptionType(exceptionToFormat.GetType());
            this.WriteMessage(exceptionToFormat.Message);
#if !SILVERLIGHT
            this.WriteSource(exceptionToFormat.Source);
            this.WriteHelpLink(exceptionToFormat.HelpLink);
#endif
            this.WriteReflectionInfo(exceptionToFormat);
            this.WriteStackTrace(exceptionToFormat.StackTrace);

            // We only want additional information on the top most exception
            if (outerException == null)
            {
                this.WriteAdditionalInfo(this.AdditionalInfo);
            }

            Exception inner = exceptionToFormat.InnerException;

            if (inner != null)
            {
                // recursive call
                this.WriteException(inner, exceptionToFormat);
            }
        }

        /// <summary>
        /// Formats an <see cref="Exception"/> using reflection to get the information.
        /// </summary>
        /// <param name="exceptionToFormat">
        /// The <see cref="Exception"/> to be formatted.
        /// </param>
        /// <remarks>
        /// <para>This method reflects over the public, instance properties 
        /// and public, instance fields
        /// of the specified exception and prints them to the formatter.  
        /// Certain property names are ignored
        /// because they are handled explicitly in other places.</para>
        /// </remarks>
        protected void WriteReflectionInfo(Exception exceptionToFormat)
        {
            if (exceptionToFormat == null) throw new ArgumentNullException("exceptionToFormat");

            Type type = exceptionToFormat.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            object value;

            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead && IgnoredProperties.IndexOf(property.Name) == -1 && property.GetIndexParameters().Length == 0)
                {
                    try
                    {
                        value = property.GetValue(exceptionToFormat, null);
                    }
                    catch (TargetInvocationException)
                    {
                        value = "Resources.PropertyAccessFailed";
                    }
                    WritePropertyInfo(property, value);
                }
            }

            foreach (FieldInfo field in fields)
            {
                try
                {
                    value = field.GetValue(exceptionToFormat);
                }
                catch (TargetInvocationException)
                {
                    value = "Resources.FieldAccessFailed";
                }
                WriteFieldInfo(field, value);
            }
        }

        /// <summary>
        /// When overridden by a class, writes a description of the caught exception.
        /// </summary>
        protected abstract void WriteDescription();

        /// <summary>
        /// When overridden by a class, writes the current time.
        /// </summary>
        /// <param name="utcNow">The current time.</param>
        protected abstract void WriteDateTime(DateTime utcNow);

        /// <summary>
        /// When overridden by a class, writes the <see cref="Type"/> of the current exception.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of the exception.</param>
        protected abstract void WriteExceptionType(Type exceptionType);

        /// <summary>
        /// When overridden by a class, writes the <see cref="System.Exception.Message"/>.
        /// </summary>
        /// <param name="message">The message to write.</param>
        protected abstract void WriteMessage(string message);

#if !SILVERLIGHT
        /// <summary>
        /// When overridden by a class, writes the value of the <see cref="System.Exception.Source"/> property.
        /// </summary>
        /// <param name="source">The source of the exception.</param>
        protected abstract void WriteSource(string source);

        /// <summary>
        /// When overridden by a class, writes the value of the <see cref="System.Exception.HelpLink"/> property.
        /// </summary>
        /// <param name="helpLink">The help link for the exception.</param>
        protected abstract void WriteHelpLink(string helpLink);
#endif

        /// <summary>
        /// When overridden by a class, writes the value of the <see cref="System.Exception.StackTrace"/> property.
        /// </summary>
        /// <param name="stackTrace">The stack trace of the exception.</param>
        protected abstract void WriteStackTrace(string stackTrace);

        /// <summary>
        /// When overridden by a class, writes the value of a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The reflected <see cref="PropertyInfo"/> object.</param>
        /// <param name="value">The value of the <see cref="PropertyInfo"/> object.</param>
        protected abstract void WritePropertyInfo(PropertyInfo propertyInfo, object value);

        /// <summary>
        /// When overridden by a class, writes the value of a <see cref="FieldInfo"/> object.
        /// </summary>
        /// <param name="fieldInfo">The reflected <see cref="FieldInfo"/> object.</param>
        /// <param name="value">The value of the <see cref="FieldInfo"/> object.</param>
        protected abstract void WriteFieldInfo(FieldInfo fieldInfo, object value);

        /// <summary>
        /// When overridden by a class, writes additional properties if available.
        /// </summary>
        /// <param name="additionalInformation">Additional information to be included with the exception report</param>
        protected abstract void WriteAdditionalInfo(NameValueCollection additionalInformation);

#if !SILVERLIGHT
        private static string GetMachineName()
        {
            string machineName;
            try
            {
                machineName = Environment.MachineName;
            }
            catch (SecurityException)
            {
                machineName = "Resources_Desktop.PermissionDenied";
            }

            return machineName;
        }

        private static string GetWindowsIdentity()
        {
            string windowsIdentity;
            try
            {
                windowsIdentity = WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException)
            {
                windowsIdentity = "Resources_Desktop.PermissionDenied";
            }

            return windowsIdentity;
        }
#endif
    }

    /// <summary>
    /// Represents an exception formatter that formats exception objects as XML.
    /// </summary>	
    public class XmlExceptionFormatter : ExceptionFormatter
    {
        private readonly XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlExceptionFormatter"/> class using the specified <see cref="XmlWriter"/> and <see cref="Exception"/> objects.
        /// </summary>
        /// <param name="xmlWriter">The <see cref="XmlWriter"/> in which to write the XML.</param>
        /// <param name="exception">The <see cref="Exception"/> to format.</param>
        /// <param name="handlingInstanceId">The id of the handling chain.</param>
        public XmlExceptionFormatter(XmlWriter xmlWriter, Exception exception, Guid handlingInstanceId)
            : base(exception, handlingInstanceId)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");

            this.xmlWriter = xmlWriter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlExceptionFormatter"/> class using the specified <see cref="TextWriter"/> and <see cref="Exception"/> objects.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> in which to write the XML.</param>
        /// <param name="exception">The <see cref="Exception"/> to format.</param>
        /// <remarks>
        /// An <see cref="XmlWriter"/> with indented formatting is created from the specified <see cref="TextWriter"/>.
        /// </remarks>
        /// <param name="handlingInstanceId">The id of the handling chain.</param>
        public XmlExceptionFormatter(TextWriter writer, Exception exception, Guid handlingInstanceId)
            : base(exception, handlingInstanceId)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true });
        }

        /// <summary>
        /// Gets the underlying <see cref="XmlWriter"/> that the formatted exception is written to.
        /// </summary>
        /// <value>
        /// The underlying <see cref="XmlWriter"/> that the formatted exception is written to.
        /// </value>
        public XmlWriter Writer
        {
            get { return xmlWriter; }
        }

        /// <summary>
        /// Formats the <see cref="Exception"/> into the underlying stream.
        /// </summary>       
        public override void Format()
        {
            Writer.WriteStartElement("Exception");
            if (this.HandlingInstanceId != Guid.Empty)
            {
                Writer.WriteAttributeString(
                    "handlingInstanceId",
                    this.HandlingInstanceId.ToString("D", CultureInfo.InvariantCulture));
            }

            base.Format();

            Writer.WriteEndElement();

            Writer.Flush();
        }

        /// <summary>
        /// Writes the current date and time to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="utcNow">The current time.</param>
        protected override void WriteDateTime(DateTime utcNow)
        {
            DateTime localTime = utcNow.ToLocalTime();
            string localTimeString = localTime.ToString("u", DateTimeFormatInfo.InvariantInfo);
            WriteSingleElement("DateTime", localTimeString);
        }

        /// <summary>
        /// Writes the value of the <see cref="Exception.Message"/> property to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="message">The message to write.</param>
        protected override void WriteMessage(string message)
        {
            WriteSingleElement("Message", message);
        }

        /// <summary>
        /// Writes a generic description to the <see cref="XmlWriter"/>.
        /// </summary>
        protected override void WriteDescription()
        {
            WriteSingleElement("Description", string.Format(CultureInfo.CurrentCulture, "Resources.ExceptionWasCaught", base.Exception.GetType().FullName));
        }

#if !SILVERLIGHT
        /// <summary>
        /// Writes the value of the specified help link taken
        /// from the value of the <see cref="Exception.HelpLink"/>
        /// property to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="helpLink">The exception's help link.</param>
        protected override void WriteHelpLink(string helpLink)
        {
            WriteSingleElement("HelpLink", helpLink);
        }

        /// <summary>
        /// Writes the value of the specified source taken from the value of the <see cref="Exception.Source"/> property to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="source">The source of the exception.</param>
        protected override void WriteSource(string source)
        {
            WriteSingleElement("Source", source);
        }
#endif

        /// <summary>
        /// Writes the value of the specified stack trace taken from the value of the <see cref="Exception.StackTrace"/> property to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="stackTrace">The stack trace of the exception.</param>
        protected override void WriteStackTrace(string stackTrace)
        {
            WriteSingleElement("StackTrace", stackTrace);
        }

        /// <summary>
        /// Writes the value of the <see cref="Type.AssemblyQualifiedName"/>
        /// property for the specified exception type to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of the exception.</param>
        protected override void WriteExceptionType(Type exceptionType)
        {
            WriteSingleElement("ExceptionType", exceptionType.AssemblyQualifiedName);
        }

        /// <summary>
        /// Writes and formats the exception and all nested inner exceptions to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="exceptionToFormat">The exception to format.</param>
        /// <param name="outerException">The outer exception. This value will be null when writing the outer-most exception.</param>
        protected override void WriteException(Exception exceptionToFormat, Exception outerException)
        {
            if (outerException != null)
            {
                Writer.WriteStartElement("InnerException");

                base.WriteException(exceptionToFormat, outerException);

                Writer.WriteEndElement();
            }
            else
            {
                base.WriteException(exceptionToFormat, outerException);
            }
        }

        /// <summary>
        /// Writes the name and value of the specified property to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="propertyInfo">The reflected <see cref="PropertyInfo"/> object.</param>
        /// <param name="value">The value of the <see cref="PropertyInfo"/> object.</param>
        protected override void WritePropertyInfo(PropertyInfo propertyInfo, object value)
        {
            string propertyValueString = "Resources.UndefinedValue";

            if (value != null)
            {
                propertyValueString = value.ToString();
            }

            Writer.WriteStartElement("Property");
            Writer.WriteAttributeString("name", propertyInfo.Name);
            Writer.WriteString(propertyValueString);
            Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the name and value of the <see cref="FieldInfo"/> object to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="fieldInfo">The reflected <see cref="FieldInfo"/> object.</param>
        /// <param name="value">The value of the <see cref="FieldInfo"/> object.</param>
        protected override void WriteFieldInfo(FieldInfo fieldInfo, object value)
        {
            string fieldValueString = "Resources.UndefinedValue";

            if (fieldValueString != null)
            {
                fieldValueString = value.ToString();
            }

            Writer.WriteStartElement("Field");
            Writer.WriteAttributeString("name", fieldInfo.Name);
            Writer.WriteString(value.ToString());
            Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes additional information to the <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="additionalInformation">Additional information to be included with the exception report</param>
        protected override void WriteAdditionalInfo(NameValueCollection additionalInformation)
        {
            Writer.WriteStartElement("additionalInfo");

#if !SILVERLIGHT
            foreach (string name in additionalInformation.AllKeys)
#else
            foreach (string name in additionalInformation.Keys)
#endif
            {
                Writer.WriteStartElement("info");
                Writer.WriteAttributeString("name", name);
                Writer.WriteAttributeString("value", additionalInformation[name]);
                Writer.WriteEndElement();
            }

            Writer.WriteEndElement();
        }

        private void WriteSingleElement(string elementName, string elementText)
        {
            Writer.WriteStartElement(elementName);
            Writer.WriteString(elementText);
            Writer.WriteEndElement();
        }
    }
}
