using MailPazarlama.Business.Common;
using MailPazarlama.Business.DTOs.User;

namespace MailPazarlama.Business.Services.User;

public interface IUserService
{
    Task<ServiceResult<UserDto>> RegisterAsync(UserRegisterDto dto);
    Task<ServiceResult<UserDto>> LoginAsync(UserLoginDto dto);
    Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto);
    Task<IReadOnlyList<UserDto>> GetAllUsersAsync();
    Task<ServiceResult<UserDto>> GetByIdAsync(int id);
    Task<ServiceResult> ToggleStatusAsync(int userId);
    Task<ServiceResult> UpdateProfileAsync(int userId, UserProfileDto dto);
}
