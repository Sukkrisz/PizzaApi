using MediatR;

namespace Infrastructure.Mediator
{
    public class Result<T>
    {
        public T Value { get; set; }

        public string Error { get; set; }

        public Result()
        { 
        }

        public Result(T value)
        {
            this.Value = value;
        }

        public Result(string error)
        {
            this.Error = error;
        }

        public static Result Ok()
        {
            return new Result();
        }

        public static Result<T> Ok<T>(T result)
        {
            return new Result<T>(result);
        }

        public static Result<Unit> Fail(string error)
        {
            return new Result<Unit>() { Error = error };
        }

        public bool IsSuccess
        {
            get { return Error is null; }
        }
    }

    public sealed class Result : Result<Unit>
    {
        public Result() : base(Unit.Value)
        {
        }

        public Result(string error)
            : base(error)
        {
        }
    }
}
