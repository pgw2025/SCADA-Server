using System.ComponentModel.DataAnnotations;

namespace ScadaServer.Application.DTOs
{
    public class DataModelDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "模型名称不能为空")]
        [StringLength(50, ErrorMessage = "模型名称不能超过50个字符")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "描述不能超过200个字符")]
        public string Description { get; set; }

        [Required(ErrorMessage = "模型类型不能为空")]
        [RegularExpression("^(S7|OPCUA|MQTT|Virtual)$", ErrorMessage = "不支持的模型类型。可选值：S7, OPCUA, MQTT, Virtual")]
        public string Type { get; set; }
    }
}
