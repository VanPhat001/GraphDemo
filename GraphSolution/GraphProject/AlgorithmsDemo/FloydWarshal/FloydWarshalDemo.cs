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

namespace GraphProject.AlgorithmsDemo.FloydWarshal
{
    public class FloydWarshalDemo
    {

        private NeighbourhoodGraph neighbour;
        private int[,] cost;
        private List<Edge> newEdge;
        private readonly static int TimeSleep = MainWindow.TimeSleep;
        public readonly static int oo = 99999999;


        public FloydWarshalDemo(List<Node> nodeList, List<Edge> edgeList, bool isDirectedGraph)
        {
            neighbour = new NeighbourhoodGraph(nodeList, edgeList, isDirectedGraph);
            newEdge = new List<Edge>();
            cost = new int[neighbour.CountNode, neighbour.CountNode];
        }

        private void Init()
        {
            newEdge.Clear();


            for (int i = 0; i < neighbour.CountNode; i++)
            {
                for (int j = 0; j < neighbour.CountNode; j++)
                {
                    if (neighbour.CheckAdjacent(i, j))
                    {
                        cost[i, j] = neighbour.GetWeight(i, j);
                    }
                    else
                    {
                        cost[i, j] = oo;
                    }
                }
                cost[i, i] = 0;
            }
        }

        /// <summary> Get 'edge' in newEdge list and neighbour.EdgeList </summary>
        private Edge GetEdge(int uIndex, int vIndex)
        {
            Edge edge = neighbour.GetEdge(uIndex, vIndex);
            if (edge == null)
            {
                edge = newEdge.Find((e) => (int)e.UNode.Tag == uIndex && (int)e.VNode.Tag == vIndex);
            }
            return edge;
        }

        private async Task FloydWarshal(Canvas canvasMain)
        {
            for (int k = 0; k < neighbour.CountNode; k++)
            {
                // [PickNode]
                await ShowAnimation(new FloydWarshalMessage(
                    FloydWarshalDigits.PickNode,
                    nodeIndex: k));


                for (int uIndex = 0; uIndex < neighbour.CountNode; uIndex++)
                {
                    // [PickNode]
                    if (uIndex == k || cost[uIndex, k] == oo) continue;
                    await ShowAnimation(new FloydWarshalMessage(
                        FloydWarshalDigits.PickNode,
                        nodeIndex: uIndex));
                    // [PickEdge]
                    var e1 = GetEdge(uIndex, k);
                    await ShowAnimation(new FloydWarshalMessage(
                        FloydWarshalDigits.PickEdge,
                        edge: e1));

                    for (int vIndex = 0; vIndex < neighbour.CountNode; vIndex++)
                    {
                        if (vIndex == uIndex || vIndex == k || cost[k, vIndex] == oo)
                        {
                            // empty code
                        }
                        else
                        {
                            // [PickNode]
                            await ShowAnimation(new FloydWarshalMessage(
                                FloydWarshalDigits.PickNode,
                                nodeIndex: vIndex));
                            // [PickEdge]
                            var e2 = GetEdge(k, vIndex);
                            await ShowAnimation(new FloydWarshalMessage(
                                FloydWarshalDigits.PickEdge,
                                edge: e2));


                            if (cost[uIndex, k] + cost[k, vIndex] < cost[uIndex, vIndex])
                            {
                                // [Update]
                                var oddValue = cost[uIndex, vIndex];
                                cost[uIndex, vIndex] = cost[uIndex, k] + cost[k, vIndex];

                                Edge edge;
                                if (oddValue == oo)
                                {
                                    edge = new Edge(
                                        canvas: canvasMain,
                                        uNode: neighbour.GetNode(uIndex),
                                        vNode: neighbour.GetNode(vIndex));
                                    newEdge.Add(edge);
                                }
                                else
                                {
                                    edge = GetEdge(uIndex, vIndex);
                                }
                                edge.Text = cost[uIndex, vIndex].ToString();

                                await ShowAnimation(new FloydWarshalMessage(
                                    FloydWarshalDigits.Update,
                                    edge: edge));
                            }

                            // [DefaultEdge]
                            await ShowAnimation(new FloydWarshalMessage(
                                FloydWarshalDigits.DefaultEdge,
                                edge: e2));
                            // [DefaultNode]
                            await ShowAnimation(new FloydWarshalMessage(
                                FloydWarshalDigits.DefaultNode,
                                nodeIndex: vIndex));
                        }
                    }

                    // [DefaultEdge]
                    await ShowAnimation(new FloydWarshalMessage(
                        FloydWarshalDigits.DefaultEdge,
                        edge: e1));
                    // [DefaultNode]
                    await ShowAnimation(new FloydWarshalMessage(
                        FloydWarshalDigits.DefaultNode,
                        nodeIndex: uIndex));
                }

                // [DefaultNode]
                await ShowAnimation(new FloydWarshalMessage(
                    FloydWarshalDigits.DefaultNode,
                    nodeIndex: k));
            }
        }


        public async Task Run(Canvas canvasMain)
        {
            Init();

            await FloydWarshal(canvasMain);

            MessageBox.Show("finish");

            ClearNewEdgeList();

            await Task.Delay(1);
        }

        private async Task ShowAnimation(FloydWarshalMessage message)
        {
            var node = message.NodeIndex == -1 ? null : neighbour.GetNode(message.NodeIndex);
            var edge = message._Edge;

            switch (message.MessageDigit)
            {
                case FloydWarshalDigits.PickNode:
                    node.BorderColor = Brushes.Blue;
                    node.BorderThickness = 2;
                    await Task.Delay(1);
                    return;


                case FloydWarshalDigits.PickEdge:
                    edge.LineColor = Brushes.Blue;
                    edge.LineThickness = 2;
                    break;


                case FloydWarshalDigits.DefaultNode:
                    node.BorderColor = Brushes.Black;
                    node.BorderThickness = 1;
                    break;


                case FloydWarshalDigits.DefaultEdge:
                    edge.LineColor = Brushes.Black;
                    edge.LineThickness = 1;
                    await Task.Delay(1);
                    return;


                case FloydWarshalDigits.Update:
                    edge.LineColor = Brushes.Blue;
                    edge.LineThickness = 2;

                    await Task.Delay(TimeSleep);

                    edge.LineColor = Brushes.Black;
                    edge.LineThickness = 1;
                    break;
            }

            await Task.Delay(TimeSleep);
        }

        private void ClearNewEdgeList()
        {
            for (int i = this.newEdge.Count - 1; i >= 0; i--)
            {
                var edge = this.newEdge[i];
                edge.Remove();
            }
            this.newEdge.Clear();
        }
    }
}
