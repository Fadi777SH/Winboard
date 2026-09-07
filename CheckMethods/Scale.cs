using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


public class Scale
  {

    
    public class Answersblock
    {
        public (string,float) Completion { get; set; }

        public (string, float) SecondaryCompletion { get; set; }

        public (string, float) Correction { get; set; }

        public (string, float) SecondaryCorrection { get; set; }

    }
    Answersblock CreateBlock = new();


    public Answersblock answersblock( int Currentstate, List<(string, float)> NgramProbabiltities, string CurrentWord = "", List<(string, float)> LevenshteinProbabiltities = null)
    {




        if (Currentstate == 1)
        {

            NgramProbabiltities.OrderByDescending(f => f.Item2);
            CreateBlock.Completion = NgramProbabiltities[0];

            if (NgramProbabiltities.Count > 1)
                CreateBlock.SecondaryCompletion = NgramProbabiltities[1];
            else CreateBlock.SecondaryCompletion = ("", 0);

            CreateBlock.Correction = ("", 0);
            CreateBlock.SecondaryCorrection = ("", 0);

            return CreateBlock;

        }

        else if (Currentstate == 3)
        {
    

            LevenshteinProbabiltities.OrderByDescending(f => f.Item2);
            DeleteRepetitions(LevenshteinProbabiltities);
            DeleteRepetitions(NgramProbabiltities);

            List<(string, float)> mostPb = MostProbable(LevenshteinProbabiltities, NgramProbabiltities, CurrentWord);


            if (NgramProbabiltities.Count != 0) 
            CreateBlock.Correction = NgramProbabiltities[0];

            CreateBlock.Completion = mostPb[0];

            if (NgramProbabiltities.Count > 1)
                CreateBlock.SecondaryCorrection = NgramProbabiltities[1];
            else
                CreateBlock.SecondaryCompletion = ("", 0);

            if (mostPb.Count > 1)
                CreateBlock.SecondaryCompletion = mostPb[1];
            else CreateBlock.SecondaryCompletion = ("", 0);

            return CreateBlock;
            
        }
        else return answersblock(Currentstate, NgramProbabiltities, CurrentWord, LevenshteinProbabiltities);
    }

    public Answersblock answersblock(int Currentstate, List<string> PreFixList, string CurrentWord = "", List<(string, float)> LevenshteinProbabiltities = null)
    {

        //Currentstate = 2 -> PreFix Needed
        DeleteRepetitions(PreFixList);
        Answersblock block = new();
        if (PreFixList.Count > 0)
        {
           

            block.Completion = (PreFixList.FirstOrDefault(), 7);

            if (PreFixList.Count > 1)
                block.SecondaryCompletion = (PreFixList[1], 7);

            block.Correction = PreFixList.Count >= 3 ? (PreFixList[2], 7) : ("", 0);

            block.SecondaryCorrection = PreFixList.Count >= 4 ? (PreFixList[3], 7) : ("", 0);
        }
        return block;
    }

    public List<(string W, float PB)> MostProbable(List<(string , float)> LevenshteinProbabiltities , List<(string, float)> NgramProbabiltities , string CurrentWord)
    {

        LevenshteinProbabiltities.OrderByDescending(F => F.Item2);
        NgramProbabiltities.OrderByDescending(F => F.Item2);

        var highestProbabilities = new List<(string W, float PP)>();



        foreach (var (wordL, PL) in LevenshteinProbabiltities)
        {
            foreach(var (wordN , PN) in NgramProbabiltities)
            {
                if(wordL == wordN)
                {
                    highestProbabilities.Add((wordN, PN * PL));
                }
            }
        }
        
        if (highestProbabilities.Count == 0)
        {
            foreach(var (word,pp) in LevenshteinProbabiltities)
            {
                highestProbabilities.Add((word, pp));
            }
        }
        highestProbabilities.Add(checkbysound(CurrentWord));


        return highestProbabilities.OrderByDescending(x => x.Item2).ToList();


    }
    private (string,float) checkbysound(string CurrentWord)
    {
        MetaPhone CheckBySound = new MetaPhone();
        return CheckBySound.ChechWord(CurrentWord);
    }
    private void DeleteRepetitions(List<(string, float)> TrieList)
    {
        for (int i = TrieList.Count - 2; i >=0; i--)
        {
            if (TrieList[i].Item1 == TrieList[i + 1].Item1)
            {
                TrieList[i] = (TrieList[i].Item1, TrieList[i].Item2 + TrieList[i + 1].Item2);
                TrieList.RemoveAt(i + 1);
            }
        }
        
    }
    private void DeleteRepetitions(List<string> TrieList)
    {
        for (int i = TrieList.Count - 2; i >= 0; i--)
        {
            if (TrieList[i] == TrieList[i + 1])
            {
                TrieList.RemoveAt(i + 1);
            }
        }

    }

}



