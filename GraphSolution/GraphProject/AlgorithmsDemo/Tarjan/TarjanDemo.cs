using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.Tarjan
{
    public class TarjanDemo
    {
        private NeighbourhoodGraph neighbour;
        private bool[] onStack;
        private Stack<int> stack;
        private int[] id;
        private int[] minId;
        private int current;
        private Brush[] colorList;
        private List<TarjanMessage> messageList;
        private readonly static int TimeSleep = 450;

        public TarjanDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);
            messageList = new List<TarjanMessage>();
            onStack = new bool[neighbour.CountNode];
            id = new int[neighbour.CountNode];
            minId = new int[neighbour.CountNode];
            stack = new Stack<int>();
            colorList = new Brush[neighbour.CountNode];
            current = -1;

        }

        private void Init()
        {
            messageList.Clear();
            stack.Clear();
            current = 0;
            double step = (1.0 * byte.MaxValue) / neighbour.CountNode;
            Random rand = new Random();

            for (int i = 0; i < id.Length; i++)
            {
                onStack[i] = false;
                id[i] = -1;
                minId[i] = id.Length + 1;
                neighbour.GetNode(i).Title = "";

                byte startValue = (byte)(step * i);
                byte endValue = (byte)(step * (i + 1));
                colorList[i] = new SolidColorBrush(Color.FromRgb(
                    (byte)rand.Next(startValue, endValue), 
                    (byte)rand.Next(startValue, endValue), 
                    (byte)rand.Next(startValue, endValue)));
            }
        }

        private void Tarjan(int uIndex)
        {
            // [Numbered]
            id[uIndex] = minId[uIndex] = current++;
            stack.Push(uIndex);
            onStack[uIndex] = true;
            this.messageList.Add(new TarjanMessage(
                TarjanDigits.Numbered,
                nodeIndex: uIndex));


            foreach (var vIndex in neighbour.GetNeighbourOf(uIndex))
            {
                // [PickEdge]
                this.messageList.Add(new TarjanMessage(
                    TarjanDigits.PickEdge,
                    edgeIndex: (int)neighbour.GetEdge(uIndex, vIndex).Tag));


                if (id[vIndex] == -1)
                {
                    Tarjan(vIndex);

                    // [UpdateMinID]
                    minId[uIndex] = Math.Min(minId[uIndex], minId[vIndex]);
                    this.messageList.Add(new TarjanMessage(
                        TarjanDigits.UpdateMinId,
                        nodeIndex: vIndex));
                }
                else if (onStack[vIndex])
                {
                    // [UpdateMinID]
                    minId[uIndex] = Math.Min(minId[uIndex], id[vIndex]);
                    this.messageList.Add(new TarjanMessage(
                        TarjanDigits.UpdateMinId,
                        nodeIndex: vIndex));

                    // [SetDefaultEdge]
                    this.messageList.Add(new TarjanMessage(
                        TarjanDigits.SetDefaultEdge,
                        edgeIndex: (int)neighbour.GetEdge(uIndex, vIndex).Tag));
                }
                else
                {
                    // [SetDefaultEdge]
                    this.messageList.Add(new TarjanMessage(
                        TarjanDigits.SetDefaultEdge,
                        edgeIndex: (int)neighbour.GetEdge(uIndex, vIndex).Tag));
                }
            }

            if (id[uIndex] == minId[uIndex])
            {
                int x;
                do
                {
                    // [PopStack]
                    x = stack.Pop();
                    onStack[x] = false;
                    this.messageList.Add(new TarjanMessage(
                        TarjanDigits.PopStack,
                        nodeIndex: x,
                        dataList: new List<int>() { uIndex }));
                } while (x != uIndex);
            }
        }


        public async Task Run()
        {
            Init();

            Tarjan(0);

            //messageList.Add(new TarjanMessage(TarjanDigits.SpanningTree, new List<int>() { 0 }));

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
                    case TarjanDigits.Numbered:
                        node.Title = $"id = {id[(int)node.Tag]}\nminId = {minId[(int)node.Tag]}";
                        node.TitleForeColor = Brushes.Blue;
                        node.TitleFontSize = nodeTitleFontSize + 2;

                        await Task.Delay(TimeSleep);

                        node.TitleForeColor = Brushes.Black;
                        node.TitleFontSize = nodeTitleFontSize;
                        break;


                    case TarjanDigits.PickEdge:
                        uNode.BorderColor = Brushes.Blue;
                        vNode.BorderColor = Brushes.Blue;
                        edge.LineColor = Brushes.Blue;

                        uNode.BorderThickness = 2;
                        vNode.BorderThickness = 2;
                        edge.LineThickness = 2;
                        break;


                    case TarjanDigits.UpdateMinId:
                        node.Title = $"id = {id[(int)node.Tag]}\nminId = {minId[(int)node.Tag]}";
                        node.TitleForeColor = Brushes.Blue;
                        node.TitleFontSize = nodeTitleFontSize + 2;

                        await Task.Delay(TimeSleep);

                        node.TitleForeColor = Brushes.Black;
                        node.TitleFontSize = nodeTitleFontSize;
                        break;


                    case TarjanDigits.SetDefaultEdge:
                        edge.LineThickness = 1;
                        edge.LineColor = Brushes.Black;
                        //vNode.BorderThickness = 1;
                        //vNode.BorderColor = Brushes.Black;
                        break;


                    case TarjanDigits.PopStack:
                        node.Background = colorList[message.DataList[0]];
                        break;
                }

                await Task.Delay(TimeSleep);
            }
        }

        //private async Task ShowSpanningTree()
        //{
        //    for (int vIndex = 0; vIndex < neighbour.CountNode; vIndex++)
        //    {
        //        int uIndex = parent[vIndex];
        //        if (uIndex != -1)
        //        {
        //            neighbour.GetEdge(uIndex, vIndex).SetSelectStatus();
        //        }
        //    }

        //    Debug.WriteLine("spanning tree");
        //    await Task.Delay(1);
        //}
    }
}
