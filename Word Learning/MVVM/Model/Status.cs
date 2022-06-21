namespace Word_Learning.MVVM.Model
{
    public class Status
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Status(int code = 0, string message = "")
        {
            Code = code;
            Message = message;
        }
    }
}
