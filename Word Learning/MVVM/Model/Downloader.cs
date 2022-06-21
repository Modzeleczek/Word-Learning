﻿using Newtonsoft.Json;
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

        private static async Task<string> GetWordDetails(string word)
        {
            return await SendRequest("https://api.dictionaryapi.dev/api/v2/entries/en/" + word, HttpMethod.Get);
        }

        private static int Index = 0;
        public static LinkedList<string> GetRandomWords(int count)
        {
            string[] hardCodedWords = {
                "bear", "arouse", "celery", "roof", "crown", "week", "crib", "tax", "money", "twig", "doctor",
                "stem", "string", "baseball", "kneel", "fax", "sip", "stroke", "bathe", "walk", "jam", "march",
                "hug", "strip", "cycle", "welcome", "goofy", "husky", "elderly", "eminent", "willing", "regular",
                "loutish", "merciful", "abounding", "light", "waggish", "gaping", "devotedly", "very", "safely",
                "ahead", "eventually", "punctually", "blissfully", "frankly", "hopefully", "somewhat", 
                "personally", "else" };
            if (count > hardCodedWords.Length)
                return null;
            LinkedList<string> ret = new LinkedList<string>();
            for (int i = 0; i < count; ++i)
            {
                ret.AddLast(hardCodedWords[Index]);
                Index = (Index + 1) % hardCodedWords.Length;
            }
            return ret;
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
            var task = GetWordDetails(word);
            task.Wait();
            var jsonString = task.Result;
            List<Word> homonyms = JsonConvert.DeserializeObject<List<Word>>(jsonString);
            // dla jednego stringa, np. "bear", API może zwrócić kilka słów (homonimów); jeżeli nie udało się pobrać słowa, to execute wyrzuca IOException
            return IsWordEligible(homonyms) ? homonyms[0] : null;
        }

        private static bool IsWordEligible(List<Word> homonyms)
        {
            if (homonyms == null || homonyms.Count < 1) // słowo nie zostało pobrane lub nie istnieje
                // żaden jego homonim w słowniku
                return false;
            Word dw = homonyms[0]; // bierzemy tylko pierwszy homonim z możliwych kilku
            if (string.IsNullOrWhiteSpace(dw.word)) // faktyczne słowo
                return false;
            List<Meaning> meanings = dw.meanings;
            if (meanings == null || meanings.Count < 1) // słowo nie ma żadnych znaczeń w słowniku
                return false;
            Meaning m = meanings[0]; // bierzemy tylko pierwsze znaczenie
            if (string.IsNullOrWhiteSpace(m.partOfSpeech)) // nazwa części mowy
                return false;
            List<Definition> definitions = m.definitions;
            if (definitions == null || definitions.Count < 1) // wybrane pierwsze znaczenie nie ma
                // żadnych definicji w słowniku
                return false;
            Definition d = definitions[0]; // bierzemy tylko pierwszą definicję
            if (string.IsNullOrWhiteSpace(d.definition)) // faktyczna definicja
                return false;
            // d.getExample(); // przykładu wybranej definicji może nie być (wtedy null)
            /* List<String> synonyms = d.getSynonyms();
            if (synonyms == null || synonyms.size() < 1) // słowo może nie mieć synonimów
                return false; */
            return true;
        }
    }
}