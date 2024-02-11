using MediatR;

namespace Infrastructure.Mediator
{
    public class WrapperResult<T> : IMyResult
    {
        public T Value { get; set; }

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

        public static WrapperResult<T> Ok(T value)
        {
            return new WrapperResult<T>(value);
        }

        public static WrapperResult<T> Failed(string errorMsg)
        {
            return new WrapperResult<T>(errorMsg);
        }
    }

    public class MyResult : WrapperResult<Unit>
    {
        MyResult() : base(Unit.Value)
        {
        }

        MyResult(string error) : base(error)
        {
        }

        public static MyResult Ok()
        {
            return new MyResult();
        }

        public static MyResult Failed(string error)
        {
            return new MyResult(error);
        }
    }
}
