using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace МенОсобФін_КП_Барило
{
    public class Report
    {


        public string ReportTxt(List<Transaction> transactions, decimal balance, Plan plan, bool isCsv)
        {

            StringBuilder textContent = new StringBuilder();

            textContent.AppendLine("==================================================");
            textContent.AppendLine("--- Менеджер Особистих Фінансів: ЗВІТ ПРО РОБОТУ ---");
            textContent.AppendLine($"Експортовано: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            textContent.AppendLine("==================================================");
            textContent.AppendLine();

            // баланс
            textContent.AppendLine("--- ПОТОЧНИЙ БАЛАНС ---");
            textContent.AppendLine($"Поточний баланс: {balance:N2}");
            textContent.AppendLine(new string('-', 30));
            textContent.AppendLine();

            // план
            textContent.AppendLine("--- ФІНАНСОВА МЕТА (ПЛАН) ---");

            if (!string.IsNullOrEmpty(plan?.Goal) && plan.End > 0)
            {
                decimal progress = (plan.Invested / plan.End) * 100;

                textContent.AppendLine($"Мета: {plan.Goal}");
                textContent.AppendLine($"Цільова сума: {plan.End:N2}");
                textContent.AppendLine($"Накопичено: {plan.Invested:N2}");
                textContent.AppendLine($"Залишок: {(plan.End - plan.Invested):N2}");
                textContent.AppendLine($"Дедлайн: {plan.GoalDeadline:dd.MM.yyyy}");
                textContent.AppendLine($"Прогрес: {progress:N2}%");
            }
            else
            {
                textContent.AppendLine("Фінансова мета не встановлена.");
            }

            textContent.AppendLine(new string('-', 30));
            textContent.AppendLine();

            // транзакції
            textContent.AppendLine("--- ТРАНЗАКЦІЇ ---");

            if (isCsv)
            {
                textContent.AppendLine("Дата;Тип;Категорія;Сума;Опис");

                foreach (var t in transactions)
                {
                    string desc = t.Description?.Replace(";", ",") ?? "";
                    string type = t is Income ? "Дохід" : "Витрата";

                    textContent.AppendLine($"{t.Date:dd.MM.yyyy};{type};{t.Category};{t.Amount};{desc}");
                }
            }
            else
            {
                foreach (var t in transactions)
                {
                    string type = t is Income ? "Дохід" : "Витрата";

                    textContent.AppendLine($"[{t.Date:dd.MM.yyyy}] | {type} ({t.Category})");
                    textContent.AppendLine($"   Сума: {t.Amount}");
                    textContent.AppendLine($"   Опис: {t.Description}");
                    textContent.AppendLine(new string('-', 20));
                }
            }

            return textContent.ToString();



            return "";
        }

        public class FileService
        {
            public void DeleteFile(string path)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        public class LoadResult
        {
            public List<Transaction> Transactions { get; set; }
            public decimal Balance { get; set; }
            public Plan Plan { get; set; }
        }

        private DataValidator validator = new DataValidator();

        public LoadResult LoadFromFile(string path, char delimiter)
        {
            if (!File.Exists(path))
            {
                return new LoadResult
                {
                    Transactions = new List<Transaction>(),
                    Balance = 0,
                    Plan = new Plan(null)
                };
            }

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);

            var result = new LoadResult
            {
                Transactions = new List<Transaction>(),
                Plan = new Plan(null)
            };

            if (lines.Length == 0)
                return result;

            // HEADER
            var header = lines[0].Split(delimiter);

            if (header.Length >= 5)
            {
                decimal.TryParse(header[0], out decimal balance);
                decimal.TryParse(header[1], out decimal end);
                decimal.TryParse(header[2], out decimal invested);
                DateTime.TryParse(header[4], out DateTime deadline);

                result.Balance = balance;
                result.Plan.End = end;
                result.Plan.Invested = invested;
                result.Plan.Goal = header[3];
                result.Plan.GoalDeadline = deadline;
            }

            // TRANSACTIONS
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(delimiter);

                if (parts.Length >= 5 && DateTime.TryParse(parts[3], out DateTime date))
                {
                    Transaction t;

                    if (parts[0] == "Дохід")
                        t = new Income();
                    else
                        t = new Expense();

                    t.Category = parts[1];

                    if (!validator.TryParseAmount(parts[2], out decimal amount))
                        continue;

                    t.Amount = amount;
                    t.Date = date;
                    t.Description = parts[4];


                    var validationError = validator.Validate(t);

                    if (validationError == null)
                    {
                        result.Transactions.Add(t);
                    }

                }
            }

            return result;
        }

        public void SaveToFile(string path, List<Transaction> transactions, decimal balance, Plan plan, char delimiter)
        {
            var lines = new List<string>();

            // HEADER
            string headerLine = $"{balance}{delimiter}{plan.End}{delimiter}{plan.Invested}{delimiter}{plan.Goal}{delimiter}{plan.GoalDeadline:yyyy-MM-dd}";
            lines.Add(headerLine);

            // TRANSACTIONS
            foreach (var t in transactions)
            {
                var validationError = validator.Validate(t);

                if (validationError != null)
                    continue; // пропускаємо невалідні

                string type = t is Income ? "Дохід" : "Витрата";

                string cleanDescription = t.Description?.Replace(delimiter.ToString(), "") ?? "";

                string line = $"{type}{delimiter}{t.Category}{delimiter}{t.Amount}{delimiter}{t.Date:yyyy-MM-dd}{delimiter}{cleanDescription}";

                lines.Add(line);
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }
    }
}
