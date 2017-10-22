using System;
using System.Windows.Input;

namespace BobOnRails.Controller.MVVM
{
    /// <summary>
    /// To register commands in MMVM pattern
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> executeDelegate;
        private Predicate<object> canExecuteDelegate;

        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommands"/> class.
        /// </summary>
        /// <param name="execute">Execute method as action.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            executeDelegate = execute ?? throw new ArgumentNullException(nameof(execute));
            canExecuteDelegate = canExecute;
        }

        /// <inheritdoc cref="ICommand.Execute(object)"/>
        public void Execute(object parameter)
        {
            executeDelegate(parameter);
        }
        
        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public bool CanExecute(object parameter)
        {
            return canExecuteDelegate == null || canExecuteDelegate(parameter);
        }
    }
}
