using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.BellmanFord
{
    public class BellmanFordDemo
    {
        private EdgeGraph edgeGraph;
        private int[] parent;
        private int[] cost;
        private List<BellmanFordMessage> messageList;
        private readonly static int TimeSleep = MainWindow.TimeSleep;
        private readonly static int oo = 99999999;
        private readonly MyListViewControl myListViewControl;

        public BellmanFordDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph, MyListViewControl myListViewControl)
        {
            edgeGraph = new EdgeGraph(nodeList, edgeList, isDirectedGraph);
            messageList = new List<BellmanFordMessage>();
            parent = new int[edgeGraph.CountNode];
            cost = new int[edgeGraph.CountNode];
            this.myListViewControl = myListViewControl;
        }

        private void Init()
        {
            messageList.Clear();

            for (int i = 0; i < parent.Length; i++)
            {
                parent[i] = -1;
                cost[i] = oo;
                var node = edgeGraph.GetNode(i);
                node.Title = "id = " + (int)node.Tag;
                node.Title = $"id = {(int)node.Tag}\ncost = {oo}";
            }


            myListViewControl.Clear();
            myListViewControl.SetVisible();

            for (int i = 0; i < edgeGraph.CountEdge; i++)
            {
                var edgeInfo = edgeGraph.GetEdgeInfo(i);
                myListViewControl.AddText($"{(int)edgeInfo.UNode.Tag} -> {(int)edgeInfo.VNode.Tag} : {edgeInfo.GetWeight()}");
            }
        }

        private void BellmanFord(int startNodeIndex)
        {
            // [Init]
            cost[startNodeIndex] = 0;
            this.messageList.Add(new BellmanFordMessage(
                BellmanFordDigits.Init,
                nodeIndex: startNodeIndex));


            for (int k = 1; k < edgeGraph.CountNode; k++)
            {
                bool canContinue = false;

                for (int i = 0; i < edgeGraph.CountEdge; i++)
                {
                    // [PickEdge]
                    var edgeInfo = edgeGraph.GetEdgeInfo(i);
                    int uIndex = (int)edgeInfo.UNode.Tag;
                    int vIndex = (int)edgeInfo.VNode.Tag;
                    var weight = edgeGraph.GetWeight(edgeInfo);
                    this.messageList.Add(new BellmanFordMessage(
                        BellmanFordDigits.PickEdge,
                        edgeIndex: (int)edgeInfo.Tag));

                    if (cost[uIndex] + weight < cost[vIndex])
                    {
                        // [Update]
                        cost[vIndex] = cost[uIndex] + weight;
                        parent[vIndex] = uIndex;
                        canContinue = true;
                        this.messageList.Add(new BellmanFordMessage(
                            BellmanFordDigits.Update,
                            edgeIndex: (int)edgeInfo.Tag));
                    }
                    else
                    {
                        // [CantUpdate]
                        this.messageList.Add(new BellmanFordMessage(
                            BellmanFordDigits.CantUpdate,
                            edgeIndex: (int)edgeInfo.Tag));
                    }

                    // [SetDefault]
                    this.messageList.Add(new BellmanFordMessage(
                        BellmanFordDigits.SetDefault,
                        edgeIndex: (int)edgeInfo.Tag));
                }

                if (!canContinue)
                {
                    break;
                }

                // [ResetTableColor]
                this.messageList.Add(new BellmanFordMessage(BellmanFordDigits.ResetTableColor));
            }
        }


        public async Task Run(Node startNode)
        {
            Init();

            BellmanFord((int)startNode.Tag);

            messageList.Add(new BellmanFordMessage(BellmanFordDigits.SpanningTree));

            await ShowAnimation();
        }

        private async Task ShowAnimation()
        {
            var nodeTitleFontSize = edgeGraph.GetNode(0).TitleFontSize;
            foreach (var message in messageList)
            {
                var node = (message.NodeIndex == -1 ? null : edgeGraph.GetNode(message.NodeIndex));
                var edge = (message.EdgeIndex == -1 ? null : edgeGraph.GetEdgeInfo(message.EdgeIndex).GetEdge());
                var uNode = (edge == null ? null : edgeGraph.GetEdgeInfo(message.EdgeIndex).UNode); // dont change edgeGraph.getEdge() = edge
                var vNode = (edge == null ? null : edgeGraph.GetEdgeInfo(message.EdgeIndex).VNode); // dont change edgeGraph.getEdge() = edge

                switch (message.MessageDigit)
                {
                    case BellmanFordDigits.Init:
                        node.Title = $"id = {(int)node.Tag}\ncost = 0";
                        node.BorderColor = Brushes.Blue;
                        node.BorderThickness = 2;
                        node.TitleFontSize = nodeTitleFontSize + 2;
                        node.TitleForeColor = Brushes.Blue;

                        await Task.Delay(TimeSleep);

                        node.BorderColor = Brushes.Black;
                        node.BorderThickness = 1;
                        node.TitleFontSize = nodeTitleFontSize;
                        node.TitleForeColor = Brushes.Black;
                        break;


                    case BellmanFordDigits.PickEdge:
                        uNode.BorderColor = Brushes.Blue;
                        vNode.BorderColor = Brushes.Blue;
                        edge.LineColor = Brushes.Blue;
                        uNode.BorderThickness = 2;
                        vNode.BorderThickness = 2;
                        edge.LineThickness = 2;
                        break;


                    case BellmanFordDigits.Update:
                        vNode.Title = $"id = {(int)vNode.Tag}\ncost = {cost[(int)vNode.Tag]}";
                        vNode.TitleFontSize = nodeTitleFontSize + 2;
                        vNode.TitleForeColor = Brushes.Blue;
                        this.myListViewControl.ChangeBackgroundItem(message.EdgeIndex, Brushes.LightBlue); // correct color
                        break;


                    case BellmanFordDigits.CantUpdate:
                        this.myListViewControl.ChangeBackgroundItem(message.EdgeIndex, Brushes.LightCoral); // incorrect color
                        break;


                    case BellmanFordDigits.SetDefault:
                        uNode.BorderColor = Brushes.Black;
                        vNode.BorderColor = Brushes.Black;
                        edge.LineColor = Brushes.Black;
                        uNode.BorderThickness = 1;
                        vNode.BorderThickness = 1;
                        edge.LineThickness = 1;
                        vNode.TitleFontSize = nodeTitleFontSize;
                        vNode.TitleForeColor = Brushes.Black;
                        break;


                    case BellmanFordDigits.ResetTableColor:
                        this.myListViewControl.ResetBackgroundItems();
                        break;


                    case BellmanFordDigits.SpanningTree:
                        await ShowSpanningTree();
                        break;
                }

                await Task.Delay(TimeSleep);
            }
        }

        private async Task ShowSpanningTree()
        {
            for (int vIndex = 0; vIndex < edgeGraph.CountNode; vIndex++)
            {
                int uIndex = parent[vIndex];
                if (uIndex != -1)
                {
                    edgeGraph.GetEdgeInfo(uIndex, vIndex).GetEdge().SetSelectStatus();
                }
            }

            Debug.WriteLine("spanning tree");
            await Task.Delay(1);
        }

    }
}
