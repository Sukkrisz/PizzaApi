using MediatR;

namespace Infrastructure.Mediator
{
    // The class is created so, that a value or an error message can be returned by mediatR, and not just the value
    public class WrapperResult<T> : IWrapperResult
    {
        public T Value { get; set; }

        // For simlicity's sake, it's just a string, but in a real life project, I'd suggest more errors (a list)
        // and more details to be returned
        public string ErrorMessage { get; set; }

        public bool IsSuccess { get => ErrorMessage is null; }

        public WrapperResult(T value)
        {
            Value = value;
        }

        public WrapperResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        // Used for readability
        public static WrapperResult<T> Ok(T value)
        {
            return new WrapperResult<T>(value);
        }

        // Used for readability
        public static WrapperResult<T> Failed(string errorMsg)
        {
            return new WrapperResult<T>(errorMsg);
        }
    }

    // Created this class, so that I can return Ok and Failed without T types, since commands usually don't
    // expect a value to be returned, so there would be no type to fill out either
    public class WrapperResult : WrapperResult<Unit>
    {
        WrapperResult() : base(Unit.Value)
        {
        }

        WrapperResult(string error) : base(error)
        {
        }

        public static WrapperResult Ok()
        {
            return new WrapperResult();
        }

        public static WrapperResult Failed(string error)
        {
            return new WrapperResult(error);
        }
    }
}
