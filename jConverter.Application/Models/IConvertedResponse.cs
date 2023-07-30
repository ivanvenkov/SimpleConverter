namespace jConverter.Application.Models
{
    public interface IConvertedResponse
    {
        public  string FileData { get; set; }
        public  string Extension{ get; }
    }
}
