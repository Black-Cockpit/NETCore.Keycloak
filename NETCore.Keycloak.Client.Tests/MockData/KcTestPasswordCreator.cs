using PasswordGenerator;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Utility class for generating passwords with configurable character options.
/// </summary>
public static class KcTestPasswordCreator
{
    /// <summary>
    /// Generates a password based on specified configuration options.
    /// </summary>
    /// <param name="passwordLength">The length of the password to generate. Default is 16 characters.</param>
    /// <param name="lowerCase">Specifies whether to include lowercase letters. Default is true.</param>
    /// <param name="upperCase">Specifies whether to include uppercase letters. Default is true.</param>
    /// <param name="digits">Specifies whether to include digits. Default is true.</param>
    /// <param name="specialAscii">Specifies whether to include special ASCII characters. Default is true.</param>
    /// <returns>A randomly generated password as a <see cref="string"/>.</returns>
    public static string Create(int passwordLength = 16, bool lowerCase = true, bool upperCase = true,
        bool digits = true, bool specialAscii = true)
    {
        // Configure password generation settings based on provided parameters.
        var pwd = new Password
        {
            Settings = new PasswordGeneratorSettings(
                lowerCase,
                upperCase,
                digits,
                specialAscii,
                passwordLength,
                8, // Minimum number of character types to include.
                false)
        };

        // Generate and return the password.
        return pwd.Next();
    }
}
