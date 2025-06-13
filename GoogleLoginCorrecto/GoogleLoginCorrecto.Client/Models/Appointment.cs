
public class Appointment
{
    // Propiedades básicas que necesita DevExpress
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Subject { get; set; }
    public bool AllDay { get; set; }
    public int StatusId { get; set; } // Para el estado (Libre, Ocupado, etc.)
    public int LabelId { get; set; }  // Para las etiquetas de color

    // Propiedades adicionales que puedes mapear
    public string? Description { get; set; }
    public string? Location { get; set; }

    // Propiedades para identificar el evento original de Google
    public string? GoogleEventId { get; set; }
}