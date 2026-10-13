using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using AutoVision.Service.IPC;
using Microsoft.Extensions.DependencyInjection;
using PCControl.Composition;
using PCControl.Machine;
using PCControl.Sequence;
using PCControl.Service.Config;
using PCControl.Service.Navigation;
using ShareLibrary.Localizations;


namespace PCControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public static SynchronizationContext? UIContext { get; private set; }
       


        //static App()
        //{
        //    //AppNavigate = new AutoNavigationService();
        //    //Devices = new MachineDevices(new IpcFactory());
        //    //seqThread = new SeqThread();
        //    //Config = new Config();
        //}
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            UIContext = SynchronizationContext.Current;

            var services = new ServiceCollection();
            services.RegisterPcControlServices();
            Services = services.BuildServiceProvider();


            LanguageManager.LoadLanguage("en");
            Services.GetRequiredService<MainWindow>().Show();
            Services.GetRequiredService<SeqThread>();
            Services.GetRequiredService<Config>();
        }
    }
}
