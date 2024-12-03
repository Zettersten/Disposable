using System.Runtime.CompilerServices;

namespace Disposable;

public static class EmailValidator
{
    /// <summary>
    /// Validates and checks if an email address is from a disposable domain.
    /// </summary>
    /// <param name="email">Email address to validate</param>
    /// <returns>True if email is invalid or from a disposable domain</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDisposable(ReadOnlySpan<char> email)
    {
        if (!TryGetDomain(email, out var domain)) return true;
        return DisposableDomains.Contains(domain.ToString());
    }

    private static bool TryGetDomain(ReadOnlySpan<char> email, out ReadOnlySpan<char> domain)
    {
        domain = default;

        if (email.IsEmpty) return false;

        var atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex == email.Length - 1) return false;

        domain = email[(atIndex + 1)..];
        return IsValidDomain(domain);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidDomain(ReadOnlySpan<char> domain)
    {
        if (domain.IsEmpty) return false;

        var lastDot = domain.LastIndexOf('.');
        return lastDot > 0 && lastDot < domain.Length - 1;
    }
}