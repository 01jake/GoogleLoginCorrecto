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
                      (DateTime.TryParse(e.End?.Date, out var endDate) ? new DateTimeOffset(endDate) : null)

            }).ToList();

            return Ok(simpleEvents);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching calendar events: {ex.Message}");
        }
    }
}