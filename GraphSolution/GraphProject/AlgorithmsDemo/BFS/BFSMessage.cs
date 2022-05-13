using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using GraphProject.AlgorithmsDemo;

namespace GraphProject.AlgorithmsDemo.BFS
{
    public class BFSMessage : Message<BFSDigits, int>
    {
        public BFSMessage(BFSDigits digit, List<int> dataList = null) : base(digit, dataList)
        {
        }
    }
}
