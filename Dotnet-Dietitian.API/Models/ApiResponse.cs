namespace Dotnet_Dietitian.API.Models;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Detail { get; set; }
}