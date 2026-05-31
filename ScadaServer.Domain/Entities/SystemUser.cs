using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    [SugarTable("SystemUsers")]
    public class SystemUser
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        /// <summary>
        /// 角色：Admin/Operator/Viewer
        /// </summary>
        public string Role { get; set; }
        public string Status { get; set; }
    }
}
