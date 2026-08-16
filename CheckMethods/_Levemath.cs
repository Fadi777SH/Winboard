
using System;
using System.Collections.Generic;
using System.Linq;
 class LevenshteinProbabiltities
{

    const string AtoZ = "abcdefghijklmnopqrstuvwxyz";
    public WordRepository database = new();
    public class Score
    {
        public List<(string word, float wordscore)> WordsList { get; set; }

        public float score { get; set; }

        public Score(float initail = 0f)
        {
            WordsList = new();
        }
    }

    class StringPair
    {
        public string a { get; set; } = string.Empty;
        public string b { get; set; } = string.Empty;
    }
    List<string> Edits1(string word)
    {
        var splits = Enumerable.Range(0, word.Length + 1)
            .Select(i => new StringPair { a = word.Substring(0, i), b = word.Substring(i) })
            .ToList();

        var deletes = splits
            .Where(s => s.b.Length > 0)
            .Select(s => s.a + s.b.Substring(1))
            .ToList();

        var transposes = splits
            .Where(s => s.b.Length > 1)
            .Select(s => s.a + s.b[1] + s.b[0] + s.b.Substring(2))
            .ToList();

        var replaces = new List<string>();
        foreach (var s in splits)
        {
            if (s.b.Length > 0)
            {
                foreach (var c in AtoZ)
                {

                    replaces.Add(s.a + c + s.b.Substring(1));
                }
            }
        }

        var inserts = new List<string>();
        foreach (var s in splits)
        {
            foreach (var c in AtoZ)
            {
                    inserts.Add(s.a + c + s.b);
            }
        }

        List<string> WordsList = deletes.Concat(transposes).Concat(replaces).Concat(inserts).ToList();
        WordsList.RemoveAll(f => !CheckWordIfTrue(f));
        
        return WordsList ;
    }
    List<string> KnownEdits2(string word)
    {

        var l = new List<string >();
        var edit1 = Edits1(word);
        foreach (var e1 in edit1)
            foreach (var e2 in Edits1(e1))
                    l.Add(e2);
        return l;
        
    }
    List<string> knownEdits3(string word)
    {
        var l = new List<string>();
        foreach (var e1 in this.KnownEdits2(word))
            foreach (var e2 in Edits1(e1))
                    l.Add(e2);
        return l;
        
    }
    List<(string , float)> Known(List<string> words)
    {
        var l = new List<(string, float)>();


        foreach (var word in words)
        {
            if (CheckWordIfTrue(word))
            {
                l.Add((word, 0.93f));
            }
            else
                l.Add((word, 0f));

            
            foreach (var W in Edits1(word))
            {
                l.Add((W, 0.07f));
            }
            foreach (var W in KnownEdits2(word))
            {
                l.Add((W, 0.05f));
            }

        }

        return l.OrderByDescending(x=> x.Item2).Take(10).ToList();
    }

    private void GetFinalScore(List<(string,float)> candidate)
    {
        const float Lambda = 0.03f;
        var MaxFrquncy = GetFrq("the");
        for(int i =0; i < candidate.Count; i++)
        {
            var CW = candidate[i];
            var GF = GetFrq(CW.Item1);
            var Log = Math.Log(GF + 1) / Math.Log(MaxFrquncy + 1);
            float S = CW.Item2 + Lambda * (float)Log;

            candidate[i] = ((CW.Item1, S));
        }
    }
    public List<(string,float)> Correct(string word)
    {

        
        var candidateWords = this.Known(new List<string> { word });

       candidateWords = candidateWords
                        .OrderByDescending(w => GetFrq(w.Item1))
                        .Take(10)
                        .ToList();

        GetFinalScore(candidateWords);

        return candidateWords;

    }

    private bool CheckWordIfTrue(string word)
    {
        Dictionary<string, int> dicword = GetData.WordID;
        Dictionary<int, (string, long)> dic = GetData.diction;
        if (dicword.TryGetValue(word, out int ID))
        {
            if (dic.TryGetValue(ID, out (string, long) val))
            {
                return true;
            }
        }
        return false;
    }
    private long GetFrq(string word)
    {

        return database.GetWordFrq(word);
        
    }

}

