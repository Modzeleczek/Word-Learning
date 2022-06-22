namespace Word_Learning.MVVM.Model
{
    public class IdWord
    {
        public int Id { get; set; }
        public Word Word { get; set; }
        public IdWord(int id, Word word) { Id = id; Word = word; }
    }
}
