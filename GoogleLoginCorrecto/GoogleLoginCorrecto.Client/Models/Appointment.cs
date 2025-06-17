
public class Appointment
{   
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Subject { get; set; }
    public bool AllDay { get; set; }
    public int StatusId { get; set; } 
    public int LabelId { get; set; }  

    public string? Description { get; set; }
    public string? Location { get; set; }

    // Propiedades para identificar el evento original de Google
    public string? GoogleEventId { get; set; }
}