using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
public class Translation
{
    public  async Task<string> TranslateAsync(string word, string fromLang, string toLang)
    {

        string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLang}&tl={toLang}&dt=t&dt=bd&q={Uri.EscapeDataString(word)}";

        using HttpClient client = new HttpClient();
        string response = await client.GetStringAsync(url);

        using JsonDocument doc = JsonDocument.Parse(response);


        string mainResult = doc.RootElement[0][0][0].GetString();


        return mainResult;
    }
}