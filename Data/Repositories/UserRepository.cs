using FigurasQE_AuthenticationService.Data.Entities;
using FigurasQE_AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;


namespace FigurasQE_AuthenticationService.Data.Repositories;

public class UserRepository
{

    private readonly AppDbContext Context;

    public UserRepository(AppDbContext context)
    {
        Context = context;
    }

    public async Task<AuthUser> GetUserWithCredentialsAsync(string email)
    {
        var student = await Context.Students.FirstOrDefaultAsync(u => u.Email == email);
        if (student != null)
            return MapStudentToUserRequest(student);

        var tutor = await Context.Tutors.FirstOrDefaultAsync(u => u.Email == email);
        if (tutor != null)
            return MapTutorToUserRequest(tutor);

        return null;
    }

    public async Task<bool> RegisterUserAsync(RegisterRequest user)
    {
        if (user.Role.Equals("student"))
        {
            var student = new Student
            {
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Age = user.Age,
                Genre = user.Genre,
                Country = user.Country,
                Neurodivergency = user.Neurodivergency
            };
            await Context.Students.AddAsync(student);
        }
        else
        {
            var tutor = new Tutor
            {
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password)
            };
            await Context.Tutors.AddAsync(tutor);
        }

        await Context.SaveChangesAsync();
        return true;
    }

    private AuthUser MapStudentToUserRequest(Student student)
    {
        return new AuthUser
        {
            Id = student.IdStudent,
            Email = student.Email,
            Password = student.PasswordHash,
            Role = "student"
        };
    }

    private AuthUser MapTutorToUserRequest(Tutor tutor)
    {
        return new AuthUser
        {
            Id = tutor.IdTutor,
            Email = tutor.Email,
            Password = tutor.PasswordHash,
            Role = "tutor"
        };
    }
}