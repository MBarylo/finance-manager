using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace МенОсобФін_КП_Барило
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            table.AutoGenerateColumns = false;
            table.DataSource = transactionBindingSource;
            LoadDataFromTextFile();
            //LoadTransactionsFromFile();
            FullCombo();
            ApplyFilters();
        }

        
        //елемент транзакції
        public class Transaction
        {
            public string Type { get; set; }        //дохід або витрата
            public string Category { get; set; }    //дім, робота або особисте
            public string Amount { get; set; }     //сума
            public DateTime Date { get; set; }      //дата
            public string Description { get; set; } //опис

            
        }

        public class FormState
        {
            //фінанси
            public decimal Balance { get; set; }
            public decimal End { get; set; }
            public string Goal { get; set; }

            public decimal Meta { get; set; }

            public DateTime GoalDeadline { get; set; }



            //список транзакцій
            public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        }


        //функція заповнення даних в меню фільтрації
        public void FullCombo()
        {
            //заповнюємо фільтр типу
            typeFilter.Items.Add("Всі");
            typeFilter.Items.Add("Дохід");
            typeFilter.Items.Add("Витрата");

            //заповнюємо фільтр початкової та кінцевої дат
            dateFilter.Value = new DateTime(2025, 1, 1);
            dateEndFilter.Value = DateTime.Today;

            //очищаємо фільтр категорій та примусово вимикаємо його
            categoryFilter.Items.Clear();
            categoryFilter.Enabled = false;

            //на початку фільтр типу теж повинен бути пустим, так як в таблиці немає даних
            typeFilter.SelectedIndex = -1;
        }

        //функція правильного заповнення категорій
        private void typeFilter_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //очищаємо весь фільтр категорій
            categoryFilter.Items.Clear();

            //якщо фільтр типу обраний елемент вище або дорівнює індексу 0... 
            if (typeFilter.SelectedIndex >= 0)
            {
                //... присвоюємо значення обраного типу змінній
                string selectedType = typeFilter.SelectedItem.ToString();

                //якщо обраний тип "Дохід"... 
                if (selectedType == "Дохід")
                {
                    //вмикаємо фільтр категорій та заповнюємо його
                    categoryFilter.Enabled = true; // Увімкнути
                    categoryFilter.Items.Add("Всі");
                    categoryFilter.Items.Add("Зарплата");
                    categoryFilter.Items.Add("Продаж");
                    categoryFilter.Items.Add("Допомога");

                    //встановлюємо початковий фільтр категорій "Всі" за індексом 0
                    categoryFilter.SelectedIndex = 0;
                }

                //якщо обираємо фільтр типу "Витрата" - те саме, але заповнюємо фільтр категорій іншими даними
                else if (selectedType == "Витрата")
                {
                    categoryFilter.Enabled = true; 
                    categoryFilter.Items.Add("Всі");
                    categoryFilter.Items.Add("Продукти");
                    categoryFilter.Items.Add("Побут. хімія");
                    categoryFilter.Items.Add("Техніка");
                    categoryFilter.Items.Add("Подарунок");
                    categoryFilter.Items.Add("Меблі");
                    categoryFilter.Items.Add("Збір");
                    categoryFilter.SelectedIndex = 0; 
                }
                //якщо ж обираємо тип "Всі" - категорії не потрібні, тому вимикаємо їх
                else
                {
                    categoryFilter.Enabled = false;
                }
            }
            //якщо фільтр типу має індекс -1 (тобто не обраний тип) - вимикаємо категорії
            else
            {
                categoryFilter.Enabled = false;
            }

            //застосовуємо фільтри
            ApplyFilters();
        }

        //базовий набір даних, з якими будемо працювати
        //private string dataFile = "transactions.json";

        private List<Transaction> transactions = new List<Transaction>();

        private BindingSource transactionBindingSource = new BindingSource();

        private decimal balance = 0;

        private const string DataFile = "financial_data.txt";
        private const char Delimiter = '|';

        private string categoryGoal = "";
        private decimal end = 0;         
        private decimal goalAmount = 0;  
        private string goal = "";        
        private DateTime goalDeadline;

        //збереження у текстовий файл
        public void SaveDataToTextFile()
        {
            var lines = new List<string>();

            //запис основного фінансового стану (Баланс, Мета, Накопичено, Дедлайн)
            string headerLine = $"{this.balance}{Delimiter}{this.end}{Delimiter}{this.goalAmount}{Delimiter}{this.goal}{Delimiter}{this.goalDeadline:yyyy-MM-dd}";
            lines.Add(headerLine);

            //запис усіх транзакцій
            foreach (var t in this.transactions)
            {
                //формат: Type|Category|Amount|Date|Description
                string transactionLine = $"{t.Type}{Delimiter}{t.Category}{Delimiter}{t.Amount}{Delimiter}{t.Date:yyyy-MM-dd}{Delimiter}{t.Description.Replace(Delimiter.ToString(), "")}";
                lines.Add(transactionLine);
            }

            try
            {
                //записуємо всі рядки у файл
                File.WriteAllLines(DataFile, lines, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні даних у файл: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        //завантаження даних з текстового файлу
        private void LoadDataFromTextFile()
        {
            if (!File.Exists(DataFile))
            {
                //якщо файл не існує, починаємо з порожніми/початковими значеннями
                this.transactions = new List<Transaction>();
                this.balance = 0;
                this.end = 0;
                this.goalAmount = 0;
                this.goal = "";
                this.goalDeadline = DateTime.MinValue;
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(DataFile, Encoding.UTF8);

                if (lines.Length == 0) return;

                //зчитування основного стану (HEADER)
                string[] headerParts = lines[0].Split(Delimiter);
                if (headerParts.Length >= 5) //перевіряємо, чи є всі 5 елементів
                {
                    //balance
                    if (decimal.TryParse(headerParts[0], out decimal loadedBalance)) this.balance = loadedBalance;
                    //end
                    if (decimal.TryParse(headerParts[1], out decimal loadedEnd)) this.end = loadedEnd;
                    //goalAmount
                    if (decimal.TryParse(headerParts[2], out decimal loadedGoalAmount)) this.goalAmount = loadedGoalAmount;
                    // goal
                    this.goal = headerParts[3];
                    //goalDeadline
                    if (DateTime.TryParse(headerParts[4], out DateTime loadedDeadline)) this.goalDeadline = loadedDeadline;
                }

                //зчитування транзакцій (починаючи з другого рядка)
                this.transactions.Clear();
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(Delimiter);
                    if (parts.Length >= 5)
                    {
                        if (DateTime.TryParse(parts[3], out DateTime tDate))
                        {
                            var transaction = new Transaction
                            {
                                Type = parts[0],
                                Category = parts[1],
                                Amount = parts[2], //зберігаємо як текст, щоб уникнути помилок форматування
                                Date = tDate,
                                Description = parts[4]
                            };
                            this.transactions.Add(transaction);
                        }
                    }
                }

                
                transactionBindingSource.DataSource = this.transactions;
                transactionBindingSource.ResetBindings(false);
                Balance(); 

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //public void SaveDataToJson()
        //{

        //    var formState = new FormState
        //    {
        //        Balance = this.balance,

        //        End = this.end,
        //        Meta = this.goalAmount, 
        //        Goal = this.goal,
        //        GoalDeadline = this.goalDeadline,
        //        Transactions = transactions

        //    };

        //    string jsonString = JsonConvert.SerializeObject(formState, Formatting.Indented);
        //    File.WriteAllText(dataFile, jsonString);
        //}

        //private void LoadTransactionsFromFile()
        //{
        //    try
        //    {
        //        if (File.Exists(dataFile))
        //        {
        //            string json = File.ReadAllText(dataFile);
        //            var formState = JsonConvert.DeserializeObject<FormState>(json);

        //            if (formState != null)
        //            {
        //                this.balance = formState.Balance;

        //                this.transactions = formState.Transactions ?? new List<Transaction>();
        //                this.end = formState.End;
        //                this.goalAmount = formState.Meta;
        //                this.goal = formState.Goal;
        //                this.goalDeadline = formState.GoalDeadline;



        //            }
        //        }


        //        transactionBindingSource.DataSource = this.transactions;


        //        Balance();

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Помилка при завантаженні: " + ex.Message);
        //        this.transactions = new List<Transaction>();
        //        transactionBindingSource.DataSource = this.transactions; 
        //    }
        //}

        //завантажувач даних з джейсон файлу


        //функція фільтрації
        private void ApplyFilters()
        {
            //створюємо відфільтрований масив операцій
            IEnumerable<Transaction> filteredList = this.transactions;

            //присвоюємо обраний тип відповідній змінній
            string selectedType = typeFilter.SelectedItem?.ToString();

            //якщо обраний тип - "Всі" і він не пустий - фільтруємо за цим обраним типом
            if (selectedType != "Всі" && !string.IsNullOrEmpty(selectedType))
            {
                filteredList = filteredList.Where(t => t.Type == selectedType);
            }

            //той самий механізм з категорією
            string selectedCategory = categoryFilter.SelectedItem?.ToString();

            if (selectedCategory != "Всі" && !string.IsNullOrEmpty(selectedCategory))
            {
                filteredList = filteredList.Where(t => t.Category == selectedCategory);
            }

            //і описом також
            string descriptionText = descFilter.Text.Trim(); 

            if (!string.IsNullOrEmpty(descriptionText))
            {
                
                filteredList = filteredList.Where(t =>
                    t.Description != null &&
                    t.Description.IndexOf(descriptionText, StringComparison.OrdinalIgnoreCase) >= 0
                );
            }

            //тут фільтр по даті. Фальтруємо дані, які за датою більші за початкову дату. Аналогічно до кінцевої нижче
            DateTime startDate = dateFilter.Value.Date;
            filteredList = filteredList.Where(t => t.Date.Date >= startDate);

            DateTime endDate = dateEndFilter.Value.Date;
            filteredList = filteredList.Where(t => t.Date.Date <= endDate);


            //присвоюємо відфільтрований масив даних масиву даних таблиці та оновлюємо таблицю
            transactionBindingSource.DataSource = filteredList.ToList();
            table.Refresh();

        }

  
        //тут ідуть фільтратори
        private void categoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dateFilter_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dateEndFilter_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }


        private void descFilter_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }



        //завантажувач форми
        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        //відкриття форми файлу
        private void btnPlan_Click(object sender, EventArgs e) 
        {
            using (Form5 form5 = new Form5(this, this.balance, this.end, this.goalAmount, this.goal, this.goalDeadline))
            {
                form5.ShowDialog();

                SaveDataToTextFile();
            }
        }



        //оновлення балансу
        private void newBalance_Click(object sender, EventArgs e)
        {
            using (Form6 form6 = new Form6(this.balance))
            {
                DialogResult результат = form6.ShowDialog();

                if (результат == DialogResult.OK)
                {

                    decimal newBalance = form6.NewBalance;





                    this.balance = newBalance;

                    Balance();

                    SaveDataToTextFile();
                    //SaveDataToJson();

                    MessageBox.Show($"Баланс успішно оновлено до: {newBalance:N2}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }


        }






        //додавання операції
        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            //викликаємо діалогове вікно
            using (Form2 form2 = new Form2())
            {
                DialogResult res = form2.ShowDialog();

                //якщо отримуємо результат ОК - присвоюємо введені значення типу і суми операції відповідним змінним
                if (res == DialogResult.OK)
                {
                    string type = form2.type;
                    string sumText = form2.sum;

                    //так, як ми отримуємо число текстом - перетворюємо текст на число і присвоюємо його змінній sumValue
                    if (decimal.TryParse(sumText, out decimal sumValue))
                    {
                        
                        //якщо операція є доходом - плюсуємо суму операції до значення балансу
                        if (type == "Дохід")
                        {
                            this.balance += sumValue;
                        }

                        //якщо витрата - то...
                        else if (type == "Витрата")
                        {
                            //... якщо сума операції більша за суму балансу - не додаємо операцію
                            if (sumValue > this.balance)
                            {
                                MessageBox.Show("Ви не можете витратити цю суму, оскільки на балансі недостатньо коштів.");
                                return; 
                            }

                            //... якщо менша - мінусуємо від балансу суму операції
                            else
                            {
                                this.balance -= sumValue;
                            }
                        }

                        //додаємо в масив транзакцій нову транзакцію, попередньо заповнивши її усіма введеними даними
                        Transaction newTransaction = new Transaction
                        {
                            Type = type,
                            Category = form2.category,
                            Amount = sumText,
                            Date = form2.date,
                            Description = form2.desc
                        };

                        //додаємо цю транзакцію в масив транзакцій (в таблицю)
                        this.transactions.Add(newTransaction);

                        //оновлюємо таблицю
                        transactionBindingSource.ResetBindings(false);

                        //оновлюємо значення балансу
                        Balance();
                        SaveDataToTextFile();
                        //SaveDataToJson();

                        //якщо фільтр типу не стоїть на "Всі" (індекс 0), ми його міняємо.
                        if (typeFilter.SelectedIndex != 0)
                        {
                            //якщо індекс інший, присвоєння 0 викличе подію SelectIndexChanged, яка вже містить виклик ApplyFilters()
                            typeFilter.SelectedIndex = 0;
                        }
                        else
                        {
                            //якщо індекс вже 0 ("Всі"), подія не спрацює. Тому ми повинні викликати ApplyFilters() вручну, щоб відобразити нову транзакцію.
                            
                            ApplyFilters();
                        }

                        MessageBox.Show($"Операцію успішно додано. Новий баланс: {this.balance:N2}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Помилка: Сума операції не є коректним числом.", "Помилка введення", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void btnDeleteFiltered_Click(object sender, EventArgs e)
        {
            
            var filteredTransactions = transactionBindingSource.Cast<Transaction>().ToList();

            if (filteredTransactions.Count == 0)
            {
                MessageBox.Show("Немає відфільтрованих операцій для видалення.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            
            string message = filteredTransactions.Count == this.transactions.Count
                ? "Ви впевнені, що хочете видалити ВСІ операції?"
                : $"Ви впевнені, що хочете видалити всі відфільтровані операції ({filteredTransactions.Count} шт.)?";

            DialogResult result = MessageBox.Show(message, "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (var t in filteredTransactions)
                    {
                        
                        if (decimal.TryParse(t.Amount, out decimal sumValue))
                        {
                            if (t.Type == "Дохід")
                            {
                                this.balance -= sumValue; 
                            }
                            else if (t.Type == "Витрата")
                            {
                                this.balance += sumValue; 
                            }
                        }

                        
                        this.transactions.Remove(t);
                    }

                    
                    Balance(); 

                    
                    transactionBindingSource.DataSource = null;
                    transactionBindingSource.DataSource = this.transactions;

                    ApplyFilters();

                    SaveDataToTextFile();
                    // SaveDataToTextFile();


                    MessageBox.Show("Операції успішно видалено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




        //закриття додатку
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        //довідка
        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "ІНСТРУКЦІЯ ДЛЯ КОРИСТУВАННЯ ФОРМОЮ:\n\n" +
                "• Натисніть на 'Додати операцію', щоб додати вашу витрату або дохід, вони відобразяться у таблиці нижче\n" +
                "• 'Дохід' та 'Витрата' мають різні категорії.\n" +
                "• Для перегляду вашої мети використовуйте 'План'.\n" +
                "• Для оновлення балансу використовуйте 'Оновити баланс'.\n" +
                "• Щоб зберегти ваші операції в текстовий файл натискайте на кнопку 'Зберегти у файл'.\n" +
                "• В підменю фільтрації можна відфільтрувати дані таблиці за типом, категорією, описом і датою";

            MessageBox.Show(
                helpText,
                "Довідка по роботі з формою",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }



       

        //вивід балансу
        private void Balance()
        {
            lblBalance.Text = balance.ToString("N2");
            lblBalance.Refresh();
        }

        //отримання балансу на форми додавання операції, відкладення на мету і плану
        public decimal GetBalance()
        {
            return this.balance;
        }

        //оновлення балансу на формі плану
        public void UpdateBalance(decimal newBalance)
        {
            this.balance = newBalance;
            Balance(); 
        }

        //оновлення плану
        public void UpdateGoal(string newGoal, decimal newEnd, DateTime newDeadline, string newCategory)
        {
            this.goal = newGoal;
            this.end = newEnd;
            this.goalDeadline = newDeadline;
            this.categoryGoal = newCategory;
            
        }

        //оновлення суми цілі
        public void UpdateGoalAmount(decimal newGoalAmount)
        {
            this.goalAmount = newGoalAmount;
        }


        //очищення
        private void Clear(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Ви впевнені, що хочете видалити всі дані (баланс, план та транзакції)? Цю дію не можна скасувати.",
        "Підтвердження видалення",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    
                  
                    this.transactions.Clear();


                    table.Rows.Clear();
                    
                    this.balance = 0;

                    
                    this.end = 0;
                    this.goalAmount = 0;
                    this.goal = "";
                    this.goalDeadline = DateTime.MinValue;
                    this.categoryGoal = "";

                    Balance();
                    SaveDataToTextFile();


                    if (File.Exists(DataFile))
                    {
                        File.Delete(DataFile);
                    }

                    MessageBox.Show("Всі фінансові дані успішно очищено.", "Очищення завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при очищенні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        //скидування фільтрів
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            dateFilter.Value = new DateTime(2025, 1, 1);


            dateEndFilter.Value = DateTime.Today;


            typeFilter.SelectedIndex = 0;
            ApplyFilters();
        }

        private void ExportAllDataToTxt(object sender, EventArgs e)
        {
            //отримуємо повний список транзакцій
            var allTransactions = this.transactions;

            if (allTransactions == null || allTransactions.Count == 0)
            {
                MessageBox.Show("Немає даних для експорту.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //створення діалогового вікна "Зберегти файл"
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовий файл (*.txt)|*.txt|CSV файл (*.csv)|*.csv";
                saveFileDialog.Title = "Зберегти фінансовий звіт";
                saveFileDialog.FileName = "Financial_Report_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //форматування даних у текстовий рядок
                        StringBuilder textContent = new StringBuilder();

                        //заголовок звіту
                        textContent.AppendLine("==================================================");
                        textContent.AppendLine("--- Менеджер Особистих Фінансів: ЗВІТ ПРО РОБОТУ ---");
                        textContent.AppendLine($"Експортовано: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                        textContent.AppendLine("==================================================");
                        textContent.AppendLine();

                        //баланс
                        textContent.AppendLine("--- ПОТОЧНИЙ БАЛАНС ---");
                        textContent.AppendLine($"Поточний баланс: {this.balance:N2}");
                        textContent.AppendLine(new string('-', 30));
                        textContent.AppendLine();


                        //фінансова мета
                        textContent.AppendLine("--- ФІНАНСОВА МЕТА (ПЛАН) ---");
                        if (!string.IsNullOrEmpty(this.goal) && this.end > 0)
                        {
                            //розрахунок прогресу
                            decimal progress = (this.end > 0) ? (this.goalAmount / this.end) * 100 : 0;

                            textContent.AppendLine($"Мета: {this.goal}");
                            textContent.AppendLine($"Цільова сума: {this.end:N2}");
                            textContent.AppendLine($"Накопичено: {this.goalAmount:N2}");
                            textContent.AppendLine($"Залишок до мети: {(this.end - this.goalAmount):N2}");
                            textContent.AppendLine($"Дедлайн: {this.goalDeadline:dd.MM.yyyy}");
                            textContent.AppendLine($"Прогрес: {progress:N2}%");
                        }
                        else
                        {
                            textContent.AppendLine("Фінансова мета не встановлена.");
                        }
                        textContent.AppendLine(new string('-', 30));
                        textContent.AppendLine();


                        //деталі транзакцій
                        textContent.AppendLine("--- ДЕТАЛІ ВСІХ ТРАНЗАКЦІЙ ---");

                        //додаємо рядок заголовків для кращої читабельності, якщо це CSV
                        if (saveFileDialog.FileName.EndsWith(".csv"))
                        {
                            //якщо користувач обрав CSV, використовуємо формат CSV
                            textContent.AppendLine("Дата;Тип;Категорія;Сума;Опис");
                            foreach (var t in allTransactions)
                            {
                                //перевіряємо, щоб опис не містив роздільника
                                string cleanDescription = t.Description?.Replace(";", ",") ?? "";

                                textContent.AppendLine($"{t.Date:dd.MM.yyyy};{t.Type};{t.Category};{t.Amount};{cleanDescription}");
                            }
                        }
                        else //звичайний текстовий звіт
                        {
                            foreach (var t in allTransactions)
                            {
                                textContent.AppendLine($"[{t.Date.ToShortDateString()}] | {t.Type} ({t.Category}):");
                                textContent.AppendLine($"   Сума: {t.Amount}");
                                textContent.AppendLine($"   Опис: {t.Description}");
                                textContent.AppendLine(new string('-', 20)); //роздільник між записами
                            }
                        }

                        //записуємо вміст у вибраний файл
                        File.WriteAllText(saveFileDialog.FileName, textContent.ToString(), Encoding.UTF8);

                        MessageBox.Show($"Звіт успішно збережено у файлі:\n{saveFileDialog.FileName}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка при збереженні файлу: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }
    }

}
