using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.Kruskal
{
    public class KruskalDemo
    {
        private EdgeGraph edgeGraph;
        private List<KruskalMessage> messageList;
        private int[] parent;
        private List<int> selectEdgesIndex;
        private bool isRunning;
        private readonly static int TimeSleep = 300;
        private readonly MyListViewControl myListViewControl;

        public bool IsRunning
        {
            get => isRunning;
        }

        public KruskalDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph, MyListViewControl myListViewControl)
        {
            edgeGraph = new EdgeGraph(nodeList, edgeList, isDirectedGraph);
            messageList = new List<KruskalMessage>();
            parent = new int[edgeGraph.CountNode];
            this.selectEdgesIndex = new List<int>();
            this.myListViewControl = myListViewControl;
            this.isRunning = false;
        }

        private void Init()
        {
            messageList.Clear();
            edgeGraph.SortWeight();
            selectEdgesIndex.Clear();

            for (int i = 0; i < parent.Length; i++)
            {
                parent[i] = -1;
            }


            myListViewControl.Clear();
            myListViewControl.SetVisible();

            for (int i = 0; i < edgeGraph.CountEdge; i++)
            {
                var edgeInfo = edgeGraph.GetEdgeInfo(i);
                myListViewControl.AddText($"{(int)edgeInfo.UNode.Tag} -> {(int)edgeInfo.VNode.Tag} : {edgeInfo.GetWeight()}");
            }
        }

        private int FindRoot(int uIndex)
        {
            if (parent[uIndex] == -1)
            {
                return uIndex;
            }
            return FindRoot(parent[uIndex]);
        }

        private void Kruskal()
        {
            for (int i = 0; i < edgeGraph.CountEdge; i++)
            {
                // [PickEdge]
                var uNode = edgeGraph.GetEdgeInfo(i).UNode;
                var vNode = edgeGraph.GetEdgeInfo(i).VNode;
                this.messageList.Add(new KruskalMessage(KruskalDigits.PickEdge, edgeIndex: i));

                int uRoot = FindRoot((int)uNode.Tag);
                int vRoot = FindRoot((int)vNode.Tag);

                if (uRoot != vRoot)
                {
                    // [Select]
                    parent[vRoot] = uRoot;
                    this.messageList.Add(new KruskalMessage(KruskalDigits.Select, edgeIndex: i));
                }
                else
                {
                    // [Skip]
                    this.messageList.Add(new KruskalMessage(KruskalDigits.Skip, edgeIndex: i));
                }

                // [SetDefaultEdge]
                this.messageList.Add(new KruskalMessage(KruskalDigits.Default, edgeIndex: i));
            }
        }


        public async Task Run()
        {
            isRunning = true;

            Init();

            Kruskal();

            this.messageList.Add(new KruskalMessage(KruskalDigits.SpanningTree, edgeIndex: 0));

            await ShowAnimation();

            isRunning = false;
        }

        private async Task ShowAnimation()
        {
            var nodeTitleFontSize = edgeGraph.GetNode(0).TitleFontSize;
            foreach (var message in messageList)
            {
                var edge = edgeGraph.GetEdgeInfo(message.EdgeIndex).GetEdge();
                var uNode = edgeGraph.GetEdgeInfo(message.EdgeIndex).UNode;
                var vNode = edgeGraph.GetEdgeInfo(message.EdgeIndex).VNode;

                switch (message.MessageDigit)
                {
                    case KruskalDigits.PickEdge:
                        uNode.BorderColor = Brushes.Blue;
                        vNode.BorderColor = Brushes.Blue;
                        edge.LineColor = Brushes.Blue;

                        uNode.BorderThickness = 2;
                        vNode.BorderThickness = 2;
                        edge.LineThickness = 2;
                        break;


                    case KruskalDigits.Select:
                        this.myListViewControl.ChangeBackgroundItem(message.EdgeIndex, Brushes.LightBlue);
                        selectEdgesIndex.Add(message.EdgeIndex);
                        break;


                    case KruskalDigits.Skip:
                        vNode.BorderColor = Brushes.Black;
                        edge.LineColor = Brushes.Black;

                        vNode.BorderThickness = 1;
                        edge.LineThickness = 1;

                        this.myListViewControl.ChangeBackgroundItem(message.EdgeIndex, Brushes.LightCoral);
                        break;


                    case KruskalDigits.Default:
                        uNode.BorderColor = Brushes.Black;
                        vNode.BorderColor = Brushes.Black;
                        edge.LineColor = Brushes.Black;

                        uNode.BorderThickness = 1;
                        vNode.BorderThickness = 1;
                        edge.LineThickness = 1;
                        break;


                    case KruskalDigits.SpanningTree:
                        await ShowSpanningTree();
                        break;
                }

                await Task.Delay(TimeSleep);
            }
        }

        private async Task ShowSpanningTree()
        {
            foreach (var edgeIndex in selectEdgesIndex)
            {
                var edge = edgeGraph.GetEdgeInfo(edgeIndex).GetEdge();
                edge.SetSelectStatus();
            }

            Debug.WriteLine("spanning tree");
            await Task.Delay(1);
        }
    }
}
