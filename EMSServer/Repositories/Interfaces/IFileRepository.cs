namespace EMSServer.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<string> SaveFile(string base64ImageData, string userFileName);
        Task<List<string>> SaveMultipleFiles(List<IFormFile> files);
        void DeleteFile(string fileName);
    }
}
