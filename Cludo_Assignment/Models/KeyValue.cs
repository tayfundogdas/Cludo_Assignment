using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cludo_Assignment.Models
{
    public class KeyValue
    {
        public int Id;
        public string Value;

        public KeyValue(int pId,string pValue)
        {
            this.Id = pId;
            this.Value = pValue;
        }
    }
}