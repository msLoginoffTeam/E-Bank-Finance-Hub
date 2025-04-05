namespace Common.ErrorHandling
{
    public class ErrorListResponse
    {
        public int status { get; set; }
        public Dictionary<string, string> errors { get; set; }

        public ErrorListResponse(int status)
        {
            this.status = status;
            this.errors = new Dictionary<string, string>();
        }
        public ErrorListResponse(int status, Dictionary<string, string> errors)
        {
            this.status = status;
            this.errors = errors;
        }

        public void addError(string key, string error)
        {
            errors.Add(key, error);
        }
    }
}
