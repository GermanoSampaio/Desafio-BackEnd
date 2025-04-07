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

}
