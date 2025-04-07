using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MotoService.Domain.Exceptions;

namespace MotoService.Domain.Entities
{
    public class Rental
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("identifier")]
        public string Identifier { get; set; } = Guid.NewGuid().ToString("N");

        [BsonElement("delivery_id")]
        public string DeliveryId { get; private set; }

        [BsonElement("motorcycle_id")]
        public string MotorcycleId { get; private set; }

        [BsonElement("start_date")]
        public DateTime StartDate { get; private set; }

        [BsonElement("terminal_date")]
        public DateTime TerminalDate { get; private set; }

        [BsonElement("expected_terminal_date")]
        public DateTime ExpectedTerminalDate { get; private set; }

        [BsonElement("rental_plan")]
        public RentalPlan RentalPlan { get; private set; }

        [BsonElement("daily_rate")]
        public double DailyRate { get; private set; }

        public Rental(string deliveryId, string motorcycleId, DateTime startDate, DateTime terminalDate)
        {
            if (string.IsNullOrWhiteSpace(deliveryId))
                throw new DomainException(ErrorMessages.DeliveryIdRequired);

            if (string.IsNullOrWhiteSpace(motorcycleId))
                throw new DomainException(ErrorMessages.MotorcycleIdRequired);

            DeliveryId = deliveryId;
            MotorcycleId = motorcycleId;
            StartDate = startDate;
            TerminalDate = terminalDate;
        }

        public void SetTerminalDate(DateTime terminalDate)
        {
            if (terminalDate < StartDate)
                throw new InvalidRentalPeriodException();

            TerminalDate = terminalDate;
        }

        public void SetRentalPlan(RentalPlan rentalPlan)
        {
            RentalPlan = rentalPlan;
        }

        public void SetExpectedTerminalDate(DateTime expectedTerminalDate)
        {
            ExpectedTerminalDate = expectedTerminalDate;

        }

        public void SetDailyRate(double dailyRate)
        {
            DailyRate = dailyRate;
        }


        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(DeliveryId))
                throw new DomainException(ErrorMessages.DeliveryIdRequired);

            if (string.IsNullOrWhiteSpace(MotorcycleId))
                throw new DomainException(ErrorMessages.MotorcycleIdRequired);

            if (StartDate > TerminalDate)
                throw new InvalidRentalPeriodException();

            if (ExpectedTerminalDate < StartDate)
                throw new ExpectedTerminalDateException();

            if (RentalPlan == null)
                throw new RentalPlanNotFoundException();

            var maxEndDate = StartDate.AddDays(RentalPlan.Days);
            if (ExpectedTerminalDate > maxEndDate)
                throw new DomainException("Data de devolução prevista ultrapassa o período permitido pelo plano.");

            
        }

    }
}
