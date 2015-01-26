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

        public Film(string naam, string categorien, string magnet, string summarry) 
        {
            this.naam = naam;
            this.categorien = categorien;
            this.magnet = magnet;
            this.summarry = summarry;

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
    }
}
