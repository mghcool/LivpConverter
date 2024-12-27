using System;
using System.Threading.Tasks;
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
            //注册全局异常事件
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex && Dispatcher != null)
                {
                    MessageBox.Show(ex.ToString(), "主线程未捕获的异常", MessageBoxButton.OK, MessageBoxImage.Exclamation); 
                }
            };
            DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show(args.Exception.ToString(), "子线程未捕获的异常", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                args.Handled = true;
            };
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                MessageBox.Show(args.Exception.ToString(), "未发现的异常", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                args.SetObserved();
            };

            MagickNET.Initialize();
            
            InitializeComponent();
        }
    }
}
