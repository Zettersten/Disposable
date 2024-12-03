using System.Runtime.CompilerServices;

namespace Disposable;

public static class DomainValidator
{
    /// <summary>
    /// Validates and checks if an domain is from a disposable domain.
    /// </summary>
    /// <param name="domain">Domain to validate</param>
    /// <returns>True if email is invalid or from a disposable domain</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDisposable(ReadOnlySpan<char> domain)
    {
        return DisposableDomains.Contains(domain);
    }

    /// <summary>
    /// Validates and checks if an domain is from a disposable domain.
    /// </summary>
    /// <param name="domain">Domain to validate</param>
    /// <returns>True if email is invalid or from a disposable domain</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDisposable(string domain)
    {
        return DisposableDomains.Contains(domain);
    }
}
