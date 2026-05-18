using EMSServer.Repositories.Interfaces;

namespace EMSServer.Repositories.Implementations
{
    public class FileRepository : IFileRepository
    {
        public async Task<string> SaveFile(string base64ImageData, string userFileName)
        {
            string base64Data = base64ImageData;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Split(',')[1];
            }

            // Convert Base64 string to byte array
            var bytes = Convert.FromBase64String(base64Data);

            // Generate a safe, unique filename
            var extension = !string.IsNullOrEmpty(userFileName)
                            ? Path.GetExtension(userFileName)
                            : ".jpg";
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(userFileName);

            var fileName = $"{nameWithoutExtension}{extension}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(folderPath, fileName);

            // Ensure the directory exists on the server
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Write the bytes to the physical file
            await File.WriteAllBytesAsync(filePath, bytes);

            return fileName;
        }

        public async Task<List<string>> SaveMultipleFiles(List<IFormFile> files)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string[] allowedExtensions = [".png", ".jpg", ".docx", ".xlsx", ".csv", ".doc"];

            var uploadedFiles = new List<string>();

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName);

                if (!allowedExtensions.Contains(extension))
                {
                    throw new FileLoadException($"Only {string.Join(",", allowedExtensions)}");
                }

                var newFileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(path, newFileName);

                using var fileStream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(fileStream);

                uploadedFiles.Add(file.FileName);
            }

            return uploadedFiles;
        }

        public void DeleteFile(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var fullPath = Path.Combine(path, fileName);

            if (!Path.Exists(fullPath))
            {
                throw new FileNotFoundException($"File {fileName} does not exist!");
            }

            File.Delete(fullPath);
        }
    }
}
