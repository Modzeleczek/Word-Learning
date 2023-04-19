using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Word_Learning.MVVM.Model
{
    public static class Downloader
    {
        private static HttpClient Client;

        static Downloader()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;
            Client = new HttpClient(handler);
        }

        private static async Task<string> SendRequest(string endpointUrl, HttpMethod method,
            string jsonString = null)
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(endpointUrl),
                Headers =
                {
                    { "Accept", "application/json" }
                },
                Content = jsonString == null ? null :
                    new StringContent(jsonString, Encoding.UTF8, "application/json")
            };
            var response = await Client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        private static readonly string DETAILS_API = "https://api.dictionaryapi.dev/api/v2/entries/en/";
        private static readonly string RANDOM_API = "https://random-word-api.herokuapp.com/word";
        private static string Get(string url)
        {
            var task = SendRequest(url, HttpMethod.Get);
            task.Wait();
            return task.Result;
        }

        public static LinkedList<string> GetRandomWords(int count)
        {
            var jsonWords = Get(RANDOM_API + $"?number={count}");
            var words = JsonConvert.DeserializeObject<LinkedList<string>>(jsonWords);
            return words;
        }

        public class Word
        {
            public string word;
            public List<Meaning> meanings;
        }

        public class Meaning
        {
            public string partOfSpeech;
            public List<Definition> definitions;
        }

        public class Definition
        {
            public string definition;
            public string example;
            public List<string> synonyms;
        }

        public static Word GetFirstHomonym(string word)
        {
            var jsonString = Get(DETAILS_API + word);
            List<Word> homonyms = JsonConvert.DeserializeObject<List<Word>>(jsonString);
            /* For one string, e.g. 'bear', API can return multiple words
            (homonyms). If could not download the word, execute throws
            IOException. */
            return IsWordEligible(homonyms) ? homonyms[0] : null;
        }

        private static bool IsWordEligible(List<Word> homonyms)
        {
            /* Word was not downloaded or no its homonym exists in the
            dictionary. */
            if (homonyms == null || homonyms.Count < 1)
                return false;
            // Take only the first homonym from multiple possible ones.
            Word dw = homonyms[0];
            if (string.IsNullOrWhiteSpace(dw.word)) // Actual word
                return false;
            List<Meaning> meanings = dw.meanings;
            // Word has no meanings in the dictionary.
            if (meanings == null || meanings.Count < 1)
                return false;
            Meaning m = meanings[0]; // Take only the first meaning.
            // Part of speech name
            if (string.IsNullOrWhiteSpace(m.partOfSpeech))
                return false;
            List<Definition> definitions = m.definitions;
            /* The selected (first) meaning has no definitions in the
            dictionary. */
            if (definitions == null || definitions.Count < 1)
                return false;
            Definition d = definitions[0]; // Take only the first definition.
            if (string.IsNullOrWhiteSpace(d.definition)) // Actual definition
                return false;
            /* There may be no example of the the selected definition (null
            then). */
            // d.getExample();
            /* List<string> synonyms = d.synonyms;
            // Word may have no synonyms.
            if (synonyms == null || synonyms.Count < 1)
                return false; */
            return true;
        }
    }
}
