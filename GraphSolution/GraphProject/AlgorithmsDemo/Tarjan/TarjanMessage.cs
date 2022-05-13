using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.Tarjan
{
    public class TarjanMessage : Message<TarjanDigits, int>
    {
        public int NodeIndex { get; set; }
        public int EdgeIndex { get; set; }

        public TarjanMessage(TarjanDigits digit, int nodeIndex = -1, int edgeIndex = -1, Brush color = null, List<int> dataList = null) : base(digit, dataList)
        {
            this.NodeIndex = nodeIndex;
            this.EdgeIndex = edgeIndex;
        }
    }
}
