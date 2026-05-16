using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace МенОсобФін_КП_Барило
{
    internal class Income : Transaction
    {
        public override string GetTransactionType()
        {
            return "Дохід";
        }
    }
}
