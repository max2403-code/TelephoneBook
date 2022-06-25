using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelephoneBook
{
    public class DayOfBirthContact : ContactInfo
    {
        public DayOfBirthContact() : base(ContactInfoType.DayOfBirth)
        {
        }

        public override bool ApplyValue(string value)
        {
            if (value == "") return true;
            if (value.Length > 10) return false;
            var pattern = @"\d{2}[.]\d{2}[.]\d{4}";
            if (!Regex.IsMatch(value, pattern))
                return false;
            var list = value.Split(".");
            try
            {
                var date = new DateTime(int.Parse(list[2]), int.Parse(list[1]), int.Parse(list[0]));
                var minDate = new DateTime(1900, 01, 01);
                var maxDate = DateTime.Today;
                if (date < minDate || date > maxDate) return false;
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public override string GetExceptionMessage()
        {
            return "Введите корректную дату, например, 26.12.1991\r\nДата должна быть не ранее 01.01.1900";
        }
    }
}
