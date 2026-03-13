using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace МенОсобФін_КП_Барило
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Full();
            InitializeToolTips();
        }

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

        public string type { get; private set; }
        public string sum { get; private set; }
        public string category { get; private set; }
        public DateTime date { get; private set; }
        public string desc { get; private set; }

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
            //присвоюємо суму операції публічній властивості amount і очищаємо цей текст від пробілів. А потім створюємо числовий тип для перевірки введених даних на число
            string sumText = amount.Text.Trim();
            decimal sumValue;

            //якщо не вводимо взагалі нічого - виводимо відповідне повідомлення
            if (string.IsNullOrWhiteSpace(amount.Text) || string.IsNullOrWhiteSpace(sumText))
            {
                MessageBox.Show(
                    "Будь ласка, введіть всі дані",
                    "Помилка введення",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                //і фокусуємося на інпуті суми операції
                amount.Focus(); 
                return;
            }

            //пробуємо перевести текстове значення суми в число, якщо не переводиться - виводимо відповідне повідомлення і фокусуємося на інпуті суми
            if (!decimal.TryParse(sumText, out sumValue))
            {
                MessageBox.Show("Введене значення Суми операції не є коректним числом.", "Помилка формату", MessageBoxButtons.OK, MessageBoxIcon.Error);
                amount.Focus();
                return;
            }

            if (typ.SelectedIndex == -1)
            {
                MessageBox.Show("Будь ласка, оберіть Тип операції (Дохід/Витрата).", "Необхідні дані", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                typ.Focus();
                return;
            }


            if (categ.SelectedIndex == -1)
            {
                MessageBox.Show("Будь ласка, оберіть Категорію операції.", "Необхідні дані", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                categ.Focus();
                return;
            }

            if (dat.Value.Date > DateTime.Today)
            {
                MessageBox.Show(
                    "Неможливо додати операцію на майбутню дату. Будь ласка, оберіть сьогоднішню або минулу дату.",
                    "Некоректна дата",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                dat.Focus();
                return;
            }

            //присвоюємо всі дані публічним властивостям
            type = typ.SelectedItem.ToString();    
            category = categ.SelectedItem.ToString();
            sum = amount.Text;
            date = dat.Value;
            desc = description.Text.Trim();

            //відправляємо на головне вікно результат діалогового вікна "ОК"
            this.DialogResult = DialogResult.OK;
            
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
