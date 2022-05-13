using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.Prim
{
    public class PrimMessage : Message<PrimDigits, int>
    {
        public int NodeIndex { get; set; }
        public int EdgeIndex { get; set; }
        public PrimMessage(PrimDigits digit, int nodeIndex = -1, int edgeIndex = -1, List<int> dataList = null) : base(digit, dataList)
        {
            this.NodeIndex = nodeIndex;
            this.EdgeIndex = edgeIndex;
        }
    }
}
