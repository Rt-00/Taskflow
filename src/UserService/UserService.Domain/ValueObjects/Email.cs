namespace UserService.Domain.ValueObjects;

using System.Text.RegularExpressions;
using UserService.Domain.Exceptions;

// Value Object: imutável, sem identidade própria, igualdade por valor
public sealed class Email
{
    public string Value { get; }

    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("E-mail não pode ser vazio.");

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new DomainException($"E-mail inválido: {value}");

        return new Email(normalized);
    }

    // ValueObjects são iguais quando seus valores são iguais
    public override bool Equals(object? obj) =>
        obj is Email other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
