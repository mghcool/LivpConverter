using System.IO;
using System.IO.Compression;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageMagick;

namespace LivpConverter
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

        [ObservableProperty]
        private bool _panelEnabled = true;

        [ObservableProperty]
        private int _filesCount;

        [ObservableProperty]
        private int _convertedCount;

        public string ProgressDescription => $" {ConvertedCount}/{FilesCount}";

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
        private async Task StartConvert()
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

            PanelEnabled = false;
            Title = "LIVP转换器 - 转换中";
            try
            {
                await Task.Run(() => ImageConvert());
                await MessageBoxShow("提示", "转换完成", 50);
            }
            catch (Exception ex)
            {
                await MessageBoxShow("错误", ex.Message);
            }
            PanelEnabled = true;
            Title = "LIVP转换器";
        }

        private void ImageConvert()
        {
            // 解压临时目录
            string tempPath = Path.Combine(OutputFolderPath, "temp-extract");
            if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);
            // 获取所有livp文件
            string[] files = Directory.GetFiles(InputFolderPath, "*.livp", SearchOption.TopDirectoryOnly);
            FilesCount = files.Length;
            ConvertedCount = 0;
            OnPropertyChanged(nameof(ProgressDescription));
            // 挨个处理文件
            foreach (string file in files)
            {
                // 获取文件名（不带扩展名）
                string fileName = Path.GetFileNameWithoutExtension(file);
                // 解压到临时目录
                string extractPath = Path.Combine(tempPath, fileName);
                ZipFile.ExtractToDirectory(file, extractPath);
                // 转换heic文件
                string[] heicFiles = Directory.GetFiles(extractPath, "*.heic", SearchOption.TopDirectoryOnly);
                if (heicFiles.Length > 0)
                {
                    string outputFilePath = Path.Combine(OutputFolderPath, $"{fileName}.{OutputFormat.ToLower()}");
                    using MagickImage image = new MagickImage(heicFiles[0]);
                    image.Format = OutputFormat == "PNG" ? MagickFormat.Png : MagickFormat.Jpeg;
                    image.Quality = OutputQuality;
                    image.Write(outputFilePath);
                }
                ConvertedCount++;
                OnPropertyChanged(nameof(ProgressDescription));
            }
            // 删除临时目录
            Directory.Delete(tempPath, true);
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
