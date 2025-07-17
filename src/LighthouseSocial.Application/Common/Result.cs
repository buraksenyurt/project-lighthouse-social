using System;

namespace LighthouseSocial.Application.Common;

public class Result<T>
{
    public bool Succes { get; }
    public T? Data { get; }
    public string? ErrorMessage { get; }
    private Result(bool success, T? data, string? errorMessage)
    {
        Succes = success;
        Data = data;
        ErrorMessage = errorMessage;
    }
    public static Result<T> Ok(T data) => new(true, data, null);
    public static Result<T> Fail(string errorMessage) => new(false, default, errorMessage);
}

public class Result
{
    public bool Success { get; }
    public string? ErrorMessage { get; set; }
    protected Result(bool success, string? errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }
    public static Result Ok() => new(true, null);
    public static Result Fail(string error) => new(false, error);
}
