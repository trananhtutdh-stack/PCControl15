
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace PCControl.Service.Navigation
{
    internal class AutoNavigationService : IAppNavigationService
    {
        private static readonly Dictionary<string, object> pageTb = new ();

        private readonly IServiceProvider _services;
        public event Action<object>? Navigated;
        public AutoNavigationService(IServiceProvider services)
        {
            _services = services;
        }
        public void NavigateRegister(string pg)
        {
            if (pageTb.ContainsKey(pg))
                return;

            var asm = Application.Current
                .GetType()
                .Assembly;

            var viewType = asm.GetTypes()
                .FirstOrDefault(t =>
                    t.Name == pg &&
                    typeof(FrameworkElement)
                        .IsAssignableFrom(t));

            var vmType = asm.GetTypes()
                .FirstOrDefault(t =>
                    t.Name == pg + "ViewModel");

            if (viewType == null)
                return;

            var viewInstance =
                Activator.CreateInstance(viewType)
                as FrameworkElement;

            if (viewInstance == null)
                return;

            pageTb.Add(pg, viewInstance);

            if (vmType == null)
                return;

            viewInstance.DataContext = _services.GetRequiredService(vmType);
        }
        public object? NavigateRequest(string pg)
        {
            if (pageTb.TryGetValue(pg, out var view1))
                return view1;
            NavigateRegister(pg);
            pageTb.TryGetValue(pg, out var view2);

            return view2;
        }
        public void Navigate(string pg)
        {
            var view = NavigateRequest(pg);
            if (view == null)
                return;
            Navigated?.Invoke(view);
        }
    }
}
