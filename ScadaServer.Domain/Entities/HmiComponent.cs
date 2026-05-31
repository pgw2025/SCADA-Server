using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// HMI 控件表 - 存储前端拖拽生成的界面数据
    /// </summary>
    [SugarTable("HmiComponents")]
    public class HmiComponent
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int PageId { get; set; }
        /// <summary>
        /// 控件类型：Pump/Valve/Gauge 等
        /// </summary>
        public string Type { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ZIndex { get; set; }
        /// <summary>
        /// 关联的变量Key
        /// </summary>
        public string BindField { get; set; }
        /// <summary>
        /// 控件属性 JSON（颜色、阈值、模式等）
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar(max)")]
        public string PropsJson { get; set; }
    }
}
