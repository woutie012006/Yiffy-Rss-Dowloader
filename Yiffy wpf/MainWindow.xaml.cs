using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Yiffy_wpf
{
    public partial class MainWindow : Window
    {
        List<Film> films;

        private NotifyIcon notifyIcon1 = new NotifyIcon();
        private DispatcherTimer timer1 = new DispatcherTimer();
        private BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        private DispatcherTimer timer2 = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            films = new List<Film>();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;

            Left = SystemParameters.PrimaryScreenWidth - Width;
            Top = SystemParameters.PrimaryScreenHeight - Height;

            timer1.Tick += timer1_Tick;
            timer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer1.Start();

            timer2.Tick += timer2_Tick;
            timer2.Interval = new TimeSpan(1, 0, 0);
            timer2.Start();

            //backgroundWorker1.DoWork += backgroundWorker1_DoWork;

            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            notifyIcon1.Icon = Properties.Resources.favicon;
            notifyIcon1.Visible = true;
            startup();
        }
        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (Visibility == Visibility.Visible)
                Visibility = Visibility.Hidden;
            else
                Visibility = Visibility.Visible;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            startup();
        }
        private void startup()
        {

            const string url = "https://yts.re/rss/0/All/All/0";
            try
            {
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            
            foreach (SyndicationItem item in feed.Items)
            {
                string sum = item.Summary.Text;

                List<string> genre = Parsing.ParseGenre(sum);
                string summary = Parsing.ParseSummary(sum);
                double imdb = Parsing.ParseImdb(sum);
                
                string _magnet = item.Links[0].Uri.ToString();
                
                BitmapImage _image = getImage(item.Title.Text);


                #region toDatabase
                //checks if the film is already in the database
                Database.Query = "select count(*) from film where titel=\"" + item.Title.Text.Replace("'", "") + "\"";
                Database.OpenConnection();
                int _inIt = Convert.ToInt32(Database.Command.ExecuteScalar());
                Database.CloseConnection();

                //if not then 
                if (_inIt == 0)
                {


                    foreach (var link in item.Links)
                    {
                        _magnet = link.Uri.ToString();
                    }
                    string title = item.Title.Text.Replace("'", "");//might need image but still figuring out bitmapimage to base64
                    Database.Query = "INSERT INTO film (titel, categorie, magnet, summarry, imdb, datum, image) values ('" + title + "', '" + genre[0]
                                            + "', '" + _magnet + "', '" + summary.Replace("'", "") + "', '" 
                                            + imdb + "', '" + DateTime.Now.ToString("dd/mm/yyyy") + 
                                            "', '" +/* BitmapToBase64( _image)*/"k" + "')";

                    Database.OpenConnection();
                    Database.Command.ExecuteNonQuery();

                    Database.CloseConnection();

                }
                #endregion

                Dispatcher.Invoke(() => { listBox1.Items.Add(item.Title.Text); 
                                            films.Add(
                                                new Film(
                                                    item.Title.Text, genre, _magnet, summary, imdb, _image)); 
                });

            }
            //updating the UI
            string k = listBox1.Items[0].ToString();

                string _category = "";
                foreach (string genre in films[0].Categorien)
                {
                    _category += genre;
                }

            string _summarry2 = films[0].Summarry;
            string _imdb2 = films[0].Rating.ToString();
            BitmapImage _image2 = films[0].Image;


            richTextBox1.Document.Blocks.Clear();
            richTextBox1.AppendText(_summarry2);
            label1.Content = _category;
            char[] p = _imdb2.ToCharArray();

            label2.Content = p[0] + "." + p[1];
            Visibility = Visibility.Visible;
            pictureBox1.Source = _image2;



            //sets the banner of how great the quality of the movie is
            if (k.ToLower().Contains("1080p"))
                qualityImage.Source = new BitmapImage(new Uri("pack://application:,,,/Yiffy wpf;component/Resources/banner1080p.png", UriKind.Absolute));
            else if(k.ToLower().Contains("3d"))
                qualityImage.Source = new BitmapImage(new Uri("pack://application:,,,/Yiffy wpf;component/Resources/banner3D.png", UriKind.Absolute));
            else
                qualityImage.Source = new BitmapImage(new Uri("pack://application:,,,/Yiffy wpf;component/Resources/banner720p.png", UriKind.Absolute));


            }
            catch (Exception)
            {
                backupstartup();
                MessageBox.Show(@"no connection could be made. Possible reasons for this might be : \n
                  \n-your internet connection is down.
                  \n-the yify website is down.
                  \n-the rss feed is being renewed
                  \n
                  \nPlease restart the application later.");
            }

        }
        /// <summary>
        /// Only occurs when the first startup threw an exception, this is possible when no connection can be made to the yiffy website.
        /// This might be sensitive when the website uses a different format might have to change that.
        /// This gets the previously 20 loaded films from the sqlite database. Unfortunately, i've nor been able to store bitmapimages to the database yet.
        /// But it will soon be implemented. 
        /// </summary>
        private void backupstartup()
        {
            Database.Query = "SELECT titel , categorie , magnet , summarry , imdb , datum, image  FROM film";
            Database.OpenConnection();

            SQLiteDataReader reader = Database.Command.ExecuteReader();
            int limit = 0;
            while (reader.Read())
            {
                string _titel = Convert.ToString(reader["titel"]);
                List<string> _categorie  = new List<string>();
                _categorie[0]= Convert.ToString(reader["categorie"]);
                string _magnet = Convert.ToString(reader["magnet"]);
                string _summarry = Convert.ToString(reader["summarry"]);
                string _imdb = Convert.ToString(reader["imdb"]);
                string _datum = Convert.ToString(reader["datum"]);
                //BitmapImage _image =new Bitmap(1,1, new PixelFormat());// Base64ToBitmap(Convert.ToString(reader["image"]));/////////////////////////////////////////////

                listBox1.Items.Add(_titel);
                label1.Content = _categorie;
                label2.Content = _imdb;
                richTextBox1.Document.Blocks.Clear();
                richTextBox1.AppendText(_summarry);
                //pictureBox1.Source = _image;//////////////////////////////////////////////////////////////////////////////////////////

                films.Add(new Film(_titel, _categorie, _magnet, _summarry, Convert.ToDouble(_imdb), null));/////////////////////////null=_image
                if (limit >= 20) { break; }
                limit++;

            }
            Visibility = Visibility.Visible;
            Database.CloseConnection();


        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            backgroundWorker1.RunWorkerAsync();
            timer1.IsEnabled = false;
        }
        /// <summary>
        /// Gets the image from the yiffy website by first parsing the string and then getting the image.
        /// </summary>
        /// <param name="_name"></param>
        /// <returns>The image you need with the name _name</returns>
        private BitmapImage getImage(string _name)
        {
            //Image _image = null;
            _name = _name.Replace("&","");
            _name = _name.Replace("?", "");
            _name = _name.Replace("!", "");
            _name = _name.Replace(@"'", "").Replace("\"","");
            _name = _name.Replace(" ", "_");
            _name = _name.Replace("(", "");
            _name = _name.Replace(")", "");
            _name = _name.Replace(":", "").Replace(";", "").Replace("_1080p", "").Replace(".", "").Replace(",", "").Replace("-", "_").Replace(".", "");
            _name = _name.Replace("_3D", "");
            _name = _name.Replace("___", "_");
            _name = _name.Replace("__", "_");

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
                //if (Url.Contains("usa")) { System.Windows.Forms.MessageBox.Show(Url); }
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
        /// <summary>
        /// Handles the event that happens when a different movie is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selected = listBox1.SelectedItem.ToString();
            
            Film ff = films.Find(delegate(Film f) { return f.Naam == selected; });
            
            richTextBox1.Document.Blocks.Clear();
            richTextBox1.AppendText(ff.Summarry);
            label1.Content = ff.Categorien;

            char[] p = ff.Rating.ToString().ToCharArray();
            label2.Content = p[0] + "." + p[1];
            pictureBox1.Source = ff.Image;

            if (selected.ToLower().Contains("1080p"))
                qualityImage.Source = new BitmapImage(new Uri("pack://application:,,,/Yiffy wpf;component/Resources/banner1080p.png", UriKind.Absolute));
            else if (selected.ToLower().Contains("3d"))
                qualityImage.Source = new BitmapImage(new Uri("pack://application:,,,/Yiffy wpf;component/Resources/banner3D.png", UriKind.Absolute));
            else
                qualityImage.Source = new BitmapImage(new Uri("pack://application:,,,/Yiffy wpf;component/Resources/banner720p.png", UriKind.Absolute));

            
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
                Process.Start("a.torrent");
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            Opacity = 1;
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
                Process.Start(_link);
            }
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            Opacity = 0.5;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            notifyIcon1.Dispose();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }
        
        #region tried bimap to bitmpaimageconverter, did NOT work -.-
        //public string BitmapToBase64(BitmapImage bi)
        //{
        //    #region MyRegion
        //    //MemoryStream ms = new MemoryStream();
        //    //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
        //    //encoder.Frames.Add(BitmapFrame.Create(bi));
        //    //encoder.Save(ms);
        //    //byte[] bitmapdata = ms.ToArray();

        //    //return Convert.ToBase64String(bitmapdata); 
        //    #endregion
        //    MemoryStream ms = new MemoryStream();


        //    return null;

        //}
        //public BitmapImage Base64ToBitmap(string b64)
        //{

        //    byte[] binaryData = Convert.FromBase64String(b64);

        //    BitmapImage bi = new BitmapImage();
        //    bi.BeginInit();
        //    bi.StreamSource = new MemoryStream(binaryData);
        //    bi.EndInit();



        //    return bi;

        //}
        //private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        //{
        //    // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        //        enc.Save(outStream);
        //        System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

        //        return new Bitmap(bitmap);
        //    }
        //}

        //[System.Runtime.InteropServices.DllImport("gdi32.dll")]
        //public static extern bool DeleteObject(IntPtr hObject);


        //private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        //{
        //    //IntPtr hBitmap = bitmap.GetHbitmap();
        //    //BitmapImage retval;

        //    //try
        //    //{
        //    //    retval = Imaging.CreateBitmapSourceFromHBitmap(
        //    //                 hBitmap,
        //    //                 IntPtr.Zero,
        //    //                 Int32Rect.Empty,
        //    //                 BitmapSizeOptions.FromEmptyOptions());
        //    //}
        //    //finally
        //    //{
        //    //    DeleteObject(hBitmap);
        //    //}

        //    //return retval;



        //    Bitmap value = new Bitmap(null,null);
        //    MemoryStream ms = new MemoryStream();
        //    ((System.Drawing.Bitmap)value).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        //    BitmapImage image = new BitmapImage();
        //    image.BeginInit();
        //    ms.Seek(0, SeekOrigin.Begin);
        //    image.StreamSource = ms;
        //    image.EndInit();

        //    return image;



        //}
        #endregion
    }


}
