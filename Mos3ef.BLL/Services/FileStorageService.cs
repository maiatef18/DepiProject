using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mos3ef.BLL.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _webRootPath;

        public FileStorageService(ILogger<FileStorageService> logger)
        {
            _logger = logger;
            _webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            try
            {
                // Create folder if it doesn't exist
                var folderPath = Path.Combine(_webRootPath, folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInformation("Created directory: {FolderPath}", folderPath);
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}_{DateTime.Now.Ticks}{fileExtension}";
                var filePath = Path.Combine(folderPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File saved successfully: {FileName}", fileName);

                // Return relative path
                return $"/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fullPath = Path.Combine(_webRootPath, filePath.TrimStart('/'));
                
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return true;
                }

                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false; // Don't throw - deletion failure shouldn't break the flow
            }
        }

        public bool ValidateFile(IFormFile file, out string errorMessage, string[] allowedExtensions, int maxSizeMB = 5)
        {
            errorMessage = string.Empty;

            if (file == null || file.Length == 0)
            {
                errorMessage = "No file uploaded.";
                return false;
            }

            // Validate file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                errorMessage = $"Only {string.Join(", ", allowedExtensions)} files are allowed.";
                return false;
            }

            // Validate file size
            var maxSizeBytes = maxSizeMB * 1024 * 1024;
            if (file.Length > maxSizeBytes)
            {
                errorMessage = $"File size must not exceed {maxSizeMB}MB.";
                return false;
            }

            // Validate MIME type (additional security)
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                errorMessage = "Invalid file type. Only image files are allowed.";
                return false;
            }

            return true;
        }
    }
}
