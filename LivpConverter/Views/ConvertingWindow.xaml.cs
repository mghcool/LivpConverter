using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LivpConverter.Models;
using LivpConverter.ViewModels;

namespace LivpConverter.Views
{
    /// <summary>
    /// ConvertingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConvertingWindow
    {
        private ConversionParams _conversionParams;

        public ConvertingWindow(ConversionParams args)
        {
            _conversionParams = args;
            InitializeComponent();
        }

        private async void WindowOnFirstShow(object sender, EventArgs e)
        {
            await ((ConvertingWindowVm)DataContext).ConvertAsync(_conversionParams);
        }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Owner.WindowState = WindowState.Minimized;
        }
    }
}
