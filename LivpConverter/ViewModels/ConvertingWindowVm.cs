using System.IO.Compression;
using System.IO;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using LivpConverter.Models;
using System;
using System.Windows.Forms;
using System.Windows.Controls;

namespace LivpConverter.ViewModels
{
    public partial class ConvertingWindowVm : ObservableObject
    {
        [ObservableProperty]
        private string _title = "正在转换...";

        [ObservableProperty]
        private string _cancelButtonText = "取消";

        [ObservableProperty]
        private string _currentFileName = string.Empty;

        [ObservableProperty]
        private int _fileConversionProgress;

        [ObservableProperty]
        private int _filesCount;

        [ObservableProperty]
        private int _convertedCount;

        public string TotalProgressDescription => $" {ConvertedCount}/{FilesCount}";

        public async Task ConvertAsync(ConversionParams args)
        {
            try
            {
                await Task.Run(() => Convert(args));
            }
            catch (Exception ex)
            {
                await new Wpf.Ui.Controls.MessageBox()
                {
                    Title = "转换出错",
                    Content = new TextBlock() { Text = ex.Message, Margin = new(0) },
                    CloseButtonText = "确定"
                }.ShowDialogAsync();
            }
        }

        private void Convert(ConversionParams args)
        {
            // 解压临时目录
            string tempPath = Path.Combine(args.OutputFolderPath, "temp-extract");
            if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);
            // 获取所有livp文件
            string[] files = Directory.GetFiles(args.InputFolderPath, "*.livp", SearchOption.TopDirectoryOnly);
            FilesCount = files.Length;
            ConvertedCount = 0;
            OnPropertyChanged(nameof(TotalProgressDescription));
            // 挨个处理文件
            foreach (string file in files)
            {
                // 获取文件名（不带扩展名）
                string fileName = Path.GetFileNameWithoutExtension(file);
                CurrentFileName = Path.GetFileName(file);
                // 解压到临时目录
                string extractPath = Path.Combine(tempPath, fileName);
                ZipFile.ExtractToDirectory(file, extractPath);
                // 转换heic文件
                string[] heicFiles = Directory.GetFiles(extractPath, "*.heic", SearchOption.TopDirectoryOnly);
                if (heicFiles.Length > 0)
                {
                    string outputFilePath = Path.Combine(args.OutputFolderPath, $"{fileName}.{(args.Format == MagickFormat.Png ? "png" : "jpg")}");
                    using MagickImage image = new MagickImage(heicFiles[0]);
                    image.Progress += (sender, eventArgs) =>
                    {
                        if (eventArgs == null) return;
                        FileConversionProgress = (int)eventArgs.Progress;
                    };
                    image.Format = args.Format;
                    image.Quality = args.Quality;
                    image.Write(outputFilePath);
                }
                ConvertedCount++;
                OnPropertyChanged(nameof(TotalProgressDescription));
            }
            // 删除临时目录
            Directory.Delete(tempPath, true);
            CancelButtonText = "完成";
            Title = "转换完成";
        }
    }
}
