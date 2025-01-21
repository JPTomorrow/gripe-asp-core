using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json.Serialization;

/*
 * GRIPE - An app for complaining about companies.
 * Backend
 */

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var complaintDb = new List<Complaint>()
{

};

var userDb = new List<User>()
{

};

// COMPLAINTS ENDPOINTS

var complaintsApi = app.MapGroup("/complaints");
complaintsApi.MapGet("/", () => complaintDb);
complaintsApi.MapGet("/{id}", (int id) =>
	complaintDb.FirstOrDefault(a => a.Id == id) is { } complaint
		? Results.Ok(complaint)
		: Results.NotFound());

complaintsApi.MapPost("/create", ([FromBody] ComplaintJson cData) =>
{
	var newComplaint = new Complaint(complaintDb.Max(x => x.Id) + 1, cData.UserId, cData.Title, cData.Body, DateOnly.FromDateTime(DateTime.Now), cData.StarRating);
	complaintDb.Add(newComplaint);
	return Results.Ok(newComplaint);
});

complaintsApi.MapGet("/by-user/{userId}", ([FromBody] int userId) => {
	return complaintDb.Where(x => x.UserId == userId).ToList();
});

complaintsApi.MapGet("/by-company/{companyName}", ([FromBody] string companyName) =>
{
	// fuzzy search companyName 

});

complaintsApi.MapGet("/by-date-range", ([FromBody] DateTime fromDate, [FromBody] DateTime toDate) =>
{

});


// USERS ENDPOINTS

var userEndpoints = app.MapGroup("/users");
userEndpoints.MapGet("/", () => userDb);
userEndpoints.MapPost("/create", ([FromBody] UserJson userData) => 
{
	User newUser = new User(userDb.Max(x => x.Id) + 1, userData.Username, userData.Email, userData.IpAddress, DateOnly.FromDateTime(DateTime.Now));
	userDb.Add(newUser);
	return Results.Ok(newUser);
});


app.Run();

public record Complaint(int Id, int UserId, string Title, string Body, DateOnly SubmittedOn, int StarRating);
public record User(int Id, string Username, string Email, string IpAddress, DateOnly JoinedDate);


[JsonSerializable(typeof(List<Complaint>))]
[JsonSerializable(typeof(List<User>))]
[JsonSerializable(typeof(UserJson))]
[JsonSerializable(typeof(ComplaintJson))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }

public class UserJson
{
	public required string Username { get; set; }
	public required string Email { get; set; }
	public required string IpAddress { get; set; }
}

public class ComplaintJson
{
	public required int UserId { get; set; }
	public required string Title { get; set; }
	public required string Body { get; set; }
	public required int StarRating { get; set; }
}