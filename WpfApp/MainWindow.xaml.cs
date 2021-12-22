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
using System.Diagnostics;

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
            try
            {
                listBox_objects.Items.Clear();
                resultCollection.Clear();
                //await client.GetAsync("https://localhost:44394/results/task/");
                //HttpContent content = new HttpContent(HttpContent kjjk, )
                var path = JsonConvert.SerializeObject(TextBox_Path.Text);
                Debug.WriteLine(path);
                var stringContent = new StringContent(path);
                stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                Debug.WriteLine(stringContent);
                await client.PutAsync("https://localhost:44394/results", stringContent);

                string result = await client.GetStringAsync("https://localhost:44394/results/types");
                var allbooks = JsonConvert.DeserializeObject<IEnumerable<string>>(result);
                foreach (var b in allbooks)
                    resultCollection.Add(b);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            btnRun.IsEnabled = true;
        }

        /*
        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            await client.GetAsync("https://localhost:44394/results/cancel");
            btnRun.IsEnabled = true;
        }
        */
        private async void listBox_types_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listBox_types.Items.Count > 0)
                {
                    listBox_objects.Items.Clear();
                    string type = listBox_types.SelectedItem.ToString();
                    string result = await client.GetStringAsync("https://localhost:44394/results/types/" + type);
                    var objects = JsonConvert.DeserializeObject<IEnumerable<byte[]>>(result);
                    foreach (var obj in objects)
                        listBox_objects.Items.Add(new { Image = obj });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                listBox_objects.Items.Clear();
                string type = listBox_types.SelectedItem.ToString();
                var result = await client.DeleteAsync("https://localhost:44394/results/types/" + type);
                resultCollection.Clear();

                string resType = await client.GetStringAsync("https://localhost:44394/results/types");
                var allbooks = JsonConvert.DeserializeObject<IEnumerable<string>>(resType);
                foreach (var b in allbooks)
                    resultCollection.Add(b);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
