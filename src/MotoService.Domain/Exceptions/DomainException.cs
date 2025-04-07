namespace MotoService.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public List<string>? Details { get; }

        public DomainException(string message, List<string>? details = null) : base(message)
        {
            Details = details;
        }
    }
}
