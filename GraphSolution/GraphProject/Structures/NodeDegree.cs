using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.Structures
{
    public class NodeDegree
    {
        private int inDegree;
        private int outDegree;

        public int InDegree { get => inDegree; set => inDegree = value; }
        public int OutDegree { get => outDegree; set => outDegree = value; }
        public int Degree => InDegree + OutDegree;


        public NodeDegree(int inDegree, int outDegree)
        {
            this.InDegree = inDegree;
            this.OutDegree = outDegree;
        }
    }
}
