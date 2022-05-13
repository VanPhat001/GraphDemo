using GraphProject.ObjectDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.Structures
{
    public class EdgeInfo
    {
        private Edge edge;
        public Node UNode { get; }
        public Node VNode { get; }
        public object Tag { get; }

        public EdgeInfo(Edge edge, Node uNode, Node vNode, object tag = null)
        {
            this.edge = edge;
            this.UNode = uNode;
            this.VNode = vNode;
            this.Tag = tag;
        }

        public Edge GetEdge()
        {
            return this.edge;
        }

        public int GetWeight()
        {
            try
            {
                return Int32.Parse(edge.Text);
            }
            catch
            {
                return 0;
            }
        }
    }
}
