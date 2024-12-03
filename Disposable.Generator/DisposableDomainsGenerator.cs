using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Disposable.Generator;

[Generator]
public sealed class DisposableDomainsGenerator : IIncrementalGenerator
{
    private const string Url = "https://raw.githubusercontent.com/disposable/disposable-email-domains/master/domains.txt";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var domains = GetDomains();

        context.RegisterPostInitializationOutput(ctx =>
        {
            var source = $$"""
                using System.Collections.Immutable;

                namespace Disposable.Generator;

                internal static class GeneratedDomains
                {
                    internal static readonly ImmutableArray<string> Values = ImmutableArray.Create(
                        {{string.Join(",\n                        ", domains.Select(d => $"\"{d}\""))}}
                    );
                }
                """;

            ctx.AddSource("GeneratedDomains.g.cs", source);
        });
    }

    private static ImmutableArray<string> GetDomains()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var response = client.GetAsync(Url).ConfigureAwait(false).GetAwaiter().GetResult();
        var content = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        return [.. content
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Where(d => d.Length > 0)
            .Select(d => d.ToLowerInvariant())
            .OrderBy(d => d)];
    }
}