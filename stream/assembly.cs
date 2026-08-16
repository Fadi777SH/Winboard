using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


class Assembly
{
    Scale _scale = new();
    NGramModel _nGramModel = new();

    
    public Scale.Answersblock SetOrder(string BasicLine, string CurrentWord)
    {
        if (string.IsNullOrEmpty(BasicLine))
            return new();

        CurrentWord ??= string.Empty;

        List<string> TokenizeLine = BasicLine
        .ToLower()
        .Split([' ', '%', '\"', '|', '{', '}', '\'', '\\', '?', ',', '<', '>', '?', '@', '#', '~', '_', '!'], StringSplitOptions.RemoveEmptyEntries)
        .Where(s => s.Length > 0)
        .ToList();

        var NgramList = _nGramModel.NgramAnswer(string.Join(" ", TokenizeLine.TakeWhile(F => F != CurrentWord)), 3 , 10);

        // do compeletion first

        if ((BasicLine != null && CurrentWord !=null) && (CurrentWord == "" || CurrentWord == " "))
        {

            if (NgramList.Count > 0)
                return Completion(NgramList, CurrentWord);

        }

        // do prefix second
        else if (BasicLine != null && (CurrentWord != "" || CurrentWord != " "))
        {

            return Prefix(NgramList, CurrentWord);
        }

        

        return new();
        
    }
    public Scale.Answersblock Completion(List<(string, float)> NgramList, string CurrentWord)
    {
        var AnswersBlock = _scale.answersblock(1,NgramList);

        return AnswersBlock;
    }

    public Scale.Answersblock Prefix(List<(string, float)> NgramList, string CurrentWord)
    {
        

        PreFix.PreFixTrie _prefixtrie = new();
        //var haveprefix = _prefixtrie.Search(CurrentWord);

        var Candidate =  _prefixtrie.GetCandidate(CurrentWord);

        if(Candidate.Count != 0 ) return _scale.answersblock(2, Candidate);        

        else return Correction(NgramList, CurrentWord);



    }

    public Scale.Answersblock Correction(List<(string, float)> NgramList, string CurrentWord)
    {
        Console.WriteLine("correction");
        LevenshteinProbabiltities spellingChecker = new();
        var Leve = spellingChecker.Correct(CurrentWord);

        var AnswersBlock = _scale.answersblock(3 ,NgramList , CurrentWord , Leve);

        return AnswersBlock;
    }

}

