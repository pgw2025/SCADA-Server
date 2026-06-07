using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("SystemUsers")]
    public class SystemUserDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }
}