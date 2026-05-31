using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 数据对流接口表 - 将内部变量暴露为标准的 RESTful API
    /// </summary>
    [SugarTable("ExposedInterfaces")]
    public class ExposedInterface
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string RouteUrl { get; set; }
        public string RequestMethod { get; set; }
        
        public int DeviceId { get; set; }
        /// <summary>
        /// 关联设备
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(DeviceId))]
        public Device Device { get; set; }

        public string ExposedKey { get; set; }
        public bool Active { get; set; }
    }
}
