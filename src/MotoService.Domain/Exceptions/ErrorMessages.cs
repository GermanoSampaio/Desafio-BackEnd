
namespace MotoService.Domain.Exceptions
{
    public static class ErrorMessages
    {
        public const string InvalidYear = "Ano da moto inválido: {0}. Aceitamos apenas motos fabricadas entre 2000 e o ano atual.";
        public const string ModelRequired = "Modelo da moto é obrigatório.";
        public const string InvalidLicensePlateFormat = "A placa deve estar no formato AAA-0000.";
        public const string LicensePlateRequired = "A placa da moto não pode ser vazia.";
        public const string MotoNotFound = "Moto não encontrada.";
        public const string RentalNotFound = "Locação não encontrada.";
        public const string RentalPlanNotFound = "Plano de locação não encontrado.";
        public const string Identificador = "Tipo de CNH inválido.";
        public const string InvalidCnhType = "Tipo de CNH inválido.";
        public const string DuplicateCnpj = "Já existe um entregador com este CNPJ.";
        public const string DuplicateCnh = "Já existe um entregador com este número de CNH.";
        public const string DeliveryNotFound = "Entregador não encontrado.";
        public const string InvalidFileFormat = "Formato inválido. Use PNG ou BMP.";
        public const string CnhFileRequired = "O arquivo da CNH é obrigatório.";
        public const string InternalServerError = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
        public const string DeliveryIdRequired = "Identificação do entregador é obrigatória.";
        public const string MotorcycleIdRequired = "Identificador da moto é obrigatório.";
        public const string InvalidRentalPeriod = "A data de devolução não pode ser anterior à data de início da locação.";
        public const string InvalidExpectedTerminalDate = "Data prevista de término não pode ser anterior à data de início.";
        public const string InvalidDailyRate = "Valor da diária deve ser maior que zero.";
        public const string RentalStartDateInvalid = "A data de início da locação deve ser posterior à data atual.";
        public const string RentalEndDateInvalid = "A data de término da locação deve ser pelo menos 7 dias após a data atual.";
        public const string RentalEndDateMustBeFuture = "A data de término deve ser futura.";
        public const string MotorcycleErrorRequired = "Erro ao tentar cadastrar motocicleta.";

    }
}
