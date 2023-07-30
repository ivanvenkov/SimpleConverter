namespace jConverter.Application.Exceptions
{
    public class FileExistsException : Exception
    {
        public FileExistsException(string message) : base(String.Format(message))
        {
        }
    }
}
