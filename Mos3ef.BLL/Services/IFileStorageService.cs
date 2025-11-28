using Microsoft.AspNetCore.Http;

namespace Mos3ef.BLL.Services
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Saves an uploaded file to the specified folder
        /// </summary>
        /// <param name="file">The file to save</param>
        /// <param name="folder">The folder path (e.g., "uploads/profiles")</param>
        /// <returns>The relative path to the saved file</returns>
        Task<string> SaveFileAsync(IFormFile file, string folder);

        /// <summary>
        /// Deletes a file from the file system
        /// </summary>
        /// <param name="filePath">The relative file path (e.g., "/uploads/profiles/file.jpg")</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        /// Validates file type and size
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <param name="errorMessage">Output error message if validation fails</param>
        /// <param name="allowedExtensions">Allowed file extensions (e.g., [".jpg", ".png"])</param>
        /// <param name="maxSizeMB">Maximum file size in MB</param>
        /// <returns>True if valid, false otherwise</returns>
        bool ValidateFile(IFormFile file, out string errorMessage, string[] allowedExtensions, int maxSizeMB = 5);
    }
}
