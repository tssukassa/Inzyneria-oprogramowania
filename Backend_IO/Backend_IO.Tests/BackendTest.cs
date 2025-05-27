using Backend_IO.Data;
using Backend_IO.Services;
using Backend_IO.Models;
using Microsoft.EntityFrameworkCore;
using Backend_IO.DTO;

public class BackendTest
{
    private DbContextOptions<ApplicationDbContext> _options;

    public BackendTest()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Register_ShouldCreateUser_WhenUsernameIsUnique()
    {
        using var context = new ApplicationDbContext(_options);
        var service = new AuthService(context);
        var dto = new RegisterDto
        {
            Username = "testuser",
            Password = "Test@123",
            RoleKey = "worker123",
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var result = service.Register(dto);

        Assert.True(result);
        var user = context.Users.FirstOrDefault(u => u.Username == dto.Username);
        Assert.NotNull(user);
        Assert.Equal("Employee", user.Role);
        Assert.NotEqual(dto.Password, user.PasswordHash);
    }

    [Fact]
    public void Register_ShouldFail_WhenUsernameExists()
    {
        using var context = new ApplicationDbContext(_options);
        context.Users.Add(new User
        {
            Username = "testuser",
            PasswordHash = "somehash",
            Role = "Client",
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateTime(1990, 1, 1)
        });
        context.SaveChanges();

        var service = new AuthService(context);
        var dto = new RegisterDto
        {
            Username = "testuser",
            Password = "AnotherPass123",
            RoleKey = "partner321",
            FirstName = "Another",
            LastName = "Person",
            Email = "another@example.com",
            DateOfBirth = new DateTime(1995, 5, 5)
        };

        var result = service.Register(dto);

        Assert.False(result);
    }

    [Fact]
    public void Register_ShouldAssignClientRole_WhenRoleKeyIsInvalid()
    {
        using var context = new ApplicationDbContext(_options);
        var service = new AuthService(context);
        var dto = new RegisterDto
        {
            Username = "newuser",
            Password = "Password123!",
            RoleKey = "invalidRoleKey",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1992, 3, 15)
        };

        var result = service.Register(dto);

        Assert.True(result);
        var user = context.Users.FirstOrDefault(u => u.Username == dto.Username);
        Assert.NotNull(user);
        Assert.Equal("Client", user.Role);
    }

    [Fact]
    public void Login_ShouldSucceed_ForValidCredentials()
    {
        using var context = new ApplicationDbContext(_options);
        var authService = new AuthService(context);

        var password = "MySecret123";
        var hash = authService.HashPassword(password);

        context.Users.Add(new User
        {
            Username = "validuser",
            PasswordHash = hash,
            Role = "Client",
            FirstName = "Valid",
            LastName = "User",
            Email = "valid@example.com",
            DateOfBirth = DateTime.Today.AddYears(-20)
        });
        context.SaveChanges();

        var result = authService.Login("validuser", password);

        Assert.True(result);
    }

    [Fact]
    public void Login_ShouldFail_ForWrongPassword()
    {
        using var context = new ApplicationDbContext(_options);
        var authService = new AuthService(context);

        var correctPassword = "Correct123";
        var hash = authService.HashPassword(correctPassword);

        context.Users.Add(new User
        {
            Username = "testuser",
            PasswordHash = hash,
            Role = "Client",
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            DateOfBirth = DateTime.Today.AddYears(-25)
        });
        context.SaveChanges();

        var result = authService.Login("testuser", "WrongPassword");

        Assert.False(result);
    }

    [Fact]
    public void Login_ShouldFail_ForNonExistingUser()
    {
        using var context = new ApplicationDbContext(_options);
        var authService = new AuthService(context);

        var result = authService.Login("unknownuser", "anything");

        Assert.False(result);
    }

    [Fact]
    public void OnlyEmployeeCanAccessProtectedFunctionality()
    {
        using var context = new ApplicationDbContext(_options);
        var authService = new AuthService(context);

        var registerDto = new RegisterDto
        {
            Username = "client1",
            Password = "pass123",
            FirstName = "Client",
            LastName = "User",
            Email = "client@example.com",
            DateOfBirth = DateTime.Today.AddYears(-30),
            RoleKey = ""
        };

        authService.Register(registerDto);

        var user = context.Users.Single(u => u.Username == "client1");

        bool hasAccess = user.Role == "Employee";

        Assert.False(hasAccess);
    }
}

