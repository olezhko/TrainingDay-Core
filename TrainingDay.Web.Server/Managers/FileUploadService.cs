using TrainingDay.Web.Data.Common.Files;

namespace TrainingDay.Web.Server.Managers; 

public class FileUploadService(IWebHostEnvironment _env) : IFileUploadService
{
    public async Task<FileUploadResponse> UploadFileAsync(FileUploadRequest request)
    {
        string folderPath = Path.Combine(_env.WebRootPath, "uploads");

        // create the directory if it does not exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // extract the file extension for some specific purpose - like validation
        string extension = Path.GetExtension(request.File.FileName);

        // You can add validation here to allow only specific files using the extension extracted - 
        if (!IsValidImageFile(extension))
        {
            return new FileUploadResponse(string.Empty);
        }
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(request.File.FileName);

        // I am concatenating a new guid to the end of the filename so that if file with same name is uploaded twice that is handled correctly
        string fileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(folderPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await request.File.CopyToAsync(stream);
        return new FileUploadResponse($"/uploads/{fileName}");
    }

    private bool IsValidImageFile(string extension)
    {
        return true;
    }

    public bool DeleteFile(DeleteFileRequest request)
    {
        string folderPath = Path.Combine(_env.WebRootPath, "uploads");
        var filePath = Path.Combine(folderPath, request.FileName);
        if (!File.Exists(filePath))
        {
            return false;
        }
        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (IOException ex)
        {
            // handle exception - Logging, etc
            return false;
        }
    }

    public Task<bool> DeleteFileAsync(DeleteFileRequest requestModel)
    {
        return Task.Run<bool>(() =>
        {
            return DeleteFile(requestModel);
        });
    }
}