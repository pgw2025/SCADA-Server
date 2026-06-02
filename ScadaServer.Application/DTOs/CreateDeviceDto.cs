using System.ComponentModel.DataAnnotations;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Application.DTOs
{
    public class CreateDeviceDto
    {
        [Required(ErrorMessage = "设备名称不能为空")]
        [StringLength(50, ErrorMessage = "设备名称不能超过50个字符")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "设备编码不能为空")]
        [StringLength(50, ErrorMessage = "设备编码不能超过50个字符")]
        public string Code { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "请选择所属区域")]
        public int AreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "请选择变量模型")]
        public int ModelId { get; set; }

        [Required(ErrorMessage = "通信协议类型不能为空")]
        [RegularExpression("^(S7|OPCUA|MQTT|Virtual)$", ErrorMessage = "不支持的协议类型。可选值：S7, OPCUA, MQTT, Virtual")]
        public string Type { get; set; } = string.Empty;

        [RegularExpression(@"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$", ErrorMessage = "IP地址格式不正确")]
        public string IpAddress { get; set; } = string.Empty;

        [Range(1, 65535, ErrorMessage = "端口号必须在1-65535之间")]
        public int? Port { get; set; }

        public string Topic { get; set; } = string.Empty;
        public DeviceStatus Status { get; set; }
        public string CpuType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Rack 是必填项")]
        [Range(0, 7, ErrorMessage = "Rack 必须在 0 到 7 之间")]
        public int? Rack { get; set; }

        [Required(ErrorMessage = "Slot 是必填项")]
        [Range(0, 31, ErrorMessage = "Slot 必须在 0 到 31 之间")]
        public int? Slot { get; set; }
    }
}
