using Gripe.Api.Dtos;
using Gripe.Api.Utils;
using Microsoft.AspNetCore.Mvc;

public static class ComplaintEndpoints
{
    private const string GetComplaintByIdName = "GetComplaintById";
    private static List<ComplaintDto> _complaintDb = InMemoryDatabases.ComplaintDb;


    public static RouteGroupBuilder MapComplaintEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/complaints")
                        .WithParameterValidation();

        group.MapGet("/", GetAllComplaints);
        group.MapGet("/{id}", GetComplaintById).WithName(GetComplaintByIdName);
        group.MapPost("/", CreateComplaint);
        group.MapPost("/thumbs-up/{complaintId}", ThumbsUpComplaint);
        group.MapPost("/thumbs-down/{complaintId}", ThumbsDownComplaint);
        group.MapGet("/date-range", ComplaintsByDateRange);
        group.MapGet("/company", ListCompanyNames);
        group.MapGet("/company/{companyName}", ComplaintsByCompany);
        group.MapGet("/company/rating/{companyName}", GetCompanyStarRating);

        return group;
    }

    private static IResult GetAllComplaints()
    {
        return Results.Ok(_complaintDb);
    }

    private static IResult GetComplaintById(int id)
    {
        var complaint = _complaintDb.FirstOrDefault(a => a.Id == id);
        return complaint != null
            ? Results.Ok(complaint)
            : Results.NotFound();
    }

    private static IResult CreateComplaint(CreateComplaintDto cData)
    {
        ComplaintDto newComplaint = new(
            _complaintDb.Max(x => x.Id) + 1,
            cData.UserId!.Value,
            cData.CompanyName,
            cData.Title,
            cData.Body,
            DateOnly.FromDateTime(DateTime.Now),
            0, 0);

        _complaintDb.Add(newComplaint);
        var routeValues = new RouteValueDictionary
            {
                { "id", newComplaint.Id }
            };
        return Results.CreatedAtRoute(GetComplaintByIdName, routeValues, newComplaint);
    }

    private static IResult ThumbsUpComplaint(int complaintId)
    {
        var idx = _complaintDb.FindIndex(x => x.Id == complaintId);
        if (idx == -1) return Results.NotFound();

        _complaintDb[idx] = new(
            _complaintDb[idx].Id,
            _complaintDb[idx].UserId,
            _complaintDb[idx].CompanyName,
            _complaintDb[idx].Title,
            _complaintDb[idx].Body,
            _complaintDb[idx].SubmittedOn,
            _complaintDb[idx].ThumbsUp + 1,
            _complaintDb[idx].ThumbsDown
        );
        return Results.Ok(_complaintDb[idx]);
    }

    private static IResult ThumbsDownComplaint(int complaintId)
    {
        var idx = _complaintDb.FindIndex(x => x.Id == complaintId);
        if (idx == -1) return Results.NotFound();

        _complaintDb[idx] = new(
            _complaintDb[idx].Id,
            _complaintDb[idx].UserId,
            _complaintDb[idx].CompanyName,
            _complaintDb[idx].Title,
            _complaintDb[idx].Body,
            _complaintDb[idx].SubmittedOn,
            _complaintDb[idx].ThumbsUp,
            _complaintDb[idx].ThumbsDown + 1
        );
        return Results.Ok(_complaintDb[idx]);
    }

    private static IResult ComplaintsByDateRange([FromBody] DateRangeDto range)
    {
        var complaintsInRange = _complaintDb
            .Where(x => range.IsInDateRange(x.SubmittedOn)).ToList();
        return Results.Ok(complaintsInRange);
    }

    private static IResult ListCompanyNames()
    {
        var companies = _complaintDb.Select(x => x.CompanyName).Distinct().ToList();
        return Results.Ok(companies);
    }

    private static IResult ComplaintsByCompany(string companyName)
    {
        var matches = FuzzySearch.MatchAll(companyName, _complaintDb, x => x.CompanyName);
        return Results.Ok(matches);
    }

    private static IResult GetCompanyStarRating(string companyName)
    {
        var companyComplaints = _complaintDb.Where(x => x.CompanyName.Equals(companyName));
        var rating = companyComplaints.CalcStarRating();
        return Results.Ok(rating);
    }
}