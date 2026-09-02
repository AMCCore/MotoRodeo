using System.Security.Cryptography;
using System.Text;

namespace MotoRodeo.BL.Services;

/// <summary>
/// Генерация одноразовых паролей.
/// </summary>
public static class PasswordGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";

    /// <summary>
    /// Генерирует случайный пароль заданной длины.
    /// </summary>
    /// <param name="length">Длина пароля.</param>
    /// <returns>Пароль в открытом виде.</returns>
    public static string Generate(int length = 12)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            sb.Append(Alphabet[bytes[i] % Alphabet.Length]);
        }

        return sb.ToString();
    }
}
