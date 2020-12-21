using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore_API.Data
{
    [Table("Books")]
    public partial class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Isbn { get; set; }
        public string Summary { get; set; }
        [Column("Image")]
        public string ImageName { get; set; }
        public decimal? Price { get; set; }
        public int? AuthorId { get; set; }

        public virtual Author Author { get; set; }
    }
}
