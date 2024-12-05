namespace OFF.PL.Utility
{
    public class DocumentSetting
    {
        public static string UploadFile(IFormFile file, string folderName)
        {
            string folderParh = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", folderName);
            string fileName = $"{Guid.NewGuid()}{file?.FileName}";
            string filePath = Path.Combine(folderParh, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);
            return fileName;
        }
        public static void DeleteFile(string fileName, string folderName)
        {
            var folderParh = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", folderName, fileName);

        }
    }

    

}

