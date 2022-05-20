using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.Converter
{
    public static class FlowGraphConverter
    {
        public static void Convert(List<Node> nodeList, List<Edge> edgeList, out double[,] flow, out double[,] capacity, out List<NodeDegree> nodeDegrees, out int sourceIndex, out int sinkIndex)
        {
            MakeNodeIndex(nodeList);
            MakeEdgeIndex(edgeList);

            // init
            flow = new double[nodeList.Count, nodeList.Count];
            capacity = new double[nodeList.Count, nodeList.Count];
            nodeDegrees = new List<NodeDegree>();

            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeDegrees.Add(new NodeDegree(0, 0));
            }

            // calc degree
            foreach (var edge in edgeList)
            {
                int uIndex = (int)edge.UNode.Tag;
                int vIndex = (int)edge.VNode.Tag;

                capacity[uIndex, vIndex] = Int32.Parse(edge.Text);
                nodeDegrees[uIndex].OutDegree++;
                nodeDegrees[vIndex].InDegree++;
            }


            // find source / sink
            int s = -1;
            int t = -1;
            for (int i = 0; i < nodeDegrees.Count; i++)
            {
                var nodeDegree = nodeDegrees[i];
                if (nodeDegree.InDegree == 0)
                {
                    s = i;
                }
                if (nodeDegree.OutDegree == 0)
                {
                    t = i;
                }
            }
            sourceIndex = s;
            sinkIndex = t;
        }


        public static void MakeNodeIndex(List<Node> nodeList)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].Tag = i;
            }
        }

        public static void MakeEdgeIndex(List<Edge> edgeList)
        {
            for (int i = 0; i < edgeList.Count; i++)
            {
                edgeList[i].Tag = i;
            }
        }
    }
}
