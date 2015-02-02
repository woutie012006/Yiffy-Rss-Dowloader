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
using System.Xml;

using System.ServiceModel.Syndication;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Resources;

using System.Windows.Forms;

namespace Yiffy_wpf
{
    public partial class MainWindow : Window
    {
        List<Film> films;

        private System.Windows.Forms.NotifyIcon notifyIcon1 = new NotifyIcon();
        private System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();
        private System.ComponentModel.BackgroundWorker backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        private System.Windows.Threading.DispatcherTimer timer2 = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            films =  new List<Film>();
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height;

            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer1.Start();

            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Interval = new TimeSpan(1, 0, 0);
            timer2.Start();

            //backgroundWorker1.DoWork += backgroundWorker1_DoWork;

            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            notifyIcon1.Icon = Yiffy_wpf.Properties.Resources.favicon;
            notifyIcon1.Visible = true;
            startup();
        }
        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
                this.Visibility = Visibility.Hidden;
            else
                this.Visibility = Visibility.Visible;
        }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            startup();
        }
        private void startup()
        {

            string url = "https://yts.re/rss/0/All/All/0";
            //try
            //{
                XmlReader reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();

                //this.Visibility = System.Windows.Visibility.Visible;
                foreach (SyndicationItem item in feed.Items)
                {


                    Database.Query = "select count(*) from film where naam=\"" + item.Title.Text + "\"";
                    Database.OpenConnection();
                    int _inIt = Convert.ToInt32(Database.Command.ExecuteScalar());
                    Database.CloseConnection();
                    string _magnet = "";
                    if (_inIt == 0)
                    {


                        foreach (var link in item.Links)
                        {
                            _magnet = link.Uri.ToString();
                        }
                        string title = item.Title.Text.Replace("'", "");
                        Database.Query = "INSERT INTO film (naam, datum, magnet) values ('" + title + "', '" + DateTime.Now.ToString("dd/mm/yyyy") + "', '" + _magnet + "')";
                        Database.OpenConnection();
                        Database.Command.ExecuteNonQuery();

                        Database.CloseConnection();

                    }

                    string first = item.Summary.Text;
                    first = first.Remove(0, first.IndexOf("Genre: "));
                    first = first.Remove(first.IndexOf('<', 0));
                    first = first.Remove(0, 7);
                    string[] genre = first.Split('|');
                    List<string> bla = new List<string>();


                    for (int i = 0; i < genre.Length; i++)
                    {
                        if (genre[i].Contains(' ') && genre[i].IndexOf(' ') != 0)
                        {
                            genre[i] = genre[i].Remove(genre[i].IndexOf(' '));

                        }
                        else if (genre[i].Contains(' '))
                        {
                            genre[i] = genre[i].Remove(0, 1);
                        }
                        bla.Add(genre[i]);
                    }



                    //summary parsing
                    string _summarry = item.Summary.Text;
                    string[] _summarryA = _summarry.Split('>');
                    _summarry = _summarryA[8];
                    _summarry = _summarry.Replace("<p>", "");
                    _summarry = _summarry.Replace("</p", "");

                    //imdb rating parsing
                    string _imdb = item.Summary.Text;//.Replace(" ", ";").Replace("/",";");
                    _imdb = _imdb.Remove(0, _imdb.IndexOf("IMDB"));
                    _imdb = _imdb.Remove(_imdb.IndexOf("/"));
                    char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
                    _imdb = _imdb.Remove(0, _imdb.IndexOfAny(numbers));
                    double _imdbD = Convert.ToDouble(_imdb);

                    foreach (var link in item.Links)
                    {
                        _magnet = link.Uri.ToString();
                    }

                    BitmapImage _image= getImage(item.Title.Text);

                    Dispatcher.Invoke(new Action(() => { listBox1.Items.Add(item.Title.Text); films.Add(new Film(item.Title.Text, first, _magnet, _summarry, _imdbD, _image)); }));

                }

                //this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
                //listBox1.SelectedItem = 0;

                string k = listBox1.Items[0].ToString();


                string _summarry2 = films[0].Summarry;
                string _category = films[0].Categorien;
                string _imdb2 = films[0].Rating.ToString();
                BitmapImage _image2 = films[0].Image;


                //Dispatcher.Invoke(new Action(() =>
                //{
                    
                    richTextBox1.Document.Blocks.Clear();
                    richTextBox1.AppendText(_summarry2);
                    label1.Content = _category;
                    char[] p = _imdb2.ToCharArray();

                    label2.Content = p[0] + "." + p[1];
                    this.Visibility = Visibility.Visible;
                    pictureBox1.Source = _image2;//getImage(films[0].Naam);

                //}));

            //}
            //catch (Exception)
            //{
            //    System.Windows.Forms.MessageBox.Show("no connection could be made. Possible reasons for this might be : \n\n-your internet connection is down.\n-the yify website is down.\n-the rss feed is being renewed\n\nPlease restart the application later.");

            //    Dispatcher.Invoke(new Action(() =>
            //    {
            //        System.Windows.Application.Current.Shutdown();
            //    }));
            //}
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            this.backgroundWorker1.RunWorkerAsync();
            timer1.IsEnabled = false;
        }
        private BitmapImage getImage(string _name)
        {
            //Image _image = null;

            _name = _name.Replace("'", "");
            _name = _name.Replace(" ", "_");
            _name = _name.Replace("(", "");
            _name = _name.Replace(")", "");
            _name = _name.Replace(":", "").Replace(";", "").Replace("_1080p", "").Replace(".", "").Replace(",", "");
            //_name = _name.ToLower();

            BitmapImage bitImage = new BitmapImage();


            string Url = @"https://s.ynet.io/assets/images/movies/" + _name + @"/medium-cover.jpg";

            HttpWebResponse response = null;
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "HEAD";


            try
            {
                response = (HttpWebResponse)request.GetResponse();
                bitImage.BeginInit();
                bitImage.UriSource = new Uri(Url);
                bitImage.EndInit();
            }
            catch (WebException)
            {
                
                Url = Url.ToLower();
                bitImage.BeginInit();
                bitImage.UriSource = new Uri(Url);
                bitImage.EndInit();
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                }
            }



            return bitImage;
        }
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string k = listBox1.SelectedItem.ToString();


            string _summarry = "";
            string _category = "";
            string _imdb = "";
            BitmapImage _image = null;

            Film ff = films.Find(delegate(Film f) { return f.Naam == k; });

            _summarry = ff.Summarry;
            _category = ff.Categorien;
            _imdb = ff.Rating.ToString();
            _image = ff.Image;

            richTextBox1.Document.Blocks.Clear();
            richTextBox1.AppendText(_summarry);
            label1.Content = _category;

            char[] p = _imdb.ToCharArray();
            label2.Content = p[0] + "." + p[1];
            pictureBox1.Source = _image;

            
        }
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string _torrent = "";

                foreach (Film f in films)
                {
                    if (f.Naam == listBox1.SelectedItem.ToString())
                    {
                        _torrent = f.Magnet;
                    }
                }
                using (WebClient Client = new WebClient())
                {
                    Client.DownloadFile(_torrent, "a.torrent");
                }
                System.Diagnostics.Process.Start("a.torrent");
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            string url = "https://yts.re/rss/0/All/All/0";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            if (feed.Items.First().Title.Text == listBox1.Items[0].ToString())
                startup();


        }
        private void listBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox1.SelectedItem == null) { }
            else
            {
                string _link = "https://yts.re/movie/" + listBox1.SelectedItem.ToString().Replace(" ", "_").Replace("(", "").Replace(")", "");
                System.Diagnostics.Process.Start(_link);
            }
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notifyIcon1.Dispose();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            
        }
    }
}
