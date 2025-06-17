using GoogleLoginCorrecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly GoogleCalendarService _calendarService;

    public CalendarController(GoogleCalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet("events")]
    public async Task<IActionResult> GetEvents()
    {
        try
        {
            var events = await _calendarService.GetUpcomingEventsAsync();

            var simpleEvents = events.Select(e => new
            {
                Id = e.Id,
                Summary = e.Summary,
                Start = e.Start?.DateTimeDateTimeOffset ??
                        (DateTime.TryParse(e.Start?.Date, out var startDate) ? new DateTimeOffset(startDate) : null),
                End = e.End?.DateTimeDateTimeOffset ??
                      (DateTime.TryParse(e.End?.Date, out var endDate) ? new DateTimeOffset(endDate) : null),
                Description = e.Description,
                Location = e.Location,
                Status = e.Status,
            }).ToList();

            return Ok(simpleEvents);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching calendar events: {ex.Message}");
        }
    }
    public record NewEventRequest(string Summary, string Description, DateTime StartTime, DateTime EndTime);

    [HttpPost("events")]
    public async Task<IActionResult> CreateEvent([FromBody] NewEventRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Summary))
        {
            return BadRequest("Event summary cannot be empty.");
        }

        try
        {
            var createdEvent = await _calendarService.CreateEventAsync(
                request.Summary,
                request.Description,
                request.StartTime,
                request.EndTime
            );

            return Ok(new { id = createdEvent.Id, summary = createdEvent.Summary });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the event: {ex.Message}");
        }
    }
}