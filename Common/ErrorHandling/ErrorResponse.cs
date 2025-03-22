namespace Common.ErrorHandling
{
    public class ErrorResponse
    {
        public int status { get; set; }

        public string message { get; set; }

        public ErrorResponse(int status, string message)
        {
            this.status = status;
            this.message = message;
        }

        public ErrorResponse(ErrorException ErrorException)
        {
            status = ErrorException.status;
            message = ErrorException.message;
        }
    }
}
