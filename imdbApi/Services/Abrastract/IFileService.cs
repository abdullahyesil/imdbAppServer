namespace imdbApi.Services.Abrastract
{
    public interface IFileService
    {
        public Tuple<int, string> SaveImage(IFormFile file, int width, int height);
        public bool DeleteImage(string imageFileName);
    }
}
