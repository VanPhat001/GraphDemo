using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.DFS
{
    public class DFSMessage : Message<DFSDigits, int>
    {
        public DFSMessage(DFSDigits digit, List<int> dataList = null) : base(digit, dataList)
        {
        }
    }
}
