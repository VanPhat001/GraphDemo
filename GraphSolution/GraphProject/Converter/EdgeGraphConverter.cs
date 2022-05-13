using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.Converter
{
    public static class EdgeGraphConverter
    {
        public static List<EdgeInfo> ConvertEdgeGraph(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            MakeNodeIndex(nodeList);
            MakeEdgeIndex(edgeList);

            List<EdgeInfo> edges = new List<EdgeInfo>();
            foreach (var edge in edgeList)
            {
                edges.Add(new EdgeInfo(edge, edge.UNode, edge.VNode, edges.Count));

                if (!isDirectedGraph)
                {
                    edges.Add(new EdgeInfo(edge, edge.VNode, edge.UNode, edges.Count));
                }
            }
            
            return edges;
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
