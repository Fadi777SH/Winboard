
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;

public class Trie
{

    public int Count { get; set; }
    public Dictionary<int, Trie> Childern { get; set; } = null;

    public Trie(int count = 0)
    {
        Count = count;
    }
    public void AddChild(int key, Trie child)
    {
        Childern ??= new Dictionary<int, Trie>();
        Childern[key] = child;
    }
}
public  class NGramModel

{
    public static Trie _trie = GetData.trie;
    Dictionary<string, int> wordid = GetData.WordID;


    public List<int> tokenize(string Context)
    {

    var context = Context.ToLower()
            .Split([' ', '%', '\"', '|', '{', '}', '\'', '\\', '?', ',', '<', '>', '?', '@', '#', '~', '_', '!', '.'], StringSplitOptions.RemoveEmptyEntries)
            .Where(s => s.Length > 0)
            .Select(F => wordid.TryGetValue(F, out int id) ? id : 0)
            .Where(id => id != 0)
            .ToList();

        return context;
    }
    public static List<List<int>> Gram(List<int> List, int n)

    {

        var Gram = new List<List<int>>();
        for (int i = 0; i <= List.Count - n; i++)

            Gram.Add(List.GetRange(i, n));
        return Gram;

    }

    public static List<(int, float)> Topk(Trie trie, List<int> Context, float backoff)
    {
        var Scorelist = new List<(int ID, float pb)>();
        if (trie?.Childern == null) return Scorelist;
        if (Context.Count == 0)
        {
            
            var CandidatesWordsList = new List<(int ID , int count)>();
            CandidatesWordsList = trie.Childern.Select(F=>(F.Key,F.Value.Count)).Take(10).ToList();
            foreach(var (ID , Count) in CandidatesWordsList)
            {
                var Score = (float)Count/trie.Count*backoff ;
                Scorelist.Add((ID, Score));
            }
            return Scorelist;

        }

        var Head = Context[0];

        if (trie.Childern == null || !trie.Childern.TryGetValue(Head, out Trie value)) return new();


        return Topk(value, Context.GetRange(1, Context.Count - 1),backoff);;
    }
    public List<(int, float)> Predict(Trie searcher, List<int> Context, int Ngram , float backoff = 1f)
    {

 

        var result = Topk(searcher, Context, backoff);
        if (result.Count > 0)
        {
            return result.Select(F => (F.Item1, F.Item2)).Take(10).ToList();
        }

        if (Context.Count == 0) return result;

        return Predict(searcher, Context.GetRange(1, Context.Count - 1), Ngram , backoff*=0.4f);
    }
    
    

    public static void BurnThisNode(int Limit, Trie MainTrie)
    {
        if (MainTrie?.Childern == null) return;

        var RemoveThisKey = new List<int>();

        foreach (var (token, child) in MainTrie.Childern)
        {
            if (child == null || child.Count < Limit)
            {
                RemoveThisKey.Add(token);
            }
            else
            {
                BurnThisNode(Limit, child);
            }
        }

        foreach (var key in RemoveThisKey)
        {
            MainTrie.Childern.Remove(key);
        }
    }

    public List<(string, float)> NgramAnswer(string Context ,int Ngram, int MaxAnswers )

    {

        var context = tokenize(Context);


        int take = Ngram - 1;

        context = context.Skip(Math.Max(0, context.Count - take)).ToList();

        var WordAhead = Predict(_trie, context, Ngram);
        var t = Converttoword(WordAhead);

        return t.Take(10).ToList();

    }


    private List<(string,float)> Converttoword(List<(int,float)> L)
    {
         WordRepository database = new();

        var r = new List<(string, float)>();
        r = L.Select(S => (database.GetWordFromId(S.Item1), S.Item2)).ToList();

        return r;
    }
    public static void insertGram(Trie trie, List<int> FormatedString)
    {
        if (trie == null || FormatedString == null) return;

        Trie current = trie;
        current.Count++;

        foreach (int head in FormatedString)
        {
            current.Childern ??= new Dictionary<int, Trie>();

            if (!current.Childern.TryGetValue(head, out Trie child))
            {
                child = new Trie(0);
                current.Childern[head] = child;
            }

            current = child;
            current.Count++;
        }
    }

    public static Trie BulidTrie(List<List<int>> Gram)
    {
        int i = 0;

        var tot = Gram.Count;

        Trie trie = new();

        foreach (var List in Gram)
        { 
            if (i++ % 10000 == 0 || i == tot - 2)

                Console.Write($"\u001b[2K\r {i++}insert in the tree out of {tot} ||{((i + 1) * 100) / tot} ");
            insertGram(trie, List);

        }

        return trie;
    }
}




