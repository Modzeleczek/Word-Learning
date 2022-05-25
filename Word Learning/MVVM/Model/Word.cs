using Newtonsoft.Json;
using System.Collections.Generic;

namespace Word_Learning.MVVM.Model
{
    public class Word
    {
        public string Word_ { get; set; }
        public string PartOfSpeech { get; set; }
        public string Definition { get; set; }
        public string Example { get; set; }
        public ICollection<string> Synonyms { get; set; }
        public int GoodAnswers { get; set; }
        [JsonIgnore] public string JoinedSynonyms { get { return string.Join(", ", Synonyms); } }

        public Word(string word, string partOfSpeech, string definition, string example, ICollection<string> synonyms, int goodAnswers)
        {
            Word_ = word;
            PartOfSpeech = partOfSpeech;
            Definition = definition;
            Example = example;
            Synonyms = synonyms;
            GoodAnswers = goodAnswers;
        }
    }
}
