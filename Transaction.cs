using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace МенОсобФін_КП_Барило
{
    public abstract class Transaction
    {
        public string Type => GetTransactionType();
        public string Category { get; set; }    //дім, робота або особисте
        public decimal Amount { get; set; }     //сума
        public DateTime Date { get; set; }      //дата
        public string Description { get; set; } //опис

        public abstract string GetTransactionType();
    }
}
