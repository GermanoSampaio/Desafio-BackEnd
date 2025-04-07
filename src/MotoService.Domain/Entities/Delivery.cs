using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MotoService.Domain.Enums;

namespace MotoService.Domain.Entities
{
    public class Delivery
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = String.Empty;

        [BsonElement("identifier")]
        public string Identifier { get; private set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; private set; }

        [BsonElement("cnpj")]
        public string Cnpj { get; private set; }

        [BsonElement("birth_date")]
        public DateTime BirthDate { get; private set; }

        [BsonElement("cnh_number")]
        public string CnhNumber { get; private set; }

        [BsonElement("cnh_type")]
        public CnhType CnhType { get; private set; }

        [BsonElement("cnh_image_path")]
        public string CnhImageURL { get; private set; } = string.Empty;

        // Construtor com validações
        public Delivery(string name, string cnpj, DateTime birthDate, string cnhNumber, CnhType cnhType)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome é obrigatório.", nameof(name));

            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ArgumentException("CNPJ é obrigatório.", nameof(cnpj));

            if (!IsValidCnpj(cnpj))
                throw new ArgumentException("CNPJ inválido.", nameof(cnpj));

            if (birthDate > DateTime.UtcNow)
                throw new ArgumentException("Data de nascimento não pode ser futura.", nameof(birthDate));

            if (string.IsNullOrWhiteSpace(cnhNumber))
                throw new ArgumentException("Número da CNH é obrigatório.", nameof(cnhNumber));

            if (!Enum.IsDefined(typeof(CnhType), cnhType))
                throw new ArgumentException("Tipo de CNH inválido.", nameof(cnhType));

            Name = name;
            Cnpj = cnpj;
            BirthDate = birthDate;
            CnhNumber = cnhNumber;
            CnhType = cnhType;

            Identifier = Guid.NewGuid().ToString("N");
        }
        public void SetIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public void SetCnhImageUrl(string url)
        {
            CnhImageURL = url;
        }

        private bool IsValidCnpj(string cnpj)
        {
            var onlyDigits = new string(cnpj.Where(char.IsDigit).ToArray());
            return onlyDigits.Length == 14;
        }
    }
}