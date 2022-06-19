using System.Collections.Generic;

namespace Word_Learning.MVVM.Model
{
    public class Word
    {
        public string Content { get; set; }
        public string PartOfSpeech { get; set; }
        public string Definition { get; set; }
        public string Example { get; set; }
        public ICollection<int> SynonymIds { get; set; }
        public int DefinitionMatches { get; set; }
        public int SynonymMatches { get; set; }
        [JsonIgnore] public string JoinedSynonyms
        {
            get
            {
                var synonyms = User.Instance.Synonyms;
                var list = new LinkedList<string>();
                foreach (var id in SynonymIds)
                    list.AddLast(synonyms[id]);
                return string.Join(", ", list);
            }
        }

        public Word(string content, string partOfSpeech, string definition, string example,
            ICollection<int> synonymIds, int definitionMatches, int synonymMatches)
        {
            Content = content;
            PartOfSpeech = partOfSpeech;
            Definition = definition;
            Example = example;
            SynonymIds = synonymIds;
            DefinitionMatches = definitionMatches;
            SynonymMatches = synonymMatches;
        }
    }
}
