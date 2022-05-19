using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.TopoSort
{
    public class TopoSortDemo
    {
        private NeighbourhoodGraph neighbour;
        private List<TopoSortMessage> messageList;
        private List<List<int>> rank;
        private readonly static int TimeSleep = MainWindow.TimeSleep;
        private readonly static int oo = 9999999;

        public List<List<int>> Order
        {
            get => rank;
        }

        public TopoSortDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);
            messageList = new List<TopoSortMessage>();
            this.rank = new List<List<int>>();
        }

        //public TopoSortDemo(NeighbourhoodGraph neighbour)
        //{
        //    this.neighbour = neighbour;
        //    messageList = new List<TopoSortMessage>();
        //    this.rank = new List<List<int>>();
        //}

        private void Init()
        {
            messageList.Clear();
        }

        private void TopoSort()
        {
            var nodeDegrees = neighbour.GetListDegree();

            var roots = new List<int>();
            for (int i = 0; i < neighbour.CountNode; i++)
            {
                // [CalcDegree]
                var nodeDegree = nodeDegrees[i];
                this.messageList.Add(new TopoSortMessage(
                    TopoSortDigits.CalcDegree,
                    nodeIndex: i,
                    dataList: new List<int>() { nodeDegree.InDegree, oo }));


                if (nodeDegree.InDegree == 0)
                {
                    // [AddNodeIntoRoot]
                    roots.Add(i);
                    this.messageList.Add(new TopoSortMessage(
                        TopoSortDigits.AddNodeIntoRoot,
                        nodeIndex: i));
                }
            }

            int currentRank = 0;
            var newRoots = new List<int>();
            while (roots.Count > 0)
            {
                int countNodeSameRank = 0;
                rank.Add(new List<int>());

                foreach (var rootIndex in roots)
                {
                    // [PickRoot]
                    rank[currentRank].Add(rootIndex);
                    countNodeSameRank++;
                    //System.Diagnostics.Debug.WriteLine($"rank[{neighbour.GetNode(rootIndex).Text}] = {currentRank}");
                    this.messageList.Add(new TopoSortMessage(
                        TopoSortDigits.PickRoot,
                        nodeIndex: rootIndex,
                        dataList: new List<int>() { currentRank }));


                    var temp = neighbour.GetNeighbourOf(rootIndex);
                    foreach (var nodeIndex in temp)
                    {
                        // [UpdateNeighbour]
                        nodeDegrees[nodeIndex].InDegree--;
                        this.messageList.Add(new TopoSortMessage(
                            TopoSortDigits.UpdateNeighbour,
                            edgeIndex: (int)neighbour.GetEdge(rootIndex, nodeIndex).Tag,
                            dataList: new List<int>() { nodeDegrees[nodeIndex].InDegree }));


                        if (nodeDegrees[nodeIndex].InDegree == 0)
                        {
                            // [NewRoots]
                            newRoots.Add(nodeIndex);
                            this.messageList.Add(new TopoSortMessage(
                                TopoSortDigits.NewRoots,
                                nodeIndex: nodeIndex
                                ));
                        }

                        // [CancelEdge]
                        this.messageList.Add(new TopoSortMessage(
                            TopoSortDigits.CancelEdge,
                            edgeIndex: (int)neighbour.GetEdge(rootIndex, nodeIndex).Tag));
                    }
                }

                roots = newRoots;
                newRoots = new List<int>();
                currentRank++;
            }
        }

        public async Task Run(Size windowSize)
        {
            Init();

            TopoSort();

            await ShowAnimation();

            MoveNodeByRank(windowSize, maxRank: rank.Count - 1);
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
                    case TopoSortDigits.CalcDegree:
                        node.Title = $"id = {(int)node.Tag}\nin degree = {message.DataList[0]}\nrank = {message.DataList[1]}";
                        node.TitleFontSize = nodeTitleFontSize + 2;
                        node.TitleForeColor = Brushes.Blue;

                        await Task.Delay(TimeSleep);

                        node.TitleFontSize = nodeTitleFontSize;
                        node.TitleForeColor = Brushes.Black;
                        break;


                    case TopoSortDigits.AddNodeIntoRoot:
                        node.Background = Brushes.LightGreen;
                        break;


                    case TopoSortDigits.PickRoot:
                        node.Title = $"id = {(int)node.Tag}\nin degree = 0\nrank = {message.DataList[0]}";
                        node.Background = Brushes.LightBlue;
                        break;


                    case TopoSortDigits.UpdateNeighbour:
                        var text = vNode.Title;
                        var rankValue = text.Substring(text.LastIndexOf('=') + 2);
                        vNode.Title = $"id = {(int)vNode.Tag}\nin degree = {message.DataList[0]}\nrank = {rankValue}";
                        vNode.TitleFontSize = nodeTitleFontSize + 2;
                        vNode.TitleForeColor = Brushes.Blue;

                        uNode.BorderColor = Brushes.Blue;
                        vNode.BorderColor = Brushes.Blue;
                        edge.LineColor = Brushes.Blue;

                        uNode.BorderThickness = 2;
                        vNode.BorderThickness = 2;
                        edge.LineThickness = 2;
                        break;


                    case TopoSortDigits.NewRoots:
                        node.Background = Brushes.LightGreen;
                        break;


                    case TopoSortDigits.CancelEdge:
                        vNode.TitleFontSize = nodeTitleFontSize;
                        vNode.TitleForeColor = Brushes.Black;

                        uNode.BorderColor = Brushes.Black;
                        vNode.BorderColor = Brushes.Black;
                        edge.LineColor = Brushes.Black;

                        uNode.BorderThickness = 1;
                        vNode.BorderThickness = 1;
                        edge.LineThickness = 1;
                        break;
                }

                await Task.Delay(TimeSleep);
            }
        }

        private void MoveNodeByRank(Size windowSize, int maxRank)
        {
            Point topLeft = new Point(40, 40);
            Point bottomRight = new Point(windowSize.Width - 40, windowSize.Height - 40);
            Point center = new Point((topLeft.X + bottomRight.X) / 2.0, (topLeft.Y + bottomRight.Y) / 2.0);
            Size displayScreen = new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
            double deltaX = displayScreen.Width / (maxRank + 1.0);

            double[] startX = new double[maxRank + 1];
            startX[0] = topLeft.X;
            for (int i = 1; i < startX.Length; i++)
            {
                startX[i] = startX[i - 1] + deltaX;
            }

            for (int currentRank = 0; currentRank < rank.Count; currentRank++)
            {
                int countNode = rank[currentRank].Count;
                double deltaY = displayScreen.Height / countNode;
                double startY = center.Y - ((countNode - 1) * deltaY) / 2;
                                 
                foreach (int nodeIndex in rank[currentRank])
                {
                    Point nodeLocate = new Point(startX[currentRank] , startY);

                    neighbour.GetNode(nodeIndex).SetLocate(nodeLocate);

                    startY += deltaY;
                }
            }
        }
    }
}
