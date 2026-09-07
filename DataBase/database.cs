using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;


class GetData
{
    //WinBoard\Assets\SingleunitData.db

    private const string WIDtable = "WinTable";

    public static readonly string GetPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "SingleunitData.db");
    private static readonly string SQLpath = $@"Data Source={GetPath}";

    public static readonly string GetPinTablePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "ClipPinLists.db");

    private const string PinListstable = "PinListsTable";
    private const string PinListsPicturetable = "PicPinList";
    private static readonly string SQLPinTablepath = $@"Data Source={GetPinTablePath}";
    private const string TrieTable = "WinTableTrie";


    static private readonly Lazy<List<string>> _PinList = new(() => GetPinListsItems(PinListstable, SQLPinTablepath));

    static public List<string> PinLists => _PinList.Value;

    static private readonly Lazy<List<BitmapSource>> _PinListPicture = new(() => GetPicturesData(PinListsPicturetable, SQLPinTablepath));

    static public List<BitmapSource> PicturesPinLists => _PinListPicture.Value;
    static private readonly Lazy<Dictionary<int, (string, long)>> _dic = new(() => GetWordUnit(WIDtable, SQLpath));
    static public Dictionary<int, (string, long)> diction => _dic.Value;

    static public readonly Lazy<Trie> _tr = new(() => GetTrieData(TrieTable, SQLpath));
    static public Trie tri => _tr.Value;


    static private readonly Lazy<Dictionary<string, int>> dicwordid = new(() => getwordid(diction));
    static public Dictionary<string, int> WordID = dicwordid.Value;

    static public Trie trie
    { 
        get { return _tr.Value; } 


        set { }
    }


    static  private  Dictionary<int , (string , long)> GetWordUnit(string NameOfTable , string path)
    {

        

        // NameOfTheTable = WinTable
        var WordUnitData = new Dictionary<int, (string, long)>();

        var WordDataTable = $"SELECT * FROM {NameOfTable}";

        try
        {
            using var connection = new SqliteConnection(path);
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
        System.Diagnostics.Debug.WriteLine("Main diction is Built");
        return WordUnitData;
    }

    static public Dictionary<string,int> getwordid(Dictionary<int, (string, long)> Dictioary)
    {
        Dictionary<string, int> dic = new();
        if (Dictioary != null)
        {
            foreach (var (key, (word, frq)) in Dictioary)
            {
                if (!dic.TryAdd(word, key))
                    Console.WriteLine($"duplicate word: {word}");
            }
        }
        return dic;
    }

    static public  Trie GetTrieData(string NameOfTable, string path)
    {
        //WinTableTrie
        try
        {
            using var connection = new SqliteConnection(path);
            connection.Open();
            using var cmd = new SqliteCommand($"SELECT Trie FROM {NameOfTable}", connection);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                using var blobStream = reader.GetStream(0);
                using var gzip = new GZipStream(blobStream, CompressionMode.Decompress);
                var trie = System.Text.Json.JsonSerializer.Deserialize<Trie>(gzip);
                NullifyEmptyChildren(trie);

                
                return trie;
            }
            reader.Close();
            //connection.Close();
            
        }
        catch(SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
        return new Trie();
    }
    static void NullifyEmptyChildren(Trie node)
    {
        if (node.Childern == null) return;

        if (node.Childern.Count == 0)
        {
            node.Childern = null;
            return;
        }

        foreach (var child in node.Childern.Values)
            NullifyEmptyChildren(child);
    }

    private static void PinListsIntoFile(List<string> Phrases , string PinListsPath)
    {




        try
        {

            using (var connection = new SqliteConnection(PinListsPath))
            {
                connection.Open();
                var Table = @"CREATE TABLE IF NOT EXISTS PinListsTable(Phrase TEXT NOT NULL) SQLITE_ENABLE_UPDATE_DELETE_LIMIT";


                using (var command = new SqliteCommand(Table, connection))
                {
                    command.ExecuteNonQuery();
                }


                var TableConnection = "INSERT OR IGNORE INTO PinListsTable(Phrase) VALUES (@Phrase) ";

                using (var command = new SqliteCommand(TableConnection, connection))
                {
                    foreach (var Phras in Phrases)
                    {
                        command.Parameters.AddWithValue("@Phrase", Phras);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
            }
        }

        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private static byte[]  GetImageBytes(BitmapSource bmp)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bmp));
        using var ms = new MemoryStream();
        encoder.Save(ms);
        return ms.ToArray();
    }
    public static void UpdatePinPictures(List<BitmapSource> Picture)
    {

        // Table name : PicPinList
        // prametares = Picture (it is blob)

        try
        {



            using (var connection = new SqliteConnection(SQLPinTablepath))
            {
                connection.Open();

                var Table = $@"CREATE TABLE IF NOT EXISTS {PinListsPicturetable}(Picture BLOB NOT NULL)";


                using (var command = new SqliteCommand(Table, connection))
                {
                    command.ExecuteNonQuery();
                }

                var DeleteItemTableConnectionPictures = $@"DELETE FROM {PinListsPicturetable}";

                using (var command = new SqliteCommand(DeleteItemTableConnectionPictures, connection))
                {

                    command.ExecuteNonQuery();
                }


                var TableConnection = $"INSERT OR IGNORE INTO {PinListsPicturetable}(Picture) VALUES (@Picture) ";



                    using (var command = new SqliteCommand(TableConnection, connection))
                    {
                        foreach (var pic in Picture)
                        {
                        //var Jsonstring = System.Text.Json.JsonSerializer.Serialize(pic);
                        byte[] jsonbyte = GetImageBytes(pic);
                        command.Parameters.AddWithValue("@Picture", jsonbyte);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                        }
                    }
                
            }
        }
        
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static public List<BitmapSource> GetPicturesData(string NameOfTable, string path)
    {
        // Table name : PicPinList

        List<BitmapSource> images = new();
      
        try
        {
            using var connection = new SqliteConnection(path);
            connection.Open();
            using var cmd = new SqliteCommand($"SELECT Picture FROM {PinListsPicturetable}", connection);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                using var blobStream = reader.GetStream(0);
                var decoder = new PngBitmapDecoder(blobStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                BitmapSource image = decoder.Frames[0];

                images.Add(image);
            }
            reader.Close();
            

        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
        return images;
    }

    public static void UpdatePinItemData(List<string> Phrases)
    {
        
        try
        {

            using (var connection = new SqliteConnection(SQLPinTablepath))
            {
                connection.Open();

                var DeleteItemTableConnection = $@"DELETE FROM {PinListstable}";

                using (var command = new SqliteCommand(DeleteItemTableConnection, connection))
                {

                    command.ExecuteNonQuery();
                }

                var AddNewItemConnection = "INSERT OR IGNORE INTO PinListsTable(Phrase) VALUES (@Phrase)";
                using (var command = new SqliteCommand(AddNewItemConnection, connection))
                {

                    foreach (var S in Phrases)
                    {
                        command.Parameters.AddWithValue("@Phrase", S);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }

            }
        }

        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static List<string> GetPinListsItems(string TableName , string PinListsPath)
    {
        var PinLists = new List<string>();

        var Table = $"SELECT * FROM {TableName}";

        try
        {
            using var connection = new SqliteConnection(PinListsPath);
            connection.Open();

            using var comman = new SqliteCommand(Table, connection);

            using var Reader = comman.ExecuteReader();
            if (Reader.HasRows)
            {
                while (Reader.Read())
                {

                    var Item = Reader.GetString(0);

                    PinLists.Add(Item);


                }
                Reader.Close();

            }

        }

        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
        return PinLists;
    }



}

public class  WordRepository
{

    private const string WIDtable = "WinTable";

    public static readonly string GetPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "SingleunitData.db");
    private static readonly string SQLpath = $@"Data Source={GetPath}";

    public static readonly string GetPinTablePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "ClipPinLists.db");


    private readonly SqliteConnection _connection;
    private readonly SqliteCommand _getWordIdCmd;
    private readonly SqliteCommand _getWordFromIdCmd;
    private readonly SqliteCommand _getFrqCmd;


    public WordRepository()
    {
        // 1. Keep a single connection open
        _connection = new SqliteConnection(SQLpath);
        _connection.Open();

        // 2. Enable WAL mode for high-speed concurrent disk reads
        using (var pragmaCmd = _connection.CreateCommand())
        {
            pragmaCmd.CommandText = "PRAGMA journal_mode = WAL; PRAGMA synchronous = NORMAL;";
            pragmaCmd.ExecuteNonQuery();
        }

        // 3. Pre-compile commands once
        _getWordIdCmd = new SqliteCommand($"SELECT SingleWordID FROM {WIDtable} WHERE SingleWord = @word COLLATE NOCASE LIMIT 1", _connection);
        _getWordIdCmd.Parameters.Add("@word", SqliteType.Text);
        _getWordIdCmd.Prepare();

        _getWordFromIdCmd = new SqliteCommand($"SELECT SingleWord FROM {WIDtable} WHERE SingleWordID = @ID LIMIT 1", _connection);
        _getWordFromIdCmd.Parameters.Add("@ID", SqliteType.Integer);
        _getWordFromIdCmd.Prepare();

        _getFrqCmd = new SqliteCommand($"SELECT SingleWordFrq FROM {WIDtable} WHERE SingleWord = @word LIMIT 1", _connection);
        _getFrqCmd.Parameters.Add("@word", SqliteType.Text);
        _getFrqCmd.Prepare();
    }
    public long GetWordFrq(string word)
    {
        if (string.IsNullOrEmpty(word)) return 1;

        try
        {
            long X = 1;
            _getFrqCmd.Parameters["@word"].Value = word;
            var result = _getFrqCmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                X = Convert.ToInt64(result);
                return X;
            }
            else return 1;

        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }
    }

    public int GetWordID(string word)
    {
        if (string.IsNullOrEmpty(word)) return 1;


        try
        {
            var X = 0;
            _getWordIdCmd.Parameters["@word"].Value = word;
            var result = _getWordIdCmd.ExecuteScalar();
            if (result != null && result !=DBNull.Value)
            {
                X = Convert.ToInt32(result);
                return X;
            }
            else return 1;

        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }
    }

    public string GetWordFromId(int ID)
    {
        try
        {
            _getWordFromIdCmd.Parameters["@ID"].Value = ID;
            var result = _getWordFromIdCmd.ExecuteScalar();
            var X = "";
            if (result != null && result != DBNull.Value)
            {
                X = Convert.ToString(result);
                return X;
            }
            else return "";

        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
            return "";
        }
    }

}


