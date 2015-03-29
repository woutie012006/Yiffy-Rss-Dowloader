using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Yiffy_wpf
{
    public class Film
    {
        string naam;
        List<string> categorien;
        string magnet;
        string summarry;
        double rating;
        BitmapImage image;

        public Film(string naam, List<string> categorien, string magnet, string summarry, double rating, BitmapImage image) 
        {
            this.naam = naam;
            this.categorien = categorien;
            this.magnet = magnet;
            this.summarry = summarry;
            this.rating = rating;
            this.image = image;

        }
        public string Naam
        {
            get{ return naam;}
        }
        public List<string> Categorien
        {
            get{ return categorien;}
        }
        public string Summarry
        {
            get{ return summarry;}
        }
        public string Magnet
        {
            get { return magnet; }
        }
         public double Rating
        {
            get{ return rating;}
        }
         public BitmapImage Image
         {
             get { return image; }
         }
    }
}
