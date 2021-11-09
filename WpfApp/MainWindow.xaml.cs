using System;
using System.Collections;
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
    /// 

    abstract public class Type
    {
        public string Info { get; set; }

        public Type (string info)
        {
            Info = info;
        }
    }

    public class Results : Type, IEnumerable<string>
    {
        //public string type { get; set; }
        public List<string> images { get; set; }

        public Results(string newType, string newImage) : base(newType)
        {
            Info = newType;
            images = new List<string>();
            images.Add(newImage);
        }

        public override string ToString()
        {
            return Info;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)images).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)images).GetEnumerator();
        }
    }

    /*
    public class ResultCollection
    {
        //Results res;
        public static ObservableCollection<Results> res = new ObservableCollection<Results>();
        
        public void Add (string type, string imageName)
        {
            //bool flag = true;
            foreach (Results r in res)
            {
                if (r.type == type)
                {
                    r.images.Add(imageName);
                    //flag = false;
                    return;
                }
            }
            res.Add(new Results(type, imageName));

            //res.IndexOf()
        }

    }*/


    public partial class MainWindow : Window
    {
        //string ImageFolder;

        public static ObservableCollection<Results> Types = new ObservableCollection<Results>();

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
                foreach (Results r in Types)
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
                    Types.Add(new Results(type, image));
                }
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
