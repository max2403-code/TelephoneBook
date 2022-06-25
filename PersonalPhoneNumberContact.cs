using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelephoneBook
{
    class PersonalPhoneNumberContact : ContactInfo
    {
        public PersonalPhoneNumberContact() : base(ContactInfoType.PersonalPhoneNumber)
        {
        }
        
        public override bool ApplyValue(string value)
        {
            if (value == "") return true;
            var pattern = @"^\+?\d+$";
            return Regex.IsMatch(value, pattern);
        }

        public override string GetExceptionMessage()
        {
            return "Номер телефона содержит только цифровые символы и может начинаться с \"+\", например, +79120000000";
        }
    }
}
