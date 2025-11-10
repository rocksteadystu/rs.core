using System;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace rs.core;

public static class Guard
{
    public static void NotNull([NotNull] object? value, [CallerArgumentExpression(parameterName: "value")] string? paramName = null, string? message = null)
    {
        if(value == null)
        {
            throw new NullReferenceException(message ?? $"{paramName} cannot be null");
        }
    }

    public static void IsFalse([NotNull] bool? value, [CallerArgumentExpression(parameterName: "value")] string? paramName = null, string? message = null)
    {
        if (value == null || value == true)
        {
            throw new Exception(message ?? $"{paramName} should be false");
        }
    }

    public static void IsNotNegative([NotNull] int? value, [CallerArgumentExpression(parameterName: "value")] string? paramName = null, string? message = null)
    {
        if (value == null || value < 0)
        {
            throw new Exception(message ?? $"{paramName} cannot be negative");
        }
    }

    public static void NotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(parameterName: "value")] string? paramName = null, string? message = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new NullReferenceException(message ?? $"{paramName} is empty");
        }
    }

    public static void ListIsEmpty<T>([NotNull] IEnumerable<T> value, [CallerArgumentExpression(parameterName: "value")] string? paramName = null, string? message = null)
    {
        if (value.Any())
        {
            throw new UnexpectedItemsException(paramName, message);
        }
    }

    public static void ListIsNotEmpty<T>([NotNull] IEnumerable<T> value, [CallerArgumentExpression(parameterName: "value")] string? paramName = null, string? message = null)
    {
        if (!value.Any())
        {
            throw new UnexpectedItemsException(paramName, message);
        }
    }

    public static void IsTypeOf<T>(Type type, string? message = null)
    {
        Guard.NotNull(type, message);

        if(typeof(T).IsAssignableFrom(type))
        {
            throw new Exception(message ?? $"{type.Name} should be type of {typeof(T)}");
        }
    }
}

    public class UnexpectedItemsException : Exception
{
    public UnexpectedItemsException(string? parameterName, string? message): 
        base(message ?? $"{parameterName} should be empty") 
    {
        ParameterName = parameterName;
    }

    public string? ParameterName { get; }
}
