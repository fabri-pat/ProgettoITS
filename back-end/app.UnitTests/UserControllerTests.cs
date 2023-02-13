using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app.Controllers;
using app.Dtos;
using app.Entities;
using app.Repositories;
using app.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace app.UnitTests;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IEMailService> emailServiceMock = new Mock<IEMailService>();

    [Fact]
    public async Task GetUserAsync_WithNullUser_ReturnsNotFound()
    {
        UserController userController = new UserController(userRepositoryMock.Object, emailServiceMock.Object);

        var result = await userController.GetUserAsync(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetUserAsync_WithExistingUser_ReturnsExpectedItem()
    {
        User expectedUser = CreateRandomUser();

        userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedUser);

        UserController userController = new UserController(userRepositoryMock.Object, emailServiceMock.Object);

        var result = await userController.GetUserAsync(Guid.NewGuid());

        result.Value.Should().BeEquivalentTo(
            expectedUser.AsDto(),
            options => options.ComparingByMembers<UserDto>());
    }

    [Fact]
    public async Task GetUsersAsync_WithExistingUsers_ReturnsAllUsers()
    {
        var expectedUsers = new[] { CreateRandomUser(), CreateRandomUser(), CreateRandomUser(), CreateRandomUser() };

        userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedUsers);

        UserController userController = new UserController(userRepositoryMock.Object, emailServiceMock.Object);

        var result = await userController.GetUsersAsync();

        result.Should().BeEquivalentTo(
            expectedUsers,
            options => options.ComparingByMembers<UserDto>().ExcludingMissingMembers()
        );
    }

    [Fact]
    public async Task GetUsersAsync_WithNoUsers_ReturnEmptyArray()
    {
        userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<User>());

        UserController userController = new UserController(userRepositoryMock.Object, emailServiceMock.Object);

        var result = await userController.GetUsersAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async void RegisterUserAsync_WithCorrectRequest_ReturnsOkActionResult()
    {
        userRepositoryMock.Setup(repo => repo.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()));

        UserController userController = new UserController(userRepositoryMock.Object, emailServiceMock.Object);

        var result = await userController.RegisterUserAsync(new RegistrationRequestDto("prova", "prova", "prova", "prova@gmail.com", "Prova322!"));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async void RegisterUserAsync_WithUnorrectRequest_ThrowsException()
    {
        userRepositoryMock.Setup(repo => repo.RegisterUserAsync(It.IsAny<RegistrationRequestDto>())).Throws(new ArgumentException());
        
        UserController userController = new UserController(userRepositoryMock.Object, emailServiceMock.Object);

        await Assert.ThrowsAnyAsync<Exception>(async() => await userController.RegisterUserAsync(new RegistrationRequestDto("prova", "prova", "prova", "prova@gmail.com", "ppp")));
    }

    private User CreateRandomUser()
    {
        return new User(
            id: Guid.NewGuid(),
            name: Guid.NewGuid().ToString(),
            surname: Guid.NewGuid().ToString(),
            username: Guid.NewGuid().ToString(),
            email: Guid.NewGuid().ToString(),
            role: Role.Guest,
            password: new PasswordService().CreatePassword(Guid.NewGuid().ToString())
        );
    }
}