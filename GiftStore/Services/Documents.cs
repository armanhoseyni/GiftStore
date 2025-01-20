using Microsoft.AspNetCore.Mvc;

namespace GiftStore.Services
{
    public class Documents
    {
        public string UploadFile(IFormFile file, string directory)
        {
            // Validate input
            if (file == null || file.Length == 0)
            {
                return "";
            }

            // Define permitted file extensions
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".mp4", ".mpeg", ".wav" };

            // Define permitted MIME types
            string[] permittedMimeTypes = { "image/jpeg", "image/png", "image/gif", "video/mp4", "audio/mpeg", "audio/wav" };

            // Ensure the provided directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Get the file extension and validate it
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return $"Invalid file type: {file.FileName}. Only image, video, and audio files are allowed.";
            }

            // Validate the MIME type
            if (!permittedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return $"Invalid MIME type: {file.ContentType}. Only image, video, and audio files are allowed.";
            }

            // Generate a unique file name
            string uniqueFileName = $"{Guid.NewGuid()}{ext}";
            string filePath = Path.Combine(directory, uniqueFileName);

            // Save the file to the specified path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Return the file path
            return filePath.Replace("wwwroot\\", "").Replace("wwwroot/", "");
        }


        public string GetContentType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tiff" => "image/tiff",
                ".tif" => "image/tiff",
                _ => "application/octet-stream" // Default content type
            };
        }

    }
    }

