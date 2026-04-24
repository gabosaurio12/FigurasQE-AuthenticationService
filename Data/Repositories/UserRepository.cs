using System.IO.Compression;
using FigurasQE_AuthenticationService.Data.Entities;
using FigurasQE_AuthenticationService.Data;
using FigurasQE_AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;


namespace FigurasQE_AuthenticationService.Data.Repositories;

public class UserRepository
{

    private readonly FigurasqeContext Context;

    public UserRepository(FigurasqeContext context)
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
            if (await Context.Students.FirstOrDefaultAsync(s => s.Email == user.Email) == null)
            {
                var student = new Student
                {
                    Name = user.Name,
                    Email = user.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    Age = user.Age,
                    Genre = user.Genre,
                    Country = user.Country,
                    Neurodivergency = user.Neurodivergency
                };
                await Context.Students.AddAsync(student);
            }
        }
        else
        {
            if (await Context.Tutors.FirstOrDefaultAsync(t => t.Email == user.Email) == null)
            {
                var tutor = new Tutor
                {
                    Name = user.Name,
                    Email = user.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    Country = user.Country
                };
                await Context.Tutors.AddAsync(tutor);
            }
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