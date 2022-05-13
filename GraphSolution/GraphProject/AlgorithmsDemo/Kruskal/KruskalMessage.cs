using GraphProject.ObjectDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.Kruskal
{
    public class KruskalMessage : Message<KruskalDigits, int>
    {
        public int EdgeIndex { get; set; }
        public KruskalMessage(KruskalDigits digit, int edgeIndex = -1, List<int> dataList = null) : base(digit, dataList)
        {
            this.EdgeIndex = edgeIndex;
        }
    }
}
