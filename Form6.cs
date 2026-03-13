using Guna.UI2.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace МенОсобФін_КП_Барило
{
    public partial class Form6 : Form
    {
        public decimal NewBalance { get; private set; } 

        public Form6(decimal currentBalance)
        {
            InitializeComponent();
           
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(balanceTextBox.Text, out decimal newBalanceValue))
            {
                this.NewBalance = newBalanceValue;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть коректне числове значення.");
            }
        }



        private void balanceTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            
     
        
        balanceTextBox.Text = string.Empty;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "ІНСТРУКЦІЯ ДЛЯ КОРИСТУВАННЯ ВІКНОМ:\n\n" +
                "• Введіть суму балансу\n" +
                "• Після чого натисніть 'Зберегти', тоді дані завантажаться на головне вікно\n" +
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
