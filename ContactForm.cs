using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelephoneBook
{
    public partial class ContactForm : Form
    {
        private Contact contact { get; set; }
        private Contact tempContact { get; set; }
        private HashSet<Control> tempListControl { get; }
        private HashSet<Control> tempContactListControl { get; }
        private Control lastControl { get; set; }
        private Label topLabel { get; }
        private OperationsDB dataBase { get; }
        private MainForm mainForm { get; }

        public ContactForm(Contact contact, OperationsDB dataBase, TypeOfEditing editing, MainForm mainForm)
        {
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Size = new Size(400, 400);
            this.mainForm = mainForm;
            this.contact = contact;
            this.dataBase = dataBase;
            topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Карточка контакта";
            topLabel.Font = new Font(topLabel.Font, FontStyle.Bold);
            Controls.Add(topLabel);
            tempContactListControl = new HashSet<Control>();
            tempListControl = new HashSet<Control>();
            AutoScroll = true;
            switch (editing)
            {
                case TypeOfEditing.UpdateContact:
                    GenerateTempControls();
                    break;
                case TypeOfEditing.CreateContact:
                    tempContact = this.contact.CopyContact();
                    GenerateTempCreateControls();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(editing), editing, null);
            }
        }

        private void GenerateTempCreateControls()
        {
            lastControl = topLabel;
            RemoveTempControls();

            foreach (var type in ContactDataFactory.GenerateTypes())
            {
                foreach (var formInfo in tempContact.GetFormInfo(type))
                {
                    var labelType = new Label();
                    UpdateControlSettings(labelType, ContactDataFactory.GetNameOfTypeInfo(formInfo.InfoType), 130, 0, new Point(5, lastControl.Location.Y + lastControl.Height + 5), true);
                    
                    var textBoxValue = new TextBox();

                    if (tempContact.IsContactInfoCanBeDeleted(type))
                    {
                        UpdateControlSettings(textBoxValue, formInfo.ContactInfoValue, 150, 0, new Point(labelType.Location.X + labelType.Width + 15, labelType.Location.Y), false,
                            TextBoxValue_Leave, formInfo);
                       
                        var butContactInfoDelete = new Button();
                        UpdateControlSettings(butContactInfoDelete, "-", 20, textBoxValue.Height, new Point(textBoxValue.Location.X + textBoxValue.Width + 5,
                            labelType.Location.Y), false, ButContactInfoDelete_Click, textBoxValue);

                        var butContactInfoAdd = new Button();
                        UpdateControlSettings(butContactInfoAdd, "+", 20, textBoxValue.Height, new Point(butContactInfoDelete.Location.X + butContactInfoDelete.Width + 5,
                            labelType.Location.Y), false, ButContactInfoAdd_Click, type);
                    }

                    else if (tempContact.IsTypeWithSeveralValues(type))
                    {
                        UpdateControlSettings(textBoxValue, formInfo.ContactInfoValue, 175, 0, new Point(labelType.Location.X + labelType.Width + 15, labelType.Location.Y), false,
                            TextBoxValue_Leave, formInfo);

                        var butContactInfoAdd = new Button();
                        UpdateControlSettings(butContactInfoAdd, "+", 20, textBoxValue.Height, new Point(textBoxValue.Location.X + textBoxValue.Width + 5,
                            labelType.Location.Y), false, ButContactInfoAdd_Click, type);
                    }
                    else
                    {
                        UpdateControlSettings(textBoxValue, formInfo.ContactInfoValue, 200, 0, new Point(labelType.Location.X + labelType.Width + 15, labelType.Location.Y), false,
                            TextBoxValue_Leave, formInfo);
                    }
                    lastControl = textBoxValue;
                }
            }
            var butAdd = new Button();
            UpdateControlSettings(butAdd, "Добавить", 0, 0, new Point(100, lastControl.Location.Y + lastControl.Height + 25), false, ButAdd_Click);
            
            var butClose = new Button();
            UpdateControlSettings(butClose, "Отмена", 0, 0, new Point(butAdd.Location.X + butAdd.Width + 15, butAdd.Location.Y), false, ButClose_Click);
        }

        private void ButContactInfoAdd_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var type = (ContactInfoType)button.Tag;
            var contactInfo = ContactDataFactory.CreateContactInfo(type);
            tempContact.AddContactInfo(contactInfo); 
            GenerateTempCreateControls();
        }

        private void ButContactInfoDelete_Click(object sender, EventArgs e)
        {
            var button = (Button) sender;
            var textBox = (TextBox)button.Tag;
            var contactInfo = (ContactInfo)textBox.Tag;
            tempContactListControl.Remove(textBox);
            Controls.Remove(textBox);
            tempContact.RemoveContactInfo(contactInfo);
            GenerateTempCreateControls();
        }
        
        private void ButAdd_Click(object sender, EventArgs e)
        {
            contact = tempContact;
            tempContact = null;
            dataBase.AddContact(contact);
            mainForm.UpdateContactList();
            GenerateTempControls();
        }

        private void GenerateTempControls()
        {
            lastControl = topLabel;
            RemoveTempControls();
            foreach (var type in ContactDataFactory.GenerateTypes())
            {
                foreach (var formInfo in contact.GetFormInfo(type))
                {
                    var labelType = new Label();

                    UpdateControlSettings(labelType, ContactDataFactory.GetNameOfTypeInfo(formInfo.InfoType), 130, 0, new Point(5, lastControl.Location.Y + lastControl.Height + 15), true);

                    var labelValue = new Label();

                    UpdateControlSettings(labelValue, formInfo.ContactInfoValue, 200, 0, new Point(labelType.Location.X + labelType.Width + 15, labelType.Location.Y), false);

                    lastControl = labelValue;
                }
            }

            var butUpdate = new Button();
            UpdateControlSettings(butUpdate, "Редактировать", 0,0, new Point(100, lastControl.Location.Y + lastControl.Height + 25), false, ButUpdate_Click);

            var butClose = new Button();
            UpdateControlSettings(butClose, "Закрыть", 0, 0, new Point(butUpdate.Location.X + butUpdate.Width + 15, butUpdate.Location.Y), false, ButClose_Click);
        }

        private void ButUpdate_Click(object sender, EventArgs e)
        {
            tempContact = contact.CopyContact();
            GenerateTempUpdateControls();
        }

        private void ButClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GenerateTempUpdateControls()
        {
            lastControl = topLabel;
            RemoveTempControls();

            foreach (var type in ContactDataFactory.GenerateTypes())
            {
                foreach (var formInfo in tempContact.GetFormInfo(type))
                {
                    var labelType = new Label();
                    UpdateControlSettings(labelType, ContactDataFactory.GetNameOfTypeInfo(formInfo.InfoType), 130, 0, new Point(5, lastControl.Location.Y + lastControl.Height + 5), true);
                    
                    var textBoxValue = new TextBox();
                    
                    if (tempContact.IsContactInfoCanBeDeleted(type))
                    {
                        UpdateControlSettings(textBoxValue, formInfo.ContactInfoValue, 150, 0, new Point(labelType.Location.X + labelType.Width + 15, labelType.Location.Y), false,
                        TextBoxValue_Leave, formInfo);

                        var butContactInfoDelete = new Button();
                        UpdateControlSettings(butContactInfoDelete, "-", 20, textBoxValue.Height, new Point(textBoxValue.Location.X + textBoxValue.Width + 5,
                            labelType.Location.Y), false, ButContactInfoDelete_Click1, textBoxValue);

                        var butContactInfoAdd = new Button();
                        UpdateControlSettings(butContactInfoAdd, "+", 20, textBoxValue.Height, new Point(butContactInfoDelete.Location.X + butContactInfoDelete.Width + 5,
                            labelType.Location.Y), false, ButContactInfoAdd_Click1, type);
                    }

                    else if (tempContact.IsTypeWithSeveralValues(type))
                    {
                        UpdateControlSettings(textBoxValue, formInfo.ContactInfoValue, 175, 0, new Point(labelType.Location.X + labelType.Width + 15, labelType.Location.Y), false,
                            TextBoxValue_Leave, formInfo);

                        var butContactInfoAdd = new Button();

                        UpdateControlSettings(butContactInfoAdd, "+", 20, textBoxValue.Height, new Point(textBoxValue.Location.X + textBoxValue.Width + 5,
                            labelType.Location.Y), false, ButContactInfoAdd_Click1, type);
                    }
                    else
                    {
                        UpdateControlSettings(textBoxValue, formInfo.ContactInfoValue, 200, 0, new Point(labelType.Location.X + labelType.Width + 15, labelType.Location.Y), false,
                            TextBoxValue_Leave, formInfo);
                    }
                    lastControl = textBoxValue;
                }
            }
            var butSave = new Button();
            UpdateControlSettings(butSave, "Сохранить", 0, 0, new Point(50, lastControl.Location.Y + lastControl.Height + 25), false, ButSave_Click);
            
            var butCancel = new Button();
            UpdateControlSettings(butCancel, "Отмена", 0,0, new Point(butSave.Location.X + butSave.Width + 15, butSave.Location.Y), false, ButCancel_Click);

            var butDelete = new Button();
            UpdateControlSettings(butDelete, "Удалить", 0, 0,new Point(butCancel.Location.X + butCancel.Width + 15, butSave.Location.Y), false, ButDelete_Click);
        }

        private void ButSave_Click(object sender, EventArgs e)
        {
            contact = tempContact;
            tempContact = null;
            dataBase.UpdateContact(contact);
            mainForm.UpdateContactList();
            GenerateTempControls();
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            GenerateTempControls();
        }

        private void ButDelete_Click(object sender, EventArgs e)
        {
            dataBase.RemoveContact(contact);
            Close();
            mainForm.UpdateContactList();
        }

        private void ButContactInfoAdd_Click1(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var type = (ContactInfoType)button.Tag;
            var contactInfo = ContactDataFactory.CreateContactInfo(type);
            tempContact.AddContactInfo(contactInfo);
            GenerateTempUpdateControls();
        }

        private void ButContactInfoDelete_Click1(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var textBox = (TextBox)button.Tag;
            var contactInfo = (ContactInfo)textBox.Tag;
            tempContactListControl.Remove(textBox);
            Controls.Remove(textBox);
            tempContact.RemoveContactInfo(contactInfo);
            GenerateTempUpdateControls(); ;
        }

        private void TextBoxValue_Leave(object sender, EventArgs e)
        {
            var control = (TextBox)sender;
            var fieldInfo = (ContactInfo)control.Tag;
            var value = control.Text;
            
            if (fieldInfo.ContactInfoValue == control.Text) return;

            if (fieldInfo.ApplyValue(value))
                fieldInfo.AssignValue(value);
            else
            {
                control.Text = fieldInfo.ContactInfoValue;
                MessageBox.Show(fieldInfo.GetExceptionMessage(), "Внимание!");
            }
        }
        
        private void RemoveTempControls()
        {
            if (tempContactListControl.Count > 0) //ActiveControl = null;
            {
                foreach (var control in tempContactListControl)
                    Controls.Remove(control);
                tempContactListControl.Clear();
            }

            if (tempListControl.Count > 0)
            {
                foreach (var control in tempListControl)
                    Controls.Remove(control);
                tempListControl.Clear();
            }
        }

        private void UpdateControlSettings(Control control, string text, int width, int height, Point location, bool isNameLineLabel, EventHandler action = null,
            object obj = null)
        {
            var isControlTextBox = control is TextBox;
            var isControlButton = control is Button;
            var isControlLabel = control is Label;

            control.Text = text;
            control.Location = location;

            if (obj != null) control.Tag = obj;

            if (isControlLabel || isControlTextBox)
            {
                if (isNameLineLabel)
                    control.Width = width;
                else
                {
                    control.AutoSize = true;
                    control.Width = width;
                    control.MaximumSize = new Size(width, 0);
                }
            }
            else
            {
                if (text == "+" || text == "-")
                    control.Size = new Size(width, height);
                else
                    control.AutoSize = true;
            }

            if (isControlTextBox)
            {
                control.Leave += action;
                tempContactListControl.Add(control);
            }
            else
            {
                if (isControlButton)
                    control.Click += action;
                tempListControl.Add(control);
            }

            Controls.Add(control);
        }
    }
}
