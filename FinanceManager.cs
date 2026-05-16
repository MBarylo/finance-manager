using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
using static МенОсобФін_КП_Барило.Form1;

namespace МенОсобФін_КП_Барило
{
    public class FinanceManager
    {
        private List<Transaction> transactions = new List<Transaction>();
        public decimal Balance { get; set; }

        public decimal StartBalance { get; set; }

        private DataValidator validator = new DataValidator();

        public void SetTransactions(List<Transaction> list)
        {
            transactions = list;
        }

        public void SetBalance(decimal value)
        {
            Balance = value;
        }

        public string AddTransaction(Transaction transaction)
        {
            var error = validator.Validate(transaction);

            if (error != null)
                return error;

            if (transaction is Income)
            {
                Balance += transaction.Amount;
            }
            else if (transaction is Expense)
            {
                if (Balance < transaction.Amount)
                    return "Недостатньо коштів";

                Balance -= transaction.Amount;
            }

            transactions.Add(transaction);
            return null;
        }

        public string RemoveTransactions(List<Transaction> toRemove)
        {
            if (toRemove == null || toRemove.Count == 0)
                return "Немає транзакцій для видалення";

            foreach (var t in toRemove)
            {
                if (t is Income)
                {
                    Balance -= t.Amount;
                }
                else if (t is Expense)
                {
                    Balance += t.Amount;
                }

                transactions.Remove(t);
            }

            return null; // успіх
        }

        public decimal CalculateBalance()
        {
            decimal total = StartBalance;

            foreach (Transaction t in transactions)
            {
                if (t is Income)
                    total += t.Amount;

                else if (t is Expense)
                    total -= t.Amount;
            }

            Balance = total;

            return Balance;
        }

        public List<Transaction> GetAll()
        {
            return transactions;
        }

        public string Invest(decimal value)
        {
            if (value <= 0)
                return "Сума має бути більше 0";

            if (Balance < value)
                return "Недостатньо коштів";

            Balance -= value;
            return null; // успіх
        }

        public void ClearAll()
        {
            transactions.Clear();
            Balance = 0;
        }


    }
}
