using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApp
{
    public class Transaction
    {
        private static List<Transaction> transactions = new List<Transaction>();

        private int id;
        private DateTime date;
        private string accountNo;
        private string type;
        private decimal amount;

        public Transaction(DateTime date, string accountNo, string type, decimal amount)
        {
            this.id = transactions.Count + 1;
            this.date = date;
            this.accountNo = accountNo;
            this.type = type;
            this.amount = amount;
        }

        public int Id => id;
        public DateTime Date => date;
        public string AccountNo => accountNo;
        public string Type => type;
        public decimal Amount => amount;

        public static decimal GetBalance(string accountNo)
        {
            return transactions.Where(t => t.accountNo == accountNo).Sum(t => t.type == "D" ? t.amount : -t.amount);
        }

        public static void CreateTransaction(DateTime date, string accountNo, string type, decimal amount)
        {
            var getBalanceBefTrx = transactions.Where(t => t.accountNo == accountNo && t.date <= date).Sum(t => t.type == "D" ? t.amount : -t.amount);
            if (type == "W" && (getBalanceBefTrx - amount) < 0)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }

            if (type == "D" && amount < 0)
            {
                throw new InvalidOperationException("Please enter a valid amount.");
            }

            else
            {
                var newTransaction = new Transaction(date, accountNo, type, amount);
                transactions.Add(newTransaction);

                Console.WriteLine("Transaction created successfully!");
            }
        }

        public static List<Transaction> GetStatementTransaction(DateTime statementMonth, string accountNo)
        {
            var statementTransactions = transactions
                    .Where(t => t.accountNo == accountNo && t.date.Year == statementMonth.Year && t.date.Month == statementMonth.Month)
                    .OrderBy(t => t.date)
                    .ThenBy(t => t.id)
                    .ToList();

            return statementTransactions;
        }

        public static List<Transaction> GetTransactionBeforeMonthEnd(DateTime monthEnd, string accountNo)
        {
            var statementTransactions = transactions
                .Where(t => t.accountNo == accountNo && t.Date < monthEnd)
                .ToList();

            return statementTransactions;
        }

        public static void DisplayTransactionInfo(string accountNo)
        {
            var accountTransactions = transactions.Where(t => t.accountNo == accountNo).OrderBy(t => t.date).ThenBy(t => t.id);

            Console.WriteLine("\nTransactions for account: " + accountNo);
            Console.WriteLine($"{"Date",-14}{"Txn ID",-10}{"Type",-10}{"Amount",-10}");
            foreach (var t in accountTransactions)
            {
                Console.WriteLine($"{t.date,-14:dd-MM-yyyy}{t.id,-10}{t.type,-10}{t.amount,-10:F2}");
            }
        }
        public static void DisplayTransactionInfoWithBalance(DateTime statementMonth, string accountNo)
        {
            var statementTransactions = GetStatementTransaction(statementMonth, accountNo);
            decimal balance = transactions
                .Where(t => t.accountNo == accountNo && t.date < statementTransactions.First().date)
                .Sum(t => t.type == "D" ? t.amount : -t.amount);

            Console.WriteLine("\nAccount Statement for: " + accountNo);
            Console.WriteLine($"{"Date",-14}{"Txn ID",-10}{"Type",-10}{"Amount",-12}{"Balance"}");
            foreach (var t in statementTransactions)
            {
                balance += t.Type == "D" ? t.Amount : t.Type == "W" ? -t.Amount : t.Amount;
                Console.WriteLine($"{t.date,-14:dd-MM-yyyy}{t.id,-10}{t.type,-10}{t.amount,-12:F2}{balance:F2}");
            }
        }
    }
}
