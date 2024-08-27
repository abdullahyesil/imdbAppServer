using imdbApi.Services.Abrastract;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace imdbApi.Services.Implementation
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment _env;
        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public bool DeleteImage(string imageFileName)
        {
            try
            {

                var wwwPath = _env.WebRootPath;
                var path = Path.Combine(wwwPath, imageFileName);
                if (System.IO.File.Exists(path))
                {

                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public Tuple<int, string> SaveImage(IFormFile file, int height, int width)
        {
            try
            {
                var contentPath = _env.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var ext = Path.GetExtension(file.FileName).ToLower(); // Uzantıyı küçük harfe çeviriyoruz
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg", ".webp" };
                if (!allowedExtensions.Contains(ext))
                {
                    string msg = string.Format("Sadece {0} uzantılı resimlere izin verilir", string.Join(",", allowedExtensions));
                    return new Tuple<int, string>(0, msg);
                }

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ".jpg"; // JPG formatında kaydetmek için uzantıyı değiştiriyoruz
                var fileWithPath = Path.Combine(path, newFileName);

                using (var image = Image.Load(file.OpenReadStream()))
                {
                    // Çözünürlük ayarlama (örneğin 800x600)
                    image.Mutate(x => x.Resize(width, height));

                    // JPG formatında kaydediyoruz
                    image.Save(fileWithPath, new JpegEncoder());
                }

                return new Tuple<int, string>(1, newFileName);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama ve başarısızlık döndürme
                return new Tuple<int, string>(0, "Bir hata oluştu");
            }
        }
    }
    }
