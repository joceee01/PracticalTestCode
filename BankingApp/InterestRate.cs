using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApp
{
    public class InterestRate
    {
        private static List<InterestRate> interestRateRules = new List<InterestRate>();

        private DateTime effectiveDate;
        private string ruleId;
        private decimal rate;

        public InterestRate(DateTime effectiveDate, string ruleId, decimal rate)
        {
            this.effectiveDate = effectiveDate;
            this.ruleId = ruleId;
            this.rate = rate;
        }

        public DateTime EffectiveDate => effectiveDate;
        public string RuleId => ruleId;
        public decimal Rate => rate;

        public static InterestRate GetRule(string ruleId)
        {
            return interestRateRules.Where(i => i.ruleId == ruleId).FirstOrDefault();
        }

        public static void UpdateRule(DateTime date, string ruleId, decimal rate)
        {
            var getRule = GetRule(ruleId);

            if (getRule != null)
            {
                getRule.effectiveDate = date;
                getRule.rate = rate;
                Console.WriteLine($"\n{ruleId} update successfully");
            }
        }

        public static void CreateRule(DateTime effectiveDate, string ruleId, decimal rate)
        {
            var newRule = new InterestRate(effectiveDate, ruleId, rate);
            interestRateRules.Add(newRule);

            Console.WriteLine("Interest Rate Rule created successfully!");
        }

        public static void DisplayRuleInfo()
        {
            var sortedRules = interestRateRules.OrderBy(r => r.EffectiveDate);
            Console.WriteLine("\nCurrent Interest Rate Rules:");
            Console.WriteLine($"{"Rule ID",-10}{"Effective Date",-18}{"Rate in %",-10}");
            foreach (var r in sortedRules)
            {
                Console.WriteLine($"{r.ruleId,-10}{r.effectiveDate,-18:dd-MM-yyyy}{r.rate,-10:F2}");
            }
        }

        public static List<InterestRate> GetRate(DateTime monthEnd)
        {
            var applicableRates = interestRateRules.Where(r => r.EffectiveDate <= monthEnd).OrderBy(r => r.EffectiveDate).ToList();

            return applicableRates;
        }

        public static decimal CalculateInterest(string account, DateTime statementMonth)
        {
            var monthStart = new DateTime(statementMonth.Year, statementMonth.Month, 1);
            var monthEnd = new DateTime(statementMonth.Year, statementMonth.Month, DateTime.DaysInMonth(statementMonth.Year, statementMonth.Month));
            var statementTransactions = Transaction.GetTransactionBeforeMonthEnd(monthEnd, account);

            // Get interest rates applicable to the month
            var applicableRates = GetRate(monthEnd);

            if (!applicableRates.Any()) return 0; // No rates defined, return interest as 0

            decimal totalInterest = 0;
            decimal dailyBalance = 0;
            DateTime currentDate = monthStart;

            // Loop through each day of the month
            while (currentDate <= monthEnd)
            {
                // Update balance for the day
                dailyBalance = statementTransactions
                    .Where(t => t.Date <= currentDate)
                    .Sum(t => t.Type == "D" ? t.Amount : -t.Amount);

                // Get the applicable interest rate for the day
                var currentRate = applicableRates
                    .Where(r => r.EffectiveDate <= currentDate)
                    .OrderByDescending(r => r.EffectiveDate)
                    .FirstOrDefault();

                if (currentRate != null)
                {
                    decimal dailyRate = currentRate.Rate / 100 / DateTime.DaysInMonth(statementMonth.Year, statementMonth.Month);
                    totalInterest += dailyBalance * dailyRate;
                }

                currentDate = currentDate.AddDays(1); // Move to the next day
            }

            return Math.Round(totalInterest, 2); // Round to 2 decimal places
        }
    }
}
