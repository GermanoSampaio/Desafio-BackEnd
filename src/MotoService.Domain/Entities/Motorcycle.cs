using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MotoService.Domain.Exceptions;

namespace MotoService.Domain.Entities
{
    public class Motorcycle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("identifier")]
        public string Identifier { get; set; } = string.Empty;

        [BsonElement("year")]
        public int Year { get; private set; }

        [BsonElement("model")]
        public string Model { get; private set; } = string.Empty;

        [BsonElement("licensePlate")]
        public string LicensePlate { get; private set; } = string.Empty;

        public Motorcycle(int year, string model, string licensePlate)
        {
            SetModel(model);
            SetYear(year);
            SetLicensePlate(licensePlate);
            Identifier = Guid.NewGuid().ToString("N");
        }
        public void SetModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new DomainException("Modelo da moto é obrigatório.");

            Model = model;
        }

        public void SetYear(int year)
        {
            if (!AnoEhValido(year))
                throw new DomainException("Ano da moto inválido. Aceitamos apenas motos a partir do ano 2000 até o ano atual.");

            Year = year;
        }
        public void SetLicensePlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                throw new DomainException("Placa não pode ser vazia.");

            if (!Regex.IsMatch(plate, "^[A-Z]{3}-[0-9]{4}$"))
                throw new DomainException("A placa deve estar no formato AAA-0000.");

            LicensePlate = plate;
        }
        private bool AnoEhValido(int ano)
        {
            var anoAtual = DateTime.UtcNow.Year;
            return ano >= 2000 && ano <= anoAtual;
        }

    }
}
