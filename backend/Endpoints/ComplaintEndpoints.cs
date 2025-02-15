using Gripe.Api.Dtos;
using Gripe.Api.Utils;
using Microsoft.AspNetCore.Mvc;

public static class ComplaintEndpoints
{
    private const string GetComplaintByIdName = "GetComplaintById";

    public static RouteGroupBuilder MapComplaintEndpoints(this WebApplication app)
    {
        var complaintDb = InMemoryDatabases.ComplaintDb;
        var group = app.MapGroup("/complaints")
                        .WithParameterValidation();

        // get all complaints
        group.MapGet("/", () =>
        {
            return Results.Ok(complaintDb);
        });

        // get complaint by ID
        group.MapGet("/{id}", (int id) =>
        {
            var complaint = complaintDb.FirstOrDefault(a => a.Id == id);
            return complaint != null
                ? Results.Ok(complaint)
                : Results.NotFound();
        }).WithName(GetComplaintByIdName);

        group.MapPost("/", (CreateComplaintDto cData) =>
        {
            ComplaintDto newComplaint = new(
                complaintDb.Max(x => x.Id) + 1,
                cData.UserId!.Value,
                cData.CompanyName,
                cData.Title,
                cData.Body,
                DateOnly.FromDateTime(DateTime.Now),
                0, 0);

            complaintDb.Add(newComplaint);
            var routeValues = new RouteValueDictionary
            {
                { "id", newComplaint.Id }
            };
            return Results.CreatedAtRoute(GetComplaintByIdName, routeValues, newComplaint);
        });

        group.MapPost("/thumbs-up/{complaintId}", (int complaintId) =>
        {
            var idx = complaintDb.FindIndex(x => x.Id == complaintId);
            if (idx == -1) return Results.NotFound();

            complaintDb[idx] = new(
                complaintDb[idx].Id,
                complaintDb[idx].UserId,
                complaintDb[idx].CompanyName,
                complaintDb[idx].Title,
                complaintDb[idx].Body,
                complaintDb[idx].SubmittedOn,
                complaintDb[idx].ThumbsUp + 1,
                complaintDb[idx].ThumbsDown
            );
            return Results.Ok(complaintDb[idx]);
        });

        group.MapPost("/thumbs-down/{complaintId}", (int complaintId) =>
        {
            var idx = complaintDb.FindIndex(x => x.Id == complaintId);
            if (idx == -1) return Results.NotFound();

            complaintDb[idx] = new(
                complaintDb[idx].Id,
                complaintDb[idx].UserId,
                complaintDb[idx].CompanyName,
                complaintDb[idx].Title,
                complaintDb[idx].Body,
                complaintDb[idx].SubmittedOn,
                complaintDb[idx].ThumbsUp,
                complaintDb[idx].ThumbsDown + 1
            );
            return Results.Ok(complaintDb[idx]);
        });

        // get all complaints within a date range
        group.MapGet("/date-range", ([FromBody] DateRangeDto range) =>
        {
            var complaintsInRange = complaintDb
                .Where(x => range.IsInDateRange(x.SubmittedOn)).ToList();
            return Results.Ok(complaintsInRange);
        });

        group.MapGet("/company", () =>
        {
            var companies = complaintDb.Select(x => x.CompanyName).Distinct().ToList();
            return Results.Ok(companies);
        });

        group.MapGet("/company/{companyName}", (string companyName) =>
        {
            var matches = FuzzySearch.MatchAll(companyName, complaintDb, x => x.CompanyName);
            return Results.Ok(matches);
        });

        group.MapGet("/company/rating/{companyName}", (string companyName) =>
        {
            var companyComplaints = complaintDb.Where(x => x.CompanyName.Equals(companyName));
            var rating = companyComplaints.CalcStarRating();
            return Results.Ok(rating);
        });

        return group;
    }
}