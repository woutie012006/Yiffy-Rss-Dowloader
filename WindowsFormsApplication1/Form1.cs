using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Collections.ObjectModel;
using ProftaakOefening;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Resources;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        List<Film> film = new List<Film>();
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = Properties.Resources.loading;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            //this.Opacity = 0.5;
        }

        private void startup()
        {

            string url = "https://yts.re/rss/0/All/All/0";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

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
                _summarry = _summarryA[10];
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


                film.Add(new Film(item.Title.Text, first, _magnet, _summarry, _imdbD));

                listBox1.Items.Add(item.Title.Text);

            }

            //this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            //listBox1.SelectedItem = 0;

            string k = listBox1.Items[0].ToString();
            pictureBox1.Image = getImage(k);
            richTextBox1.Clear();

            string _summarry2 = "";
            string _category = "";
            string _imdb2 = "";
            foreach (Film f in film)
            {
                if (f.Naam == listBox1.Items[0].ToString())
                {
                    _summarry2 = f.Summarry;
                    _category = f.Categorien;
                    _imdb2 = f.Rating.ToString();
                }
            }
            richTextBox1.Text = _summarry2;
            label1.Text = _category;
            char[] p = _imdb2.ToCharArray();

            label2.Text = p[0] + "." + p[1];


        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //startup();
            this.backgroundWorker1.RunWorkerAsync();
            timer1.Enabled = false;
        }


        private Image getImage(string _name)
        {
            Image _image = null;

            _name = _name.Replace("'", "");
            _name = _name.Replace(" ", "_");
            _name = _name.Replace("(", "");
            _name = _name.Replace(")", "");


            try
            {
                string Url = @"https://static.yts.re/attachments/" + _name + @"/poster_large.jpg";

                WebRequest req = WebRequest.Create(Url);
                WebResponse response = req.GetResponse();
                Stream stream = response.GetResponseStream();

                _image = Image.FromStream(stream);
                stream.Close();
            }
            catch (Exception) { }


            return _image;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible == true)
                this.Visible = false;
            else
                this.Visible = true;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // MessageBox.Show( listBox1.SelectedItem.ToString());
            string k = listBox1.SelectedItem.ToString();
            pictureBox1.Image = getImage(k);
            richTextBox1.Clear();

            string _summarry = "";
            string _category = "";
            string _imdb = "";
            foreach (Film f in film)
            {
                if (f.Naam == listBox1.SelectedItem.ToString())
                {
                    _summarry = f.Summarry;
                    _category = f.Categorien;
                    _imdb = f.Rating.ToString();
                }
            }
            richTextBox1.Text = _summarry;
            label1.Text = _category;

            char[] p = _imdb.ToCharArray();
            label2.Text = p[0] + "." + p[1];
            
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            string _torrent = "";

            foreach (Film f in film)
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            startup();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string _link = "https://yts.re/movie/" + listBox1.SelectedItem.ToString().Replace(" ", "_").Replace("(", "").Replace(")", "");

            System.Diagnostics.Process.Start(_link);
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.Opacity = 0.5;

        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }
    }
}
