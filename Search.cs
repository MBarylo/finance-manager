using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace МенОсобФін_КП_Барило
{
    public class Search
    {

        public class TransactionFilter
        {

            public class FilterCriteria
            {
                public string Type { get; set; }
                public string Category { get; set; }
                public string Description { get; set; }
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
            }

            public List<Transaction> Apply(List<Transaction> transactions, FilterCriteria criteria)
            {
                IEnumerable<Transaction> filtered = transactions;

                if (!string.IsNullOrEmpty(criteria.Type) && criteria.Type != "Всі")
                {
                    filtered = filtered.Where(t =>
                        (t is Income && criteria.Type == "Дохід") ||
                        (t is Expense && criteria.Type == "Витрата"));
                }

                if (!string.IsNullOrEmpty(criteria.Category) && criteria.Category != "Всі")
                {
                    filtered = filtered.Where(t => t.Category == criteria.Category);
                }

                if (!string.IsNullOrWhiteSpace(criteria.Description))
                {
                    filtered = filtered.Where(t =>
                        t.Description != null &&
                        t.Description.IndexOf(criteria.Description, StringComparison.OrdinalIgnoreCase) >= 0
                    );
                }

                filtered = filtered.Where(t => t.Date.Date >= criteria.StartDate.Date);
                filtered = filtered.Where(t => t.Date.Date <= criteria.EndDate.Date);

                return filtered.ToList();
            }
        }

        public static List<Transaction> FindByCategory(List<Transaction> list, string category)
        {
            return list.Where(t => t.Category == category).ToList();
        }

        public static List<Transaction> FindByDate(List<Transaction> list, DateTime date)
        {
            return list.Where(t => t.Date.Date == date.Date).ToList();
        }

        public static List<Transaction> FindByType(List<Transaction> list, string type)
        {
            return list.Where(t => t.GetTransactionType() == type).ToList();
        }

        public static List<Transaction> FindByDescription(List<Transaction> list, string desc)
        {
            return list.Where(t => t.Description == desc).ToList();
        }
    }
}
