﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelephoneBook
{
    public class LastNameContact : ContactInfo
    {
        public LastNameContact() : base(ContactInfoType.LastName)
        {
        }
        
        public override bool ApplyValue(string value)
        {
            return value.Length <= 50;
        }

        public override string GetExceptionMessage()
        {
            return "Длина строки не более 50 символов";
        }
    }
}
