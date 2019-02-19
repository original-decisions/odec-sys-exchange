using Autofac;
using Microsoft.Extensions.Configuration;
using odec.Framework.Extensions;
using odec.Framework.Infrastructure;
using System.Collections.Generic;
using System.IO;

using odec.Framework.Infrastructure.Autofac;

namespace odec.Framework.SysExchange.Helpers
{
    public static class CfgUtils
    {
        private static readonly IList<string> DefaultLookupFiles = new List<string>
        {
            "appsettings.json",
            "AutofacCfg.json",
            "lookupAssemblies.json",
            "exchangeCfg.json"
        };
        public static IConfiguration LookupDefaultConfigs()
        {
            var builder = new ConfigurationBuilder();
            AddFilesToConfig(builder, DefaultLookupFiles);
            builder.AddEnvironmentVariables();
            return builder.Build();
        }
        public static IConfiguration CustomConfigsLookup(IList<string> customConfigFiles)
        {
            var builder = new ConfigurationBuilder();
            AddFilesToConfig(builder, DefaultLookupFiles);
            AddFilesToConfig(builder, customConfigFiles);
            //builder.AddEnvironmentVariables();
            return builder.Build();
        }

        public static void SetupIoC(IConfiguration config, string customSection = "Exchange:Registrant")
        {
            var registrantSection = config.GetSection(customSection);
            var registrantOptions = new RegistrantOptions();

            if (registrantSection != null)
                registrantSection.Bind(registrantOptions);
            else
                registrantOptions.InitDefault();

            var ioCBuilder = new ContainerBuilder();
            ioCBuilder.RegisterModule(new ModuleRegistrant(config, registrantOptions));
            IoCHelper.Container = ioCBuilder.Build();
        }

        private static void AddFilesToConfig(IConfigurationBuilder builder, IList<string> configFiles)
        {
            foreach (var configFile in configFiles)
            {
                //Todo: helper class in framework to add different types of cfgs.
                var ext = Path.GetExtension(configFile);
                if (ext.Contains(ConfigurationFileTypes.Json.GetCode()))
                {
                    builder.AddJsonFile(configFile, optional: true, reloadOnChange: true);
                }
                if (ext.Contains(ConfigurationFileTypes.Xml.GetCode()))
                {
                    builder.AddXmlFile(configFile, optional: true, reloadOnChange: true);
                }
            }
        }

//        public class ModuleRegistrant : Module
//        {
//            protected readonly RegistrantOptions RegistrantOptions = new RegistrantOptions();
//            /// <summary>
//            /// By Default registeres all modules specified in lookupAsseblies.json in root directory which ends with Repository
//            /// </summary>
//            public ModuleRegistrant()
//            {
//                var cfgBuilder = new ConfigurationBuilder()
//                    .SetBasePath(Directory.GetCurrentDirectory());
//                foreach (var fileName in RegistrantOptions.LookupFileNames)
//                {
//                    var ext = Path.GetExtension(fileName);
//                    if (ext == ConfigurationFileTypes.Json.GetCode())
//                    {
//                        cfgBuilder.AddJsonFile(fileName, optional: true);
//                    }
//                    if (ext == ConfigurationFileTypes.Xml.GetCode())
//                    {
//                        cfgBuilder.AddXmlFile(fileName, optional: true);
//                    }

//                }



//                Cfg = cfgBuilder.Build();
//            }
//            public ModuleRegistrant(IConfiguration cfg, RegistrantOptions options) : this(cfg)
//            {
//                RegistrantOptions = options;
//            }
//            /// <summary>
//            /// Based on Configuration with section assemblies. Default registration for repositories.
//            /// </summary>
//            /// <param name="cfg"></param>
//            public ModuleRegistrant(IConfiguration cfg)
//            {
//                Cfg = cfg;
//            }

//            /// <summary>
//            /// Based on Configuration with section assemblies.
//            /// </summary>
//            /// <param name="cfg"></param>
//            /// <param name="lookFor"></param>
//            public ModuleRegistrant(IConfiguration cfg, IList<StringFilter> lookFor) : this(cfg)
//            {
//                RegistrantOptions.LookFor = lookFor ?? throw new ArgumentNullException(nameof(lookFor), nameof(lookFor) + "should be defined");
//            }
//            public ModuleRegistrant(IConfiguration cfg, string cfgSection, IList<StringFilter> lookFor) : this(cfg, lookFor)
//            {
//                RegistrantOptions.CfgSectionName = cfgSection;
//            }

//            protected IConfiguration Cfg { get; }

//            protected override void Load(ContainerBuilder builder)
//            {
//                try
//                {
//                    var searchAssemblies = Cfg.GetSection(RegistrantOptions.CfgSectionName).GetChildren().Select(it => it.Value);
//                    //TODO:should be a bind here to options
//                    foreach (var assembly in searchAssemblies)
//                    {
//                        foreach (var lookFor in RegistrantOptions.LookFor)
//                        {
//                            var possiblePath = GetAssemblyName(assembly);
//                            switch (lookFor.CompareOperation)
//                            {
//                                case StringCompareOperation.Prefix:
//                                    builder.RegisterAssemblyTypes(Assembly.Load(possiblePath))
//                                        .Where(t => t.Name.StartsWith(lookFor.Target))
//                                        .AsImplementedInterfaces();
//                                    break;
//                                case StringCompareOperation.Contains:
//                                    builder.RegisterAssemblyTypes(Assembly.Load(possiblePath))
//                                        .Where(t => t.Name.Contains(lookFor.Target))
//                                        .AsImplementedInterfaces();
//                                    break;
//                                case StringCompareOperation.Postfix:
//                                    builder.RegisterAssemblyTypes(Assembly.Load(possiblePath))
//                                        .Where(t => t.Name.EndsWith(lookFor.Target))
//                                        .AsImplementedInterfaces();
//                                    builder.RegisterAssemblyTypes(Assembly.Load(possiblePath))
//                                        .Where(t => t.Name.EndsWith(lookFor.Target))
//                                        .AsImplementedInterfaces();
//                                    break;
//                                default:
//                                    throw new NotImplementedException("Currently we don'");
//                            }

//                        }
//                    }

//                    //.InstancePerLifetimeScope();
//                    //base.Load(builder);
//                }
//                catch (Exception ex)
//                {
//                    LogEventManager.Logger.Error(ex.Message, ex);
//                    throw;
//                }

//            }

//            private AssemblyName GetAssemblyName(string name)
//            {
//                var entryAssembly = Assembly.GetEntryAssembly();
//                var location = entryAssembly.Location;

//                var directory = Path.GetDirectoryName(location);
//                var targetPath = Path.Combine(directory, name);
//                // Lookup working directory
//                if (!File.Exists(targetPath))
//                {
//                    directory = Directory.GetCurrentDirectory();
//                    targetPath = Path.Combine(directory, name);
//                }
                
//                AssemblyName assemblyName = null;

//#if NETCOREAPP1_0 || NETCOREAPP2_0 || NETCOREAPP2_1
//                assemblyName = AssemblyLoadContext.GetAssemblyName(targetPath);
//#endif
//#if NET452
//                if (!targetPath.EndsWith(".dll"))
//                    targetPath += ".dll";
//                assemblyName = AssemblyName.GetAssemblyName(targetPath);
//#endif
                
//                return assemblyName;
//            }
//        }
    }
}
