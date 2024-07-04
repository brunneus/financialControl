using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FinanceControl.Infra;

public enum CommandResultStatus
{
    Unknown = 0,
    Ok = 1,
    AlreadyExists = 2,
    InvalidInput = 3,
    NotFound = 4,
    BusinessError = 5,
    Unauthorized = 6,
}

public record struct ErrorMessage(string Code, string Message);

public class ResultResponse<TEntity>    
{
    public ResultResponse(ErrorMessage errorMessage, CommandResultStatus commandResultStatus)
    {
        Success = false;
        Status = commandResultStatus;
        Errors = [errorMessage];
    }

    public ResultResponse(CommandResultStatus commandResultStatus)
    {
        Success = false;
        Status = commandResultStatus;
        Errors = [];
    }

    public ResultResponse(IEnumerable<ErrorMessage> errorMessages, CommandResultStatus commandResultStatus)
    {
        Success = false;
        Errors = errorMessages;
        Status = commandResultStatus;
    }

    private ResultResponse(TEntity result)
    {
        Result = result;
        Success = true;
        Status = CommandResultStatus.Ok;
        Errors = [];
    }

    public bool Success { get; }

    public IEnumerable<ErrorMessage> Errors { get; }

    public CommandResultStatus Status { get; }

    public TEntity? Result { get; private set; }

    public static implicit operator ResultResponse<TEntity>(TEntity entity) => new(entity);

    public static implicit operator ResultResponse<TEntity>(ErrorMessage[] errors) => new(errors, CommandResultStatus.InvalidInput);

    public static implicit operator ResultResponse<TEntity>(ErrorMessage error) => new([error], CommandResultStatus.InvalidInput);

    public IStatusCodeActionResult ToActionResult(ControllerBase controller)
    {
        return Status switch
        {
            CommandResultStatus.Ok => Result == null ? controller.NoContent() : controller.Ok(Result),
            CommandResultStatus.InvalidInput => controller.BadRequest(Errors),
            CommandResultStatus.AlreadyExists => controller.BadRequest(Errors),
            CommandResultStatus.Unknown => controller.StatusCode(500, Errors),
            CommandResultStatus.NotFound => controller.NotFound(),
            CommandResultStatus.BusinessError => controller.UnprocessableEntity(Errors),
            CommandResultStatus.Unauthorized => controller.Unauthorized(),
            _ => throw new InvalidOperationException($"Status {Status} was not mapped to a status code")
        };

    }
}