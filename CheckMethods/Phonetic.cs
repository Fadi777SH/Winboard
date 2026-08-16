
using System.Collections.Generic;
using System.Linq;
using Universal.Phonetic; // The new namespace

class MetaPhone
{
     public (string,float) ChechWord(string W)
    {
        Dictionary<string, int> s = GetData.WordID;
        var x = s.Where(f => ToPhoneticID(f.Key) == ToPhoneticID(W)).Select(f=>f.Key).Take(3).ToList().FirstOrDefault();
        if (x != null) return (x, 5f);
        else return new();
        
    }
    private (string,string) ToPhoneticID(string Sound)
    {
        var Twins = new List<(string, string)>();
        Universal.Phonetic.Metaphone.Metaphone3 metaphone = new();

        metaphone.SetEncodeVowels(true); 
        metaphone.SetEncodeExact(true); 
        metaphone.SetKeyLength(30);

        metaphone.SetWord(Sound);
        metaphone.Encode();
        
        string primaryKey = metaphone.GetMetaph();    
        string alternateKey = metaphone.GetAlternateMetaph();

        return (primaryKey,alternateKey);

    }



}
