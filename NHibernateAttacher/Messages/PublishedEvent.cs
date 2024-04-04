namespace NHibernateProfilerLibrary.Messages
{
    [Serializable]
    public class PublishedEvent
    {
        public string SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}
