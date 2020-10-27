namespace HandiCrafts.Web.Models
{
    public class ResponseState<T>
    {
        public bool Success { set; get; }

        public string Message { set; get; }

        public ResponseStateMessageTypes MessageType { set; get; }

        public T Data { get; set; }
    }
}

