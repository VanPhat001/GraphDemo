using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.Dijkstra
{
    public class DijkstraMessage : Message<DijkstraDigits, int>
    {
        public int NodeIndex
        {
            get; set;
        }
        public DijkstraMessage(DijkstraDigits digit, int nodeIndex = -1, List<int> dataList = null) : base(digit, dataList)
        {
            this.NodeIndex = nodeIndex;
        }
    }
}
