using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace TelephoneBook
{
    public class OperationsDB
    {
        private readonly NpgsqlConnection connection;

        public OperationsDB(string userName, string password)
        {
            connection = new NpgsqlConnection($"Host=localhost;Port=5432;Database=telephonebook;Username={userName};Password={password}");
        }

        public List<(string, int)> FindContacts(string searchname)
        {
            var result = new List<(string, int)>();
            
            connection.Open();
            var command = new NpgsqlCommand($"SELECT firstname, lastname, middlename, id FROM tbcontacts WHERE searchname LIKE '%{searchname}%';", connection);
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                foreach (DbDataRecord readerItem in reader)
                    result.Add((GetFullName(readerItem), (int)readerItem["id"]));
            }
            else
                result.Add(("Такого контакта не существует", -1));

            connection.Close();

            return result.OrderBy(p=> p.Item1).ToList();
        }

        private string GetFullName(DbDataRecord readerItem)
        {
            var sbName = new StringBuilder();
            var fullNameTypes = ContactDataFactory.GenerateFullNameTypes();

            foreach (var type in fullNameTypes)
            {
                var info = (string) readerItem[type.ToString()];
                if (!sbName.Equals(""))
                    sbName.Append(' ');

                sbName.Append(info);
            }

            var result = sbName.ToString();

            return result == "" ? " " : result;
        }

        public Contact GetCurrentContact(int id)
        {
            var resultContact = new Contact();
            connection.Open();
            var command = new NpgsqlCommand($"SELECT * FROM tbcontacts WHERE id = {id};", connection);
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                foreach (DbDataRecord readerItem in reader)
                    resultContact.Fill(readerItem);
            }
            else
                resultContact = null;
            connection.Close();

            return resultContact;
        }

        public void AddContact(Contact contact)
        {
            var searchLine = Contact.GetSearchName(contact);
            var sbCommandInsert1 = new StringBuilder();
            var sbCommandInsert2 = new StringBuilder();
            var sbCommandFind = new StringBuilder();
            var dbContact = contact.ConvertDataForDB();
            var indexer = 0;

            sbCommandInsert1.Append("INSERT INTO tbcontacts (");
            sbCommandInsert2.Append("VALUES (");
            sbCommandFind.Append("SELECT id FROM tbcontacts WHERE ");
            
            foreach (var contactInfo in dbContact)
            {
                sbCommandInsert1.Append($"{contactInfo.Key}, ");
                sbCommandInsert2.Append($"'{contactInfo.Value}', ");
                sbCommandFind.Append($"{contactInfo.Key} = '{contactInfo.Value}'");
                if (indexer == dbContact.Count - 1)
                {
                    sbCommandInsert1.Append("searchname) ");
                    sbCommandInsert2.Append($"'{searchLine}')");
                    sbCommandFind.Append(";");
                }
                else
                {
                    sbCommandFind.Append(" and ");
                    indexer++;
                }
            }
            
            connection.Open();
            var command = new NpgsqlCommand(string.Concat(sbCommandInsert1.ToString(), sbCommandInsert2.ToString()), connection);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand(sbCommandFind.ToString(), connection);
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                foreach (DbDataRecord readerItem in reader)
                {
                    contact.Id = (int)readerItem["id"];
                    break;
                }
            }

            connection.Close();
        }

        public void RemoveContact(Contact contact)
        {
            connection.Open();
            var command = new NpgsqlCommand($"DELETE FROM tbcontacts where id = {contact.Id};", connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public void UpdateContact(Contact contact)
        {
            var searchLine = Contact.GetSearchName(contact);
            var sbCommand = new StringBuilder();
            var dbContact = contact.ConvertDataForDB();

            sbCommand.Append("UPDATE tbcontacts SET ");

            foreach (var contactInfo in dbContact)
                sbCommand.Append($"{contactInfo.Key} = '{contactInfo.Value}', ");

            sbCommand.Append($"searchname = '{searchLine}' WHERE id = {contact.Id};");

            connection.Open();
            var command = new NpgsqlCommand(sbCommand.ToString(), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
