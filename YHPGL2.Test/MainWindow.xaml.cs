using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YHPGL2.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private HPGLFile _hpglFile;

        private void _OnClicked(object sender, RoutedEventArgs e)
        {
            _ShowOpenIgesFileDialog();
        }

        private void _ShowOpenIgesFileDialog()
        {
            var dialog = new OpenFileDialog() { Filter = string.Format("{0}|*.{1};*.{2};*.{3};", "HPGL File", "plt", "hgl", "hpgl") };
            if (dialog.ShowDialog() == true)
                _hpglFile = HPGLFile.Load(dialog.FileName);
        }
    }
}