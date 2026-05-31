using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 变量模型表 - 定义一类设备的变量结构（模板）
    /// </summary>
    [SugarTable("DataModels")]
    public class DataModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 类型：OPCUA/S7/MQTT/Virtual
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 模型下的变量列表
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ModelVariable.ModelId))]
        public List<ModelVariable> Variables { get; set; }
    }
}
