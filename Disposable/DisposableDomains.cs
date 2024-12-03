using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Disposable;

internal static class DisposableDomains
{
    private static readonly FrozenSet<string> domains = GetDomains();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(ReadOnlySpan<char> domain)
    {
        if (domain.IsEmpty)
            return false;
        return domains.Contains(domain.ToString());
    }

    private static FrozenSet<string> GetDomains() =>
        Generator.GeneratedDomains.Values.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    public static ImmutableArray<string> Domains => Generator.GeneratedDomains.Values;
}
