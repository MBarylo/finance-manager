using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace МенОсобФін_КП_Барило
{
    public class DataValidator
    {

        public string Validate(Transaction t)
        {
            if (t == null)
                return "Транзакція не задана";

            if (string.IsNullOrWhiteSpace(t.Category))
                return "Оберіть категорію";

            if (t.Amount <= 0)
                return "Сума має бути більше 0";

            if (t.Date == default)
                return "Оберіть дату";

            return null;
        }

        public bool TryParseAmount(string value, out decimal result)
        {
            return decimal.TryParse(value.Trim(), out result);
        }
    }
}
