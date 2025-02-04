using System.Diagnostics.CodeAnalysis;
using NLog;

namespace FlexiSphere;

public static class CustomExtensions
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Converts a boolean value to 'Yes' or 'No'
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToYesNo(this bool value) =>
        value ? "Yes" : "No";

    /// <summary>
    /// Converts a boolean value to 'enabled' or 'disabled'
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToEnabledDisabled(this bool value) =>
        value ? "enabled" : "disabled";

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the value is null
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static TObject ThrowArgumentExceptionIfNull<TObject>([NotNull][DoesNotReturnIf(false)] this TObject? value, string message)
    {
        if (value is null)
        {
            var exp = new ArgumentNullException(message);

            _logger.Error(exp, message);
            throw exp;
        }

        return value!;
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the value is null or empty
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string ThrowExceptionIfNullOrEmpty<TException>([NotNull] this string? value, string message)
        where TException : Exception
    {
        if (value is null || string.IsNullOrEmpty(value))
        {
            var exp = (TException)Activator.CreateInstance(typeof(TException), message)!;

            _logger.Error(exp, message);
            throw exp;
        }

        return value;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the value is not null or empty
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string? ThrowExceptionIfNotNullOrEmpty<TException>([NotNullWhen(false)] this string? value, string message)
        where TException : Exception
    {
        if (!(value is null || string.IsNullOrEmpty(value)))
        {
            var exp = (TException)Activator.CreateInstance(typeof(TException), message)!;

            _logger.Error(exp, message);
            throw exp;
        }

        return value;
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the value is null
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static object ThrowExceptionIfNull<TException>([NotNull] this object? value, string message)
        where TException : Exception
    {
        if (value is null)
        {
            var exp = (TException)Activator.CreateInstance(typeof(TException), message)!;

            _logger.Error(exp, message);
            throw exp;
        }

        return value;
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the value is not null
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static object? ThrowExceptionIfNotNull<TException>([NotNullWhen(false)] this object? value, string message)
        where TException : Exception
    {
        if (value is not null)
        {
            var exp = (TException)Activator.CreateInstance(typeof(TException), message)!;

            _logger.Error(exp, message);
            throw exp;
        }

        return value;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the value is null or empty
    /// </summary>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ThrowArgumentExceptionIfNullOrEmpty([NotNull] this string? value, string message)
    {
        if (value is null || string.IsNullOrEmpty(value))
        {
            var exp = new ArgumentNullException(message);
            _logger.Error(exp, message);

            throw exp;
        }

        return value;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the value is true
    /// </summary>
    /// <param name="value"></param>
    /// <param name="varName"></param>
    /// <param name="message"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowArgumentExceptionIfTrue([DoesNotReturnIf(true)] this bool value, string varName, string message)
    {
        if (value)
        {
            var exp = new ArgumentException($"[{varName}]: {message}");

            _logger.Error(exp, message);
            throw exp;
        }
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the value is true
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="value"></param>
    /// <param name="message"></param>
    public static void ThrowExceptionIfTrue<TException>([DoesNotReturnIf(true)] this bool value, string message) where TException : Exception
    {
        if (value)
        {
            var exp = (TException)Activator.CreateInstance(typeof(TException), message)!;

            _logger.Error(exp, message);
            throw exp;
        }
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the value is false
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="value"></param>
    /// <param name="message"></param>
    public static void ThrowExceptionIfFalse<TException>([DoesNotReturnIf(false)] this bool value, string message) where TException : Exception =>
        (!value).ThrowExceptionIfTrue<TException>(message);

    /// <summary>
    /// Throws an <see cref="RequiredObjectNullReferenceException"/> if the value is null
    /// </summary>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="RequiredObjectNullReferenceException"></exception>
    public static object ThrowExceptionIfNull([NotNull] this object? value, string message)
    {
        if (value is null)
        {
            var exp = new NullReferenceException(message);

            _logger.Error(exp, message);
            throw exp;
        }

        return value!;
    }
}
