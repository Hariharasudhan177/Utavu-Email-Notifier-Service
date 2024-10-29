    public class LoginEvent
    {
        public string Id { get; set; }

        public string Topic { get; set; }

        public string Subject { get; set; }

        public string Type { get; set; }

        public DateTime Time { get; set; }

        public IDictionary<string, object> Data { get; set; }
    }