using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneBook
{
    public abstract class ContactInfo
    {
        public string ContactInfoValue { get; private set; } = "";
        public ContactInfoType InfoType { get; }
        public int Id { get; set; }

        protected ContactInfo(ContactInfoType type)
        {
            InfoType = type;
        }

        public void AssignValue(string value)
        {
            ContactInfoValue = value;
        }

        public abstract bool ApplyValue(string value);
        public abstract string GetExceptionMessage();
    }
}
