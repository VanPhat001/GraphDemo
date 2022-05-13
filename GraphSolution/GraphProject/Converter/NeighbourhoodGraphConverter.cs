using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphProject.ObjectDisplay;
using GraphProject.Structures;

namespace GraphProject.Converter
{
    public static class NeighbourhoodGraphConverter
    {
        public static List<List<PairNodeEdge>> ConvertNeighbourGraph(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph, out int countEdge)
        {
            var graph = new List<List<PairNodeEdge>>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                graph.Add(new List<PairNodeEdge>());
            }

            MakeNodeIndex(nodeList);
            MakeEdgeIndex(edgeList);

            countEdge = 0;
            foreach (var edge in edgeList)
            {
                var uNode = edge.UNode;
                var vNode = edge.VNode;

                graph[(int)uNode.Tag].Add(new PairNodeEdge((int)vNode.Tag, (int)edge.Tag));
                if (!isDirectedGraph)
                {
                    graph[(int)vNode.Tag].Add(new PairNodeEdge((int)uNode.Tag, (int)edge.Tag));
                }
            }

            countEdge = edgeList.Count * (isDirectedGraph ? 1 : 2);

            return graph;
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
