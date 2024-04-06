using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FinanceControl.Application
{
    public enum CommandResultStatus
    {
        Unknown = 0,
        Ok = 1,
        AlreadyExists = 2,
        InvalidInput = 3,
        NotFound = 4,
        BusinessError = 5
    }

    public record struct ErrorMessage(string Code, string Message);

    public class ResultResponse : ResultResponse<object>
    {
        public ResultResponse() : base(default!)
        {

        }

        public ResultResponse(ErrorMessage errorMessage, CommandResultStatus commandResultStatus)
            : base(errorMessage, commandResultStatus)
        {
        }

        public ResultResponse(IEnumerable<ErrorMessage> errorMessages, CommandResultStatus commandResultStatus)
            : base(errorMessages, commandResultStatus)
        {
        }
    }

    public class ResultResponse<TEntity>
    {
        public ResultResponse(ErrorMessage errorMessage, CommandResultStatus commandResultStatus)
        {
            Success = false;
            Errors = new ErrorMessage[] { errorMessage };
            Status = commandResultStatus;
        }

        public ResultResponse(IEnumerable<ErrorMessage> errorMessages, CommandResultStatus commandResultStatus)
        {
            Success = false;
            Errors = errorMessages;
            Status = commandResultStatus;
        }

        public ResultResponse(TEntity result)
        {
            Result = result;
            Success = true;
            Status = CommandResultStatus.Ok;
            Errors = Enumerable.Empty<ErrorMessage>();
        }

        public bool Success { get; }

        public IEnumerable<ErrorMessage> Errors { get; }

        public CommandResultStatus Status { get; }

        public TEntity? Result { get; private set; }

        public static implicit operator ResultResponse<TEntity>(TEntity entity) => new(entity);

        public static implicit operator ResultResponse<TEntity>(ErrorMessage[] errors) => new(errors, CommandResultStatus.InvalidInput);

        public IStatusCodeActionResult ToActionResult(ControllerBase controller)
        {
            if (Status == CommandResultStatus.Ok)
            {
                return Result == null ? controller.NoContent() : controller.Ok(Result);
            }

            if (Status == CommandResultStatus.InvalidInput)
            {
                return controller.BadRequest(Errors);
            }

            if (Status == CommandResultStatus.AlreadyExists)
            {
                return controller.BadRequest(Errors);
            }

            if (Status == CommandResultStatus.Unknown)
            {
                return controller.StatusCode(500, Errors);
            }

            if (Status == CommandResultStatus.NotFound)
            {
                return controller.NotFound();
            }

            if (Status == CommandResultStatus.BusinessError)
            {
                return controller.UnprocessableEntity(Errors);
            }

            throw new InvalidOperationException($"Status {Status} was not mapped to a status code");
        }
    }
}
