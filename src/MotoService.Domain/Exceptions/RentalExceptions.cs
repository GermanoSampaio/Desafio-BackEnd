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

   
    public class RentalStartDateInvalidException : DomainException
    {
        public RentalStartDateInvalidException()
             : base(ErrorMessages.RentalStartDateInvalid) { }
    }

    public class RentalEndDateInvalidException : DomainException
    {
        public RentalEndDateInvalidException()
             : base(ErrorMessages.RentalEndDateInvalid) { }
    }
    public class RentalEndDateMustBeFutureInvalidException : DomainException
    {
        public RentalEndDateMustBeFutureInvalidException()
             : base(ErrorMessages.RentalEndDateMustBeFuture) { }
    }

    public class ActiveRentalExistsException : DomainException
    {
        public ActiveRentalExistsException(string motorcycleId)
            : base($"Não é possível remover a moto (ID: {motorcycleId}) porque existe uma locação cadastrada.") { }
    }

}
