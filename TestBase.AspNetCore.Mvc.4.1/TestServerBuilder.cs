using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace TestBase
{
    /// <summary>
    ///     Build a running server, similar to WebHost.Build(), but using <see cref="TestServer" />
    ///     <see cref="RunningServerUsingStartup{TStartup}" />
    /// </summary>
    /// <remarks>
    ///     Use <see cref="TestServer.Host" />.<see cref="ServiceProvider" /> to get instances of complex dependencies defined
    ///     in your Startup class.
    /// </remarks>
    public static class TestServerBuilder
    {
        /// <summary>
        ///     Build a running server, similar to WebHost.Build(), but using <see cref="TestServer" />,
        ///     using <typeparamref name="TStartup" /> as a startup class
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="environmentName"></param>
        /// <param name="featureCollection"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Use <see cref="TestServer.Host" />.<see cref="ServiceProvider" /> to get instances of complex dependencies defined
        ///     in your Startup class.
        /// </remarks>
        public static TestServer RunningServerUsingStartup<TStartup>(
            string            environmentName   = "Development",
            FeatureCollection featureCollection = null)
        {
            var webHostBuilder = new WebHostBuilder()
                                .ConfigureServices(InitializeServices<TStartup>)
                                .UseEnvironment(environmentName)
                                .UseStartup(typeof(TStartup));
            return new TestServer(webHostBuilder, featureCollection ?? new FeatureCollection());
        }

        /// <summary>
        ///     Build a running server, similar to WebHost.Build(), but using <see cref="TestServer" />,
        ///     using <typeparamref name="TStartup" /> as a startup class and
        ///     <paramref name="webProjectPhysicalPathForContentRoot" /> as ContentRoot
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="webProjectPhysicalPathForContentRoot">
        ///     This parameter is optional (i.e. can be correctly guessed) if the
        ///     the AssemblyName of your <typeparamref name="TStartup" /> matches the physical directory of Web project under test.
        ///     Which it will if you haven't deliberately changed it.
        /// </param>
        /// <param name="environmentName"></param>
        /// <param name="featureCollection"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Use <see cref="TestServer.Host" />.<see cref="ServiceProvider" /> to get instances of complex dependencies defined
        ///     in your Startup class.
        /// </remarks>
        public static TestServer RunningServerUsingStartupAndContentRoot<TStartup>(
            string            webProjectPhysicalPathForContentRoot = null,
            string            environmentName                      = "Development",
            FeatureCollection featureCollection                    = null)
        {
            webProjectPhysicalPathForContentRoot = webProjectPhysicalPathForContentRoot
                                                ?? InternalGuessWebProjectPathFromAssemblyName(typeof(TStartup)
                                                                                              .GetTypeInfo()
                                                                                              .Assembly);

            var webHostBuilder = new WebHostBuilder()
                                .UseContentRoot(webProjectPhysicalPathForContentRoot)
                                .ConfigureServices(InitializeServices<TStartup>)
                                .UseEnvironment(environmentName)
                                .UseStartup(typeof(TStartup));
            return new TestServer(webHostBuilder, featureCollection ?? new FeatureCollection());
        }

        internal static void InitializeServices<TStartup>(IServiceCollection services)
        {
            var assembly               = typeof(TStartup).GetTypeInfo().Assembly;
            var implementationInstance = new ApplicationPartManager();
            implementationInstance.ApplicationParts.Add(new AssemblyPart(assembly));
            implementationInstance.FeatureProviders.Add(new ControllerFeatureProvider());
            implementationInstance.FeatureProviders.Add(new ViewComponentFeatureProvider());
            services.AddSingleton(implementationInstance);
        }

        /// <summary>
        ///     Attempts to locate a project given an Assembly.
        ///     Used by <see cref="RunningServerUsingStartup{TStartup}" /> to find the physical directory suggested by TStartup
        ///     if you do not give it the path to the your AspNetCore web project
        /// </summary>
        /// <param name="startupAssembly"></param>
        /// <param name="projectFilePattern"></param>
        /// <returns></returns>
        public static string InternalGuessWebProjectPathFromAssemblyName(
            Assembly startupAssembly,
            string   projectFilePattern = "*.*proj")
        {
            var name          = startupAssembly.GetName().Name;
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var directoryInfo = new DirectoryInfo(baseDirectory);
            while (!directoryInfo.GetFileSystemInfos("*.sln").Any())
            {
                directoryInfo = directoryInfo.Parent;
                if (directoryInfo.Parent == null)
                    throw new Exception($"Solution root could not be located using application root {baseDirectory}.");
            }

            var directoriesUnderSolution = directoryInfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
            var projectFilesInSolution =
            directoriesUnderSolution.SelectMany(d => d.GetFileSystemInfos(projectFilePattern));
            var originalProjectFile = projectFilesInSolution.FirstOrDefault(p => p.Name == name + p.Extension);
            if (originalProjectFile == null)
                throw new ArgumentException(
                                            $"Failed to find a Project file {projectFilePattern} whose name matched the "
                                          + $"Startup classes' AssemblyName {name} under solution directory {directoryInfo.FullName}"
                                          + "Is your WebProject's filename different to the AssemblyName it generates?",
                                            directoryInfo.FullName);

            var originalProjectDirectoryPath = Path.GetDirectoryName(originalProjectFile.FullName);
            return originalProjectDirectoryPath;
        }
    }
}
