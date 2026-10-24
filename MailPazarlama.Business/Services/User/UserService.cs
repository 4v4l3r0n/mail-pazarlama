using MailPazarlama.Business.Common;
using MailPazarlama.Business.DTOs.User;
using MailPazarlama.Business.Services.Security;
using MailPazarlama.Data.UnitOfWork;
using UserEntity = MailPazarlama.Entities.User;

namespace MailPazarlama.Business.Services.User;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryptionService;

    public UserService(IUnitOfWork unitOfWork, IEncryptionService encryptionService)
    {
        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
    }

    public async Task<ServiceResult<UserDto>> RegisterAsync(UserRegisterDto dto)
    {
        if (!IsPasswordValid(dto.Password))
        {
            return ServiceResult<UserDto>.Fail("Şifre en az 8 karakter uzunluğunda olmalı; en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.");
        }

        var normalizedEmail = dto.Email.Trim().ToLower();
        var existingUsers = await _unitOfWork.Repository<UserEntity>().FindAsync(u => u.Email.ToLower() == normalizedEmail);
        if (existingUsers.Count > 0)
        {
            return ServiceResult<UserDto>.Fail("Bu e-posta adresi zaten kayıtlı.");
        }

        var user = new UserEntity
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = normalizedEmail,
            EncryptedPassword = _encryptionService.Encrypt(dto.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<UserEntity>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<UserDto>.Ok(MapToDto(user), "Kayıt işlemi başarıyla tamamlandı.");
    }

    public async Task<ServiceResult<UserDto>> LoginAsync(UserLoginDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLower();
        var users = await _unitOfWork.Repository<UserEntity>().FindAsync(u => u.Email.ToLower() == normalizedEmail);
        var user = users.FirstOrDefault();

        if (user == null)
        {
            return ServiceResult<UserDto>.Fail("Kullanıcı bulunamadı.");
        }

        if (!user.IsActive)
        {
            return ServiceResult<UserDto>.Fail("Hesabınız pasife alınmıştır.");
        }

        var decryptedPassword = _encryptionService.Decrypt(user.EncryptedPassword);
        if (decryptedPassword != dto.Password)
        {
            return ServiceResult<UserDto>.Fail("Parola yanlış.");
        }

        return ServiceResult<UserDto>.Ok(MapToDto(user), "Giriş başarılı.");
    }

    public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLower();
        var users = await _unitOfWork.Repository<UserEntity>().FindAsync(u => u.Email.ToLower() == normalizedEmail);
        var user = users.FirstOrDefault();

        if (user == null)
        {
            return ServiceResult.Fail("Kullanıcı bulunamadı.");
        }

        if (!IsPasswordValid(dto.NewPassword))
        {
            return ServiceResult.Fail("Yeni şifre en az 8 karakter uzunluğunda olmalı; en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.");
        }

        user.EncryptedPassword = _encryptionService.Encrypt(dto.NewPassword);
        _unitOfWork.Repository<UserEntity>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Şifreniz başarıyla güncellendi.");
    }

    public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Repository<UserEntity>().GetAllAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(int id)
    {
        var user = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(id);
        if (user == null)
        {
            return ServiceResult<UserDto>.Fail("Kullanıcı bulunamadı.");
        }

        return ServiceResult<UserDto>.Ok(MapToDto(user));
    }

    public async Task<ServiceResult> ToggleStatusAsync(int userId)
    {
        var user = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("Kullanıcı bulunamadı.");
        }

        user.IsActive = !user.IsActive;
        _unitOfWork.Repository<UserEntity>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        var statusText = user.IsActive ? "aktif" : "pasif";
        return ServiceResult.Ok($"Kullanıcı durumu {statusText} yapıldı.");
    }

    public async Task<ServiceResult> UpdateProfileAsync(int userId, UserProfileDto dto)
    {
        var user = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("Kullanıcı bulunamadı.");
        }

        var normalizedEmail = dto.Email.Trim().ToLower();
        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
        {
            var existingUsers = await _unitOfWork.Repository<UserEntity>().FindAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != userId);
            if (existingUsers.Count > 0)
            {
                return ServiceResult.Fail("Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");
            }
            user.Email = normalizedEmail;
        }

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();

        _unitOfWork.Repository<UserEntity>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Profil başarıyla güncellendi.");
    }

    private static bool IsPasswordValid(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);

        return hasUpper && hasLower && hasDigit;
    }

    private static UserDto MapToDto(UserEntity user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
