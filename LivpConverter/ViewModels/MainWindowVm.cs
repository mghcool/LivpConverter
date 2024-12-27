using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageMagick;
using LivpConverter.Models;
using LivpConverter.Views;

namespace LivpConverter.ViewModels
{
    public partial class MainWindowVm : ObservableObject
    {
        [ObservableProperty]
        private string _title = "LIVP转换器";

        [ObservableProperty]
        private string _inputFolderPath = string.Empty;

        [ObservableProperty]
        private string _outputFolderPath = string.Empty;

        public string[] OutputFormatItems { get; } = { "JPG", "PNG" };

        [ObservableProperty]
        private string _outputFormat = string.Empty;

        [ObservableProperty]
        private uint _outputQuality = 75;

        [RelayCommand]
        private void SelectInputFolderPath()
        {
            using FolderBrowserDialog dialog = new();
            dialog.Description = "请选择LIVP实况图文件夹";
            dialog.ShowNewFolderButton = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (dialog.ShowDialog() == DialogResult.OK) InputFolderPath = dialog.SelectedPath;
        }

        [RelayCommand]
        private void SelectOutputFolderPath()
        {
            using FolderBrowserDialog dialog = new();
            dialog.Description = "请选择输出文件夹";
            dialog.ShowNewFolderButton = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (dialog.ShowDialog() == DialogResult.OK) OutputFolderPath = dialog.SelectedPath;
        }

        [RelayCommand]
        private async Task StartConvert(System.Windows.Window parentWindow)
        {
            
            if (string.IsNullOrWhiteSpace(InputFolderPath) || string.IsNullOrWhiteSpace(OutputFolderPath)) return;
            if (!Directory.Exists(InputFolderPath))
            {
                await MessageBoxShow("错误", "输入文件夹不存在");
                return;
            }
            if (!Directory.Exists(OutputFolderPath))
            {
                await MessageBoxShow("错误", "输出文件夹不存在");
                return;
            }

            ConversionParams args = new()
            {
                InputFolderPath = InputFolderPath,
                OutputFolderPath = OutputFolderPath,
                Format = OutputFormat == "PNG" ? MagickFormat.Png : MagickFormat.Jpeg,
                Quality = OutputQuality
            };

            Title = "LIVP转换器 - 转换中";
            ConvertingWindow convertingWindow = new(args);
            convertingWindow.Owner = parentWindow;
            convertingWindow.ShowDialog();
            Title = "LIVP转换器";
        }

        private async Task MessageBoxShow(string title, string message, double sideMargin = 0)
        {
            await new Wpf.Ui.Controls.MessageBox()
            {
                Title = title,
                Content = new TextBlock() { Text = message, Margin = new(sideMargin, 0, sideMargin, 0) },
                CloseButtonText = "确定"
            }.ShowDialogAsync();
        }
    }
}
