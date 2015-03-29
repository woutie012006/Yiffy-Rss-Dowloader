using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Yiffy_wpf
{
    class Database
    {

        // De bestandsnaam voor de database
        private static readonly string databaseFilename = "Database.sqlite";
        // Variabele voor het opzetten van de verbinding
        private static SQLiteConnection connection;
        // Variable waar de SQL-commandos tijdelijk in opgeslagen worden
        private static SQLiteCommand command;

        /// <summary>
        /// Stel de SQL query in die uitgevoerd moet gaan worden.
        /// </summary>
        public static string Query
        {
            set
            {
                // Zorg ervoor dat er een verbinding gemaakt kan worden
                PrepareConnection();
                // Stel het SQL commando in met de gegeven query
                command = new SQLiteCommand(value, connection);
            }
        }

        /// <summary>
        /// Haalt het command-object op waarmee queries uitgevoerd kunnen worden.
        /// </summary>
        public static SQLiteCommand Command
        {
            get { return command; }
        }

        /// <summary>
        /// Haal de bestandsnaam op van de database.
        /// </summary>
        public static string DatabaseFilename
        {
            get { return databaseFilename; }
        }

        /// <summary>
        /// Open de verbinding met de database
        /// </summary>
        public static void OpenConnection()
        {
            // Controleer of de verbinding niet al open is
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        /// <summary>
        /// Sluit de verbinding met de database
        /// </summary>
        public static void CloseConnection()
        {
            // Controleer of de verbinding niet al gesloten is
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Controleert of de database al bestaat. Zo niet, wordt deze aangemaakt
        /// en gevuld met wat dummy data. Daarnaast wordt altijd de connectie opgezet
        /// met de database indien deze nog niet opgezet was.
        /// </summary>
        private static void PrepareConnection()
        {
            // Controleer of we een nieuwe database met dummy data moeten aanmaken
            bool createNew = !File.Exists("Database.sqlite");

            // Bestand bestaat niet: maak een lege database aan
            if (createNew)
            {
                SQLiteConnection.CreateFile(databaseFilename);
            }

            // Zet een verbinding op met de database
            if (connection == null)
            {
                connection = new SQLiteConnection("Data Source=" + databaseFilename + ";Version=3");
            }

            // Als we een nieuwe database gemaakt hebben, voegen we alvast wat records toe.
            // We doen dit nu pas omdat we een connection nodig hebben om te communiceren met
            // de database: vandaar dat deze code niet boven bij de CreateFile functie staat.
            if (createNew)
            {
                CreateDummyData();
            }
        }

        /// <summary>
        /// Functie die een nieuwe tabel aanmaakt op een lege database, en deze vult met een
        /// aantal records.
        /// </summary>
        private static void CreateDummyData()
        {
            OpenConnection();

            try
            {
                // Nieuwe tabel met drie kolommen: number (INT), name (VARCHAR), email (VARCHAR)
                Query = "CREATE TABLE film (titel Text , categorie Text, magnet Text, summarry Text, imdb Text, datum Text, image Text)";
                Command.ExecuteNonQuery();


            }
            catch (SQLiteException)
            {
                // Er is iets mis gegaan: waarschijnlijk bestond de tabel al. Voor nu is er
                // verder geen foutafhandeling nodig.
            }

            CloseConnection();
        }
    }
}
