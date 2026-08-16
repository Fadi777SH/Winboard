using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
class Database
{

    static public Dictionary<string,int> diction = GetData.WordID;

    static public string c = GetData.GetPath;
    public const string Mainsqlitepath = @"Data Source=C:\Users\FadiSK\source\repos\files\SingleunitData.db";
    private class WordUnitData
    {
        public void SubMitNewWordUnit(Dictionary<int, Dictionary<string, long>> NewWord)
        {

            var Connection = new SqliteConnection(@"Data Source=C:\Users\FadiSK\source\repos\files\SingleunitData.db");
            Connection.Open();

            var Table = @"INSERT OR IGNORE INTO WinTable(SingleWord, SingleWordID, SingleWordFrq) VALUES (@SingleWord, @SingleWordID, @SingleWordFrq)";
            var command = new SqliteCommand(Table, Connection);
            foreach (var (ID, Dictionary) in NewWord)
            {
                foreach (var (Word, Frq) in Dictionary)
                {
                    command.Parameters.AddWithValue("@SingleWord", Word);
                    command.Parameters.AddWithValue("@SingleWordID", ID);
                    command.Parameters.AddWithValue("@SingleWordFrq", Frq);

                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
        }
        public void Run(Dictionary<string, long> WordIdDictionary)
        {
            IntoFile(WordIdDictionary);
        }
        public static Dictionary<int, (string, long)> GetWordUnitData(string SourceFile)
        {
            return GetDataFromFile(SourceFile);
        }

        private Dictionary<int, Dictionary<string, long>> GetAndMakeID(Dictionary<string, long> WordFrq)
        {


            var returnDict = new Dictionary<int, Dictionary<string, long>>();
            int ID = 1;

            foreach (var (word, frq) in WordFrq)
            {
                returnDict[ID++] = new Dictionary<string, long> { { word, frq } };
            }

            return returnDict;

        }
        private void IntoFile(Dictionary<string, long> WordIdDictionary)
        {

            var InsertThisDictionary = GetAndMakeID(WordIdDictionary);


            try
            {

                using (var connection = new SqliteConnection(@"Data Source=C:\Users\FadiSK\source\repos\files\SingleunitData.db"))
                {
                    connection.Open();
                    var Table = @"CREATE TABLE IF NOT EXISTS WinTable(SingleWord TEXT NOT NULL , SingleWordID INTEGER PRIMARY KEY , SingleWordFrq INTEGER NOT NULL)";


                    using (var command = new SqliteCommand(Table, connection))
                    {
                        command.ExecuteNonQuery();
                    }


                    var TableConnection = "INSERT OR IGNORE INTO WinTable(SingleWord, SingleWordID, SingleWordFrq) VALUES (@SingleWord, @SingleWordID, @SingleWordFrq) ";

                    using (var command = new SqliteCommand(TableConnection, connection))
                    {
                        foreach (var (ID, Dictionary) in InsertThisDictionary)
                        {
                            foreach (var (Word, Frq) in Dictionary)
                            {
                                command.Parameters.AddWithValue("@SingleWord", Word);
                                command.Parameters.AddWithValue("@SingleWordID", ID);
                                command.Parameters.AddWithValue("@SingleWordFrq", Frq);

                                command.ExecuteNonQuery();
                                command.Parameters.Clear();
                            }
                        }
                    }
                }
            }

            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static Dictionary<int, (string, long)> GetDataFromFile(string Sourcefile)
        {

            var WordUnitData = new Dictionary<int, (string, long)>();

            var WordDataTable = "SELECT * FROM WinTable";

            try
            {
                using var connection = new SqliteConnection(@"Data Source=C:\Users\FadiSK\source\repos\files\SingleunitData.db");
                connection.Open();

                using var comman = new SqliteCommand(WordDataTable, connection);

                using var Reader = comman.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {

                        var SingleWord = Reader.GetString(0);
                        var SingleWordID = Reader.GetInt32(1);
                        var SingleWordFrq = Reader.GetString(2);

                        WordUnitData[SingleWordID] = (SingleWord, long.Parse(SingleWordFrq));

                    }
                    Reader.Close();
                }

            }

            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return WordUnitData;
        }

    }


    public class AccessWordUnitData
    {

        private Dictionary<string, int> wordID(Dictionary<int, (string, long)> data)
        {
            var buff = new Dictionary<string, int>();
            foreach (var (ID, wordFrq) in data)
            {
                buff[wordFrq.Item1] = ID;
            }
            return buff;
        }

        private Dictionary<int, string> CreateIDWORD(Dictionary<string, int> WORDID)
        {
            return WORDID.Select(F => (F.Value, F.Key)).ToDictionary(f => f.Value, f => f.Key);

        }



        public void DeleteTable(string TableName)
        {
            using var Connection = new SqliteConnection(@"Data Source=C:\Users\FadiSK\source\repos\files\SingleunitData.db");
            Connection.Open();
            using var command = Connection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {TableName};";
            command.ExecuteNonQuery();


        }

        public void Vacum(string Path)
        {
            using var conn = new SqliteConnection(Path);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "VACUUM;";
            cmd.ExecuteNonQuery();
        }
    }

    private class JSON
    {

        public string SerializeData(Trie trie)
        {

            string JsonString = System.Text.Json.JsonSerializer.Serialize(trie);
            return JsonString;

        }
        public string SerializeData(PreFix.PreFixTrie trie)
        {

            string JsonString = System.Text.Json.JsonSerializer.Serialize(trie);
            return JsonString;

        }
        public Trie DeSerializedata(string filename)
        {


            var trie = System.Text.Json.JsonSerializer.Deserialize<Trie>(filename);

            return trie;
        }
        public byte[] CompressJsonData(string Jsontrie)
        {
            byte[] CompressedData = Encoding.UTF8.GetBytes(Jsontrie);
            using var memoryStream = new MemoryStream();
            using (var Gzip = new GZipStream(memoryStream, CompressionLevel.Optimal))
            {
                Gzip.Write(CompressedData, 0, CompressedData.Length);
            }
            return memoryStream.ToArray();
        }
        public string DeComressJsonData(byte[] TrieBytes)
        {
            using var input = new MemoryStream(TrieBytes);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }

    private class NgramTRIE
    {
        JSON json = new();

        private const string WIDtable = "WinTable";
        private const string SQLpath = @"Data Source=C:\Users\FadiSK\source\repos\files\SingleunitData.db";
        private const string TrieTable = "WinTableTrie";

        private Trie ReConstructJson(string JsonFormat)
        {
            return json.DeSerializedata(JsonFormat);
        }
        public void InserFullTrieData(Trie trie, string path , string TableName)
        {
            var JsonString = json.SerializeData(trie);
            var JsonBytes = json.CompressJsonData(JsonString);

            try
            {

                using (var connection = new SqliteConnection(path))
                {
                    connection.Open();
                    var Table = $@"CREATE TABLE IF NOT EXISTS {TableName}(Trie BLOB NOT NULL)";


                    using (var command = new SqliteCommand(Table, connection))
                    {
                        command.ExecuteNonQuery();
                    }


                    var TableConnection = $"INSERT OR IGNORE INTO {TableName}(Trie) VALUES (@Trie) ";

                    using (var command = new SqliteCommand(TableConnection, connection))
                    {
                        command.Parameters.AddWithValue("@Trie", JsonBytes);
                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Trie GetTrie(string TableName, string Path)
        {
            //WinTableTrie

            using var connection = new SqliteConnection($"Data Source={Path}");
            connection.Open();
            using var cmd = new SqliteCommand($"SELECT Trie FROM {TableName}", connection);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                using var blobStream = reader.GetStream(0);
                using var gzip = new GZipStream(blobStream, CompressionMode.Decompress);
                return System.Text.Json.JsonSerializer.Deserialize<Trie>(gzip);
            }
            return new Trie();

        }

        public void UpdataTrie(Trie NewTrie, string path, string NameOfTheTable)
        {
            using var connection = new SqliteConnection(path);
            connection.Open();
            var JsonString = json.SerializeData(NewTrie);
            var JsonBytes = json.CompressJsonData(JsonString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $@"UPDATE WinTableTrie SET Trie = @Trie";
            cmd.Parameters.Add("@Trie", SqliteType.Blob).Value = JsonBytes;
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }


        void Traverse(Trie node, ref int  tot , ref int zz)
        {
            tot++;
            if (node.Childern != null)
            {
                foreach (var kvp in node.Childern)
                {
                    if (kvp.Key == 0)
                        zz++;
                    Traverse(kvp.Value ,ref tot,ref zz);
                }
            }

            
        }

        public static PreFix.PreFixTrie Buildprefixtrie()
        {
            List<string> l = new();
            foreach(var s in diction)
            {
                l.Add(s.Key);
            }

            
            return PreFix.PreFixTrie.TrainEntry(l);
            
        }

        public void InsertPreFixIntoSQL(PreFix.PreFixTrie trie , string path , string TableName)
        {
            var JsonString = json.SerializeData(trie);
            var JsonBytes = json.CompressJsonData(JsonString);

            try
            {

                using (var connection = new SqliteConnection(path))
                {
                    connection.Open();
                    Console.WriteLine("DB path in use: " + connection.DataSource);
                    var Table = $@"CREATE TABLE IF NOT EXISTS {TableName}(PreFixTrie BLOB NOT NULL)";


                    using (var command = new SqliteCommand(Table, connection))
                    {
                        command.ExecuteNonQuery();
                    }


                    var TableConnection = $"INSERT OR IGNORE INTO {TableName}(PreFixTrie) VALUES (@PreFixTrie) ";

                    using (var command = new SqliteCommand(TableConnection, connection))
                    {
                        command.Parameters.AddWithValue("@PreFixTrie", JsonBytes);
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SqliteCommand("PRAGMA wal_checkpoint(FULL);", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        static void Mai(string[] arg)
        {
            Database.NgramTRIE x = new();
            NGramModel nGramModel = new();
            ParquetClass accessWordUnitData = new();
            Database.AccessWordUnitData w = new();
            //var node = GetData.trie;
            //var tot = 0;
            //var zz = 0;
            //x.Traverse(node, ref tot, ref zz);
            //Console.WriteLine(tot + " " + zz);

            //var trie = x.Buildprefixtrie();
            //x.InsertPreFixIntoSQL(trie, @"Data Source=C:\Users\FadiSK\source\repos\files\SingleunitData.db", "prefixtable");
            Console.WriteLine("done");

            
        }
    }


}

