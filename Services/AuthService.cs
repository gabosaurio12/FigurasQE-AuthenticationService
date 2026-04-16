using FigurasQE_AuthenticationService.Data.Repositories;
using FigurasQE_AuthenticationService.Models;


namespace FigurasQE_AuthenticationService.Services
{
    public class AuthService
    {
        private readonly JwtService JwtProvider;
        private readonly UserRepository UserRepo;

        public AuthService(JwtService jwt, UserRepository dao)
        {
            JwtProvider = jwt;
            UserRepo = dao;
        }

        public async Task<string> Signup(RegisterRequest user)
        {
            var created = await UserRepo.RegisterUserAsync(user);
            if (!created) throw new Exception("Error creating user");

            var loginUser = await UserRepo.GetUserWithCredentialsAsync(user.Email);
            return JwtProvider.EncodeToken(loginUser);
        }

        public async Task<string> Login(LoginRequest user)
        {
            var loginUser = await UserRepo.GetUserWithCredentialsAsync(user.Email);

            if (loginUser != null && BCrypt.Net.BCrypt.Verify(user.Password, loginUser.Password))
            {
                loginUser.Password = user.Password;
                return JwtProvider.EncodeToken(loginUser);
            }
            throw new UnauthorizedAccessException("Invalid credentials");
        }

    }
}