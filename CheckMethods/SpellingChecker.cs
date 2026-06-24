
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Automation;

class SpellingCheckerBySingleWord
{
    Dictionary<string, long> DWORDS = new Dictionary<string, long>();
    const string AtoZ = "absdefghijklmnopqrstuvwxyz";
    
    public SpellingCheckerBySingleWord()
    {
        string[] TwoSplites;
        foreach (var FullFile in File.ReadAllLines("W_F.txt"))
        {
            //split every line into two parts by whitespace
            TwoSplites = FullFile.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //making sure that every line is formated like this {string ' ' int };
            if (TwoSplites.Length >= 2 && long.TryParse(TwoSplites[1], out long value)) {
                DWORDS[TwoSplites[0]] = long.Parse(TwoSplites[1]); 
            }
            else { continue; }

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

        return deletes.Concat(transposes).Concat(replaces).Concat(inserts).ToList();//sent all the generated word as a single list
    }
    List<string> KnownEdits2(string word)
    {

        var l = new List<string>();
        foreach (var e1 in this.Edits1(word))
            foreach (var e2 in Edits1(e1))
                if (this.DWORDS.ContainsKey(e2))
                    l.Add(e2);
        return l;
    }
    List<string> Known(List<string> words)
    {
        return words.Where(w => this.DWORDS.ContainsKey(w)).ToList();
    }
    public string Correct(string word)
    {

    var candidateWords = this.Known(new List<string> { word });
        if (candidateWords.Count == 0)
            candidateWords = this.Known(this.Edits1(word));
        if (candidateWords.Count == 0)
            candidateWords = this.KnownEdits2(word);
        if (candidateWords.Count == 0)
            candidateWords = new List<string> { word };
        return candidateWords
            .OrderByDescending(w => DWORDS.TryGetValue(w, out long count) ? count : 0)
            .First();

    }
}

