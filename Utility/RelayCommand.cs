using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PCControl.Utility
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action _execute;
        private readonly Func<object, bool> _canExecute;

        public bool CanExecute(object? parameter) => _canExecute== null || _canExecute(parameter);
        public RelayCommand(Action ex, Func<object, bool> CanExecute = null)
        {
            _execute = ex;
            _canExecute = CanExecute;
        }

        public void Execute(object? parameter)
        {
            _execute();
        }
    }

    public class RelayCommand1 : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action<object?> _execute;
        private readonly Func<bool>? _canExecute;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }
        public RelayCommand1(Action<object?> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<bool>? _canExecute;
        public event EventHandler? CanExecuteChanged;
        public RelayCommand(Action<T?> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter)
        {
           return _canExecute?.Invoke() ?? true;
        }
        public void Execute(object? parameter)
        {
            if (parameter is T tParam)
                _execute(tParam);
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }


    public class AsyncRelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Func<CancellationToken, Task> _execute;

        private CancellationTokenSource? _cts;

        public AsyncRelayCommand(Func<CancellationToken, Task> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter)
        {
            return _cts == null;
        }

        public async void Execute(object? parameter)
        {
            _cts = new CancellationTokenSource();

            try
            {
                await _execute(_cts.Token);
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
            }
        }

        public void Cancel()
        {
            _cts?.Cancel();
        }

    }

}