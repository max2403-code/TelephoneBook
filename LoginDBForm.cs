using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace TelephoneBook
{
    public partial class LoginDBForm : Form
    {
        private MainForm mainForm { get; }
        private TextBox loginTBox { get; }
        private TextBox passwordTBox { get; }

        public LoginDBForm(MainForm mainForm)
        {
            this.mainForm = mainForm;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Size = new Size(400, 300);
            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Введите логин и пароль для входа в PostgreSQL";
            topLabel.Font = new Font(topLabel.Font, FontStyle.Bold);
            Controls.Add(topLabel);
            var loginLabel = new Label();
            loginLabel.Location = new Point(5, topLabel.Location.Y + topLabel.Height + 5);
            loginLabel.Width = 50;
            loginLabel.Text = "Логин";
            Controls.Add(loginLabel);
            loginTBox = new TextBox();
            loginTBox.Location = new Point(loginLabel.Location.X + loginLabel.Width + 10, loginLabel.Location.Y);
            loginTBox.Width = 290;
            loginTBox.Text = "postgres";
            Controls.Add(loginTBox);
            var passwordLabel = new Label();
            passwordLabel.Location = new Point(5, loginLabel.Location.Y + loginLabel.Height + 10);
            passwordLabel.Width = 50;
            passwordLabel.Text = "Пароль";
            Controls.Add(passwordLabel);
            passwordTBox = new TextBox();
            passwordTBox.Location = new Point(passwordLabel.Location.X + passwordLabel.Width + 10, passwordLabel.Location.Y);
            passwordTBox.Width = 290;
            passwordTBox.Text = "user";
            Controls.Add(passwordTBox);
            var okButton = new Button();
            okButton.Location = new Point(110, passwordTBox.Location.Y + passwordTBox.Height + 20);
            okButton.AutoSize = true;
            okButton.Text = "Ок";
            okButton.Click += OkButton_Click;
            Controls.Add(okButton);
            var closeButton = new Button();
            closeButton.Location = new Point(210, okButton.Location.Y);
            closeButton.AutoSize = true;
            closeButton.Text = "Отмена";
            closeButton.Click += CloseButtonOnClick;
            Controls.Add(closeButton);
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var login = loginTBox.Text;
            var password = passwordTBox.Text;
            var connectionPostgres = new NpgsqlConnection($"Host=localhost;Port=5432;Database=postgres;Username={login};Password={password}");
            var dbExistCmd = new NpgsqlCommand("select datname from pg_database where datname = 'telephonebook';",
                connectionPostgres);
            var dbCreateCmd = new NpgsqlCommand("CREATE DATABASE telephonebook;", connectionPostgres);
            var connectionTB = new NpgsqlConnection($"Host=localhost;Port=5432;Database=telephonebook;Username={login};Password={password}");

            var dbCheckTable = new NpgsqlCommand("create table if not exists tbcontacts (" +
                                                 "Id SERIAL PRIMARY KEY," +
                                                 "searchname text," +
                                                 "FirstName CHARACTER VARYING(50)," +
                                                 "LastName CHARACTER VARYING(50)," +
                                                 "MiddleName CHARACTER VARYING(50)," +
                                                 "PersonalPhoneNumber text," +
                                                 "WorkPhoneNumber text," +
                                                 "Email text," +
                                                 "DayOfBirth CHARACTER VARYING(10)," +
                                                 "Note text" +
                                                 ")", connectionTB);

            try
            {
                connectionPostgres.Open();
                var reader = dbExistCmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    connectionPostgres.Close();
                    connectionPostgres.Open();
                    dbCreateCmd.ExecuteNonQuery();
                }
                connectionPostgres.Close();

                connectionTB.Open();
                dbCheckTable.ExecuteNonQuery();
                connectionTB.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Неверный логин и(или) пароль", "Внимание!");
                return;
            }

            mainForm.AssignDB(login, password);
            Close();
        }
    }
}
