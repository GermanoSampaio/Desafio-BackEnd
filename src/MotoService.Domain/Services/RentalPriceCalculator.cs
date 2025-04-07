using MotoService.Domain.Entities;

namespace MotoService.Domain.Services
{
    public class RentalPriceCalculator
    {
        public static double CalculateTotal(Rental rental, RentalPlan plan)
        {
            int actualDays = (rental.TerminalDate - rental.StartDate).Days;
            int expectedDays = plan.Days;
            double baseRate = plan.DailyRate;

            if (actualDays < expectedDays)
            {
                int unusedDays = expectedDays - actualDays;
                double dailyTotal = actualDays * baseRate;
                double finePercentage = plan.Days == 7 ? 0.20 : plan.Days == 15 ? 0.40 : 0.0;
                double fine = unusedDays * baseRate * finePercentage;
                return dailyTotal + fine;
            }
            else if (actualDays > expectedDays)
            {
                int extraDays = actualDays - expectedDays;
                double baseTotal = expectedDays * baseRate;
                double extraFee = extraDays * 50.0;
                return baseTotal + extraFee;
            }

            // Devolução no prazo
            return expectedDays * baseRate;
        }
    }
}