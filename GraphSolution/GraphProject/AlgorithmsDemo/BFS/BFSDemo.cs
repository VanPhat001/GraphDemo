using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GraphProject.ObjectDisplay;
using GraphProject.Structures;

namespace GraphProject.AlgorithmsDemo.BFS
{
    public class BFSDemo
    {
        private NeighbourhoodGraph neighbour;
        private bool[] mark;
        private int[] parent;
        private List<BFSMessage> messageList;
        private readonly static int TimeSleep = MainWindow.TimeSleep;


        public BFSDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);
            messageList = new List<BFSMessage>();
            mark = new bool[neighbour.CountNode];
            parent = new int[neighbour.CountNode];
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

        private void BFS(int startNodeIndex)
        {
            // [Init]
            Queue<int> queue = new Queue<int>();
            queue.Clear();
            queue.Enqueue(startNodeIndex);
            messageList.Add(new BFSMessage(
                BFSDigits.Init,
                new List<int>() { startNodeIndex }));


            while (queue.Count > 0)
            {
                // [Pick]
                int uNodeIndex = queue.Dequeue();
                messageList.Add(new BFSMessage(
                    BFSDigits.Pick,
                    new List<int>() { uNodeIndex }));


                if (mark[uNodeIndex])
                {
                    // [CantVisit]
                    messageList.Add(new BFSMessage(
                        BFSDigits.CantVisit,
                        new List<int>() { uNodeIndex }));
                    continue;
                }

                // [Visited]
                mark[uNodeIndex] = true;
                Debug.WriteLine("Visit " + uNodeIndex);
                messageList.Add(new BFSMessage(
                    BFSDigits.Visited,
                    new List<int>() { uNodeIndex }));


                // [Candidates]
                foreach (var vNodeInex in neighbour.GetNeighbourOf(uNodeIndex))
                {
                    if (!mark[vNodeInex])
                    {
                        if (parent[vNodeInex] == -1)
                        {
                            parent[vNodeInex] = uNodeIndex;
                        }

                        queue.Enqueue(vNodeInex);
                        messageList.Add(new BFSMessage(
                            BFSDigits.Candidates,
                            new List<int>() { vNodeInex }));
                    }
                }
            }
        }


        public async Task Run()
        {
            Init();

            BFS(0);

            messageList.Add(new BFSMessage(BFSDigits.SpanningTree, new List<int>() { 0 }));

            await ShowAnimation();
        }

        private async Task ShowAnimation()
        {
            foreach (var message in messageList)
            {
                var node = neighbour.GetNode(message.DataList[0]);

                switch (message.MessageDigit)
                {
                    case BFSDigits.Init:
                        break;


                    case BFSDigits.Pick:
                        var oddBackColor = node.Background;
                        node.Background = Brushes.LightGreen;

                        await Task.Delay(TimeSleep);

                        node.Background = oddBackColor;
                        break;


                    case BFSDigits.CantVisit:
                        oddBackColor = node.Background;
                        node.Background = Brushes.LightCoral;

                        await Task.Delay(TimeSleep);

                        node.Background = oddBackColor;
                        break;


                    case BFSDigits.Visited:
                        node.Background = Brushes.LightBlue;
                        break;


                    case BFSDigits.Candidates:
                        node.Background = Brushes.LightSlateGray;
                        break;


                    case BFSDigits.SpanningTree:
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
