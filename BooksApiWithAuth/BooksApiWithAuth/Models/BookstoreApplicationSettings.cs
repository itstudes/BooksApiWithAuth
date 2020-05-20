namespace BooksApiWithAuth.Models
{
    public class BookstoreApplicationSettings : IBookstoreApplicationSettings
    {
        public string JwtTokenSecret { get; set; }
    }

    public interface IBookstoreApplicationSettings
    {
        string JwtTokenSecret { get; set; }
    }
}
