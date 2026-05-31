using System.Security.Claims;
using CompetenceAssessment.Domain.UserManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;

namespace CompetenceAssessment.Application.UserManagement;

public class AuthService: IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AuthService(IUserRepository userRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> GetCurrentUserAsync(CancellationToken token = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;
        
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return null;
            
            var user = await _userRepository.GetUserAsync(new UserQuery(id: userId), token);
            
            if (user != null)
            {
                var roles = httpContext.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
                user.Roles = roles;
            }
        
            return user;
        }

        public async Task<LoginResult> LoginAsync(string email, string password, CancellationToken token = default)
        {
            var user = await _userRepository.GetUserAsync(new UserQuery(email: email), token);
            if (user == null)
            {
                return LoginResult.Failed("Неверный логин или пароль");
            }
            
            var isValidPassword = await _userRepository.ValidatePasswordAsync(user.Id, password, token);
            if (!isValidPassword)
            {
                return LoginResult.Failed("Неверный логин или пароль");
            }
            
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return LoginResult.Failed("Ошибка при авторизации");
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim("FullName", user.FullName),
                new Claim("Position", user.Position),
                new Claim("Department", user.Department),
                new Claim("DepartmentId", user.DepartmentId.ToString()),
                new Claim("PositionId", user.PositionId.ToString())
            };
            
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            
            await httpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                AllowRefresh = true
            });
            
            return LoginResult.Success(user.Id, user.Email,  user.FullName, user.Roles);
        }

        public async Task LogoutAsync(CancellationToken token = default)
        {
           var httpContext = _httpContextAccessor.HttpContext;
           if (httpContext != null)
           {
               await httpContext.SignOutAsync("Cookies");
           }
        }
}