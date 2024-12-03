using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Disposable;

/// <summary>
/// Provides high-performance email validation utilities following RFC 5322 standards.
/// </summary>
internal static partial class ValidatorHelpers
{
    // Maximum length constraints as per RFC 5321
    private const int MaxLocalPartLength = 64;

    private const int MaxDomainLength = 255;
    private const int MaxTotalLength = 254;

    // Span-based validation for high performance
    public static bool IsValidEmail(ReadOnlySpan<char> email)
    {
        if (email.IsEmpty || email.Length > MaxTotalLength)
            return false;

        int atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex == email.Length - 1)
            return false;

        var localPart = email[..atIndex];
        var domain = email[(atIndex + 1)..];

        return IsValidLocalPart(localPart) && IsValidDomain(domain);
    }

    // Fast path using Regex for common cases
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidEmailQuick(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > MaxTotalLength)
            return false;
        return GetEmailRegex().IsMatch(email);
    }

    public static bool TryParseEmailParts(
        ReadOnlySpan<char> email,
        out ReadOnlySpan<char> localPart,
        out ReadOnlySpan<char> domain
    )
    {
        localPart = default;
        domain = default;

        if (email.IsEmpty || email.Length > MaxTotalLength)
            return false;

        int atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex == email.Length - 1)
            return false;

        localPart = email[..atIndex];
        domain = email[(atIndex + 1)..];
        return true;
    }

    private static bool IsValidLocalPart(ReadOnlySpan<char> localPart)
    {
        if (localPart.IsEmpty || localPart.Length > MaxLocalPartLength)
            return false;

        // Check for quoted string format
        if (localPart[0] == '"' && localPart[^1] == '"')
        {
            return ValidateQuotedLocalPart(localPart[1..^1]);
        }

        // Unquoted local part validation
        return ValidateUnquotedLocalPart(localPart);
    }

    private static bool IsValidDomain(ReadOnlySpan<char> domain)
    {
        if (domain.IsEmpty || domain.Length > MaxDomainLength)
            return false;

        // Handle IP address literals [IPv4 or IPv6]
        if (domain[0] == '[' && domain[^1] == ']')
        {
            return ValidateIpLiteral(domain[1..^1]);
        }

        int lastDotIndex = -1;
        int startIndex = 0;

        for (int i = 0; i < domain.Length; i++)
        {
            if (domain[i] == '.')
            {
                if (!IsValidDomainLabel(domain[startIndex..i]))
                    return false;

                lastDotIndex = i;
                startIndex = i + 1;
            }
        }

        // Validate the last label
        return lastDotIndex > 0 && IsValidDomainLabel(domain[startIndex..]);
    }

    private static bool ValidateQuotedLocalPart(ReadOnlySpan<char> quotedPart)
    {
        for (int i = 0; i < quotedPart.Length; i++)
        {
            char c = quotedPart[i];
            if (c == '\\')
            {
                if (i == quotedPart.Length - 1)
                    return false;
                i++; // Skip next character as it's escaped
                continue;
            }
            if (c < 32 || c == 127)
                return false; // Control characters
        }
        return true;
    }

    private static bool ValidateUnquotedLocalPart(ReadOnlySpan<char> localPart)
    {
        bool hasDot = false;
        char previousChar = '\0';

        foreach (char c in localPart)
        {
            if (c == '.')
            {
                if (previousChar == '.' || previousChar == '\0')
                    return false;
                hasDot = true;
            }
            else if (!IsAllowedLocalPartChar(c))
            {
                return false;
            }
            previousChar = c;
        }

        return previousChar != '.' && (!hasDot || localPart.Length <= MaxLocalPartLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAllowedLocalPartChar(char c) =>
        (c >= 'a' && c <= 'z')
        || (c >= 'A' && c <= 'Z')
        || (c >= '0' && c <= '9')
        || c
            is '!'
                or '#'
                or '$'
                or '%'
                or '&'
                or '\''
                or '*'
                or '+'
                or '-'
                or '/'
                or '='
                or '?'
                or '^'
                or '_'
                or '`'
                or '{'
                or '|'
                or '}'
                or '~';

    private static bool IsValidDomainLabel(ReadOnlySpan<char> label)
    {
        if (label.IsEmpty || label.Length > 63)
            return false;

        // Check first and last character restrictions
        if (!char.IsLetterOrDigit(label[0]) || !char.IsLetterOrDigit(label[^1]))
            return false;

        // Check middle characters
        for (int i = 1; i < label.Length - 1; i++)
        {
            char c = label[i];
            if (!char.IsLetterOrDigit(c) && c != '-')
                return false;
        }

        return true;
    }

    private static bool ValidateIpLiteral(ReadOnlySpan<char> ipLiteral)
    {
        // Handle IPv6 format
        if (ipLiteral.StartsWith("IPv6:"))
        {
            return IPAddress.TryParse(ipLiteral[5..].ToString(), out _);
        }

        // Handle IPv4 format
        return IPAddress.TryParse(ipLiteral.ToString(), out _);
    }

    public static bool ValidateMailbox(string email)
    {
        try
        {
            var host = email[(email.IndexOf('@') + 1)..];
            var entries = System.Net.Dns.GetHostAddresses(host);
            return entries.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    [GeneratedRegex(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture
    )]
    private static partial Regex GetEmailRegex();
}
