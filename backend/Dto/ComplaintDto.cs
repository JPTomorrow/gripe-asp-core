using System.ComponentModel.DataAnnotations;

namespace Gripe.Api.Dtos;


public record class ComplaintDto(
    int Id,
    int UserId,
    string CompanyName,
    string Title,
    string Body,
    DateOnly SubmittedOn,
    int ThumbsUp,
    int ThumbsDown
);

public record class CreateComplaintDto(
    [Required] int? UserId,
    [Required][StringLength(50)] string CompanyName,
    [Required][StringLength(200, MinimumLength = 3)] string Title,
    [Required][StringLength(4000)] string Body
);


// STAR RATINGS

public record class CompanyRatingDto(
    double StarRating
);

public record class DateRangeDto(
    [Required] DateOnly StartDate,
    [Required] DateOnly EndDate
);

public static class ComplaintExtensions
{
    public static CompanyRatingDto CalcStarRating(this IEnumerable<ComplaintDto> complaints)
    {
        var thumbsUp = complaints.Sum(x => x.ThumbsUp);
        var thumbsDown = complaints.Sum(x => x.ThumbsDown);
        int totalVotes = thumbsUp + thumbsDown;
        if (totalVotes == 0)
        {
            return new CompanyRatingDto(0.0);
        }

        double upRatio = (double)thumbsUp / totalVotes;
        return new CompanyRatingDto(upRatio * 5.0);
    }

    public static bool IsInDateRange(this DateRangeDto range, DateOnly date)
    {
        var c1 = DateTime.Compare(
            range.StartDate.ToDateTime(TimeOnly.MinValue),
            date.ToDateTime(TimeOnly.MinValue));
        var c2 = DateTime.Compare(
            range.EndDate.ToDateTime(TimeOnly.MinValue),
            date.ToDateTime(TimeOnly.MinValue));

        return c1 <= 0 && c2 >= 0;
    }
}