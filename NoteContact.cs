using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneBook
{
    public class NoteContact : ContactInfo
    {
        public NoteContact() : base(ContactInfoType.Note)
        {
        }
        
        public override bool ApplyValue(string value)
        {
            return true;
        }

        public override string GetExceptionMessage()
        {
            return "";
        }
    }
}
