using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.FordFulkerson
{
    public class FordFulkersonMessage : Message<FordFulkersonDigits, object>
    {
        public int NodeIndex { get; set; }
        public int EdgeIndex { get; set; }
        public FordFulkersonMessage(FordFulkersonDigits digit, int nodeIndex = -1, int edgeIndex = -1, List<object> dataList = null) : base(digit, dataList)
        {
            this.NodeIndex = nodeIndex;
            this.EdgeIndex = edgeIndex;
        }
    }
}
