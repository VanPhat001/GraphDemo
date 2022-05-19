using GraphProject.AlgorithmsDemo.TopoSort;
using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GraphProject.AlgorithmsDemo.PlanningAlgorithm
{
    public class PlanningAlgorithmDemo
    {
        private List<PlanningAlgorithmMessage> messageList;
        private List<Node> nodeList;
        private List<Edge> edgeList;
        private NeighbourhoodGraph neighbour;
        private TopoSortDemo topo;
        private Node alphaNode;
        private Node betaNode;
        private readonly Canvas canvas;
        private readonly static int oo = 999999;
        private readonly static int TimeSleep = MainWindow.TimeSleep;

        public PlanningAlgorithmDemo(Canvas canvas, List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            this.canvas = canvas;
            this.nodeList = nodeList;
            this.edgeList = edgeList;
            this.alphaNode = new Node(canvas, "alpha");
            this.betaNode = new Node(canvas, "beta");
            this.messageList = new List<PlanningAlgorithmMessage>();

            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);


            this.alphaNode.SetLocate(new Point(0, 0));
            this.betaNode.SetLocate(new Point(0, 0));
            //this.alphaNode.Click += Node_Click_SelectNode;

            AddAlphaBetaNode();
        }

        private void AddAlphaBetaNode()
        {
            var nodeDegrees = neighbour.GetListDegree();

            for (int i = 0; i < nodeList.Count; i++)
            {
                var nodeDegree = nodeDegrees[i];

                if (nodeDegree.InDegree == 0)
                {
                    Edge aphaLink = new Edge(canvas, alphaNode, nodeList[i], "0");
                   
                    this.edgeList.Add(aphaLink);
                }
                if (nodeDegree.OutDegree == 0)
                {
                    Edge betaLink = new Edge(canvas, nodeList[i], betaNode, nodeList[i].Text);
                    this.edgeList.Add(betaLink);
                }
            }

            nodeList.Add(alphaNode);
            nodeList.Add(betaNode);

            // update status
            neighbour = new NeighbourhoodGraph(this.nodeList, this.edgeList, isDirectedGraph: true);
        }

        private void RemoveAlphaBetaNode()
        {
            for (int i = edgeList.Count - 1; i >= 0; i--)
            {
                var edge = edgeList[i];
                if (edge.UNode == alphaNode || edge.UNode == betaNode || edge.VNode == alphaNode || edge.VNode == betaNode)
                {
                    edge.Remove();
                    edgeList.RemoveAt(i);
                }
            }

            nodeList.Remove(alphaNode);
            nodeList.Remove(betaNode);
            alphaNode.Remove();
            betaNode.Remove();
        }

        private void InitTopo(Size windowSize)
        {
            alphaNode.SetLocate(new Point(alphaNode.GetNodeInnerSize().Width, windowSize.Height / 2));
            betaNode.SetLocate(new Point(windowSize.Width - betaNode.GetNodeInnerSize().Width, windowSize.Height / 2));
        }

        private void InitPlanning()
        {
            foreach (var node in nodeList)
            {
                node.Title = $"id = {(int)node.Tag}\nt = {0}\nT = {oo}";
            }
        }

        private void PlanningAlgorithm(List<List<int>> Order)
        {
            int[] t = new int[neighbour.CountNode];
            int[] T = new int[neighbour.CountNode];

            List<int> orderNode = new List<int>();
            for (int i = 0; i < Order.Count; i++)
            {
                for (int j = 0; j < Order[i].Count; j++)
                {
                    orderNode.Add(Order[i][j]);
                }
            }

            for (int i = 0; i < t.Length; i++)
            {
                t[i] = 0;
                T[i] = oo;
            }

            foreach (int uIndex in orderNode)
            {
                var neighbourUNode = neighbour.GetNeighbourOf(uIndex);
                foreach (int vIndex in neighbourUNode)
                {
                    t[vIndex] = Math.Max(t[uIndex] + neighbour.GetWeight(uIndex, vIndex), t[vIndex]);
                }
            }

            T[(int)betaNode.Tag] = t[(int)betaNode.Tag];
            for (int i = orderNode.Count - 1; i >= 0; i--)
            {
                int vIndex = orderNode[i];
                for (int uIndex = 0; uIndex < neighbour.CountNode; uIndex++)
                {
                    if (neighbour.CheckAdjacent(uIndex, vIndex))
                    {
                        T[uIndex] = Math.Min(T[uIndex], t[vIndex] - neighbour.GetWeight(uIndex, vIndex));
                    }
                }
            }
        }

        public async Task Run(Size windowSize)
        {
            InitTopo(windowSize);

            topo = new TopoSortDemo(this.nodeList, this.edgeList, isDirectedGraph: true);
            await topo.Run(windowSize);

            InitPlanning();

            PlanningAlgorithm(topo.Order);

            ShowAnimation();


            MessageBox.Show("finish");
            RemoveAlphaBetaNode();
        }

        private void ShowAnimation()
        {

        }
    }
}
