using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class Program
{
    // Data Storage
    static List<Account> accounts = new List<Account>();
    static List<Transaction> transactions = new List<Transaction>();
    static List<InterestRateRule> interestRateRules = new List<InterestRateRule>();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nWelcome to Corsiva Lab Bank");
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Input Transactions");
            Console.WriteLine("2. Define Interest Rate");
            Console.WriteLine("3. Print Account Statement");
            Console.WriteLine("4. Tranfer Transaction");
            Console.WriteLine("5. Quit");
            Console.WriteLine("\nEnter Option:");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    InputTransactions();
                    break;
                case "2":
                    DefineInterestRate();
                    break;
                case "3":
                    PrintAccountStatement();
                    break;
                case "4":
                    TransferTransaction();
                    break;
                case "5":
                    Console.WriteLine("Exiting the application.");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void InputTransactions()
    {
        Console.WriteLine("\nFormat: Date|Account|Type|Amount. Enter a blank line to finish");
        Console.WriteLine("Date: DDMMYYYY format");
        Console.WriteLine("Type: Withdrawal (W) or Deposit (D)");
        Console.WriteLine("Amount: Transaction amount");

        while (true)
        {
            Console.WriteLine("\nEnter Transaction:");

            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                break;

            try
            {
                // Split the input by '|' as each parts
                var parts = input.Split('|');
                DateTime date = DateTime.ParseExact(parts[0], "ddMMyyyy", CultureInfo.InvariantCulture);
                string account = parts[1];
                string type = parts[2].ToUpper();               // Set type input to uppercase
                decimal amount = decimal.Parse(parts[3]);       // Set amount to decimal data type
                decimal balance = transactions
                    .Where(t => t.Account == account && t.Date <= date)
                    .Sum(t => t.Type == "D" ? t.Amount : -t.Amount);

                if (type != "D" && type != "W")
                {
                    Console.WriteLine("Invalid transaction type. Use 'D' for Deposit or 'W' for Withdrawal.");
                    continue;
                }

                if (type == "W" && (balance - amount) < 0)
                {
                    Console.WriteLine("\nInsufficient balance for withdrawal.");
                    continue;
                }

                if (type == "D" && amount < 0)
                {
                    Console.WriteLine("\nPlease enter a valid amount.");
                    continue;
                }

                else
                {
                    var checkAccount = accounts.Where(a => a.AccountNo == account).FirstOrDefault();
                    // Add transaction
                    var txn = new Transaction
                    {
                        Id = transactions.Count + 1,    // Auto increment Id
                        Date = date,
                        Account = account,
                        Type = type,
                        Amount = amount
                    };
                    transactions.Add(txn);      // add txn into transactions list

                    balance = transactions.Where(t => t.Account == account).Sum(t => t.Type == "D" ? t.Amount : -t.Amount);

                    if (checkAccount == null)
                    {
                        // Add account
                        var acc = new Account
                        {
                            AccountNo = account,
                            Balance = balance
                        };
                        accounts.Add(acc);
                    }

                    else
                    {
                        // Update balance
                        checkAccount.Balance = balance;
                    }

                    // Sort transactions by date then by id and display for the input account
                    var accountTransactions = transactions.Where(t => t.Account == account).OrderBy(t => t.Date).ThenBy(t => t.Id);

                    Console.WriteLine("\nTransactions for account: " + account);
                    Console.WriteLine($"{"Date",-14}{"Txn ID",-10}{"Type",-10}{"Amount",-10}");
                    foreach (var t in accountTransactions)
                    {
                        Console.WriteLine($"{t.Date,-14:dd-MM-yyyy}{t.Id,-10}{t.Type,-10}{t.Amount,-10:F2}");
                    }

                    var getAccount = accounts.Where(a => a.AccountNo == account).FirstOrDefault();

                    Console.WriteLine("\nAccount Information");
                    Console.WriteLine($"{"Account Number",-20}{"Balance"}");
                    Console.WriteLine($"{getAccount.AccountNo,-20}{getAccount.Balance:F2}");

                    continue;
                }

            }
            catch
            {
                Console.WriteLine("Invalid input. Please follow the format: DDMMYYYY|Account|Type|Amount");
            }
        }
    }

    static void DefineInterestRate()
    {
        Console.WriteLine("\nFormat: Date|RuleId|Rate. Enter a blank line to finish");
        Console.WriteLine("Date: the date the interest rule goes into effect, DDMMYYYY format");
        Console.WriteLine("RuleId: unique name");
        Console.WriteLine("Rate: decimal number in %");

        while (true)
        {
            Console.WriteLine("\nEnter Interest Rate Rules:");

            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                break;

            try
            {
                var parts = input.Split('|');
                DateTime date = DateTime.ParseExact(parts[0], "ddMMyyyy", CultureInfo.InvariantCulture);
                string ruleId = parts[1].ToUpper();
                decimal rate = decimal.Parse(parts[2]);

                var checkRuleId = interestRateRules
                    .Where(i => i.RuleId == ruleId)
                    .FirstOrDefault();

                if (checkRuleId != null)
                {
                    Console.WriteLine($"\nRule ID {ruleId} is not unique. Do you want to update the existing rule? (Y/N). Enter blank line to restart.");
                    Console.WriteLine($"Update existing {ruleId} (Y) or Re-enter Rule Id (N)");

                    while (true)
                    {
                        Console.WriteLine("\nEnter Y or N:");

                        string invalidIdinput = Console.ReadLine().ToUpper();
                        if (string.IsNullOrWhiteSpace(invalidIdinput))
                            break;

                        if (invalidIdinput != "Y" && invalidIdinput != "N")
                        {
                            Console.WriteLine($"Invalid input. Use 'Y' to update existing {ruleId} or 'N' re-enter Rule Id.");
                            continue;
                        }

                        else
                        {
                            if (invalidIdinput == "Y")
                            {
                                var findRuleId = interestRateRules
                                    .Where(i => i.RuleId == ruleId)
                                    .FirstOrDefault();

                                // Update interest rate
                                findRuleId.EffectiveDate = date;
                                findRuleId.Rate = rate;
                                Console.WriteLine($"\n{ruleId} update successfully");
                                break;
                            }
                            else if (invalidIdinput == "N")
                            {
                                Console.WriteLine("\nRe-enter Rule Id:");
                                string newRuleId = Console.ReadLine().ToUpper();

                                // Add new interest rate rule with newly entered Rule Id
                                var rule = new InterestRateRule
                                {
                                    EffectiveDate = date,
                                    RuleId = newRuleId,
                                    Rate = rate
                                };
                                interestRateRules.Add(rule);
                                Console.WriteLine($"\n{newRuleId} added successfully");
                                break;
                            }
                        }
                    }
                }

                else
                {
                    var rule = new InterestRateRule
                    {
                        EffectiveDate = date,
                        RuleId = ruleId,
                        Rate = rate
                    };
                    interestRateRules.Add(rule);
                }

                // Sort and display rules
                var sortedRules = interestRateRules.OrderBy(r => r.EffectiveDate);
                Console.WriteLine("\nCurrent Interest Rate Rules:");
                Console.WriteLine($"{"Rule ID",-10}{"Effective Date",-18}{"Rate in %",-10}");
                foreach (var r in sortedRules)
                {
                    Console.WriteLine($"{r.RuleId,-10}{r.EffectiveDate,-18:dd-MM-yyyy}{r.Rate,-10:F2}");
                }

                continue;
            }
            catch
            {
                Console.WriteLine("Invalid input. Please follow the format: DDMMYYYY|RuleId|Rate");
            }
        }
    }

    static void PrintAccountStatement()
    {
        Console.WriteLine("\nFormat: Account|Month. Enter a blank line to finish");
        Console.WriteLine("Month: MMYY format");

        while (true)
        {
            Console.WriteLine("\nEnter Account and Month:");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                break;

            try
            {
                var parts = input.Split('|');
                string account = parts[0];
                string month = parts[1];

                var statementMonth = DateTime.ParseExact("01" + month, "ddMMyy", CultureInfo.InvariantCulture);
                var statementTransactions = transactions
                    .Where(t => t.Account == account && t.Date.Year == statementMonth.Year && t.Date.Month == statementMonth.Month)
                    .OrderBy(t => t.Date)
                    .ThenBy(t => t.Id)
                    .ToList();

                if (!statementTransactions.Any())
                {
                    Console.WriteLine("No transactions found for the specified account and month.");
                    continue;
                }

                // Get the balance before the first date of the requested statementTransactions month
                decimal balance = transactions
                .Where(t => t.Account == account && t.Date < statementTransactions.First().Date)
                .Sum(t => t.Type == "D" ? t.Amount : -t.Amount);

                // Calculate interest
                decimal interest = CalculateInterest(account, statementMonth);

                // Add interest as a transaction
                var interestTransaction = new Transaction
                {
                    Id = transactions.Count + 1,
                    Date = new DateTime(statementMonth.Year, statementMonth.Month, DateTime.DaysInMonth(statementMonth.Year, statementMonth.Month)),
                    Account = account,
                    Type = "I",
                    Amount = interest
                };
                statementTransactions.Add(interestTransaction);

                Console.WriteLine("\nAccount Statement for: " + account);
                Console.WriteLine($"{"Date",-14}{"Txn ID",-10}{"Type",-10}{"Amount",-12}{"Balance"}");

                foreach (var t in statementTransactions)
                {
                    // Type D: add amount to balance, Type W: deduct amount from balance, Others Type: add amount to balance
                    balance += t.Type == "D" ? t.Amount : t.Type == "W" ? -t.Amount : t.Amount;
                    Console.WriteLine($"{t.Date,-14:dd-MM-yyyy}{t.Id,-10}{t.Type,-10}{t.Amount,-12:F2}{balance:F2}");
                }
                break;

            }
            catch
            {
                Console.WriteLine("Invalid input. Please follow the format: Account|MMYY");
            }
        }

    }

    static void TransferTransaction()
    {
        Console.WriteLine("\nFormat: Payer Account|Payee Account|Amount. Enter a blank line to finish");
        Console.WriteLine("Payer Account: Account to transfer money from");
        Console.WriteLine("Payee Account: Account to transfer money to");
        Console.WriteLine("Amount: Transfer amount");

        while (true)
        {
            Console.WriteLine("\nEnter Transfer Transaction:");

            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                break;

            try
            {
                // Split the input by '|' as each parts
                var parts = input.Split('|');
                string payerAcc = parts[0].Trim();
                string payeeAcc = parts[1].Trim();
                if (!decimal.TryParse(parts[2].Trim(), out decimal amount) || amount <= 0)
                {
                    Console.WriteLine("Invalid amount. Please enter a positive numeric value.");
                    continue;
                }

                if (payeeAcc == payerAcc)
                {
                    Console.WriteLine("Cannot transfer to the same account.");
                    continue;
                }

                var getPayerAcc = accounts.Where(a => a.AccountNo == payerAcc).FirstOrDefault();
                var getPayeeAcc = accounts.Where(a => a.AccountNo == payeeAcc).FirstOrDefault();

                if (getPayerAcc == null)
                {
                    Console.WriteLine($"Payer Account {payerAcc} does not exist.");
                    continue;
                }

                if (getPayeeAcc == null)
                {
                    Console.WriteLine($"Payee Account {payeeAcc} does not exist.");
                    continue;
                }

                if (getPayerAcc.Balance < amount)
                {
                    Console.WriteLine($"Transaction failed. Payer Account {payerAcc} has insufficient funds.");
                    continue;
                }

                else
                {
                    getPayerAcc.Balance -= amount;
                    getPayeeAcc.Balance += amount;

                    var getAccount = accounts.Where(a => a.AccountNo == payerAcc || a.AccountNo == payeeAcc).ToList();
                    Console.WriteLine("\nTransaction successful. Updated account information:");
                    Console.WriteLine($"{"Account Number",-20}{"Balance"}");
                    foreach (var a in getAccount)
                    {
                        Console.WriteLine($"{a.AccountNo,-20}{a.Balance:F2}");
                    }
                }

            }
            catch
            {
                Console.WriteLine("Invalid input. Please follow the format: Payer Account|Payee Account|Amount");
            }
        }
    }


    static decimal CalculateInterest(string account, DateTime statementMonth)
    {
        var monthStart = new DateTime(statementMonth.Year, statementMonth.Month, 1);
        var monthEnd = new DateTime(statementMonth.Year, statementMonth.Month, DateTime.DaysInMonth(statementMonth.Year, statementMonth.Month));
        var statementTransactions = transactions
                .Where(t => t.Account == account && t.Date < monthEnd)
                .ToList();

        // Get interest rates applicable to the month
        var applicableRates = interestRateRules
            .Where(r => r.EffectiveDate <= monthEnd)
            .OrderBy(r => r.EffectiveDate)
            .ToList();

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

// Model Classes
class Account
{
    public int Id { get; set; }
    public string AccountNo { get; set; }
    public decimal Balance { get; set; }
}

class Transaction
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Account { get; set; }
    public string Type { get; set; }
    public decimal Amount { get; set; }
}

class InterestRateRule
{
    public DateTime EffectiveDate { get; set; }
    public string RuleId { get; set; }
    public decimal Rate { get; set; }
}