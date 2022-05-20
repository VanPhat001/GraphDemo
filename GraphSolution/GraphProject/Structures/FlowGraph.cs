using GraphProject.Converter;
using GraphProject.ObjectDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.Structures
{
    public class FlowGraph
    {
        #region fields
        private List<Node> nodeList;
        private List<Edge> edgeList;
        private double[,] flow;
        private double[,] capacity;
        private int sourceIndex; // s
        private int sinkIndex; // t
        private List<NodeDegree> nodeDegrees;
        #endregion

        #region properties
        public int CountNode => nodeList.Count;
        public int CountEdge => edgeList.Count;
        public int SourceIndex => sourceIndex;
        public int SinkIndex => sinkIndex;
        #endregion


        #region contructors / destructor
        public FlowGraph(List<Node> nodeList, List<Edge> edgeList)
        {
            this.nodeList = nodeList;
            this.edgeList = edgeList;

            FlowGraphConverter.Convert(nodeList, edgeList, out flow, out capacity, out nodeDegrees, out sourceIndex, out sinkIndex);
        }
        #endregion


        #region methods
        /// <summary>O(1)</summary>
        public bool CheckAdjacent(int uIndex, int vIndex)
        {
            return capacity[uIndex, vIndex] != 0;
        }
        #endregion


        #region getters / setters
        /// <summary>O(1)</summary>
        public double GetFLow(int uIndex, int vIndex)
        {
            return flow[uIndex, vIndex];
        }

        /// <summary>O(1)</summary>
        public double GetFLow(int edgeIndex)
        {
            var edge = GetEdge(edgeIndex);
            return flow[(int)edge.UNode.Tag, (int)edge.VNode.Tag];
        }

        /// <summary>O(1)</summary>
        public double GetCapacity(int uIndex, int vIndex)
        {
            return capacity[uIndex, vIndex];
        }

        /// <summary>O(1)</summary>
        public double GetCapacity(int edgeIndex)
        {
            var edge = GetEdge(edgeIndex);
            return capacity[(int)edge.UNode.Tag, (int)edge.VNode.Tag];
        }

        /// <summary>O(1)</summary>
        public void SetFlow(int uIndex, int vIndex, double flowValue)
        {
            flow[uIndex, vIndex] = flowValue;
        }

        /// <summary>O(1)</summary>
        public void SetCapacity(int uIndex, int vIndex, double capacityValue)
        {
            capacity[uIndex, vIndex] = capacityValue;
        }

        /// <summary>O(1)</summary>
        public Node GetNode(int nodeIndex)
        {
            return this.nodeList[nodeIndex];
        }

        /// <summary>O(1)</summary>
        public Edge GetEdge(int edgeIndex)
        {
            return this.edgeList[edgeIndex];
        }

        /// <summary>O(n)</summary>
        public Edge GetEdge(int uIndex, int vIndex)
        {
            Edge edge = edgeList.Find((e) => (int)e.UNode.Tag == uIndex && (int)e.VNode.Tag == vIndex);
            return edge;
        }
        #endregion
    }
}
