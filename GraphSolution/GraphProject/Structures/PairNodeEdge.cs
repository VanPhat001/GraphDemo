using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.Structures
{
    public class PairNodeEdge
    {
        public int NodeIndex { get; set; }
        public int EdgeIndex { get; set; }

        public PairNodeEdge(int nodeIndex, int edgeIndex)
        {
            this.NodeIndex = nodeIndex;
            this.EdgeIndex = edgeIndex;
        }
    }
}
