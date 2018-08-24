namespace DotNetCoreTestApi.Models
{
    public interface IRequestResult<T>
    {
        T Data { get; set; }

        string Message { get; set; }

        bool Success { get; set; }
    }
}