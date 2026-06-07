namespace ScadaServer.Domain.Entities
{
    public class SystemUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }
}