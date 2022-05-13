using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo
{
    public class Message<TypeDigits, TypeList>
    {
        public TypeDigits MessageDigit { get; set; }
        public List<TypeList> DataList { get; set; }

        public Message(TypeDigits digit, List<TypeList> dataList = null)
        {
            this.MessageDigit = digit;
            this.DataList = dataList;
        }
    }
}
