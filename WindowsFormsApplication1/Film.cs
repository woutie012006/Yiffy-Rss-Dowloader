using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class Film
    {
        string naam;
        string categorien;
        string magnet;
        string summarry;
        double rating;

        public Film(string naam, string categorien, string magnet, string summarry, double rating) 
        {
            this.naam = naam;
            this.categorien = categorien;
            this.magnet = magnet;
            this.summarry = summarry;
            this.rating = rating;

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
    }
}
