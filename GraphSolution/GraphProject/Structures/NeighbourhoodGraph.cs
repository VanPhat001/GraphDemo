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


        #region methods
        /// <summary>O(1) ~ O(n)</summary>
        public bool CheckAdjacent(int uIndex, int vIndex)
        {
            var p = neighbour[uIndex].Find((pair) => pair.NodeIndex == vIndex);
            return p != null;
        }
        #endregion


        #region getters
        /// <summary>O(1) ~ O(n)</summary>
        public List<int> GetNeighbourOf(int nodeIndex)
        {
            var temp = new List<int>();
            foreach (var item in neighbour[nodeIndex])
            {
                temp.Add(item.NodeIndex);
            }
            return temp;
        }

        /// <summary>O(1)</summary>
        public Node GetNode(int index)
        {
            try
            {
                return this.nodeList[index];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>O(1)</summary>
        public Edge GetEdge(int index)
        {
            try
            {
                return this.edgeList[index];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>O(1) ~ O(n) </summary>
        public Edge GetEdge(int uIndex, int vIndex)
        {
            PairNodeEdge pair = neighbour[uIndex].Find(nodeEdge => nodeEdge.NodeIndex == vIndex);
            if (pair == null) return null;

            try
            {
                return this.edgeList[pair.EdgeIndex];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>O(1) ~ O(n)</summary>
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

        /// <summary>O(1)</summary>
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

        /// <summary>O(n)</summary>
        public int GetInDegree(int nodeIndex)
        {
            int count = 0;
            foreach (var edge in this.edgeList)
            {
                if ((int)edge.VNode.Tag == nodeIndex)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>O(1)</summary>
        public int GetOutDegree(int nodeIndex)
        {
            return this.neighbour[nodeIndex].Count;
        }

        /// <summary>O(n)</summary>
        public List<NodeDegree> GetListDegree()
        {
            List<NodeDegree> list = new List<NodeDegree>();

            for (int i = 0; i < CountNode; i++)
            {
                list.Add(new NodeDegree(0, 0));
            }

            foreach (var edge in edgeList)
            {
                int uIndex = (int)edge.UNode.Tag;
                int vIndex = (int)edge.VNode.Tag;

                list[uIndex].OutDegree++;
                list[vIndex].InDegree++;
            }

            return list;
        }
        #endregion
    }
}
