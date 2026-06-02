using System.ComponentModel.DataAnnotations;

namespace ScadaServer.Application.DTOs
{
    public class CreateDataModelDto
    {
        [Required(ErrorMessage = "模型名称不能为空")]
        [StringLength(50, ErrorMessage = "模型名称不能超过50个字符")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "描述不能超过200个字符")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "模型类型不能为空")]
        [RegularExpression("^(S7|OPCUA|MQTT|Virtual)$", ErrorMessage = "不支持的模型类型。可选值：S7, OPCUA, MQTT, Virtual")]
        public string Type { get; set; } = string.Empty;
    }
}
