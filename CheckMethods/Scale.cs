using Microsoft.VisualBasic;
using System.Collections.Generic;
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

        //Currentstate = 1 -> completion only


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

        //Currentstate = 2 -> PreFix Needed

        //else if (Currentstate == 2)
        //{
        //    NgramProbabiltities.OrderByDescending(f => f.Item2);
        //    NgramProbabiltities.RemoveAll(f => !(f.Item1 != CurrentWord));
        //    if (NgramProbabiltities.Count > 0)
        //        CreateBlock.Completion = NgramProbabiltities[0];

        //    else answersblock(3, NgramProbabiltities, CurrentWord, LevenshteinProbabiltities);

        //    if (NgramProbabiltities.Count > 1)
        //        CreateBlock.SecondaryCompletion = NgramProbabiltities[1];
        //    else CreateBlock.SecondaryCompletion = ("", 0);

        //    CreateBlock.Correction = ("", 0);
        //    CreateBlock.SecondaryCorrection = ("", 0);

        //    return CreateBlock;
        //}


        // Currentstate = 3 -> correction needed
        else if (Currentstate == 3)
        {
            List<(string, float)> mostPb;

            LevenshteinProbabiltities.OrderByDescending(f => f.Item2);
            DeleteRepetitions(LevenshteinProbabiltities);
            DeleteRepetitions(NgramProbabiltities);

            mostPb = MostProbable(LevenshteinProbabiltities, NgramProbabiltities, CurrentWord);


            if (NgramProbabiltities.Count != 0) 
            CreateBlock.Completion = NgramProbabiltities[0];

            CreateBlock.Correction = mostPb[0];

            if (NgramProbabiltities.Count > 1)
                CreateBlock.SecondaryCompletion = NgramProbabiltities[1];
            else
                CreateBlock.SecondaryCompletion = ("", 0);

            if (mostPb.Count > 1)
                CreateBlock.SecondaryCorrection = mostPb[1];
            else CreateBlock.SecondaryCorrection = ("", 0);

            return CreateBlock;
            
        }
        else return answersblock(Currentstate, NgramProbabiltities, CurrentWord, LevenshteinProbabiltities);
    }

    public Answersblock answersblock(int Currentstate, List<string> PreFixList, string CurrentWord = "", List<(string, float)> LevenshteinProbabiltities = null)
    {

        //Currentstate = 2 -> PreFix Needed

        Answersblock block = new();
        if (PreFixList.Count > 0)
        {
            //DeleteRepetitions(PreFixList);

            block.Completion = (PreFixList.FirstOrDefault(), 7);

            if (PreFixList.Count > 0)
                block.SecondaryCompletion = (PreFixList[1], 7);

            block.Correction = PreFixList.Count >= 2 ? (PreFixList[2], 7) : ("", 0);

            block.SecondaryCorrection = PreFixList.Count >= 3 ? (PreFixList[3], 7) : ("", 0);
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

            highestProbabilities.Add(checkbysound(CurrentWord));
            foreach(var (word,pp) in LevenshteinProbabiltities)
            {
                highestProbabilities.Add((word, pp));
            }
        }


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



