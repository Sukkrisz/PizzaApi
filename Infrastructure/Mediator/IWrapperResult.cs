namespace Infrastructure.Mediator
{
    public interface IWrapperResult
    {
        public bool IsSuccess { get; }

        public string ErrorMessage { get; set; }
    }
}
