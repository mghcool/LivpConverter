using System.Configuration;
using System.Data;
using System.Windows;
using ImageMagick;
using Application = System.Windows.Application;

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
