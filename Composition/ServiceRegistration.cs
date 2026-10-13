using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoVision.Service.IPC;
using Microsoft.Extensions.DependencyInjection;
using PCControl.Machine;
using PCControl.Sequence;
using PCControl.Service;
using PCControl.Service.Config;
using PCControl.Service.Navigation;
using PCControl.ViewModel;
using PCControl.Views;

namespace PCControl.Composition
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterPcControlServices(this IServiceCollection services)
        {
            //services.AddSingleton(sp =>
            //{
            //    var config = new Config();
            //    //PosControl.Bind(config);
            //    return config;
            //});

            //services.AddSingleton<Config>();
            //services.AddSingleton<IIpcFactory, IpcFactory>();
            //services.AddSingleton<IAppNavigationService, AutoNavigationService>();
            //services.AddSingleton<MachineDevices>();
            //services.AddSingleton<SeqThread>();
            //services.AddSingleton<SeqStatus>();
            //services.AddSingleton<MainWindow>();
            //services.AddSingleton<MainWindowViewModel>();
            //services.AddSingleton<HomeViewModel>();
            //services.AddSingleton<CylinderViewModel>();
            //services.AddSingleton<ManualViewModel>();
            //services.AddSingleton<IoMonitorViewModel>();
            //services.AddSingleton<MotionViewModel>();
            //services.AddSingleton<MotionService>();
            //services.AddSingleton<TeachingViewModel>();
            //services.AddSingleton<MachineStatus>();


            services.AddSingleton<Config>();
            services.AddSingleton<IIpcFactory, IpcFactory>();
            services.AddSingleton<IAppNavigationService, AutoNavigationService>();
            services.AddSingleton<AppDialogService>();
            services.AddSingleton<IActuatorDialogService>(sp => sp.GetRequiredService<AppDialogService>());
            services.AddSingleton<IAppDialogService>(sp => sp.GetRequiredService<AppDialogService>());
            services.AddSingleton<MachineDevices>();
            services.AddSingleton<Sequence.SeqThread>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<ManualViewModel>();
            services.AddSingleton<IoMonitorViewModel>();
            services.AddSingleton<MotionViewModel>();
            //services.AddSingleton<TestViewModel>();
            services.AddSingleton<TeachingViewModel>();

            services.AddSingleton<MachineStatus>();
            services.AddSingleton<CylinderViewModel>();
            services.AddSingleton<MotionViewModel>();
            services.AddSingleton<MotionService>();

            return services;

        }
    }
}
