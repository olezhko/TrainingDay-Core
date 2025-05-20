using Microsoft.AspNetCore.Http;

namespace TrainingDay.Web.Data.Common.Files;

public interface IFileUploadService
{
    Task<FileUploadResponse> UploadFileAsync(FileUploadRequest requestModel);
    Task<bool> DeleteFileAsync(DeleteFileRequest requestModel);
}

public record DeleteFileRequest(string FileName);

public record FileUploadRequest(IFormFile File);

public record FileUploadResponse(string FileUrl);