using Microsoft.AspNetCore.Mvc;
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
	new Complaint(0, 0, "Amazon", "lol", "chicken butt...", DateTime.Now,   5, 3),
	new Complaint(0, 0, "Amazon", "lol", "chicken butt...", DateTime.Now,   1, 0),
	new Complaint(0, 0, "Amazon", "lol", "chicken butt...", DateTime.Now,   2, 2),
	new Complaint(0, 0, "Netflix", "lol", "chicken butt...", DateTime.Now,  5, 0),
	new Complaint(0, 0, "Netflix", "lol", "chicken butt...", DateTime.Now,  5, 0),
	new Complaint(0, 0, "Google", "lol", "chicken butt...", DateTime.Now,   5, 0),
	new Complaint(0, 0, "Google", "lol", "chicken butt...", DateTime.Now,   5, 0),
	new Complaint(0, 0, "Google", "lol", "chicken butt...", DateTime.Now,   5, 0),
	new Complaint(0, 0, "Google", "lol", "chicken butt...", DateTime.Now,   5, 0),
	new Complaint(0, 0, "Zerox", "lol", "chicken butt...", DateTime.Now,    5, 0),
	new Complaint(0, 0, "Zerox", "lol", "chicken butt...", DateTime.Now,    5, 0),
	new Complaint(0, 0, "Apple", "lol", "chicken butt...", DateTime.Now,    5, 0),
	new Complaint(0, 0, "Meta", "lol", "chicken butt...", DateTime.Now,     5, 0),
	new Complaint(0, 0, "Meta", "lol", "chicken butt...", DateTime.Now,     5, 0),
};

var userDb = new List<User>()
{
	new User(0, "derp", "derp@gmail.com", "192.168.1.100", DateTime.Now),
	new User(1, "dee", "dee@gmail.com", "192.168.1.100", DateTime.Now),
	new User(2, "diddly", "diddly@gmail.com", "192.168.1.100", DateTime.Now),
};

//
// COMPLAINTS ENDPOINTS
//

var complaintsApi = app.MapGroup("/complaints");
complaintsApi.MapGet("/", () => complaintDb);
complaintsApi.MapGet("/{id}", (int id) =>
	complaintDb.FirstOrDefault(a => a.Id == id) is { } complaint
		? Results.Ok(complaint)
		: Results.NotFound());

complaintsApi.MapPost("/create", ([FromBody] ComplaintJson cData) =>
{
	var newComplaint = new Complaint(complaintDb.Max(x => x.Id) + 1, cData.UserId, cData.CompanyName, cData.Title, cData.Body, DateTime.Now, 0, 0);
	complaintDb.Add(newComplaint);
	return Results.Ok(newComplaint);
});

complaintsApi.MapGet("/by-user/{userId}", (int userId) =>
{
	var complaintsByUser = complaintDb.Where(x => x.UserId == userId).ToList();
	return Results.Ok(complaintsByUser);
});

complaintsApi.MapGet("/by-company/{companyName}", (string companyName) =>
{
	var matches = FuzzySearch.FuzzyMatch(companyName, complaintDb, x => x.CompanyName);
	return Results.Ok(matches);
});

complaintsApi.MapGet("/by-date-range", ([FromBody] DateRangeJson dateRange) =>
{
	var complaintsInRange = complaintDb.Where(x => dateRange.IsInRange(x.SubmittedOn)).ToList();
	return Results.Ok(complaintsInRange);
});

complaintsApi.MapGet("/list-companies", () =>
{
	var companies = complaintDb.Select(x => x.CompanyName).Distinct().ToList();
	return Results.Ok(companies);
});

complaintsApi.MapGet("/company-rating/{companyName}", (string companyName) =>
{
	var companyComplaints = complaintDb.Where(x => x.CompanyName.Equals(companyName));
	var rating = new CompanyRatingJson(companyComplaints);
	return Results.Ok(rating);
});

//
// USERS ENDPOINTS
//

var userEndpoints = app.MapGroup("/users");
userEndpoints.MapGet("/", () => userDb);
userEndpoints.MapPost("/create", ([FromBody] UserJson userData) =>
{
	User newUser = new User(userDb.Max(x => x.Id) + 1, userData.Username, userData.Email, userData.IpAddress, DateTime.Now);
	userDb.Add(newUser);
	return Results.Ok(newUser);
});


app.Run();

public record Complaint(int Id, int UserId, string CompanyName, string Title, string Body, DateTime SubmittedOn, int ThumbsUp, int ThumbsDown);
public record User(int Id, string Username, string Email, string IpAddress, DateTime JoinedDate);


[JsonSerializable(typeof(List<Complaint>))]
[JsonSerializable(typeof(List<User>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(CompanyRatingJson))]
[JsonSerializable(typeof(UserJson))]
[JsonSerializable(typeof(ComplaintJson))]
[JsonSerializable(typeof(DateRangeJson))]
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
	public required string CompanyName { get; set; }
	public required string Title { get; set; }
	public required string Body { get; set; }
}

public class DateRangeJson
{
	public required DateTime StartTime { get; set; }
	public required DateTime EndTime { get; set; }

	public bool IsInRange(DateTime time)
	{
		var c1 = DateTime.Compare(StartTime, time);
		var c2 = DateTime.Compare(EndTime, time);
		return c1 >= 0 && c2 <= 0;
	}
}

public class CompanyRatingJson
{
	public int ThumbsUp { get; set; }
	public int ThumbsDown { get; set; }
	public double StarRating { get => calcStarRating(); }

	public CompanyRatingJson(IEnumerable<Complaint> complaints)
	{

		ThumbsUp = complaints.Sum(x => x.ThumbsUp);
		ThumbsDown = complaints.Sum(x => x.ThumbsDown);
	}

	private double calcStarRating()
	{
		int totalVotes = ThumbsUp + ThumbsDown;
		if (totalVotes == 0)
		{
			return 0.0;
		}

		double upRatio = (double)ThumbsUp / totalVotes;
		return upRatio * 5.0;
	}
}

public static class FuzzySearch
{
	public static List<T> FuzzyMatch<T>(
		string searchTerm, List<T> list,
		Func<T, string> propertySelector,
		int maxDistance = 2, bool ignoreCase = true)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list));

		// Return all items if the search term is empty or null
		if (string.IsNullOrEmpty(searchTerm))
			return new List<T>(list);

		// Preprocess the search term
		string processedSearchTerm = ignoreCase ?
			searchTerm.Trim().ToLowerInvariant() :
			searchTerm.Trim();

		var resultsWithDistance = new List<Tuple<T, int>>();

		foreach (var item in list)
		{
			var prop = propertySelector(item);
			if (item == null || prop.GetType() != typeof(string))
				continue;

			string? itemStr = prop as string;
			string processedItem = ignoreCase ?
				itemStr!.Trim().ToLowerInvariant() :
				itemStr!.Trim();

			int distance = LevenshteinDistance(processedSearchTerm, processedItem);
			if (distance <= maxDistance)
			{
				resultsWithDistance.Add(Tuple.Create(item, distance));
			}
		}

		return resultsWithDistance
			.OrderBy(t => t.Item2)
			.Select(t => t.Item1)
			.ToList();
	}

	private static int LevenshteinDistance(string a, string b)
	{
		if (string.IsNullOrEmpty(a))
			return string.IsNullOrEmpty(b) ? 0 : b.Length;
		if (string.IsNullOrEmpty(b))
			return a.Length;

		// Use a single array to store costs, reducing space complexity
		int[] costs = new int[b.Length + 1];
		for (int i = 0; i <= b.Length; i++)
			costs[i] = i;

		for (int i = 1; i <= a.Length; i++)
		{
			costs[0] = i;
			int previousDiagonal = i - 1;
			for (int j = 1; j <= b.Length; j++)
			{
				int temp = costs[j];
				if (a[i - 1] == b[j - 1])
				{
					costs[j] = previousDiagonal;
				}
				else
				{
					costs[j] = Math.Min(Math.Min(costs[j - 1], costs[j]), previousDiagonal) + 1;
				}
				previousDiagonal = temp;
			}
		}

		return costs[b.Length];
	}
}