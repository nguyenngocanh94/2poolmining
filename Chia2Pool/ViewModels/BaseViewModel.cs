using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Chia2Pool.Common;

namespace Chia2Pool.ViewModels
{
    public class BaseViewModel: Screen
    {
        public static readonly byte[] TLAS = { 0x4D, 0x61, 0x70, 0x32, 0x58, 0x5F, 0x32, 0x30, 0x31, 0x39 };

        public const string COMMAND = "Command";

        public const string EXECUTE = "Execute";

        public const string CAN_EXECUTE = "CanExecute";
        

        protected readonly IEventAggregator events;

        public IEventAggregator EventAggregator => events;

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructs without event aggregator.
        /// </summary>
        public BaseViewModel()
        {
            InitDelegateCommands(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="events"></param>
        public BaseViewModel(IEventAggregator events)
        {
            this.events = events;
            events?.Subscribe(this);
            InitDelegateCommands(this);
        }

        /// <summary>
        /// Publishes an event message on UI thread.
        /// </summary>
        /// <param name="eventMessage"></param>
        protected void PublishOnUIThread(object eventMessage)
        {
            events?.PublishOnUIThread(eventMessage);
        }

        /// <summary>
        /// Publishes an event message on a background thread.
        /// </summary>
        /// <param name="eventMessage"></param>
        protected void PublishOnBackgroundThread(object eventMessage)
        {
            events?.PublishOnBackgroundThread(eventMessage);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            events?.Unsubscribe(this);

            if (GetView() is IHandle view)
            {
                events?.Unsubscribe(view);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>fd
        ///
        ///
        /// 
        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);

            if (view is IHandle)
            {
                events?.Unsubscribe(view);
                events?.Subscribe(view);
            }
        }
        
        /// <summary>
        /// Bind all commands based on convention: {Action}Command <-> Execute{Action}, CanExecute{Action}
        /// </summary>
        /// <param name="model"></param>
        public static void InitDelegateCommands(BaseViewModel model)
        {
            var iCommandType = typeof(ICommand);
            Type modelType = model.GetType();

            // TODO: use ICommand instead of DelegateCommand in all VMs
            IEnumerable<PropertyInfo> allCommands = modelType.GetProperties().Where(x => iCommandType.IsAssignableFrom(x.PropertyType));
            foreach (PropertyInfo command in allCommands)
            {
                string executeMethodName = EXECUTE + command.Name.Replace(COMMAND, "");
                MethodInfo executeMethod = modelType.GetMethod(executeMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (executeMethod != null)
                {
                    string canExecuteMethodName = CAN_EXECUTE + command.Name.Replace(COMMAND, "");
                    MethodInfo canExecuteMethod = modelType.GetMethod(canExecuteMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (command.PropertyType.IsGenericType)
                    {
                        InitGenericDelegateCommand(model, command, executeMethod, canExecuteMethod);
                    }
                    else
                    {
                        InitDelegateCommand(model, command, executeMethod, canExecuteMethod);
                    }
                }
            }
        }

        private static void InitGenericDelegateCommand(BaseViewModel model, PropertyInfo command, MethodInfo executeMethod, MethodInfo canExecuteMethod = null)
        {
            Type actionType = typeof(Action<>).MakeGenericType(command.PropertyType.GetGenericArguments()[0]);
            Delegate executeAction = Delegate.CreateDelegate(actionType, model, executeMethod);

            Type delegateCommandType = typeof(RelayCommand<>).MakeGenericType(command.PropertyType.GetGenericArguments()[0]);
            object delegateCommand = null;

            if (canExecuteMethod != null)
            {
                Type[] genericTypes = new Type[] { command.PropertyType.GetGenericArguments()[0], typeof(bool) };
                Type funcType = typeof(Func<,>).MakeGenericType(genericTypes);
                Delegate canExecuteAction = Delegate.CreateDelegate(funcType, model, canExecuteMethod);
                delegateCommand = delegateCommandType.GetConstructors()[2].Invoke(new object[] { executeAction, canExecuteAction, null });
            }
            else
            {
                delegateCommand = delegateCommandType.GetConstructors()[0].Invoke(new object[] { executeAction });
            }

            command.SetValue(model, delegateCommand);
        }

        private static void InitDelegateCommand(BaseViewModel model, PropertyInfo command, MethodInfo executeMethod, MethodInfo canExecuteMethod = null)
        {
            System.Action executeAction = (System.Action)Delegate.CreateDelegate(typeof(System.Action), model, executeMethod);
            RelayCommand delegateCommand = null;

            if (canExecuteMethod != null)
            {
                Func<bool> canExecuteAction = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), model, canExecuteMethod);
                delegateCommand = new RelayCommand(executeAction, canExecuteAction);
            }
            else
            {
                delegateCommand = new RelayCommand(executeAction);
            }

            command.SetValue(model, delegateCommand);
        }
        
    }
}