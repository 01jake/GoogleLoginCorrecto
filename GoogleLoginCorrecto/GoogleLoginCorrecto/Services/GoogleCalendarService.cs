// EN: GoogleLoginCorrecto/Services/GoogleCalendarService.cs
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
}