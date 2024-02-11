namespace Infrastructure.Mediator
{
    public interface IMyResult
    {
        public bool IsSuccess { get; }

        public string ErrorMessage { get; set; }
    }
}
