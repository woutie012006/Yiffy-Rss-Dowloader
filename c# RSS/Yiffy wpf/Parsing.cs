using System;
using System.Collections.Generic;
using System.Linq;

namespace Yiffy_wpf
{
    public static class Parsing
    {
       
        public static List<string> ParseGenre(string summary)
        {

            summary = summary.Remove(0, summary.IndexOf("Genre: "));
            summary = summary.Remove(summary.IndexOf('<', 0));
            summary = summary.Remove(0, 7);
            string[] genre = summary.Split('|');
            List<string> LGenre = new List<string>();


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
                LGenre.Add(genre[i]);
            }

            return LGenre;
        }
        public static string ParseTitle()
        {
            return null;
        }
        public static string ParseSummary(string summary)
        {
            string[] _summarryA = summary.Split('>');
            summary = _summarryA[8];
            summary = summary.Replace("<p>", "");
            summary = summary.Replace("</p", "");

            return summary;
        }
        public static double ParseImdb(string summmary)
        {
            string _imdb = summmary;
            _imdb = _imdb.Remove(0, _imdb.IndexOf("IMDB"));
            _imdb = _imdb.Remove(_imdb.IndexOf("/"));
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            _imdb = _imdb.Remove(0, _imdb.IndexOfAny(numbers));
            double _imdbD = Convert.ToDouble(_imdb);
            return _imdbD;
        }
        
     


    }
}
