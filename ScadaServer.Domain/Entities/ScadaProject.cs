namespace ScadaServer.Domain.Entities
{
    public class ScadaProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ScadaPage> Pages { get; set; }
    }
}