namespace Application.Workflows.ReWoo.Dto;

public class RequestInputsDto(string message)
{
    public string Message { get; } = message;
}