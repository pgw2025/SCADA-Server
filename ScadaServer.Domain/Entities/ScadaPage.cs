namespace ScadaServer.Domain.Entities
{
    public class ScadaPage
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public ScadaProject Project { get; set; }
        public string Name { get; set; }
        public bool IsHome { get; set; }
        public List<HmiComponent> Components { get; set; }
    }
}