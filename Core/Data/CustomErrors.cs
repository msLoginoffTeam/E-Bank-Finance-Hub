namespace CoreApi.Models.innerModels;

public class CustomException : Exception
{
    public string Type { get; }
    public string Object { get; }
    public int Code { get; }
    public CustomException(string message, string type, string @object, int code)
        : base(message)
    {
        Type = type;
        Object = @object;
        Code = code;
    }
}