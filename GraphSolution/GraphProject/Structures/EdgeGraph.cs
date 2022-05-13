using GraphProject.Converter;
using GraphProject.ObjectDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.Structures
{
    public class EdgeGraph
    {
        #region fields
        private List<Node> nodeList;
        private List<EdgeInfo> edgeList;
        #endregion

        #region properties
        public int CountNode => nodeList.Count;
        public int CountEdge => edgeList.Count;
        #endregion

        #region constructors / destructor
        public EdgeGraph(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            this.nodeList = nodeList;

            this.edgeList =  EdgeGraphConverter.ConvertEdgeGraph(nodeList, edgeList, isDirectedGraph);
        }
        #endregion

        #region methods
        public void SortWeight()
        {
            edgeList.Sort((a, b) => a.GetWeight().CompareTo(b.GetWeight()));
        }
        #endregion

        #region getters
        public int GetWeight(EdgeInfo edge)
        {
            return edge.GetWeight();
        }

        public Node GetNode(int index)
        {
            return this.nodeList[index];
        }

        /// <returns>Edge in edgeList (alway directed)</returns>
        public EdgeInfo GetEdgeInfo(int index)
        {
            return this.edgeList[index];
        }

        /// <returns>
        /// if found return EdgeInfo(edge, unode, vnode); else return null
        /// </returns>
        public EdgeInfo GetEdgeInfo(int uIndex, int vIndex)
        {
            var edge = this.edgeList.Find(e => ((int)e.UNode.Tag == uIndex && (int)e.VNode.Tag == vIndex));
            return edge;
        }
        #endregion
    }
}
