namespace ScadaServer.Application.Options
{
    public class SystemDbOptions
    {
        public const string SectionName = "SystemDbConfig";
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        public string GetConnectionString() => 
            $"Server={Host};Port={Port};Database={DatabaseName};Uid={Username};Pwd={Password};";
    }
}
