using Gripe.Api.Dtos;

public static class UserEndpoints
{
    private const string GetUserByIdName = "GetUserById";

    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var userDb = InMemoryDatabases.UserDb;
        var group = app.MapGroup("/users")
                        .WithParameterValidation();

        // get all users
        group.MapGet("/", () =>
        {
            return Results.Ok(userDb);
        });

        // get user by id
        group.MapGet("/{id}", (int id) =>
        {
            var user = userDb.FirstOrDefault(x => x.Id == id);
            return user != null
                ? Results.Ok(user)
                : Results.NotFound();
        }).WithName(GetUserByIdName);

        // create new user
        group.MapPost("/", (CreateUserDto userData) =>
        {
            UserDto newUser = new(
                userDb.Max(x => x.Id) + 1,
                userData.Username,
                userData.Email,
                userData.IpAddress,
                DateOnly.FromDateTime(DateTime.Now));

            userDb.Add(newUser);
            var routeValues = new RouteValueDictionary
            {
                { "id", newUser.Id }
            };
            return Results.CreatedAtRoute(GetUserByIdName, routeValues, newUser);
        });

        return group;
    }
}