using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoVision.Service.IPC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCControl.Machine;

namespace PCControl.ViewModel
{
    public partial class IoMonitorViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<IO> input = new();
        [ObservableProperty] private ObservableCollection<IO> output = new();
        [ObservableProperty] private string inputText = "";
        [ObservableProperty] private string outputText = "";
        [ObservableProperty] private IIpc? ipc;

        private readonly List<IO> _inputList;
        private readonly List<IO> _outputList;
        private int inputPage = 0;
        private int outputPage = 0;

        public const int Row = 8;
        public int MaxPageIn = 0;
        public int MaxPageOut = 0;

        public IoMonitorViewModel(MachineDevices devices)
        {
            Ipc = devices.Ipc;

            var ioLst = new IOLst(Ipc);

            _inputList = ioLst.IOMap
                .Where(x => x.Key.StartsWith("X"))
                .Select(x => new IO(x.Key, x.Value))
                .ToList();

            _outputList = ioLst.IOMap
                .Where(x => x.Key.StartsWith("Y"))
                .Select(x => new IO(x.Key, x.Value))
                .ToList();

            MaxPageIn = _inputList.Count == 0 ? 0 : (_inputList.Count - 1) / Row;
            MaxPageOut = _outputList.Count == 0 ? 0 : (_outputList.Count - 1) / Row;

            Update(_inputList, inputPage, Input, v => InputText = v);
            Update(_outputList, outputPage, Output, v => OutputText = v);
        }

        [RelayCommand(CanExecute = nameof(CanInputNavigate))]
        private void InputNavigate(string action)
        {
            inputPage = action switch
            {
                "First" => 0,
                "Back" => Math.Max(0, inputPage - 1),
                "Next" => Math.Min(MaxPageIn, inputPage + 1),
                "Last" => MaxPageIn,
                _ => inputPage
            };

            Update(_inputList, inputPage, Input, v => InputText = v);
            InputNavigateCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanOutputNavigate))]
        private void OutputNavigate(string action)
        {
            outputPage = action switch
            {
                "First" => 0,
                "Back" => Math.Max(0, outputPage - 1),
                "Next" => Math.Min(MaxPageOut, outputPage + 1),
                "Last" => MaxPageOut,
                _ => outputPage
            };

            Update(_outputList, outputPage, Output, v => OutputText = v);
            OutputNavigateCommand.NotifyCanExecuteChanged();
        }

        private void Update(List<IO> source, int page, ObservableCollection<IO> target, Action<string> setText)
        {
            target.Clear();

            var pageData = source
                .Skip(page * Row)
                .Take(Row)
                .ToList();

            foreach (var io in pageData)
                target.Add(io);

            if (pageData.Count > 0)
                setText($"{pageData.First().Device}-{pageData.Last().Device}");
        }

        private bool CanInputNavigate(string action)
        {
            return action switch
            {
                "First" => inputPage > 0,
                "Back" => inputPage > 0,
                "Next" => inputPage < MaxPageIn,
                "Last" => inputPage < MaxPageIn,
                _ => false
            };
        }

        private bool CanOutputNavigate(string action)
        {
            return action switch
            {
                "First" => outputPage > 0,
                "Back" => outputPage > 0,
                "Next" => outputPage < MaxPageOut,
                "Last" => outputPage < MaxPageOut,
                _ => false
            };
        }
    }

    public record IO(string Device, string Name);
}
