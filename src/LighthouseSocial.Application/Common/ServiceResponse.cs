namespace LighthouseSocial.Application.Common;

public class ServiceResponse<T>
{
    public ServiceResponseStatus Status { get; set; } = ServiceResponseStatus.Success;
    public string? ErrorMessage { get; set; }
    public T? Data { get; set; }
}

public enum ServiceResponseStatus
{
    Success,
    NotFound,
}