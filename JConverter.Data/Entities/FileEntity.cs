namespace JConverter.Data.Entities
{
    public class FileEntity
    {       
        public int Id { get; set; }

        public string OriginalFile { get; set; }

        public string OriginalFileName { get; set; }

        public string ConvertedFile { get; set; }

        public string ConvertedFileName { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
