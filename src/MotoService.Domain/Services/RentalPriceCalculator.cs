using MotoService.Domain.Entities;

namespace MotoService.Domain.Services
{
    public class RentalPriceCalculator
    {
        public static double CalculateTotal(Rental rental, DateTime terminalDate)
        {
            int contractedDays = rental.RentalPlan.Days;
            double dailyRate = rental.DailyRate;

            int actualDaysUsed = (terminalDate - rental.StartDate).Days;
            int expectedDays = (rental.ExpectedTerminalDate - rental.StartDate).Days;

            if (terminalDate < rental.ExpectedTerminalDate)
            {
                int unusedDays = expectedDays - actualDaysUsed;
                double baseAmount = actualDaysUsed * dailyRate;

                double penaltyPercentage = contractedDays == 7 ? 0.20 : 0.40;
                double penaltyAmount = unusedDays * dailyRate * penaltyPercentage;

                return baseAmount + penaltyAmount;
            }

            if (terminalDate > rental.ExpectedTerminalDate)
            {
                int extraDays = (terminalDate - rental.ExpectedTerminalDate).Days;
                double lateFee = extraDays * 50.00;

                double expectedAmount = expectedDays * dailyRate;
                return expectedAmount + lateFee;
            }

            return expectedDays * dailyRate;
        }
    }
}