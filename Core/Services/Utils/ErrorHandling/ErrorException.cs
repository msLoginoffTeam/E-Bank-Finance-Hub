namespace Core.Services.Utils.ErrorHandling
{
    public class ErrorException : Exception
    {
        public int status { get; set; }

        public string message { get; set; }

        public ErrorException(int status, string message)
        {
            this.status = status;
            this.message = message;
        }
    }
}
