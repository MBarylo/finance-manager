using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace МенОсобФін_КП_Барило
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            Full();
        }

        public decimal balance { get; private set; }
        public decimal end { get; private set; }
        public string category { get; private set; }
        public DateTime date { get; private set; }
        public string goal { get; private set; }

        private void Full()
        {


            guna2DateTimePicker1.Value = DateTime.Today;
            guna2ComboBox1.Items.Add("Робота");
            guna2ComboBox1.Items.Add("Дім");
            guna2ComboBox1.Items.Add("Особисте");



        }



      

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            
            string endText = guna2TextBox2.Text.Trim(); 
            string goalText = guna2TextBox3.Text.Trim(); 

            
            decimal endValue;


            
            if (
                string.IsNullOrWhiteSpace(guna2ComboBox1.Text) ||
                string.IsNullOrWhiteSpace(endText))
            {
                MessageBox.Show(
                    "Будь ласка, заповніть обов'язкові поля (Баланс, Категорія, Сума).",
                    "Помилка введення",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                guna2TextBox2.Focus();
                return;
            }

            

            
            if (!decimal.TryParse(endText, out endValue))
            {
                MessageBox.Show("Сума операції має бути коректним числом.", "Помилка формату", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2TextBox2.Focus();
                return;
            }

            if (guna2ComboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Будь ласка, оберіть Категорію операції.", "Необхідні дані", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2ComboBox1.Focus();
                return;
            }

            if (guna2DateTimePicker1.Value.Date < DateTime.Today)
            {
                MessageBox.Show(
                    "Неможливо спланувати мету на минувшу дату. Будь ласка, оберіть майбутнє або сьогоднішню дату.",
                    "Некоректна дата",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                guna2DateTimePicker1.Focus(); 
                return; 
            }



            category = guna2ComboBox1.Text.Trim();
            end = endValue;          
            date = guna2DateTimePicker1.Value;
            goal = goalText;        

            
            this.DialogResult = DialogResult.OK;

        } 

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "ІНСТРУКЦІЯ ДЛЯ КОРИСТУВАННЯ ВІКНОМ:\n\n" +
                "• Введіть дані балансу, категорії, назви мети, потребуємої на неї суми та дату її досягнення\n" +
                "• Після чого натисніть 'Зберегти', тоді дані завантажаться на вікно Плану\n" +
                "• Для очищення полів 'Скасувати'.\n" +
                "• Треба обов'язково ввести дані в усі поля";

            MessageBox.Show(
                helpText,
                "Довідка по роботі з формою",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}
