using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageMagick;

namespace LivpConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // 自动适应系统主题
            Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
                    this,                                    // Window class
                    Wpf.Ui.Controls.WindowBackdropType.Mica, // Background type
                    true                                     // Whether to change accents automatically
                );
            };

            string heicPath = @"C:\Users\MGH\Desktop\2024-12-25 113045.livp\IMG_0598.HEIC.heic";
            string jpgPath = @"C:\Users\MGH\Desktop\2024-12-25 113045.livp\IMG_0598.HEIC.png";
            string livpPath = @"C:\Users\MGH\Desktop\2024-12-25 113045 (2).livp";
            string extractPath = @"C:\Users\MGH\Desktop\livp";


            //ZipFile.ExtractToDirectory(livpPath, extractPath);

            //ImageConvert(heicPath, jpgPath, MagickFormat.Png);
        }

        private void ImageConvert(string sourcePath, string destinationPath, MagickFormat destinationFormat)
        {
            MagickNET.Initialize();
            using MagickImage image = new MagickImage(sourcePath);
            if (image.HasProfile("icc"))
            {
                var iccProfile = image.GetProfile("icc");
                image.SetProfile(iccProfile!);
            }

            image.Format = destinationFormat;
            //image.Quality = 100;
            image.Write(destinationPath);
            Array.Copy(Array.Empty<byte>(), Array.Empty<byte>(), 0);
        }
    }
}