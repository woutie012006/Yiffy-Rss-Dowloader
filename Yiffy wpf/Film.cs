using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yiffy_wpf
{
    class Film
    {
        string naam;
        string categorien;
        string magnet;
        string summarry;
        double rating;
        BitmapImage image;

        public Film(string naam, string categorien, string magnet, string summarry, double rating, BitmapImage image) 
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
        public string Categorien
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
