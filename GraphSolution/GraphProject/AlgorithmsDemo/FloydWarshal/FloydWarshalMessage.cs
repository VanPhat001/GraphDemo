using GraphProject.ObjectDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.FloydWarshal
{
    public class FloydWarshalMessage : Message<FloydWarshalDigits, int>
    {
        public int NodeIndex { get; set; }
        public Edge _Edge { get; set; }
        public FloydWarshalMessage(FloydWarshalDigits digit, int nodeIndex = -1, Edge edge = null, List<int> dataList = null) : base(digit, dataList)
        {
            this.NodeIndex = nodeIndex;
            this._Edge = edge;
        }
    }
}
