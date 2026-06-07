using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("SystemUsers")]
    public class SystemUser : EntityBase
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }
}