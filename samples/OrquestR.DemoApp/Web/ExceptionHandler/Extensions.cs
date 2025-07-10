using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Web.ExceptionHandler;

public static class MiddlewareExtensions
{
    private const string ErrorCodeKey = "errorCode";

    public static Exception AddErrorCode(this Exception exception)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(exception.Message));
        var errorCode = string.Concat(hash[..5].Select(b => b.ToString("x")));
        exception.Data[ErrorCodeKey] = errorCode;
        return exception;
    }

    public static string? GetErrorCode(this Exception exception)
    {
        return (string?)exception.Data[ErrorCodeKey];
    }

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public static string ToJson(in ProblemDetails problemDetails)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(problemDetails, SerializerOptions);
            return UnescapeJsonString(jsonString);

        }
        catch (Exception ex)
        {
            throw new Exception($"An exception has occurred while serializing error to JSON, Exception: {ex.Message}");
        }
    }

    private static string UnescapeJsonString(string jsonString)
    {
        return Regex.Unescape(jsonString);
    }
}