using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql.Replication.PgOutput.Messages;

namespace TelephoneBook
{
    public partial class MainForm : Form
    {
        private TextBox nameLine { get; }
        private Timer searchTimer { get; }
        private OperationsDB dataBase { get; set; }
        private Control lastControl { get; set; }
        private string lastNameText { get; set; }
        private List<Control> temporaryControl { get; }
        private Button signIn { get; }
        private Button addButton { get; }


        public MainForm()
        {
            //dataBase = new OperationsDB();
            temporaryControl = new List<Control>();
            AutoScroll = true;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Size = new Size(400, 400);
            nameLine = new TextBox();
            nameLine.Location = new Point(3, 3);
            nameLine.Size = new Size(215, 23);
            Controls.Add(nameLine);
            searchTimer = new Timer();
            searchTimer.Interval = 10;
            searchTimer.Tick += SearchTimer_Tick; 
            //searchTimer.Start();
            addButton = new Button();
            addButton.Height = nameLine.Height;
            addButton.AutoSize = true;
            addButton.Click += AddButtonOnClick;
            addButton.Location = new Point(nameLine.Location.X + nameLine.Width + 5, nameLine.Location.Y);
            addButton.Text = "Добавить";
            Controls.Add(addButton);
            addButton.Enabled = false;
            var closeButton = new Button();
            closeButton.Height = nameLine.Height;
            closeButton.AutoSize = true;
            closeButton.Click += CloseButton_Click;
            closeButton.Location = new Point(addButton.Location.X + addButton.Width + 5, nameLine.Location.Y);
            closeButton.Text = "Выход";
            Controls.Add(closeButton);

            signIn = new Button();
            signIn.Location = new Point(nameLine.Location.X, nameLine.Location.Y + nameLine.Height + 5);
            signIn.AutoSize = true;
            signIn.Click += SignIn_Click;
            signIn.Text = "Войти в базу данных";
            Controls.Add(signIn);


            
        }

        private void SignIn_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginDBForm(this);
            loginForm.ShowDialog();
        }

        public void AssignDB(string userName, string password)
        {
            dataBase = new OperationsDB(userName, password);
            addButton.Enabled = true;
            Controls.Remove(signIn);
            searchTimer.Start();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddButtonOnClick(object sender, EventArgs e)
        {
            var contact = new Contact();
            foreach (var type in ContactDataFactory.GenerateTypes())
            {
                var info = ContactDataFactory.CreateContactInfo(type);
                contact.AddContactInfo(info);
            }
            var contactForm = new ContactForm(contact, dataBase, TypeOfEditing.CreateContact, this);
            contactForm.ShowDialog();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            if (nameLine.Text != lastNameText)
            {
                UpdateContactList();
            }
        }

        public void UpdateContactList()
        {
            lastControl = nameLine;
            lastNameText = nameLine.Text;
            foreach (var control in temporaryControl)
                Controls.Remove(control);

            var listNames = dataBase.FindContacts(nameLine.Text.ToLower());
            foreach (var contact in listNames)
            {
                var label = new Label();
                label.Width = 200;
                label.Cursor = Cursors.Hand;
                label.AutoSize = true;
                label.Text = contact.Item1;
                label.Tag = contact.Item2;
                label.Location = new Point(5, lastControl.Location.Y + lastControl.Height + 5);
                temporaryControl.Add(label);
                if (contact.Item2 != -1)
                    label.Click += Label_Click;
                Controls.Add(label);
                lastControl = label;
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            var id = (int)((Label)sender).Tag;
            var contact = dataBase.GetCurrentContact(id);
            if (contact == null)
            {
                MessageBox.Show("Такого контакта не существует! Обновите список контактов");
                return;
            }
            var contactForm = new ContactForm(contact, dataBase, TypeOfEditing.UpdateContact, this);
            contactForm.ShowDialog();
        }
    }
}
