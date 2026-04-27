namespace BaseCore.Entities
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Country { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
    }
}
