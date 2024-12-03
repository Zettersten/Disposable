![Disposable Icon](https://raw.githubusercontent.com/Zettersten/Disposable/main/icon.png)

# Disposable 📧

[![NuGet version](https://badge.fury.io/nu/Disposable.Email.svg)](https://badge.fury.io/nu/Disposable.Email)

A high-performance .NET library for validating disposable/temporary email domains. Built with performance and efficiency in mind, it provides a simple way to check if an email address or domain is from a known disposable email provider.

## Overview

The **Disposable** library offers:

- **High Performance**: Uses `Span<T>` and aggressive inlining for optimal performance
- **Memory Efficient**: Operates on string spans to minimize allocations
- **Up-to-date Domain List**: Uses the comprehensive domain list from [disposable/disposable](https://github.com/disposable/disposable)
- **Simple API**: Just two static methods to validate emails or domains
- **Zero Dependencies**: No external package dependencies
- **Lightweight**: Minimal memory footprint

## Getting Started

### Installation

Install the Disposable package:

```sh
dotnet add package Disposable
```

### Basic Usage

```csharp
using Disposable;

// Check an email address
bool isDisposable = EmailValidator.IsDisposable("user@tempmail.com");

// Check a domain directly
bool isDomainDisposable = DomainValidator.IsDisposable("tempmail.com");
```

## Features

### Email Validation

The `EmailValidator` class provides methods to validate if an email address uses a disposable domain:

```csharp
// Using string
bool result1 = EmailValidator.IsDisposable("user@example.com");

// Using ReadOnlySpan<char> for better performance
ReadOnlySpan<char> email = "user@example.com";
bool result2 = EmailValidator.IsDisposable(email);
```

### Domain Validation

The `DomainValidator` class allows direct domain checking:

```csharp
// Using string
bool result1 = DomainValidator.IsDisposable("example.com");

// Using ReadOnlySpan<char> for better performance
ReadOnlySpan<char> domain = "example.com";
bool result2 = DomainValidator.IsDisposable(domain);
```

## Performance Considerations

- Uses `ReadOnlySpan<char>` to minimize allocations
- Aggressive inlining with `MethodImplOptions.AggressiveInlining`
- Efficient domain parsing and validation
- Zero heap allocations for common operations

## Domain List Source

The disposable domain list is generated from the [disposable/disposable](https://github.com/disposable/disposable) project, specifically using their [domains.txt](https://raw.githubusercontent.com/disposable/disposable-email-domains/master/domains.txt) file. This ensures the library stays current with the latest known disposable email providers.

## Requirements

- .NET 7.0 or higher
- C# 11.0 or higher

## License

This library is available under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues and feature requests, please open an issue in the GitHub repository.

---

Thank you for using **Disposable**. We look forward to seeing how you use it in your projects!
