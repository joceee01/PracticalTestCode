using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApp
{
    public class BankAccount
    {
        private static List<BankAccount> accounts = new List<BankAccount>();

        private string accountNo;
        private decimal balance;

        public BankAccount(string accountNo, decimal balance)
        {
            this.accountNo = accountNo;
            this.balance = balance;
        }

        public string AccountNo => accountNo;
        public decimal Balance => balance;

        public static BankAccount GetAccount(string accountNo)
        {
            var getAcc = accounts.Where(a => a.accountNo == accountNo).FirstOrDefault();
            return getAcc;
        }

        public static void CreateAccount(string accountNo, decimal balance)
        {
            var checkAcc = BankAccount.GetAccount(accountNo);
            if (checkAcc != null)
            {
                checkAcc.balance = balance;
                Console.WriteLine("Account balance updated successfully!");
            }

            // Create a new account
            var newAccount = new BankAccount(accountNo, balance);
            accounts.Add(newAccount);

            Console.WriteLine("Account created successfully!");
        }

        public static decimal GetBalance(string accountNo)
        {
            var getAccount = BankAccount.GetAccount(accountNo);
            return getAccount.balance;
        }

        public static void UpdateBalance(BankAccount getPayerAcc, BankAccount getPayeeAcc, decimal amount)
        {
            getPayerAcc.balance -= amount;
            getPayeeAcc.balance += amount;
        }

        public static void DisplayAccountInfo(string accountNo)
        {
            var getAccount = BankAccount.GetAccount(accountNo);

            Console.WriteLine("\nAccount Information");
            Console.WriteLine($"{"Account Number",-20}{"Balance"}");
            Console.WriteLine($"{getAccount.AccountNo,-20}{getAccount.Balance:F2}");
        }

        public static void DisplayTransferAccountInfo(string payerAcc, string payeeAcc)
        {
            var getAccount = accounts.Where(a => a.accountNo == payerAcc || a.accountNo == payeeAcc).ToList();
            Console.WriteLine("\nTransaction successful. Updated account information:");
            Console.WriteLine($"{"Account Number",-20}{"Balance"}");
            foreach (var a in getAccount)
            {
                Console.WriteLine($"{a.AccountNo,-20}{a.Balance:F2}");
            }
        }
    }
}
