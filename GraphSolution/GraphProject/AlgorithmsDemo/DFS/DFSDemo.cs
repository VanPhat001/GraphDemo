using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.DFS
{
    public class DFSDemo
    {
        private NeighbourhoodGraph neighbour;
        private bool[] mark;
        private int[] parent;
        private List<DFSMessage> messageList;
        private bool isRunning;
        private readonly static int TimeSleep = MainWindow.TimeSleep;

        public bool IsRunning
        {
            get => isRunning;
        }

        public DFSDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);
            messageList = new List<DFSMessage>();
            mark = new bool[neighbour.CountNode];
            parent = new int[neighbour.CountNode];
            this.isRunning = false;
        }

        private void Init()
        {
            messageList.Clear();
            for (int i = 0; i < mark.Length; i++)
            {
                mark[i] = false;
                parent[i] = -1;
                var node = neighbour.GetNode(i);
                node.Title = "id = " + (int)node.Tag;
            }
        }

        private void DFS(int startNodeIndex, bool useStack)
        {
            // [Init]
            Stack<int> stack = new Stack<int>();
            stack.Clear();
            stack.Push(startNodeIndex);
            messageList.Add(new DFSMessage(
                DFSDigits.Init,
                new List<int>() { startNodeIndex }));


            while (stack.Count > 0)
            {
                // [Pick]
                int uNodeIndex = stack.Pop();
                messageList.Add(new DFSMessage(
                    DFSDigits.Pick,
                    new List<int>() { uNodeIndex }));


                if (mark[uNodeIndex])
                {
                    // [CantVisit]
                    messageList.Add(new DFSMessage(
                        DFSDigits.CantVisit,
                        new List<int>() { uNodeIndex }));
                    continue;
                }

                // [Visited]
                mark[uNodeIndex] = true;
                Debug.WriteLine("Visit " + uNodeIndex);
                messageList.Add(new DFSMessage(
                    DFSDigits.Visited,
                    new List<int>() { uNodeIndex }));


                var temp = neighbour.GetNeighbourOf(uNodeIndex);
                int first, last, increment;
                if (useStack)
                {
                    first = 0;
                    last = temp.Count - 1;
                    increment = 1;
                }
                else
                {
                    first = temp.Count - 1;
                    last = 0;
                    increment = -1;
                }

                // [Candidates]
                for (int i = first; 0 <= i && i < temp.Count; i += increment)
                {
                    int vNodeIndex = temp[i];
                    if (!mark[vNodeIndex])
                    {
                        parent[vNodeIndex] = uNodeIndex;
                        stack.Push(vNodeIndex);
                        messageList.Add(new DFSMessage(
                            DFSDigits.Candidates,
                            new List<int>() { vNodeIndex }));
                    }
                }
            }
        }


        public async Task Run(bool useStack)
        {
            isRunning = true;

            Init();

            DFS(0, useStack);

            messageList.Add(new DFSMessage(DFSDigits.SpanningTree, new List<int>() { 0 }));

            await ShowAnimation();

            isRunning = false;
        }

        private async Task ShowAnimation()
        {
            foreach (var message in messageList)
            {
                var node = neighbour.GetNode(message.DataList[0]);
                switch (message.MessageDigit)
                {
                    case DFSDigits.Init:
                        break;


                    case DFSDigits.Pick:
                        var oddBackColor = node.Background;
                        node.Background = Brushes.LightGreen;

                        await Task.Delay(TimeSleep);

                        node.Background = oddBackColor;
                        break;


                    case DFSDigits.CantVisit:
                        oddBackColor = node.Background;
                        node.Background = Brushes.LightCoral;

                        await Task.Delay(TimeSleep);

                        node.Background = oddBackColor;
                        break;


                    case DFSDigits.Visited:
                        node.Background = Brushes.LightBlue;
                        break;


                    case DFSDigits.Candidates:
                        node.Background = Brushes.DarkSlateGray;
                        break;


                    case DFSDigits.SpanningTree:
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

            Debug.WriteLine("spanning tree");
            await Task.Delay(1);
        }
    }

}
