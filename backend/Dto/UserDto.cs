using System.ComponentModel.DataAnnotations;

namespace Gripe.Api.Dtos;

public record class UserDto(
    int Id,
    string Username,
    string Email,
    string IpAddress,
    DateOnly JoinedDate
);

public record class CreateUserDto(
    [Required][StringLength(100, MinimumLength = 1)] string Username,
    [Required][EmailAddress] string Email,
    [Required] string IpAddress
);