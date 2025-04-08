namespace MotoService.Domain.Exceptions
{
    public class RentalPlanNotFoundException : DomainException
    {
        public RentalPlanNotFoundException() : base(ErrorMessages.RentalPlanNotFound) { }
    }

    public class InvalidRentalPeriodException : DomainException
    {
        public InvalidRentalPeriodException() : base(ErrorMessages.InvalidRentalPeriod) { }
    }

    public class ExpectedTerminalDateException : DomainException
    {
        public ExpectedTerminalDateException() : base(ErrorMessages.InvalidExpectedTerminalDate) { }
    }

    public class InvalidDailyRateException : DomainException
    {
        public InvalidDailyRateException() : base(ErrorMessages.InvalidDailyRate) { }
    }
    public class RentalNotFoundException : DomainException
    {
        public RentalNotFoundException() : base(ErrorMessages.RentalNotFound) { }
    }

    public class MotorcycleUnavailableException : DomainException
    {
        public MotorcycleUnavailableException(string motorcycleId)
            : base($"Moto {motorcycleId} está indisponível para o período informado.") { }
    }
    public class RentalStartDateInvalidException : DomainException
    {
        public RentalStartDateInvalidException()
             : base(ErrorMessages.RentalStartDateInvalid) { }
    }
        
}
