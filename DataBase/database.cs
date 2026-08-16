using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;


class GetData
{
    //WinBoard\Assets\SingleunitData.db
    
    private const string WIDtable = "WinTable";

    public static readonly string GetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "SingleunitData.db");
    private static readonly string SQLpath = $@"Data Source={GetPath}";
    private const string TrieTable = "WinTableTrie";


    static private readonly Lazy<Dictionary<int, (string, long)>> _dic = new(() => GetWordUnit(WIDtable, SQLpath));
    static public Dictionary<int, (string, long)> diction => _dic.Value;

    static private readonly Lazy<Trie> _tr = new(() => GetTrieData(TrieTable, SQLpath));
    static public Trie trie => _tr.Value;


    static private readonly Lazy<Dictionary<string, int>> dicwordid = new(() => getwordid(diction));
    static  public Dictionary<string, int> WordID => dicwordid.Value;


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
                //connection.Close();
            }

        }

        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
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



}

public class  WordRepository
{

    private const string WIDtable = "WinTable";
    private const string SQLpath = @"Data Source=C:\Users\FadiSK\source\repos\files\SingleunitData.db";


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


