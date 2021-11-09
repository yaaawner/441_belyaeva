using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static ObservableCollection<string> Types = new ObservableCollection<string>();

        /*
        public MainWindow()
        {
            this.InitializeComponents();
            this.DataContext = this;
            this.MyCollection = new ObservableCollection<MyObject>();
        }
        */

        private static async Task Consumer()
        {
            string type;
            string[] images;

            while (true)
            {
                type = await Detector.resultBufferBlock.ReceiveAsync();
                Types.Add(type);
                //Console.WriteLine(type + images.ToString());
                //listBox_types.
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Types;
            //Types = new List<string>();
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
            /*
            var task = Task.Run(async () =>
            {
                await Detector.DetectImage(TextBox_Path.Text);
            });
            */
            await Task.WhenAll(Detector.DetectImage(TextBox_Path.Text), Consumer());

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
