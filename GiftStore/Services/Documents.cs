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
                return "No file uploaded or the file is empty.";
            }

            // Define permitted image extensions
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

            // Ensure the provided directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Get the file extension and validate it
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return $"Invalid file type: {file.FileName}. Only image files are allowed.";
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
            return filePath;
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

