namespace VirtualBookshelfAPI.DTOs
{
    public class BookEditingDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Notes { get; set; }
        public bool Read { get; set; }
    }
}
