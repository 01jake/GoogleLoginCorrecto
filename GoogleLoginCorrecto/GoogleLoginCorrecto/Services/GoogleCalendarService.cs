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
        request.TimeMin = DateTime.Now;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = maxResults;

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

        // Creamos el nuevo objeto de evento de Google
        var newEvent = new Event()
        {
            Summary = summary,
            Description = description,
            Start = new EventDateTime()
            {
                // Es importante especificar la zona horaria.
               
                DateTimeDateTimeOffset = new DateTimeOffset(startTime, TimeSpan.Zero).ToUniversalTime(),
                TimeZone = "UTC" // O puedes intentar obtener la del usuario
            },
            End = new EventDateTime()
            {
                DateTimeDateTimeOffset = new DateTimeOffset(endTime, TimeSpan.Zero).ToUniversalTime(),
                TimeZone = "UTC"
            }
          
        };

        // Creamos la petición de inserción
        var request = calendarService.Events.Insert(newEvent, "primary");

        // Ejecutamos la petición y devolvemos el evento creado 
        Event createdEvent = await request.ExecuteAsync();
        return createdEvent;
    }
}