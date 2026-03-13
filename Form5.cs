using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;
using static МенОсобФін_КП_Барило.Form1;

namespace МенОсобФін_КП_Барило
{
    public partial class Form5 : Form
    {
        private Form1 _parentForm;
        public Form5(Form1 parentForm, decimal currentBalance, decimal currentEnd, decimal currentGoalAmount, string currentGoal, DateTime currentGoalDeadline)
        {
            InitializeComponent();

            _parentForm = parentForm;
            this._currentBalance = currentBalance;
            this.end = currentEnd;
            this.goal = currentGoal;
            this.goalAmount = currentGoalAmount;
            this._goalDeadline = currentGoalDeadline;
            

            End();
            Goal();
            Meta();
            UpdateDaysRemaining();
            UpdateProgressBar();
        }


        private decimal _currentBalance;
        private decimal end = 0;
        private string goal = "";
        private decimal goalAmount = 0;
        private DateTime _goalDeadline;
        private string categor = "";
        

        private void Balance()
        {
            
            _parentForm.GetBalance();
        }

        private void End()
        {
            lblEnd.Text = end.ToString("N2");
            lblEnd.Refresh();
        }

        private void Goal()
        {
            lblGoal.Text = goal;
            lblGoal.Refresh();
        }

        private void Meta()
        {
            metaAmount.Text = goalAmount.ToString("N2");
            metaAmount.Refresh();
        }

        private void Category()
        {
            cat.Text = categor;
            cat.Refresh();
        }

        private void UpdateProgressBar()
        {

            if (this.end <= 0)
            {
                
                guna2ProgressBar1.Maximum = 1;
                guna2ProgressBar1.Value = 0;
                return;
            }

            int max = (int)Math.Ceiling(this.end);
            guna2ProgressBar1.Maximum = max;

            int value = (int)Math.Round(Math.Min(this.goalAmount, this.end));

            
            guna2ProgressBar1.Value = Math.Min(value, max);
        }

        //скільки днів до досягнення мети залишається
        private void UpdateDaysRemaining()
        {

            DateTime today = DateTime.Now.Date;





            TimeSpan remainingTime = this._goalDeadline.Date - today;


            int days = (int)Math.Ceiling(remainingTime.TotalDays);


            if (days > 0)
            {
                lblDaysRemaining.Text = $"залишилося {days} днів";
            }
            else if (days == 0)
            {
                lblDaysRemaining.Text = "Сьогодні останній день!";
            }
            else
            {
                lblDaysRemaining.Text = "не встановлено";
            }
        }

        //статистика - в розробці
        private void btnStatistics_Click(object sender, EventArgs e)
        {
            if (this.end > 0)
            {
                using (Form3 form3 = new Form3())
                {

                    DialogResult res = form3.ShowDialog();
                    decimal currentBalance = _parentForm.GetBalance();
                    if (res == DialogResult.OK && form3.SavedAmount < _parentForm.GetBalance())
                    {
                        decimal amountToSave = form3.SavedAmount;
                        this.goalAmount += amountToSave;

                        _parentForm.UpdateBalance(currentBalance - amountToSave);
                        _parentForm.UpdateGoalAmount(this.goalAmount);

                        if (this.goalAmount >= this.end)
                        {
                            MessageBox.Show("Мета досягнута! Оновіть план");


                            this._currentBalance = _parentForm.GetBalance();


                            decimal remainder = this.goalAmount - this.end;


                            _parentForm.UpdateBalance(currentBalance + remainder);
                            this.goalAmount = 0;
                            this.end = 0;
                            this.goal = "";
                        }

                        //_parentForm.SaveDataToJson();


                        Balance();
                        Meta();
                        UpdateProgressBar();

                    }
                    else
                    {
                        UpdateProgressBar();
                        MessageBox.Show("Неможливо відкласти, немає грошей на балансі");
                        Meta();
                        Balance();
                        //_parentForm.SaveDataToJson();
                    }



                }
            }
            else
            {
                MessageBox.Show("Спочатку потрібно оновити план");
            }
        }

        //оновлення балансу
        private void newBalance_Click(object sender, EventArgs e)
        {
            using (Form4 form4 = new Form4())
            {
                DialogResult res = form4.ShowDialog();

                if (res == DialogResult.OK)
                {

                    decimal newBalance = form4.balance;
                    string newCategory = form4.category;
                    decimal newEnd = form4.end;

                    DateTime newData = form4.date;
                    string newGoal = form4.goal;

                    this.categor = newCategory;
                    this._currentBalance = newBalance;
                    this.end = newEnd;
                    this.goal = newGoal;
                    this._goalDeadline = newData;

                    this._parentForm.UpdateGoal(newGoal, newEnd, newData, newCategory);

                    Balance();
                    End();
                    Goal();
                    Category();
                    UpdateDaysRemaining();


                    UpdateProgressBar();

                    //_parentForm.SaveDataToJson();

                    MessageBox.Show($"План успішно оновлено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }


        }





        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "ІНСТРУКЦІЯ ДЛЯ КОРИСТУВАННЯ ВІКНОМ:\n\n" +
                "• Відстежуйте прогрес досягнення своєї мети\n" +
                "• Щоб змінити план - натисніть 'Оновити план'\n" +
                "• Щоб відкласти гроші з балансу на мету - натисніть 'Відкласти на мету'\n" +
                "• Для очищення полів 'Очистити все'.\n" +
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
