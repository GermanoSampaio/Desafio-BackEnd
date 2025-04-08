namespace MotoService.Domain.Exceptions
{
   
    public class DuplicateCnpjException : DomainException
    {
        public DuplicateCnpjException() : base(ErrorMessages.DuplicateCnpj) { }
    }

    public class DuplicateCnhException : DomainException
    {
        public DuplicateCnhException() : base(ErrorMessages.DuplicateCnh) { }
    }

    public class InvalidCnhTypeException : DomainException
    {
        public InvalidCnhTypeException(string? type = null)
       : base($"{ErrorMessages.InvalidCnhType} Tipo informado: {type}") { }
    }

    public class DeliveryNotFoundException : DomainException
    {
        public DeliveryNotFoundException() : base(ErrorMessages.DeliveryNotFound) { }
    }

    public class InvalidFileFormatException : DomainException
    {
        public InvalidFileFormatException() : base(ErrorMessages.InvalidFileFormat) { }
    }


}
