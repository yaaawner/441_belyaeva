using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModelLibrary;
using Ookii.Dialogs.Wpf;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //string ImageFolder;
        private static async Task Consumer()
        {
            string print;
            while (true)
            {
                print = await Detector.bufferBlock.ReceiveAsync();
                if (print == "end")
                    break;
                Console.WriteLine(print);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            //ImageFolder = TextBox_Path.Text;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            //mainCollection.AddDefaults();
            //await Detector.DetectImage(TextBox_Path.Text);
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            //if ((bool)dialog.ShowDialog())
            //  mainCollection.AddElementFromFile(dialog.FileName);

            if ((bool)dialog.ShowDialog())
                TextBox_Path.Text = dialog.SelectedPath;
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            
            //await Task.WhenAll(Detector.DetectImage(TextBox_Path.Text), Consumer());
            //btnRun.IsEnabled = false;
            var task = Task.Run(async () =>
            {
                await Detector.DetectImage(TextBox_Path.Text);
            });

            /*
            await task.ContinueWith((t) =>
            {
                 Dispatcher.Invoke(() =>
                 {
                     btnRun.IsEnabled = true;
                 });
            });
            */
        }
    }
}
