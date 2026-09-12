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
        public (string, float) TriCompletion { get; set; }

        public (string, float) Correction { get; set; }

        public (string, float) SecondaryCorrection { get; set; }
        public (string, float) TriCorrection { get; set; }

    }
    Answersblock CreateBlock = new();


    public Answersblock answersblock( int Currentstate, List<(string, float)> NgramProbabiltities, string CurrentWord = "", List<(string, float)> LevenshteinProbabiltities = null)
    {




        if (Currentstate == 1)
        {

            NgramProbabiltities.OrderByDescending(f => f.Item2);
            CreateBlock.Completion = NgramProbabiltities[0];
            CreateBlock.SecondaryCompletion = NgramProbabiltities.Count >= 2 ? NgramProbabiltities[1] : ("", 0);
            CreateBlock.TriCompletion = NgramProbabiltities.Count >= 3 ? NgramProbabiltities[2] : ("", 0);

            return CreateBlock;

        }

        else if (Currentstate == 3)
        {
    

            LevenshteinProbabiltities.OrderByDescending(f => f.Item2);
            DeleteRepetitions(LevenshteinProbabiltities);
            DeleteRepetitions(NgramProbabiltities);

            List<(string, float)> mostPb = MostProbable(LevenshteinProbabiltities, NgramProbabiltities, CurrentWord);


           
            CreateBlock.Correction =  NgramProbabiltities.Count >=1  ? NgramProbabiltities[0] : ("",0);
            CreateBlock.SecondaryCorrection = NgramProbabiltities.Count >= 2 ? NgramProbabiltities[2] : ("", 0);
            CreateBlock.TriCorrection = NgramProbabiltities.Count >= 3 ? NgramProbabiltities[3] : ("", 0);

            CreateBlock.Completion = mostPb[0];
            CreateBlock.SecondaryCompletion = mostPb.Count >= 2 ? mostPb[1] : ("", 0);
            CreateBlock.TriCompletion = mostPb.Count >= 3 ? mostPb[2] : ("", 0);

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

            
            block.SecondaryCompletion = PreFixList.Count >=2 ? (PreFixList[1], 7) : ("",0);
            block.TriCompletion = PreFixList.Count >= 3 ? (PreFixList[2], 7) : ("", 0);

            block.Correction = PreFixList.Count >= 4 ? (PreFixList[3], 7) : ("", 0);

            block.SecondaryCorrection = PreFixList.Count >= 5 ? (PreFixList[4], 7) : ("", 0);
            block.TriCorrection = PreFixList.Count >= 6 ? (PreFixList[5], 7) : ("", 0);
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



