namespace HangFireBot.Models.Entites.ResponseEntites
{
    public class CommonResponse
    {
        public string status_code { get; set; }
        public string? status_message { get; set; }
        public object data { get; set; } = new object();
        public object Data { get; set; } = new object();
        public List<ErrorResponseData> error_messages { get; set; }
        public class ErrorResponseData
        {
            public string error_code { get; set; }
            public string error_message { get; set; }
        }

        public class Holiday
        {
            public string Name { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
