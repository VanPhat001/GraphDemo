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
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.PlanningAlgorithm
{
    public class PlanningAlgorithmDemo
    {
        private List<PlanningAlgorithmMessage> messageList;
        private List<Node> nodeList;
        private List<Edge> edgeList;
        private List<string> oddValue;
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
            this.oddValue = new List<string>();

            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);

            alphaNode.TitleForeColor = Brushes.Black;
            betaNode.TitleForeColor = Brushes.Black;

            this.alphaNode.SetLocate(new Point(0, 0));
            this.betaNode.SetLocate(new Point(0, 0));

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

            this.nodeList.Add(alphaNode);
            this.nodeList.Add(betaNode);

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

        private async Task ShowNodeID()
        {
            for (int i = 0; i < this.nodeList.Count; i++)
            {
                this.nodeList[i].Title = "id = " + i;
                await Task.Delay(1);
            }
        }

        private void SetNewEdgeText(List<double> results)
        {
            oddValue.Clear();
            foreach (var edge in this.edgeList)
            {
                int uIndex = (int)edge.UNode.Tag;
                oddValue.Add(edge.Text);
                edge.Text = results[uIndex].ToString();
            }
        }

        private void SetBackEdgeText()
        {
            foreach (var edge in this.edgeList)
            {
                int uIndex = (int)edge.UNode.Tag;
                edge.Text = oddValue[uIndex];
            }
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
                node.SetDefaultStatus();
            }
        }

        private void PlanningAlgorithm(List<List<int>> Order)
        {
            int[] t = new int[neighbour.CountNode];
            int[] T = new int[neighbour.CountNode];

            // create orderNode
            List<int> orderNode = new List<int>();
            for (int i = 0; i < Order.Count; i++)
            {
                for (int j = 0; j < Order[i].Count; j++)
                {
                    orderNode.Add(Order[i][j]);
                }
            }

            // init data
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = 0;
                T[i] = oo;
            }

            #region build t[]
            // build t[]
            foreach (int uIndex in orderNode)
            {
                // [SelectNode]
                var neighbourUNode = neighbour.GetNeighbourOf(uIndex);
                this.messageList.Add(new PlanningAlgorithmMessage(
                    PlanningAlgorithmDigits.SelectNode,
                    nodeIndex: uIndex));


                foreach (int vIndex in neighbourUNode)
                {
                    // [SelectEdge]
                    var oddValue = t[vIndex];
                    var edge = neighbour.GetEdge(uIndex, vIndex);
                    t[vIndex] = Math.Max(t[uIndex] + neighbour.GetWeight(edge), t[vIndex]);
                    this.messageList.Add(new PlanningAlgorithmMessage(
                        PlanningAlgorithmDigits.SelectEdge,
                        edgeIndex: (int)edge.Tag));


                    if (oddValue != t[vIndex])
                    {
                        // [UpdateNode]
                        this.messageList.Add(new PlanningAlgorithmMessage(
                            PlanningAlgorithmDigits.UpdateNode,
                            nodeIndex: vIndex,
                            dataList: new List<int>() { t[vIndex], T[vIndex] }));
                    }

                    // [DefaultEdge]
                    this.messageList.Add(new PlanningAlgorithmMessage(
                        PlanningAlgorithmDigits.DefaultEdge,
                        edgeIndex: (int)edge.Tag));
                }
            }
            #endregion


            // [tComplete]
            this.messageList.Add(new PlanningAlgorithmMessage(PlanningAlgorithmDigits.tComplete));

            #region build T[]
            // build T[]
            // [UpdateNode]
            T[(int)betaNode.Tag] = t[(int)betaNode.Tag];
            this.messageList.Add(new PlanningAlgorithmMessage(
                PlanningAlgorithmDigits.UpdateNode,
                nodeIndex: (int)betaNode.Tag,
                dataList: new List<int>() { t[(int)betaNode.Tag], T[(int)betaNode.Tag] }));


            for (int i = orderNode.Count - 1; i >= 0; i--)
            {
                // [SelectNode]
                int vIndex = orderNode[i];
                this.messageList.Add(new PlanningAlgorithmMessage(
                    PlanningAlgorithmDigits.SelectNode,
                    nodeIndex: vIndex));


                for (int uIndex = 0; uIndex < neighbour.CountNode; uIndex++)
                {
                    if (neighbour.CheckAdjacent(uIndex, vIndex))
                    {
                        // [SelectEdge]
                        var edge = neighbour.GetEdge(uIndex, vIndex);
                        var oddValue = T[uIndex];
                        T[uIndex] = Math.Min(T[uIndex], t[vIndex] - neighbour.GetWeight(edge));
                        this.messageList.Add(new PlanningAlgorithmMessage(
                            PlanningAlgorithmDigits.SelectEdge,
                            edgeIndex: (int)edge.Tag));


                        if (oddValue != T[uIndex])
                        {
                            // [UpdateNode]
                            this.messageList.Add(new PlanningAlgorithmMessage(
                                PlanningAlgorithmDigits.UpdateNode,
                                nodeIndex: uIndex,
                                dataList: new List<int>() { t[uIndex], T[uIndex] }));
                        }

                        // [DefaultEdge]
                        this.messageList.Add(new PlanningAlgorithmMessage(
                            PlanningAlgorithmDigits.DefaultEdge,
                            edgeIndex: (int)edge.Tag));
                    }
                }
            }
            #endregion
        }

        public async Task Run(Size windowSize)
        {
            InitTopo(windowSize);
            await ShowNodeID();

            PlanningCostWindow planningWindow = new PlanningCostWindow(this.neighbour.CountNode);
            planningWindow.ShowDialog();
            var resultData = planningWindow.Tag as PlanningCostResult;
            if (resultData.Digit == PlanningCostDigits.Accept)            
            {
                SetNewEdgeText(resultData.Results);
                topo = new TopoSortDemo(this.nodeList, this.edgeList, isDirectedGraph: true);
                await topo.Run(windowSize);

                InitPlanning();
                PlanningAlgorithm(topo.Order);

                await ShowAnimation();
                await Task.Delay(1);

                MessageBox.Show("finish");
                SetBackEdgeText();
            }
            else
            {
                MessageBox.Show("can't not run this algorithm");
            }

            RemoveAlphaBetaNode();
        }

        private async Task ShowAnimation()
        {
            var nodeTitleFontSize = neighbour.GetNode(0).TitleFontSize;
            foreach (var message in this.messageList)
            {
                var node = message.NodeIndex == -1 ? null : neighbour.GetNode(message.NodeIndex);
                var edge = message.EdgeIndex == -1 ? null : neighbour.GetEdge(message.EdgeIndex);
                var uNode = edge == null ? null : edge.UNode;
                var vNode = edge == null ? null : edge.VNode;

                switch (message.MessageDigit)
                {
                    case PlanningAlgorithmDigits.SelectNode:
                        node.Background = Brushes.LightBlue;
                        break;


                    case PlanningAlgorithmDigits.SelectEdge:
                        uNode.BorderColor = Brushes.Blue;
                        vNode.BorderColor = Brushes.Blue;
                        edge.LineColor = Brushes.Blue;

                        uNode.BorderThickness = 2;
                        vNode.BorderThickness = 2;
                        edge.LineThickness = 2;
                        break;


                    case PlanningAlgorithmDigits.UpdateNode:
                        node.Title = $"id = {(int)node.Tag}\nt = {message.DataList[0]}\nT = {message.DataList[1]}";
                        node.TitleForeColor = Brushes.Blue;
                        node.TitleFontSize = nodeTitleFontSize + 2;

                        await Task.Delay(TimeSleep);

                        node.TitleForeColor = Brushes.Black;
                        node.TitleFontSize = nodeTitleFontSize;
                        break;


                    case PlanningAlgorithmDigits.DefaultEdge:
                        uNode.BorderColor = Brushes.Black;
                        vNode.BorderColor = Brushes.Black;
                        edge.LineColor = Brushes.Black;

                        uNode.BorderThickness = 1;
                        vNode.BorderThickness = 1;
                        edge.LineThickness = 1;
                        break;


                    case PlanningAlgorithmDigits.tComplete:
                        foreach (var inode in this.nodeList)
                        {
                            inode.SetDefaultStatus();
                        }
                        break;
                }

                await Task.Delay(TimeSleep);
            }
        }
    }
}