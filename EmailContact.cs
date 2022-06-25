using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelephoneBook
{
    public class EmailContact : ContactInfo
    {
        public EmailContact() : base(ContactInfoType.Email)
        {
        }

        public override bool ApplyValue(string value)
        {
            if (value == "") return true;
            var pattern = @"^[^;]*@[a-zA-Z]+\.[a-zA-Z]+$";
            return Regex.IsMatch(value, pattern);
        }

        public override string GetExceptionMessage()
        {
            return "Введите корректный Email, например, addressname@example.com\r\nEmail может содержать только латинские и цифровые символы";
        }
    }
}
