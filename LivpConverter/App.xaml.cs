using System.Configuration;
using System.Data;
using System.Windows;
using ImageMagick;

namespace LivpConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MagickNET.Initialize();
            InitializeComponent();
        }
    }

}
