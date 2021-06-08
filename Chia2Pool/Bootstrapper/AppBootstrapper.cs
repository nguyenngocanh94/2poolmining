using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using Chia2Pool.KeyBinding;
using Chia2Pool.ViewModels;
using Chia2Pool.Views;
using ControlzEx.Theming;
using log4net.Repository.Hierarchy;
using KeyTrigger = Chia2Pool.KeyBinding.KeyTrigger;

namespace Chia2Pool.Bootstrapper
{
    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer _compositionContainer;

        private Dictionary<string, Assembly> _nameToAssemblyDict;

        private const string ChiaRpc = "ChiaRpc.dll";
        private const string Chia2Pool = "Chia2Pool.dll";


        private readonly string[] _assembliesForCaliburn = {Chia2Pool, Chia2Pool};

        public AppBootstrapper()
        {
            Initialize();
        }

        static AppBootstrapper()
        {
            Common.Logger.GetLog = () => log4net.LogManager.GetLogger(typeof(AppBootstrapper));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            ThemeManager.Current.ChangeTheme(sender as App, "Light.Green");
            string runtimeDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (File.Exists(runtimeDirectory + "/keys.2pool"))
            {
                var self = JsonSerializer.Deserialize<SettingTemp>(File.ReadAllText(runtimeDirectory + "/keys.2pool"));
                Settings.GetInstance().ApiKey = self.ApiKey;
                Settings.GetInstance().SslDirectory = self.SslDirectory;
                Settings.GetInstance().PoolUrl = self.PoolUrl;
            }
            DisplayRootViewFor<DashboardViewModel>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            string mainAssemblyPath = Assembly.GetEntryAssembly().Location;
            string rootFolder = Path.GetDirectoryName(mainAssemblyPath);

            _nameToAssemblyDict = new Dictionary<string, Assembly>();

            foreach (string assembly in _assembliesForCaliburn)
            {
                _nameToAssemblyDict[assembly] = Assembly.LoadFrom(rootFolder + "\\" + assembly);
            }

            return _nameToAssemblyDict.Values;
        }

        protected override void Configure()
        {
            _compositionContainer = ContainerUtils.CreateCompositionContainer();

            // IMPORTANT: this is necessary to split Views and ViewModels in Caliburn Micro
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = ConfigsConstant.VIEWS_NS,
                DefaultSubNamespaceForViewModels = ConfigsConstant.VIEW_MODELS_NS
            };

            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);

            ExtendViewLocator();
            CreateTriggerParser();
        }

        /// <summary>
        /// This method extends the feature of view locator. If it cannot find the desired view in
        /// convention namespace, then it will try again in Views.
        /// </summary>
        private void ExtendViewLocator()
        {
            Func<Type, DependencyObject, object, UIElement> originalLocateFunc = ViewLocator.LocateForModelType;
            ViewLocator.LocateForModelType = (modelType, displayLocation, context) =>
            {
                // get the ui element that the original function returns
                UIElement originalOutput = originalLocateFunc(modelType, displayLocation, context);

                if (originalOutput is TextBlock)
                {
                    var textBlock = originalOutput as TextBlock;
                    if (textBlock.Text != null && textBlock.Text.Contains("Cannot find view") && context != null)
                    {
                        // the original function did not find the corresponding view in convention namespace try
                        // to search for the view in *.Views
                        var viewTypeName = modelType.FullName.Replace(modelType.Name, "")
                            .Replace(ConfigsConstant.MODEL, string.Empty) + context;
                        var viewType = (from assmebly in AssemblySource.Instance
                            from type in assmebly.GetExportedTypes()
                            where type.FullName == viewTypeName
                            select type).FirstOrDefault();
                        if (viewType != null)
                        {
                            return ViewLocator.GetOrCreateViewType(viewType);
                        }
                    }
                }

                return originalOutput;
            };
        }

        private static void CreateTriggerParser()
        {
            var defaultCreateTrigger = Parser.CreateTrigger;

            Parser.CreateTrigger = (target, triggerText) =>
            {
                if (triggerText == null)
                {
                    return defaultCreateTrigger(target, null);
                }

                var triggerDetail = triggerText
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                var splits = triggerDetail.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                // switch (splits[0])
                // {
                //     case "Key":
                //         var key = (Key)Enum.Parse(typeof(Key), splits[1], true);
                //         return new KeyTrigger { Key = key };
                //
                //     case "Gesture":
                //         var mkg = (MultiKeyGesture)(new MultiKeyGestureConverter()).ConvertFrom(splits[1]);
                //         return new KeyTrigger { Modifiers = mkg.KeySequences[0].Modifiers, Key = mkg.KeySequences[0].Keys[0] };
                // }

                return defaultCreateTrigger(target, triggerText);
            };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = key;

            if (string.IsNullOrEmpty(key))
            {
                contract = AttributedModelServices.GetContractName(serviceType);
            }

            var exports = _compositionContainer.GetExportedValues<object>(contract);

            if (exports.Any())
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _compositionContainer.GetExportedValues<object>(
                AttributedModelServices.GetContractName(serviceType));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void BuildUp(object instance)
        {
            _compositionContainer.SatisfyImportsOnce(instance);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
        }
    }

    public class ConfigsConstant
    {
        public const string VIEWS_NS = "Views";

        public const string VIEW_MODELS_NS = "ViewModels";

        public const string MODEL = "Model";
    }

    public class ContainerUtils
    {
        public static CompositionContainer CreateCompositionContainer()
        {
            var catalog = new AggregateCatalog(
                AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
            );
            var container = new CompositionContainer(
                catalog
            );


            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);

            container.Compose(batch);

            return container;
        }
    }
}