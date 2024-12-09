using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
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
                        string accountNo = parts[1];
                        string type = parts[2].ToUpper();               // Set type input to uppercase
                        if (!decimal.TryParse(parts[3], out decimal amount) || amount <= 0)
                        {
                            throw new InvalidOperationException("Amount must be a positive decimal number.");
                        }

                        if (type != "D" && type != "W")
                        {
                            throw new Exception("Invalid transaction type. Use 'D' for Deposit or 'W' for Withdrawal.");
                        }

                        Transaction.CreateTransaction(date, accountNo, type, amount);

                        decimal balance = Transaction.GetBalance(accountNo);
                        BankAccount.CreateAccount(accountNo, balance);

                        Transaction.DisplayTransactionInfo(accountNo);
                        BankAccount.DisplayAccountInfo(accountNo);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
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
                        if (!decimal.TryParse(parts[2], out decimal rate) || rate <= 0)
                        {
                            throw new InvalidOperationException("Rate must be a positive decimal number.");
                        }

                        var checkRuleId = InterestRate.GetRule(ruleId);

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
                                    throw new Exception($"Invalid input. Use 'Y' to update existing {ruleId} or 'N' re-enter Rule Id.");
                                }

                                else
                                {
                                    if (invalidIdinput == "Y")
                                    {
                                        InterestRate.UpdateRule(date, ruleId, rate);
                                        break;
                                    }
                                    else if (invalidIdinput == "N")
                                    {
                                        Console.WriteLine("\nRe-enter Rule Id:");
                                        string newRuleId = Console.ReadLine().ToUpper();

                                        InterestRate.CreateRule(date, newRuleId, rate);
                                        break;
                                    }
                                }
                            }
                        }

                        else
                        {
                            InterestRate.CreateRule(date, ruleId, rate);
                        }

                        InterestRate.DisplayRuleInfo();
                        continue;
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input. Please follow the format: DDMMYYYY|RuleId|Rate in %");
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
                        var lastDateOfMonth = new DateTime(statementMonth.Year, statementMonth.Month, DateTime.DaysInMonth(statementMonth.Year, statementMonth.Month));
                        var statementTransactions = Transaction.GetStatementTransaction(statementMonth, account);

                        if (!statementTransactions.Any())
                        {
                            throw new Exception("No transactions found for the specified account and month.");
                        }

                        // Calculate interest
                        decimal interest = InterestRate.CalculateInterest(account, statementMonth);

                        // Add interest as a transaction
                        Transaction.CreateTransaction(lastDateOfMonth, account, "I", interest);

                        Transaction.DisplayTransactionInfoWithBalance(statementMonth, account);
                        break;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
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
                            throw new InvalidOperationException("Rate must be a positive decimal number.");
                        }

                        if (payeeAcc == payerAcc)
                        {
                            throw new Exception("Cannot transfer to the same account.");
                        }

                        var getPayerAcc = BankAccount.GetAccount(payerAcc);
                        var getPayeeAcc = BankAccount.GetAccount(payeeAcc);

                        if (getPayerAcc == null)
                        {
                            throw new Exception($"Payer Account {payerAcc} does not exist.");
                        }

                        if (getPayeeAcc == null)
                        {
                            throw new Exception($"Payee Account {payeeAcc} does not exist.");
                        }

                        if (BankAccount.GetBalance(payerAcc) < amount)
                        {
                            throw new Exception($"Transaction failed. Payer Account {payerAcc} has insufficient funds.");
                        }

                        else
                        {
                            BankAccount.UpdateBalance(getPayerAcc, getPayeeAcc, amount);
                            BankAccount.DisplayTransferAccountInfo(payerAcc, payeeAcc);
                        }

                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input. Please follow the format: Payer Account|Payee Account|Amount");
                    }
                }
            }
        }
    }
}
