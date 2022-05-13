using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.BellmanFord
{
    public class BellmanFordMessage : Message<BellmanFordDigits, int>
    {
        public int NodeIndex { get; set; }
        public int EdgeIndex { get; set; }
        public BellmanFordMessage(BellmanFordDigits digit, int nodeIndex = -1, int edgeIndex = -1, List<int> dataList = null) : base(digit, dataList)
        {
            this.NodeIndex = nodeIndex;
            this.EdgeIndex = edgeIndex;
        }
    }
}
