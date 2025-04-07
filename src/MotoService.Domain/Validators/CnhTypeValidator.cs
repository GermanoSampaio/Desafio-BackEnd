using MotoService.Domain.Enums;

namespace MotoService.Domain.Validators
{
    public static class CnhTypeValidator
    {
        public static bool IsValid(CnhType type)
        {
            return Enum.IsDefined(typeof(CnhType), type);
        }
    }
}
