

using Gripe.Api.Dtos;

public static class InMemoryDatabases
{
    public static List<ComplaintDto> ComplaintDb = [
        new(0, 0, "Amazon", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),   5, 3),
        new(1, 0, "Amazon", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),   1, 0),
        new(2, 0, "Amazon", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),   2, 2),
        new(3, 0, "Netflix", "lol", "chicken butt...",  DateOnly.FromDateTime(DateTime.Now),  5, 0),
        new(4, 0, "Netflix", "lol", "chicken butt...",  DateOnly.FromDateTime(DateTime.Now),  5, 0),
        new(5, 0, "Google", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),   5, 0),
        new(6, 0, "Google", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),   5, 0),
        new(7, 0, "Google", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),   5, 0),
        new(8, 0, "Google", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),   5, 0),
        new(9, 0, "Zerox", "lol", "chicken butt...",    DateOnly.FromDateTime(DateTime.Now),    5, 0),
        new(10, 0, "Zerox", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),    5, 0),
        new(11, 0, "Apple", "lol", "chicken butt...",   DateOnly.FromDateTime(DateTime.Now),    5, 0),
        new(12, 0, "Meta", "lol", "chicken butt...",    DateOnly.FromDateTime(DateTime.Now),     5, 0),
        new(13, 0, "Meta", "lol", "chicken butt...",    DateOnly.FromDateTime(DateTime.Now),     5, 0),
    ];

    public static List<UserDto> UserDb = [

        new(0, "derp", "derp@gmail.com", "192.168.1.100",       DateOnly.FromDateTime(DateTime.Now)),
        new(1, "dee", "dee@gmail.com", "192.168.1.100",         DateOnly.FromDateTime(DateTime.Now)),
        new(2, "diddly", "diddly@gmail.com", "192.168.1.100",   DateOnly.FromDateTime(DateTime.Now)),
    ];
}