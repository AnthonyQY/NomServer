using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using NomServer.Application.DTOs;
using NomServer.Application.Interfaces;
using NomServer.Application.Models;
using NomServer.Core.Entities;

namespace NomServer.Application.Services;

public class AuthService(IUserService userService, ITokenService tokenService) : IAuthService
{
    public async Task<AuthBundle> RegisterAsync(string name, IEnumerable<string> roles)
    {
        if (await userService.ExistsByNameAsync(name))
        {
            throw new Exception("User name already exists");
        }
        
        var recoveryCode = GenerateRecoveryCode();
        var (hash, salt) = await HashRecoveryCodeAsync(recoveryCode);

        await userService.CreateAsync(new User
        {
            Name = name,
            IsActive = true,
            RecoveryCodeHash = hash,
            RecoveryCodeSalt = Convert.ToBase64String(salt)
        });

        return new AuthBundle
        {
            JwtToken = tokenService.GenerateToken(name, roles.ToList()),
            RecoveryCode = recoveryCode
        };
    }

    public async Task<AuthBundle> RecoverAsync(string name, string code)
    {
        var user = await userService.GetByNameAsync(name);
        if (user == null)
            throw new Exception("User not found");

        var salt = Convert.FromBase64String(user.RecoveryCodeSalt ?? throw new Exception("Missing salt"));
        if (!await VerifyRecoveryCodeAsync(code, user.RecoveryCodeHash, salt))
            throw new Exception("Invalid recovery code");

        var newRecoveryCode = GenerateRecoveryCode();
        var (newHash, newSalt) = await HashRecoveryCodeAsync(newRecoveryCode);

        user.RecoveryCodeHash = newHash;
        user.RecoveryCodeSalt = Convert.ToBase64String(newSalt);
        await userService.UpdateAsync(user.Id, user);

        return new AuthBundle
        {
            JwtToken = tokenService.GenerateToken(name, user.Roles),
            RecoveryCode = newRecoveryCode
        };
    }

    private static string GenerateRecoveryCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var codeChars = new char[6];
        using var rng = RandomNumberGenerator.Create();
        for (var i = 0; i < codeChars.Length; i++)
            codeChars[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        return new string(codeChars);
    }

    private static async Task<(string hash, byte[] salt)> HashRecoveryCodeAsync(string code)
    {
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(code))
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            Iterations = 4,
            MemorySize = 1024 * 16
        };

        var hash = await argon2.GetBytesAsync(32);
        return (Convert.ToBase64String(hash), salt);
    }

    private static async Task<bool> VerifyRecoveryCodeAsync(string code, string storedHash, byte[] salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(code))
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            Iterations = 4,
            MemorySize = 1024 * 16
        };

        var hash = await argon2.GetBytesAsync(32);
        return Convert.ToBase64String(hash) == storedHash;
    }
}
