using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static МенОсобФін_КП_Барило.Form1;

namespace МенОсобФін_КП_Барило
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            Full();



            InitializeToolTips();

            typ.SelectedIndexChanged += typ_SelectedIndexChanged;
        }

        public Form2(Transaction transaction)
        {
            InitializeComponent();

            Full();

            InitializeToolTips();

            // запам’ятовуємо транзакцію
            EditingTransaction = transaction;

            // заповнюємо форму даними
            typ.SelectedItem = transaction.Type;
            typ.Enabled = false;
            categ.SelectedItem = transaction.Category;
            amount.Text = transaction.Amount.ToString();
            dat.Value = transaction.Date;
            description.Text = transaction.Description;
        }

        public Transaction EditingTransaction = null;

        public Transaction CreatedTransaction { get; private set; }
        public DataValidator validator { get; private set; } = new DataValidator();

        private void InitializeToolTips()
        {
            //заповнення тултіпів
            toolTip1.SetToolTip(amount, "Введіть суму операції (обов'язково)");

            
            toolTip1.SetToolTip(description, "Короткий опис транзакції (необов'язково)");

            
            toolTip1.SetToolTip(typ, "Оберіть тип (обов'язково)");

            toolTip1.SetToolTip(categ, "Оберіть категорію (обов'язково)");

            toolTip1.SetToolTip(dat, "Оберіть дату операції (обов'язково)");

            toolTip1.SetToolTip(save, "Зберегти і додати нову транзакцію");

            
            toolTip1.InitialDelay = 500; //час до появи підказки (мс)
        }

        //оголошення менеджера
        private FinanceManager manager;

        public Form2(FinanceManager manager) : this()
        {
            this.manager = manager;

        }

        // заповнення полів
        private void Full()
        {
            
            typ.Items.Add("Дохід");
            typ.Items.Add("Витрата");

            

            categ.Items.Clear();

            
            categ.Enabled = false;

            
            typ.SelectedIndex = -1;
            dat.Value = DateTime.Today;


        }

        private void typ_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            categ.Items.Clear();

            if (typ.SelectedIndex >= 0)
            {
                categ.Enabled = true;
                string selectedType = typ.SelectedItem.ToString();

                if (selectedType == "Дохід")
                {
                    categ.Items.Add("Зарплата");
                    categ.Items.Add("Продаж");
                    categ.Items.Add("Допомога");

                }
                else if (selectedType == "Витрата")
                {
                    categ.Items.Add("Продукти");
                    categ.Items.Add("Побутова хімія");
                    categ.Items.Add("Техніка");
                    categ.Items.Add("Подарунок");
                    categ.Items.Add("Меблі");
                    categ.Items.Add("Збір");

                }

                categ.SelectedIndex = -1;
            }
            else
            {
                categ.Enabled = false;
            }
        }

        //кнопка збереження
        private void save_Click(object sender, EventArgs e)
        {
            decimal sumValue;

            // перевірка суми
            if (!validator.TryParseAmount(amount.Text, out sumValue))
            {
                MessageBox.Show("Введіть числову суму");
                return;
            }

            // перевірка типу
            if (typ.SelectedItem == null)
            {
                MessageBox.Show("Оберіть тип операції");
                return;
            }

            // перевірка категорії
            if (categ.SelectedItem == null)
            {
                MessageBox.Show("Оберіть категорію");
                return;
            }

            // РЕДАГУВАННЯ
            if (EditingTransaction != null)
            {
                

                

                EditingTransaction.Category = categ.SelectedItem.ToString();
                EditingTransaction.Amount = sumValue;
                EditingTransaction.Date = dat.Value;
                EditingTransaction.Description = description.Text.Trim();
            }

            // СТВОРЕННЯ
            else
            {
                Transaction transaction;

                if (typ.SelectedItem.ToString() == "Дохід")
                    transaction = new Income();
                else
                    transaction = new Expense();

                
                transaction.Category = categ.SelectedItem.ToString();
                transaction.Amount = sumValue;
                transaction.Date = dat.Value;
                transaction.Description = description.Text.Trim();

                CreatedTransaction = transaction;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void clear_Click(object sender, EventArgs e)
        {
            typ.SelectedIndex = -1;
            amount.Text = string.Empty;
            categ.SelectedIndex = -1;   
            description.Text = string.Empty;
            dat.Value = DateTime.Today;
            typ.Focus();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "ІНСТРУКЦІЯ ДЛЯ КОРИСТУВАННЯ ВІКНОМ:\n\n" +
                "• Введіть дані балансу, категорії, назви мети, потребуємої на неї суми та дату її досягнення\n" +
                "• Після чого натисніть 'Зберегти', тоді дані завантажаться на головне меню\n" +
                "• Для очищення полів 'Скасувати'.\n" +
                "• Треба обов'язково ввести дані в усі поля (Опис за бажанням)";

            MessageBox.Show(
                helpText,
                "Довідка по роботі з формою",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }


    }
}
