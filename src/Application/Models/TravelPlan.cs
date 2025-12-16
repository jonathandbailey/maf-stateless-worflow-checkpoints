using Application.Workflows.Dto;

namespace Application.Models;

public class TravelPlan
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public TravelPlanStatus TravelPlanStatus { get; set; } = TravelPlanStatus.NotStarted;

    public FlightOptionDto? SelectedFlightOption { get; set; }

    public FlightPlan FlightPlan { get; set; } = new();

    public void AddFlightSearchOption(FlightOptionSearch flightOptions)
    {
        FlightPlan.AddFlightOptions(flightOptions);
    }

    public void SelectFlightOption(FlightOption flightOption)
    {
        FlightPlan.SelectFlightOption(flightOption);
    }
}

public class FlightOptionSearch
{
    public FlightOptionSearch(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}

public class FlightPlan
{
    public FlightOptionsStatus FlightOptionsStatus { get; set; } = FlightOptionsStatus.NotCreated;

    public UserFlightOptionsStatus UserFlightOptionStatus { get; set; } = UserFlightOptionsStatus.NotSelected;

    public List<FlightOptionSearch> FlightOptions { get; set; } = new();

    public FlightOption FlightOption { get; set; }

    public void SelectFlightOption(FlightOption flightOption)
    {
        FlightOption = flightOption;
        UserFlightOptionStatus = UserFlightOptionsStatus.Selected;
    }

    public void AddFlightOptions(FlightOptionSearch flightOptions)
    {
        FlightOptions.Add(flightOptions);

        FlightOptionsStatus = FlightOptionsStatus.Created;
        UserFlightOptionStatus = UserFlightOptionsStatus.UserChoiceRequired;
    }
}

public class FlightOption
{
    public string Airline { get; set; }
    public string FlightNumber { get; set; }
    public FlightEndpoint Departure { get; set; }
    public FlightEndpoint Arrival { get; set; }
    public string Duration { get; set; }
    public FlightPrice Price { get; set; }
}

public class FlightEndpoint
{
    public string Airport { get; set; }
    public DateTime Datetime { get; set; }
}

public class FlightPrice
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}

public class TravelPlanSummary(TravelPlan plan)
{
    public string Origin { get; set; } = !string.IsNullOrEmpty(plan.Origin) ? plan.Origin : TravelPlanSummaryConstants.NotSet;

    public string Destination { get; set; } = !string.IsNullOrEmpty(plan.Destination) ? plan.Destination : TravelPlanSummaryConstants.NotSet;

    public string StartDate { get; set; } = plan.StartDate?.ToString("yyyy-MM-dd") ?? TravelPlanSummaryConstants.NotSet;

    public string EndDate { get; set; } = plan.EndDate?.ToString("yyyy-MM-dd") ?? TravelPlanSummaryConstants.NotSet;

    public string FlightOptionStatus { get; set; } = plan.FlightPlan.FlightOptionsStatus.ToString();

    public string UserFlightOptionStatus { get; set; } = plan.FlightPlan.UserFlightOptionStatus.ToString();

    public string TravelPlanStatus { get; set; } = plan.TravelPlanStatus.ToString();
}

public static class TravelPlanSummaryConstants
{
    public const string NotSet = "Not_Set";
}

public enum FlightOptionsStatus
{
    Created,
    NotCreated
}

public enum UserFlightOptionsStatus
{
    Selected,
    UserChoiceRequired,
    NotSelected
}

public enum TravelPlanStatus
{
    InProgress,
    Completed,
    Cancelled,
    NotStarted,
    Error
}

