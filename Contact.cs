using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneBook
{
    public class Contact 
    {
        public int Id { get; set; } = -1;
        private readonly Dictionary<ContactInfoType, Dictionary<int, ContactInfo>> ContactInfoFields;
        private readonly HashSet<ContactInfoType> typesWithSeveralValues;
        private readonly HashSet<ContactInfoType> searchNameTypes;
        private int ContactInfoId { get; set; }

        public Contact()
        {
            ContactInfoFields = GenerateContact();
            typesWithSeveralValues = ContactDataFactory.GenerateTypesWithSeveralValues();
            searchNameTypes = ContactDataFactory.GenerateSearchNameTypes();
        }

        public Contact CopyContact()
        {
            var result = new Contact();
            result.Id = Id;
            foreach (var contactInfoField in ContactInfoFields)
            {
                foreach (var contactInfo in contactInfoField.Value)
                {
                    var newContactInfo = ContactDataFactory.CreateContactInfo(contactInfo.Value.InfoType);
                    newContactInfo.AssignValue(contactInfo.Value.ContactInfoValue);
                    result.AddContactInfo(newContactInfo);
                }
            }

            return result;
        }

        public bool IsContactInfoCanBeDeleted(ContactInfoType type)
        {
            return IsTypeWithSeveralValues(type) && ContactInfoFields[type].Count > 1;
        }

        public bool IsTypeWithSeveralValues(ContactInfoType type)
        {
            return typesWithSeveralValues.Contains(type);
        }

        public List<ContactInfo> GetFormInfo(ContactInfoType type)
        {
            var result = new List<ContactInfo>();

            foreach (var contactInfo in ContactInfoFields[type])
                result.Add(contactInfo.Value);

            return result;
        }
        
        public void AddContactInfo(ContactInfo info)
        {
            info.Id = ContactInfoId;
            ContactInfoFields[info.InfoType][info.Id] = info;
            ContactInfoId++;
        }

        public void RemoveContactInfo(ContactInfo info)
        {
            ContactInfoFields[info.InfoType].Remove(info.Id);
        }

        public Dictionary<ContactInfoType, string> ConvertDataForDB()
        {
            var result = new Dictionary<ContactInfoType, string>();

            foreach (var infoItem in ContactInfoFields)
            {
                var infoString = "";
                if(infoItem.Value.Count > 1) 
                    infoString = string.Join(';', infoItem.Value.Values.Select(info => info.ContactInfoValue));
                else 
                    infoString = infoItem.Value.Values.Select(info => info.ContactInfoValue).First(); 

                result[infoItem.Key] = infoString;
            }

            return result;
        }

        public void Fill(DbDataRecord r)
        {
            Id = (int)r["id"];

            foreach (var type in ContactDataFactory.GenerateTypes())
            {
                foreach (var info in ConvertDataForContact(type, (string)r[type.ToString()]))
                    AddContactInfo(info);
            }
            
        }

        public  List<ContactInfo> ConvertDataForContact(ContactInfoType type, string dbString)
        {
            var result = new List<ContactInfo>();

            if (typesWithSeveralValues.Contains(type))
            {
                var dataArray = dbString.Split(";");
                foreach (var dataItem in dataArray)
                    AssignDataToContactInfo(result, dataItem, type);
            }
            else
                AssignDataToContactInfo(result, dbString, type);

            return result;
        }

        public static string GetSearchName(Contact contact)
        {
            var sb = new StringBuilder();
            foreach (var infoItem in contact.ContactInfoFields)
            {
                if (contact.searchNameTypes.Contains(infoItem.Key))
                {
                    foreach (var contactInfo in infoItem.Value)
                        sb.Append(contactInfo.Value.ContactInfoValue.ToLower());
                }
            }

            return sb.ToString();
        }

        private static void AssignDataToContactInfo(List<ContactInfo> list, string value, ContactInfoType type)
        {
            var contactInfo = ContactDataFactory.CreateContactInfo(type);
            contactInfo.AssignValue(value);
            list.Add(contactInfo);
        }

        private static Dictionary<ContactInfoType, Dictionary<int, ContactInfo>> GenerateContact()
        {
            var result = new Dictionary<ContactInfoType, Dictionary<int, ContactInfo>>();
            foreach (var type in ContactDataFactory.GenerateTypes())
                result[type] = new Dictionary<int, ContactInfo>();

            return result;
        }
    }
}
