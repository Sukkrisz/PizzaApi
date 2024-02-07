using MediatR;

namespace Infrastructure.Mediator
{
    public class Result : Result<Unit>
    {
        public Result() : base(Unit.Value)
        {
        }
    }

    public class Result<T>
    {
        public T Value { get; set; }

        public List<string> Errors { get; set; }

        public Result()
        { 
        }

        public Result(T value)
        {
            this.Value = value;
        }

        public static Result Ok()
        {
            return new Result();
        }

        public static Result<T> Ok<T>(T result)
        {
            return new Result<T>(result);
        }

        public static Result<Unit> Fail(List<string> errors)
        {
            return new Result<Unit>() { Errors = errors };
        }

        public bool IsSuccess
        {
            get { return Errors is null; }
        }
    }
}
