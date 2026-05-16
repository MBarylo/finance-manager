using Guna.UI2.WinForms;
using Newtonsoft.Json.Linq;
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
using static МенОсобФін_КП_Барило.Report;
using static МенОсобФін_КП_Барило.Search;
using static МенОсобФін_КП_Барило.Search.TransactionFilter;

namespace МенОсобФін_КП_Барило
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            manager = new FinanceManager();
            plan = new Plan(manager);

            table.AutoGenerateColumns = false;
            table.DataSource = transactionBindingSource;

            LoadDataFromTextFile();
            FullCombo();
            ApplyFilters();
        }

        //оголошення менеджера
        private FinanceManager manager;
        private Plan plan;


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



        private const string DataFile = "financial_data.txt";
        private const char Delimiter = '|';



        //збереження у текстовий файл
        public void SaveDataToTextFile()
        {
            try
            {
                var saver = new Report();

                saver.SaveToFile(
                    DataFile,
                    manager.GetAll(),
                    manager.Balance,
                    plan,
                    Delimiter
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        //завантаження даних з текстового файлу
        private void LoadDataFromTextFile()
        {
            try
            {
                var loader = new Report();
                var data = loader.LoadFromFile(DataFile, Delimiter);

                manager.SetTransactions(data.Transactions);
                manager.SetBalance(data.Balance);

                plan.End = data.Plan.End;
                plan.Invested = data.Plan.Invested;
                plan.Goal = data.Plan.Goal;
                plan.GoalDeadline = data.Plan.GoalDeadline;

                transactionBindingSource.DataSource = manager.GetAll();
                transactionBindingSource.ResetBindings(false);

                Balance();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
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
            var criteria = new FilterCriteria
            {
                Type = typeFilter.SelectedItem?.ToString(),
                Category = categoryFilter.SelectedItem?.ToString(),
                Description = descFilter.Text.Trim(),
                StartDate = dateFilter.Value,
                EndDate = dateEndFilter.Value
            };

            var filter = new TransactionFilter();

            var result = filter.Apply(manager.GetAll(), criteria);

            transactionBindingSource.DataSource = result;
            transactionBindingSource.ResetBindings(false);
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
            using (Form5 form5 = new Form5(this, manager.Balance, plan.End, plan.Invested, plan.Goal, plan.GoalDeadline))
            {
                form5.ShowDialog();

                SaveDataToTextFile();
            }
        }



        //оновлення балансу
        private void newBalance_Click(object sender, EventArgs e)
        {
            using (Form6 form6 = new Form6(manager.Balance))
            {
                DialogResult результат = form6.ShowDialog();

                if (результат == DialogResult.OK)
                {

                    decimal newBalance = form6.NewBalance;





                    
                    manager.StartBalance = newBalance;
                    manager.CalculateBalance();

                    Balance();

                    SaveDataToTextFile();
                    //SaveDataToJson();

                    MessageBox.Show($"Баланс успішно оновлено до: {newBalance:N2}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }


        }






        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            using (Form2 form2 = new Form2(manager)) 
            {
                if (form2.ShowDialog() == DialogResult.OK)
                {
                    var transaction = form2.CreatedTransaction;

                    var result = manager.AddTransaction(transaction);

                    if (result != null)
                    {
                        MessageBox.Show(result);
                        return;
                    }

                    // оновлюємо таблицю
                    transactionBindingSource.DataSource = manager.GetAll();
                    transactionBindingSource.ResetBindings(false);

                    // оновлюємо баланс
                    Balance();

                    SaveDataToTextFile();

                    if (typeFilter.SelectedIndex != 0)
                        typeFilter.SelectedIndex = 0;
                    else
                        ApplyFilters();

                    MessageBox.Show(
                        $"Операцію успішно додано. Новий баланс: {manager.Balance:N2}",
                        "Успіх",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }


        private void btnDeleteFiltered_Click(object sender, EventArgs e)
        {
            var filteredTransactions = transactionBindingSource.Cast<Transaction>().ToList();

            if (filteredTransactions.Count == 0)
            {
                MessageBox.Show("Немає відфільтрованих операцій.");
                return;
            }

            string message = filteredTransactions.Count == manager.GetAll().Count
                ? "Ви впевнені, що хочете видалити ВСІ операції?"
                : $"Видалити {filteredTransactions.Count} операцій?";

            if (MessageBox.Show(message, "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var result = manager.RemoveTransactions(filteredTransactions);

                if (result != null)
                {
                    MessageBox.Show(result);
                    return;
                }

                // оновлення UI
                transactionBindingSource.DataSource = manager.GetAll();
                transactionBindingSource.ResetBindings(false);

                Balance();
                ApplyFilters();
                SaveDataToTextFile();

                MessageBox.Show("Операції видалено.");
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
            lblBalance.Text = manager.Balance.ToString("N2");
            lblBalance.Refresh();
        }

        //отримання балансу на форми додавання операції, відкладення на мету і плану
        public decimal GetBalance()
        {
            return manager.Balance;
        }

        //оновлення балансу на формі плану
        public void UpdateBalance(decimal newBalance)
        {
            manager.Balance = newBalance;
            Balance(); 
        }

        //оновлення плану
        public void UpdateGoal(string newGoal, decimal newEnd, DateTime newDeadline, string newCategory)
        {
            plan.Goal = newGoal;
            plan.End = newEnd;
            plan.GoalDeadline = newDeadline;
            plan.Category = newCategory;
            
        }

        //оновлення суми цілі
        public void UpdateGoalAmount(decimal newGoalAmount)
        {
            plan.Invested = newGoalAmount;
        }


        //очищення
        private void Clear(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Ви впевнені, що хочете видалити всі дані?",
                "Підтвердження",
                MessageBoxButtons.YesNo
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    manager.ClearAll();
                    plan.Clear();

                    // UI оновлення
                    transactionBindingSource.DataSource = manager.GetAll();
                    transactionBindingSource.ResetBindings(false);

                    Balance();

                    SaveDataToTextFile();

                    // файл
                    var fileService = new FileService();
                    fileService.DeleteFile(DataFile);

                    MessageBox.Show("Дані очищено");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
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
            var allTransactions = manager.GetAll();

            if (allTransactions == null || allTransactions.Count == 0)
            {
                MessageBox.Show("Немає даних для експорту.");
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "TXT (*.txt)|*.txt|CSV (*.csv)|*.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var exporter = new Report();

                        bool isCsv = saveFileDialog.FileName.EndsWith(".csv");

                        string content = exporter.ReportTxt(
                            allTransactions,
                            manager.Balance,
                            plan,
                            isCsv
                        );

                        File.WriteAllText(saveFileDialog.FileName, content, Encoding.UTF8);

                        MessageBox.Show("Звіт збережено!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка: " + ex.Message);
                    }
                }
            }
        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void btnEditTransaction_Click(object sender, EventArgs e)
        {
            // якщо після фільтрації немає транзакцій
            if (table.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Немає транзакцій для редагування."
                );
                return;
            }

            // якщо знайдено більше однієї транзакції
            if (table.Rows.Count > 1)
            {
                MessageBox.Show(
                    "Для редагування потрібно знайти лише одну транзакцію."
                );
                return;
            }

            // отримуємо єдину транзакцію
            Transaction selectedTransaction =
                (Transaction)table.Rows[0].DataBoundItem;

            // відкриваємо форму редагування
            using (Form2 form2 = new Form2(selectedTransaction))
            {
                DialogResult res = form2.ShowDialog();

                if (res == DialogResult.OK)
                {
                    // перерахунок балансу
                    manager.CalculateBalance();

                    // оновлення таблиці
                    transactionBindingSource.ResetBindings(false);

                    // збереження
                    SaveDataToTextFile();

                    // оновлення інтерфейсу
                    Balance();

                    ApplyFilters();

                    MessageBox.Show(
                        "Транзакцію успішно оновлено.",
                        "Успіх",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            

        }
        }
    }

}
