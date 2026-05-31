namespace ScadaServer.Application.DTOs
{
    public class ModelVariableDto
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DataType { get; set; }
        public string Unit { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsStored { get; set; }
        public string StoreMode { get; set; }
    }
}
