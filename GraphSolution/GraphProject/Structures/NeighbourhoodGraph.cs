using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphProject.ObjectDisplay;
using GraphProject.Converter;

namespace GraphProject.Structures
{
    public class NeighbourhoodGraph
    {
        #region fields
        private List<Node> nodeList;
        private List<Edge> edgeList;
        private List<List<PairNodeEdge>> neighbour;
        private readonly bool isDirectedGraph;
        private int countEdge;
        #endregion

        #region properties
        public int CountNode => nodeList.Count;
        public int CountEdge => countEdge;
        #endregion

        #region constructors / destructor
        public NeighbourhoodGraph(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            this.nodeList = nodeList;
            this.edgeList = edgeList;
            this.isDirectedGraph = isDirectedGraph;
            this.neighbour = NeighbourhoodGraphConverter.ConvertNeighbourGraph(nodeList, edgeList, isDirectedGraph, out countEdge);

            foreach (var item in neighbour)
            {
                item.Sort((a, b) => a.NodeIndex.CompareTo(b.NodeIndex));
            }
        }
        #endregion

        #region getters
        public List<int> GetNeighbourOf(int nodeIndex)
        {
            var temp = new List<int>();
            foreach (var item in neighbour[nodeIndex])
            {
                temp.Add(item.NodeIndex);
            }
            return temp;
        }

        public Node GetNode(int index)
        {
            return this.nodeList[index];
        }

        public Edge GetEdge(int index)
        {
            return this.edgeList[index];
        }

        public Edge GetEdge(int uIndex, int vIndex)
        {
            PairNodeEdge pair = neighbour[uIndex].Find(nodeEdge => nodeEdge.NodeIndex == vIndex);
            return this.edgeList[pair.EdgeIndex];
        }

        public int GetWeight(int uIndex, int vIndex)
        {
            try
            {
                return Int32.Parse(GetEdge(uIndex, vIndex).Text);
            }
            catch
            {
                return 0;
            }
        }

        public int GetWeight(Edge edge)
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
        #endregion
    }
}
