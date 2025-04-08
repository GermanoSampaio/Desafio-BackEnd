using MotoService.Domain.Enums;
using MotoService.Domain.Exceptions;

namespace MotoService.Domain.Validators
{
    public static class CnhTypeValidator
    {
        public static bool IsValid(string tipo)
        {
            return Enum.TryParse(typeof(CnhType), tipo, ignoreCase: true, out _);
        }

        public static CnhType Parse(string tipo)
        {
            if (!IsValid(tipo))
                throw new InvalidCnhTypeException();

            return Enum.Parse<CnhType>(tipo, ignoreCase: true);
        }
    }
}
