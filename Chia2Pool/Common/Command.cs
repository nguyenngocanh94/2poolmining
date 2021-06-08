using System;
using System.Windows.Input;

namespace Chia2Pool.Common
{
       public class RelayCommand : ICommand
    {
        private Action execute;    
        private Func<bool> canExecute;    
     
        public event EventHandler CanExecuteChanged    
        {    
            add { CommandManager.RequerySuggested += value; }    
            remove { CommandManager.RequerySuggested -= value; }    
        }    
     
        public RelayCommand(Action execute, Func<bool> canExecute = null)    
        {    
            this.execute = execute;    
            this.canExecute = canExecute;    
        }    
     
        public bool CanExecute(object parameter)    
        {    
            return this.canExecute == null || this.canExecute();    
        }    
     
        public void Execute(object parameter)    
        {    
            this.execute();    
        }    
    }
    
    public class RelayCommand<T> : ICommand
    {
        private Action<T> execute;    
        private Func<bool> canExecute;    
     
        public event EventHandler CanExecuteChanged    
        {    
            add { CommandManager.RequerySuggested += value; }    
            remove { CommandManager.RequerySuggested -= value; }    
        }    
     
        public RelayCommand(Action<T> execute, Func<bool> canExecute = null)    
        {    
            this.execute = execute;    
            this.canExecute = canExecute;    
        }    
     
        public bool CanExecute(object parameter)    
        {    
            return this.canExecute == null || this.canExecute();    
        }    
     
        public void Execute(object parameter)    
        {    
            this.execute((T)parameter);    
        }    
    }
    
    public class RelayCommandWithParam : ICommand    
    {    
        private Action<object> execute;    
        private Func<object, bool> canExecute;    
     
        public event EventHandler CanExecuteChanged    
        {    
            add { CommandManager.RequerySuggested += value; }    
            remove { CommandManager.RequerySuggested -= value; }    
        }    
     
        public RelayCommandWithParam(Action<object> execute, Func<object, bool> canExecute = null)    
        {    
            this.execute = execute;    
            this.canExecute = canExecute;    
        }    
     
        public bool CanExecute(object parameter)    
        {    
            return this.canExecute == null || this.canExecute(parameter);    
        }    
     
        public void Execute(object parameter)    
        {    
            this.execute(parameter);    
        }    
    }  
}