namespace ScadaServer.Domain.Entities
{
    public class SystemLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
    }
}