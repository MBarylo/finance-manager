using Guna.UI2.WinForms.Suite;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static МенОсобФін_КП_Барило.Form1;

namespace МенОсобФін_КП_Барило
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            

          
        }

        private decimal _savedAmount = 0;

        public decimal SavedAmount
        {
            get { return _savedAmount; }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string sumText = meta.Text.Trim();
            decimal sumValue;

            if (string.IsNullOrWhiteSpace(meta.Text))
            {
                MessageBox.Show(
                    "Будь ласка, введіть всі дані",
                    "Помилка введення",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                meta.Focus();
                return;
            }
            if (!decimal.TryParse(sumText, out sumValue))
            {
                MessageBox.Show("Введене значення Суми операції не є коректним числом.", "Помилка формату", MessageBoxButtons.OK, MessageBoxIcon.Error);
                meta.Focus();
                return;
            }

            _savedAmount = sumValue;




            this.DialogResult = DialogResult.OK;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            meta.Text = string.Empty;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "ІНСТРУКЦІЯ ДЛЯ КОРИСТУВАННЯ ВІКНОМ:\n\n" +
                "• Введіть суму, яку ви хочете відкласти на вашу мету\n" +
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
    }
    }

