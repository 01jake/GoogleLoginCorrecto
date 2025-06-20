using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace GoogleLoginCorrecto.Services;

public class GoogleCalendarService
{
    private readonly IGoogleAuthProvider _auth;

    public GoogleCalendarService(IGoogleAuthProvider auth)
    {
        _auth = auth;
    }

    public async Task<IList<Event>> GetUpcomingEventsAsync(int maxResults = 10)
    {
        
        var credential = await _auth.GetCredentialAsync();

        if (credential is null)
        {
            throw new InvalidOperationException("Could not retrieve Google credentials for the user. Is the user authenticated?");
        }

        var calendarService = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "GoogleLoginCorrecto"
        });

        var request = calendarService.Events.List("primary");
        var today = DateTime.Now;
        request.TimeMinDateTimeOffset = new DateTime(today.Year, today.Month, 1);

        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 250;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        Events events = await request.ExecuteAsync();
        return events.Items;
    }
    public async Task<Event> CreateEventAsync(string summary, string description, DateTime startTime, DateTime endTime)
    {
        var credential = await _auth.GetCredentialAsync();
        if (credential is null)
        {
            throw new InvalidOperationException("Could not retrieve Google credentials.");
        }

        var calendarService = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "GoogleLoginCorrecto"
        });

    
        var newEvent = new Event()
        {
            Summary = summary,
            Description = description,
            Start = new EventDateTime()
            {
              
            
                // La API de Google prefiere este formato.
                DateTime = startTime.ToUniversalTime()
            },
            End = new EventDateTime()
            {
                // Hacemos lo mismo para la fecha de fin.
                DateTime = endTime.ToUniversalTime()
            }
        };

        var request = calendarService.Events.Insert(newEvent, "primary");

        Event createdEvent = await request.ExecuteAsync();
        return createdEvent;
    }
}