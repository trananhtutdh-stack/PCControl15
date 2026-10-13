using AutoVision.Service.IPC;
using PCControl.Machine;
using PCControl.ViewModel;
using ShareLibrary.Logging;
using System.Diagnostics;
using System.Text;
using System.Windows;

using static PLCVir.Device.GlobDev.T;

namespace PCControl
{
    
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }

}