namespace MotoService.Domain.Exceptions
{
    public class InvalidMotorcycleYearException : DomainException
    {
        public InvalidMotorcycleYearException(int year)
            : base(string.Format(ErrorMessages.InvalidYear, year)) { }
    }

    public class InvalidMotorcycleModelException : DomainException
    {
        public InvalidMotorcycleModelException()
            : base(ErrorMessages.ModelRequired) { }
    }

    public class InvalidLicensePlateException : DomainException
    {
        public InvalidLicensePlateException()
            : base(ErrorMessages.InvalidLicensePlateFormat) { }
    }

    public class EmptyLicensePlateException : DomainException
    {
        public EmptyLicensePlateException()
            : base(ErrorMessages.LicensePlateRequired) { }
    }

    public class MotorcycleNotFoundException : DomainException
    {
        public MotorcycleNotFoundException() : base(ErrorMessages.MotoNotFound) { }
    }
    public class MotorcycleUnavailableException : DomainException
    {
        public MotorcycleUnavailableException(string motorcycleId)
            : base($"Moto {motorcycleId} está indisponível para o período informado.") { }
    }

    public class MotorcycleErrorException : DomainException
    {
        public MotorcycleErrorException() : base(ErrorMessages.MotorcycleErrorRequired) { }
    }
    public class MotorcycleDuplicateRequiredException : DomainException
    {
        public MotorcycleDuplicateRequiredException(string licensePlate) : base($"Moto de placa {licensePlate} já cadastrada!") { }
    }
    
}
