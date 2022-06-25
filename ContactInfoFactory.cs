using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneBook
{
    public class ContactDataFactory
    {
        public static ContactInfo CreateContactInfo(ContactInfoType type)
        {
            switch (type)
            {
                case ContactInfoType.LastName: return new LastNameContact();
                case ContactInfoType.FirstName: return new FirstNameContact();
                case ContactInfoType.MiddleName: return new MiddleNameContact();
                case ContactInfoType.PersonalPhoneNumber: return new PersonalPhoneNumberContact();
                case ContactInfoType.WorkPhoneNumber: return new WorkPhoneNumberContact();
                case ContactInfoType.Email: return new EmailContact();
                case ContactInfoType.DayOfBirth: return new DayOfBirthContact();
                case ContactInfoType.Note: return new NoteContact();
                default: return null;
            }
        }

        public static string GetNameOfTypeInfo(ContactInfoType type)
        {
            switch (type)
            {
                case ContactInfoType.LastName: return "Фамилия";
                case ContactInfoType.FirstName: return "Имя";
                case ContactInfoType.MiddleName: return "Отчество";
                case ContactInfoType.PersonalPhoneNumber: return "Личный номер";
                case ContactInfoType.WorkPhoneNumber: return "Рабочий номер";
                case ContactInfoType.Email: return "Электронная почта";
                case ContactInfoType.DayOfBirth: return "Дата рождения";
                case ContactInfoType.Note: return "Заметка";
                default: return null;
            }
        }
        public static List<ContactInfoType> GenerateTypes()
        {
            return new List<ContactInfoType>
            {
                ContactInfoType.LastName,
                ContactInfoType.FirstName,
                ContactInfoType.MiddleName,
                ContactInfoType.PersonalPhoneNumber,
                ContactInfoType.WorkPhoneNumber,
                ContactInfoType.Email,
                ContactInfoType.DayOfBirth,
                ContactInfoType.Note
            };
        }
        public static HashSet<ContactInfoType> GenerateTypesWithSeveralValues()
        {
            return new HashSet<ContactInfoType>
            {
                ContactInfoType.PersonalPhoneNumber,
                ContactInfoType.WorkPhoneNumber,
                ContactInfoType.Email
            };
        }
        public static HashSet<ContactInfoType> GenerateSearchNameTypes()
        {
            return new HashSet<ContactInfoType>
            {
                ContactInfoType.FirstName,
                ContactInfoType.LastName,
                ContactInfoType.MiddleName,
                ContactInfoType.PersonalPhoneNumber,
                ContactInfoType.WorkPhoneNumber
            };
        }

        public static List<ContactInfoType> GenerateFullNameTypes()
        {
            return new List<ContactInfoType>
            {
                ContactInfoType.LastName,
                ContactInfoType.FirstName,
                ContactInfoType.MiddleName
            };
        }
    }
}
