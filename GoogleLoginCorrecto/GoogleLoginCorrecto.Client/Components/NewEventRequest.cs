namespace GoogleLoginCorrecto.Client.Components
{
    public class NewEventRequest
    {
        public string Summary { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
