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
using Ookii.Dialogs.Wpf;
using Microsoft.EntityFrameworkCore;
//using System.Linq;
using System.Drawing;
using System.Net.Http;
using Newtonsoft.Json;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        //public static ResultContext db = new ResultContext();
        public static HttpClient client = new HttpClient();
        public static ObservableCollection<string> resultCollection = new ObservableCollection<string>();
        //public static ObservableCollection<Results> resultCollection = db.Results.Local.ToObservableCollection();

        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = resultCollection;
            //DataContext = db.Results;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if ((bool)dialog.ShowDialog())
                TextBox_Path.Text = dialog.SelectedPath;
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            btnRun.IsEnabled = false;
            string result = await client.GetStringAsync("https://localhost:44394/results/types");
            var allbooks = JsonConvert.DeserializeObject<IEnumerable<string>>(result);
            foreach (var b in allbooks)
                resultCollection.Add(b);
                //Console.WriteLine($"{b.Title}");
            //resultCollection.Clear();
            //db.Clear();
            //Detector.cancelTokenSource = new CancellationTokenSource();
            //Detector.token = Detector.cancelTokenSource.Token;

            //await Task.WhenAll(Detector.DetectImage(TextBox_Path.Text), Consumer());
            // запустить процесс разпознавания 
            // отправляем путь, ответ все данные 
            btnRun.IsEnabled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Detector.cancelTokenSource.Cancel();
            // запрос на отмену
            btnRun.IsEnabled = true;
        }

        private void listBox_types_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //db.DeleteType(listBox_types.SelectedItem.ToString());
            // запрос на удаление типа
        }
    }
}
