using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace МенОсобФін_КП_Барило
{
    public class Plan
    {
        public string Goal { get; set; }
        public decimal End { get; set; }
        public decimal Invested { get; set; }
        public DateTime GoalDeadline { get; set; }
        public string Category { get; set; }

        private FinanceManager manager;

        public Plan(FinanceManager manager)
        {
            this.manager = manager;
        }

        public string MakeInvestment(decimal value)
        {
            var result = manager.Invest(value);

            if (result != null)
                return result;

            Invested += value;
            return null;
        }

        public void Clear()
        {
            End = 0;
            Invested = 0;
            Goal = "";
            GoalDeadline = DateTime.MinValue;
            Category = "";
        }


    }
}
