namespace Tests;

public static class Data
{
    public const string AskUserDepartureCityResponse = "Hi there! To help you find flights to Paris, could you please tell me your departure city? \r\n\r\n```json\r\n{\r\n  \"route\": \"ask_user\",\r\n  \"metadata\": {\r\n    \"reason\": \"asking for the user's departure city for flight search\"\r\n  }\r\n}\r\n```\r\n";

    public const string ReasonTripToParisDeparturePointRequired =
        "User want to plan a trip to Paris.Departure Point is required";

    public const string PlanTripToParisUserRequest = "I want to plan a trip to Paris";
}
