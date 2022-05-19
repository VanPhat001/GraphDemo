using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.Dijkstra
{
    public class DijkstraDemo
    {

        private NeighbourhoodGraph neighbour;
        private bool[] mark;
        private int[] parent;
        private int[] cost;
        private List<DijkstraMessage> messageList;
        private readonly static int TimeSleep = MainWindow.TimeSleep;
        public readonly static int oo = 99999999;

        public DijkstraDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);
            messageList = new List<DijkstraMessage>();
            mark = new bool[neighbour.CountNode];
            parent = new int[neighbour.CountNode];
            cost = new int[neighbour.CountNode];
        }

        private void Init()
        {
            messageList.Clear();
            for (int i = 0; i < mark.Length; i++)
            {
                mark[i] = false;
                parent[i] = -1;
                cost[i] = oo;
                var node = neighbour.GetNode(i);
                //node.Title = "id = " + (int)node.Tag;
                node.Title = $"id = {(int)node.Tag}\ncost = {oo}";
            }
        }

        private void Dijkstra(int startNodeIndex)
        {
            // [Init]
            cost[startNodeIndex] = 0;
            this.messageList.Add(new DijkstraMessage(
                DijkstraDigits.Init,
                nodeIndex: startNodeIndex));


            for (int k = 1; k < neighbour.CountNode; k++)
            {
                int uIndex = -1, minCost = oo;
                for (int i = 0; i < neighbour.CountNode; i++)
                {
                    // [Move]
                    this.messageList.Add(new DijkstraMessage(
                        DijkstraDigits.Move,
                        nodeIndex: i));

                    if (!mark[i] && cost[i] < minCost)
                    {
                        // [UpdateUIndex]
                        uIndex = i;
                        minCost = cost[i];
                        this.messageList.Add(new DijkstraMessage(
                            DijkstraDigits.UpdateUIndex,
                            nodeIndex: i));
                    }

                    // [MoveComplete]
                    this.messageList.Add(new DijkstraMessage(
                        DijkstraDigits.MoveComplete,
                        nodeIndex: i));
                }

                if (uIndex == -1)
                {
                    break;
                }

                // [Visited]
                mark[uIndex] = true;
                this.messageList.Add(new DijkstraMessage(
                    DijkstraDigits.Visited,
                    nodeIndex: uIndex));


                foreach (var vIndex in neighbour.GetNeighbourOf(uIndex))
                {
                    // [PickEdge]
                    var edge = neighbour.GetEdge(uIndex, vIndex);
                    int weightEdge = neighbour.GetWeight(edge);
                    this.messageList.Add(new DijkstraMessage(
                        DijkstraDigits.PickEdge,
                        dataList: new List<int>() { (int)edge.Tag }));


                    if (cost[uIndex] + weightEdge < cost[vIndex])
                    {
                        // [UpdateNeighbour]
                        cost[vIndex] = cost[uIndex] + weightEdge;
                        parent[vIndex] = uIndex;
                        this.messageList.Add(new DijkstraMessage(
                            DijkstraDigits.UpdateNeighbour,
                            nodeIndex: vIndex));
                    }

                    // [PickEdgeComplete]
                    this.messageList.Add(new DijkstraMessage(
                        DijkstraDigits.PickEdgeComplete,
                        dataList: new List<int>() { (int)edge.Tag }));
                }
            }
        }


        public async Task Run(Node startNode)
        {
            Init();

            Dijkstra((int)startNode.Tag);

            messageList.Add(new DijkstraMessage(DijkstraDigits.SpanningTree, dataList: new List<int>() { 0 }));

            await ShowAnimation();
        }

        private async Task ShowAnimation()
        {
            double nodeTitleFontSize = neighbour.GetNode(0).TitleFontSize;

            foreach (var message in messageList)
            {
                Node uNode = null, vNode = null, node = null;
                Edge edge = null;

                switch (message.MessageDigit)
                {
                    case DijkstraDigits.Init:
                        node = neighbour.GetNode(message.NodeIndex);
                        node.Title = $"id = {(int)node.Tag}\ncost = 0";
                        break;


                    case DijkstraDigits.Move:
                        node = neighbour.GetNode(message.NodeIndex);
                        node.BorderColor = Brushes.Blue;
                        node.BorderThickness = 2;
                        break;


                    case DijkstraDigits.UpdateUIndex:
                        node = neighbour.GetNode(message.NodeIndex);
                        node.Title = $"id = {(int)node.Tag}\ncost = {cost[(int)node.Tag]}";
                        node.TitleForeColor = Brushes.Blue;
                        node.TitleFontSize = nodeTitleFontSize + 2;
                        break;


                    case DijkstraDigits.MoveComplete:
                        node = neighbour.GetNode(message.NodeIndex);
                        node.BorderColor = Brushes.Black;
                        node.BorderThickness = 1;
                        node.TitleForeColor = Brushes.Black;
                        node.TitleFontSize = nodeTitleFontSize;
                        break;


                    case DijkstraDigits.Visited:
                        node = neighbour.GetNode(message.NodeIndex);
                        node.Background = Brushes.LightBlue;
                        break;


                    case DijkstraDigits.PickEdge:
                        edge = neighbour.GetEdge(message.DataList[0]);
                        uNode = edge.UNode;
                        vNode = edge.VNode;

                        uNode.BorderColor = Brushes.Blue;
                        vNode.BorderColor = Brushes.Blue;
                        edge.LineColor = Brushes.Blue;
                        uNode.BorderThickness = 2;
                        vNode.BorderThickness = 2;
                        edge.LineThickness = 2;
                        break;


                    case DijkstraDigits.UpdateNeighbour:
                        node = neighbour.GetNode(message.NodeIndex);
                        node.Title = $"id = {(int)node.Tag}\ncost = {cost[(int)node.Tag]}";
                        node.TitleForeColor = Brushes.Blue;
                        node.TitleFontSize = nodeTitleFontSize + 2;
                        break;


                    case DijkstraDigits.PickEdgeComplete:
                        edge = neighbour.GetEdge(message.DataList[0]);
                        uNode = edge.UNode;
                        vNode = edge.VNode;

                        uNode.BorderColor = Brushes.Black;
                        vNode.BorderColor = Brushes.Black;
                        edge.LineColor = Brushes.Black;
                        uNode.BorderThickness = 1;
                        vNode.BorderThickness = 1;
                        edge.LineThickness = 1;

                        vNode.TitleForeColor = Brushes.Black;
                        vNode.TitleFontSize = nodeTitleFontSize;
                        break;


                    case DijkstraDigits.SpanningTree:
                        await ShowSpanningTree();
                        break;


                    default:
                        break;
                }
                await Task.Delay(TimeSleep);
            }
        }

        private async Task ShowSpanningTree()
        {
            for (int vIndex = 0; vIndex < neighbour.CountNode; vIndex++)
            {
                int uIndex = parent[vIndex];
                if (uIndex != -1)
                {
                    neighbour.GetEdge(uIndex, vIndex).SetSelectStatus();
                }
            }

            System.Diagnostics.Debug.WriteLine("spanning tree");
            await Task.Delay(1);
        }

    }
}
