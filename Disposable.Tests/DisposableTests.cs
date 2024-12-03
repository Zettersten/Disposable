namespace Disposable.Tests;

public class DisposableTests
{
    [Theory]
    [InlineData("user@gmail.com", false)]
    [InlineData("test@tempmail.com", true)]
    [InlineData("user@disposable.com", true)]
    [InlineData("test@outlook.com", false)]
    public void EmailValidator_ShouldValidateCommonDomains(string email, bool expected)
    {
        Assert.Equal(expected, EmailValidator.IsDisposable(email));
    }

    [Theory]
    [InlineData("gmail.com", false)]
    [InlineData("tempmail.com", true)]
    [InlineData("disposable.com", true)]
    [InlineData("outlook.com", false)]
    public void DomainValidator_ShouldValidateCommonDomains(string domain, bool expected)
    {
        Assert.Equal(expected, DomainValidator.IsDisposable(domain));
    }

    [Theory]
    [InlineData("")]
    [InlineData("@")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user@@domain.com")]
    public void EmailValidator_ShouldReturnTrue_ForInvalidEmails(string email)
    {
        Assert.True(EmailValidator.IsDisposable(email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(".")]
    [InlineData("domain")]
    [InlineData(".com")]
    [InlineData("domain.")]
    public void DomainValidator_ShouldReturnTrue_ForInvalidDomains(string domain)
    {
        Assert.True(DomainValidator.IsDisposable(domain));
    }

    [Theory]
    [InlineData("user@subdomain.domain.com")]
    [InlineData("user@multi.level.domain.com")]
    public void EmailValidator_ShouldHandleSubdomains(string email)
    {
        var result = EmailValidator.IsDisposable(email);
        Assert.False(result);
    }

    [Theory]
    [InlineData("subdomain.domain.com")]
    [InlineData("multi.level.domain.com")]
    public void DomainValidator_ShouldHandleSubdomains(string domain)
    {
        var result = DomainValidator.IsDisposable(domain);
        Assert.False(result);
    }

    [Fact]
    public void EmailValidator_ShouldHandleSpanInput()
    {
        ReadOnlySpan<char> email = "user@gmail.com";
        Assert.False(EmailValidator.IsDisposable(email));
    }

    [Fact]
    public void DomainValidator_ShouldHandleSpanInput()
    {
        ReadOnlySpan<char> domain = "gmail.com";
        Assert.False(DomainValidator.IsDisposable(domain));
    }

    [Fact]
    public void EmailValidator_ShouldHandleLongEmails()
    {
        var longEmail = new string('a', 64) + "@" + new string('b', 255) + ".com";
        Assert.False(EmailValidator.IsDisposable(longEmail));
    }

    [Fact]
    public void DomainValidator_ShouldHandleLongDomains()
    {
        var longDomain = new string('a', 255) + ".com";
        Assert.False(DomainValidator.IsDisposable(longDomain));
    }

    [Theory]
    [InlineData("USER@GMAIL.COM")]
    [InlineData("user@GMAIL.com")]
    [InlineData("User@gmail.COM")]
    public void EmailValidator_ShouldHandleMixedCase(string email)
    {
        Assert.False(EmailValidator.IsDisposable(email));
    }

    [Theory]
    [InlineData("GMAIL.COM")]
    [InlineData("Gmail.com")]
    [InlineData("gmail.COM")]
    public void DomainValidator_ShouldHandleMixedCase(string domain)
    {
        Assert.False(DomainValidator.IsDisposable(domain));
    }

    [Fact]
    public void EmailValidator_ShouldHandleUnicodeEmails()
    {
        Assert.False(EmailValidator.IsDisposable("user@bücher.com"));
    }

    [Fact]
    public void DomainValidator_ShouldHandleUnicodeDomains()
    {
        Assert.False(DomainValidator.IsDisposable("bücher.com"));
    }
}
