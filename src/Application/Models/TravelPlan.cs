namespace Application.Models;

public class TravelPlan
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? FlightPlanId { get; set; }
    public string? HotelPlanId { get; set; }

    public bool FlightsBooked => !string.IsNullOrEmpty(FlightPlanId);
    public bool HotelsBooked => !string.IsNullOrEmpty(HotelPlanId);
}

public class TravelPlanSummary(TravelPlan plan)
{
    public bool HasOrigin { get; set; } = !string.IsNullOrEmpty(plan.Origin);
    public bool HasDestination { get; set; } = !string.IsNullOrEmpty(plan.Destination);
    public bool HasDates { get; set; } = plan.StartDate.HasValue && plan.EndDate.HasValue;
    public bool HasFlightPlan { get; set; } = !string.IsNullOrEmpty(plan.FlightPlanId);
    public bool HasHotelPlan { get; set; } = !string.IsNullOrEmpty(plan.HotelPlanId);
}



