using System.Data.Common;
using Gripe.Api.Dtos;

public static class UserEndpoints
{
    private const string GetUserByIdName = "GetUserById";
    private static List<UserDto> _userDb = InMemoryDatabases.UserDb;

    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users")
                        .WithParameterValidation();

        group.MapGet("/", GetAllUsers);
        group.MapGet("/{id}", GetUserById).WithName(GetUserByIdName);
        group.MapPost("/", CreateUser);

        return group;
    }

    private static IResult GetAllUsers()
    {
        return Results.Ok(_userDb);
    }

    private static IResult GetUserById(int id)
    {
        var user = _userDb.FirstOrDefault(x => x.Id == id);
        return user != null
            ? Results.Ok(user)
            : Results.NotFound();
    }

    private static IResult CreateUser(CreateUserDto userData)
    {
        UserDto newUser = new(
            _userDb.Max(x => x.Id) + 1,
            userData.Username,
            userData.Email,
            userData.IpAddress,
            DateOnly.FromDateTime(DateTime.Now));

        _userDb.Add(newUser);
        var routeValues = new RouteValueDictionary
            {
                { "id", newUser.Id }
            };
        return Results.CreatedAtRoute(GetUserByIdName, routeValues, newUser);
    }
}