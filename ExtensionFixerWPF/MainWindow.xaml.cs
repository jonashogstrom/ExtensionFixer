using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using ExtensionFixer.Shared;

namespace ExtensionFixerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MyProgress _progressObj;

        public MainWindow()
        {
            InitializeComponent();
            _progressObj = new MyProgress(_progress, Dispatcher);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    _folder.Text = dialog.SelectedPath;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var t = new ExtensionFixerTool(s => Dispatcher.Invoke(() => _log.Text += s + Environment.NewLine));
            if (Directory.Exists(_folder.Text))
            {
                selectFolderBtn.IsEnabled = false;
                runBtn.IsEnabled = false;
                var folder = _folder.Text;
                var verbose = cbVerbose.IsChecked == true;
                var doRename = cbDoRename.IsChecked == true;
                await Task.Run(() =>
                {
                    t.Main(folder, verbose, doRename, _progressObj);
                });
                selectFolderBtn.IsEnabled = true;
                runBtn.IsEnabled = true;
            }
        }
    }

    internal class MyProgress : IMyProgress<double>
    {
        private readonly System.Windows.Controls.ProgressBar _bar;
        private readonly Dispatcher _dispatcher;

        public MyProgress(System.Windows.Controls.ProgressBar bar, Dispatcher dispatcher)
        {
            _bar = bar;
            _dispatcher = dispatcher;
        }

        public void Report(double value)
        {
            _dispatcher.Invoke(() => _bar.Value = (int)value);
        }

        public void SetMax(int maxValue)
        {
            _dispatcher.Invoke(() =>
            {
                _bar.IsIndeterminate = false;
                return _bar.Maximum = maxValue;
            });

        }

        public void SetIndefinite()
        {
            _dispatcher.Invoke(() => _bar.IsIndeterminate = true);
        }
    }

}
