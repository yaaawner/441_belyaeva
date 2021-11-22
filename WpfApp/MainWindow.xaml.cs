using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// 

 
    public partial class MainWindow : Window
    {
        //string ImageFolder;

        public static ObservableCollection<Results> resultCollection = new ObservableCollection<Results>();

        //static ResultCollection resultCollection = new ResultCollection();

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
            string image;

            while (true)
            {
                (type, image) = await Detector.resultBufferBlock.ReceiveAsync();
                //Types.Add(new Results(type, "jkjk"));
                //Console.WriteLine(type + images.ToString());
                //listBox_types.

                bool flag = true;
                foreach (Results r in resultCollection)
                {
                    if (r.Info == type)
                    {
                        r.images.Add(image);
                        flag = false;
                        break;
                        //return;
                    }
                }
                if (flag)
                {
                    resultCollection.Add(new Results(type, image));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = resultCollection;
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

            resultCollection.Clear();
            Detector.cancelTokenSource = new CancellationTokenSource();
            Detector.token = Detector.cancelTokenSource.Token;
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Detector.cancelTokenSource.Cancel();
        }
    }
}
