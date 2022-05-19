using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.Prim
{
    public class PrimDemo
    {

        private NeighbourhoodGraph neighbour;
        private List<PrimMessage> messageList;
        private bool[] mark;
        private int[] cost;
        private int[] parent;
        private readonly static int TimeSleep = 300;
        private readonly static int oo = 99999999;

        public PrimDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);
            messageList = new List<PrimMessage>();
            mark = new bool[neighbour.CountNode];
            cost = new int[neighbour.CountNode];
            parent = new int[neighbour.CountNode];
        }

        private void Init()
        {
            messageList.Clear();
            for (int i = 0; i < mark.Length; i++)
            {
                mark[i] = false;
                cost[i] = oo;
                parent[i] = -1;
                var node = neighbour.GetNode(i);
                node.Title = $"id = {i}\ncost = {oo}";
            }
        }

        private void Prim(int startIndex)
        {
            // [Init]
            cost[startIndex] = 0;
            this.messageList.Add(new PrimMessage(
                PrimDigits.Init,
                nodeIndex: startIndex));


            for (int k = 1; k < neighbour.CountNode; k++)
            {
                int uIndex = -1, minCost = oo;
                for (int i = 0; i < neighbour.CountNode; i++)
                {
                    // [SelectNode]
                    this.messageList.Add(new PrimMessage(
                        PrimDigits.SelectNode,
                        nodeIndex: i));

                    if (!mark[i] && minCost > cost[i])
                    {
                        uIndex = i;
                        minCost = cost[i];
                    }

                    // [CancelNode]
                    this.messageList.Add(new PrimMessage(
                        PrimDigits.CancelNode,
                        nodeIndex: i));
                }

                if (uIndex == -1)
                {
                    break;
                }

                // [Marked]
                mark[uIndex] = true;
                this.messageList.Add(new PrimMessage(
                    PrimDigits.Marked,
                    nodeIndex: uIndex));


                foreach (var vIndex in neighbour.GetNeighbourOf(uIndex))
                {
                    // [SelectEdge]
                    var edge = neighbour.GetEdge(uIndex, vIndex);
                    this.messageList.Add(new PrimMessage(
                        PrimDigits.SelectEdge,
                        edgeIndex: (int)edge.Tag));

                    if (!mark[vIndex] && neighbour.GetWeight(edge) < cost[vIndex])
                    {
                        // [UpdateCost]
                        cost[vIndex] = neighbour.GetWeight(edge);
                        parent[vIndex] = uIndex;
                        this.messageList.Add(new PrimMessage(
                            PrimDigits.UpdateCost,
                            nodeIndex: vIndex));
                    }

                    // [CancelEdge]
                    this.messageList.Add(new PrimMessage(
                        PrimDigits.CancelEdge,
                        edgeIndex: (int)edge.Tag));
                }
            }
        }


        public async Task Run()
        {
            Init();

            Prim(0);

            messageList.Add(new PrimMessage(PrimDigits.SpanningTree));

            await ShowAnimation();
        }

        private async Task ShowAnimation()
        {
            var nodeTitleFontSize = neighbour.GetNode(0).TitleFontSize;

            foreach (var message in messageList)
            {
                var node = message.NodeIndex == -1 ? null : neighbour.GetNode(message.NodeIndex);
                var edge = message.EdgeIndex == -1 ? null : neighbour.GetEdge(message.EdgeIndex);
                var uNode = edge == null ? null : edge.UNode;
                var vNode = edge == null ? null : edge.VNode;

                switch (message.MessageDigit)
                {
                    case PrimDigits.Init:
                        node.Title = $"id = {(int)node.Tag}\ncost = {0}";
                        node.TitleForeColor = Brushes.Blue;
                        node.TitleFontSize = nodeTitleFontSize + 2;

                        await Task.Delay(TimeSleep);

                        node.TitleForeColor = Brushes.Black;
                        node.TitleFontSize = nodeTitleFontSize;
                        break;


                    case PrimDigits.SelectNode:
                        node.BorderColor = Brushes.Blue;
                        node.BorderThickness = 2;
                        break;


                    case PrimDigits.CancelNode:
                        node.BorderColor = Brushes.Black;
                        node.BorderThickness = 1;
                        break;


                    case PrimDigits.Marked:
                        node.Background = Brushes.LightBlue;
                        break;


                    case PrimDigits.SelectEdge:
                        uNode.BorderColor = Brushes.Blue;
                        vNode.BorderColor = Brushes.Blue;
                        edge.LineColor = Brushes.Blue;

                        uNode.BorderThickness = 2;
                        vNode.BorderThickness = 2;
                        edge.LineThickness = 2;
                        break;


                    case PrimDigits.UpdateCost:
                        node.Title = $"id = {(int)node.Tag}\ncost = {cost[(int)node.Tag]}";
                        node.TitleFontSize = nodeTitleFontSize + 2;
                        node.TitleForeColor = Brushes.Blue;

                        await Task.Delay(TimeSleep);

                        node.TitleFontSize = nodeTitleFontSize;
                        node.TitleForeColor = Brushes.Black;
                        break;


                    case PrimDigits.CancelEdge:
                        uNode.BorderColor = Brushes.Black;
                        vNode.BorderColor = Brushes.Black;
                        edge.LineColor = Brushes.Black;

                        uNode.BorderThickness = 1;
                        vNode.BorderThickness = 1;
                        edge.LineThickness = 1;
                        break;


                    case PrimDigits.SpanningTree:
                        await ShowSpanningTree();
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
