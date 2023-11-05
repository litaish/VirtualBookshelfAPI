using System.ComponentModel.DataAnnotations;

namespace VirtualBookshelfAPI.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        [Url]
        public string ImgSrc { get; set; }
        public int Rating { get; set; }
        public string Notes { get; set; }
        public bool Read { get; set; }
        public List<Author> Authors { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}
