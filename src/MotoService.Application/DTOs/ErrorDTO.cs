namespace MotoService.Application.DTOs
{
    public class ErrorDto
    {
        public string Message { get; set; }
        public int? Code { get; set; }
        public List<string>? Details { get; set; }

        public ErrorDto(string message, int? code = null, List<string>? details = null)
        {
            Message = message;
            Code = code;
            Details = details;
        }
    }
}