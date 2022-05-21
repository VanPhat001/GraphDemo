using GraphProject.ObjectDisplay;
using GraphProject.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphProject.AlgorithmsDemo.FordFulkerson
{
    public class FordFulkersonDemo
    {
        private FlowGraph flowGraph;
        private List<FlowLabel> lablels;
        private readonly static int oo = 999999;
        private readonly static int TimeSleep = MainWindow.TimeSleep;
        private readonly MyListViewControl myListViewControl;

        public FordFulkersonDemo(List<Node> nodeList, List<Edge> edgeList, MyListViewControl myListViewControl)
        {
            flowGraph = new FlowGraph(nodeList, edgeList);
            lablels = new List<FlowLabel>();
            this.myListViewControl = myListViewControl;
        }

        private void Init()
        {
            for (int i = 0; i < flowGraph.CountNode; i++)
            {
                lablels.Add(new FlowLabel());
            }
            for (int i = 0; i < flowGraph.CountEdge; i++)
            {
                var edge = flowGraph.GetEdge(i);
                edge.Text = $"{flowGraph.GetFLow(i)}/{flowGraph.GetCapacity(i)}";
            }
        }

        private void ClearFlow()
        {
            for (int i = 0; i < flowGraph.CountEdge; i++)
            {
                var c = flowGraph.GetCapacity(i);
                flowGraph.GetEdge(i).Text = c.ToString();
            }
        }

        private async Task<double> FordFulkerson()
        {
            // init flow
            double total = 0;

            while (true)
            {
                // [ClearAllLabel]
                await ShowAnimation(new FordFulkersonMessage(FordFulkersonDigits.ClearAllLabel));

                #region init label step
                // init label
                for (int i = 0; i < lablels.Count; i++)
                {
                    lablels[i].SetValue(dir: FlowLableStatus.Nothing, parent: -1, flow: 0);
                }

                lablels[flowGraph.SourceIndex].SetValue(dir: FlowLableStatus.Increase, parent: flowGraph.SourceIndex, flow: oo);
                Queue<int> queue = new Queue<int>();
                queue.Enqueue(flowGraph.SourceIndex);

                // [ClearAllQueue]
                await ShowAnimation(new FordFulkersonMessage(FordFulkersonDigits.ClearAllQueue));

                // [ShowLabel]
                await ShowAnimation(new FordFulkersonMessage(
                    FordFulkersonDigits.ShowLabel,
                    nodeIndex: flowGraph.SourceIndex,
                    dataList: new List<object>() { new FlowLabel(lablels[flowGraph.SourceIndex]) }));

                // [PushQueue]
                await ShowAnimation(new FordFulkersonMessage(
                    FordFulkersonDigits.PushQueue,
                    dataList: new List<object>() { flowGraph.SourceIndex }));
                #endregion


                #region update label step
                // update label
                bool found = false;
                while (queue.Count > 0)
                {
                    int uIndex = queue.Dequeue();

                    // [PickNode]
                    await ShowAnimation(new FordFulkersonMessage(
                        FordFulkersonDigits.PickNode,
                        nodeIndex: uIndex));

                    // [PopQueue]
                    await ShowAnimation(new FordFulkersonMessage(
                        FordFulkersonDigits.PopQueue,
                        dataList: new List<object>() { uIndex }));


                    #region increase [u, v] step
                    // increase [u, v]
                    for (int vIndex = 0; vIndex < flowGraph.CountNode; vIndex++)
                    {
                        if (flowGraph.CheckAdjacent(uIndex, vIndex))
                        {
                            // [PickNode]
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.PickNode,
                                nodeIndex: vIndex));

                            // [PickEdge]
                            var e = flowGraph.GetEdge(uIndex, vIndex);
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.PickEdge,
                                edgeIndex: (int)e.Tag));


                            var f = flowGraph.GetFLow(uIndex, vIndex);
                            var c = flowGraph.GetCapacity(uIndex, vIndex);
                            if (lablels[vIndex].Dir == FlowLableStatus.Nothing && f < c)
                            {
                                lablels[vIndex].SetValue(
                                    dir: FlowLableStatus.Increase, parent: uIndex,
                                    flow: Math.Min(lablels[uIndex].Flow, c - f));
                                queue.Enqueue(vIndex);

                                // [ShowLabel]
                                await ShowAnimation(new FordFulkersonMessage(
                                    FordFulkersonDigits.ShowLabel,
                                    nodeIndex: vIndex,
                                    dataList: new List<object>() { new FlowLabel(lablels[vIndex]) }));

                                // [PushQueue]
                                await ShowAnimation(new FordFulkersonMessage(
                                    FordFulkersonDigits.PushQueue,
                                    dataList: new List<object>() { vIndex }));
                            }

                            // [DefaultEdge]
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.DefaultEdge,
                                edgeIndex: (int)e.Tag));

                            // [DefaultNode]
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.DefaultNode,
                                nodeIndex: vIndex));
                        }
                    }
                    #endregion


                    #region decrease [x, u] step
                    // decrease [x, u]
                    for (int xIndex = 0; xIndex < flowGraph.CountNode; xIndex++)
                    {
                        if (flowGraph.CheckAdjacent(xIndex, uIndex))
                        {
                            // [PickNode]
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.PickNode,
                                nodeIndex: xIndex));

                            // [PickEdge]
                            var e = flowGraph.GetEdge(xIndex, uIndex);
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.PickEdge,
                                edgeIndex: (int)e.Tag));


                            var f = flowGraph.GetFLow(xIndex, uIndex);
                            if (lablels[xIndex].Dir == FlowLableStatus.Nothing && f > 0)
                            {
                                lablels[xIndex].SetValue(
                                    dir: FlowLableStatus.Decrease, parent: uIndex,
                                    flow: Math.Min(lablels[uIndex].Flow, f));
                                queue.Enqueue(xIndex);

                                // [ShowLabel]
                                await ShowAnimation(new FordFulkersonMessage(
                                    FordFulkersonDigits.ShowLabel,
                                    nodeIndex: xIndex,
                                    dataList: new List<object>() { new FlowLabel(lablels[xIndex]) }));

                                // [PushQueue]
                                await ShowAnimation(new FordFulkersonMessage(
                                    FordFulkersonDigits.PushQueue,
                                    dataList: new List<object>() { xIndex }));
                            }


                            // [DefaultEdge]
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.DefaultEdge,
                                edgeIndex: (int)e.Tag));

                            // [DefaultNode]
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.DefaultNode,
                                nodeIndex: xIndex));
                        }
                    }
                    #endregion

                    // [DefaultNode]
                    await ShowAnimation(new FordFulkersonMessage(
                        FordFulkersonDigits.DefaultNode,
                        nodeIndex: uIndex));

                    if (lablels[flowGraph.SinkIndex].Dir != FlowLableStatus.Nothing)
                    {
                        found = true;
                        break;
                    }
                }
                #endregion


                #region increase flow step
                // increase flow
                if (found)
                {
                    var flow = lablels[flowGraph.SinkIndex].Flow;
                    int vIndex = flowGraph.SinkIndex;
                    total += flow;

                    while (true)
                    {
                        int uIndex = lablels[vIndex].Parent;
                        if (uIndex == vIndex) break;

                        // [PickNode]
                        await ShowAnimation(new FordFulkersonMessage(
                            FordFulkersonDigits.PickNode,
                            nodeIndex: uIndex));

                        // [PickNode]
                        await ShowAnimation(new FordFulkersonMessage(
                            FordFulkersonDigits.PickNode,
                            nodeIndex: vIndex));


                        Edge e;
                        if (lablels[uIndex].Dir == FlowLableStatus.Increase) // increase edge
                        {
                            #region increase flow
                            flowGraph.SetFlow(uIndex, vIndex, flowGraph.GetFLow(uIndex, vIndex) + flow);

                            // [PickEdge]
                            e = flowGraph.GetEdge(uIndex, vIndex);
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.PickEdge,
                                edgeIndex: (int)e.Tag));

                            // [UpdateEdge]
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.UpdateEdge,
                                edgeIndex: (int)e.Tag,
                                dataList: new List<object>()
                                {
                                    flowGraph.GetFLow(uIndex, vIndex),
                                    flowGraph.GetCapacity(uIndex, vIndex)
                                }));
                            #endregion
                        }
                        else // decrease edge
                        {
                            #region decrease flow
                            flowGraph.SetFlow(vIndex, uIndex, flowGraph.GetFLow(vIndex, uIndex) - flow);

                            // [PickEdge]
                            e = flowGraph.GetEdge(vIndex, uIndex);
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.PickEdge,
                                edgeIndex: (int)e.Tag));

                            // [UpdateEdge]
                            await ShowAnimation(new FordFulkersonMessage(
                                FordFulkersonDigits.UpdateEdge,
                                edgeIndex: (int)e.Tag,
                                dataList: new List<object>()
                                {
                                    flowGraph.GetFLow(vIndex, uIndex),
                                    flowGraph.GetCapacity(vIndex, uIndex)
                                }));
                            #endregion
                        }

                        // [DefaultEdge]
                        await ShowAnimation(new FordFulkersonMessage(
                            FordFulkersonDigits.DefaultEdge,
                            edgeIndex: (int)e.Tag));

                        // [DefaultNode]
                        await ShowAnimation(new FordFulkersonMessage(
                            FordFulkersonDigits.DefaultNode,
                            nodeIndex: vIndex));

                        // [DefaultNode]
                        await ShowAnimation(new FordFulkersonMessage(
                            FordFulkersonDigits.DefaultNode,
                            nodeIndex: uIndex));

                        vIndex = uIndex;
                    }
                }
                else
                {
                    break;
                }
                #endregion
            }

            return total;
        }

        public async Task Run()
        {
            Init();

            this.myListViewControl.SetVisible();

            double max_flow = await FordFulkerson();

            await ShowAnimation(new FordFulkersonMessage(FordFulkersonDigits.STCut));

            MessageBox.Show("max_flow = " + max_flow);

            ClearFlow();
        }

        private async Task ShowAnimation(FordFulkersonMessage message)
        {
            var node = message.NodeIndex == -1 ? null : flowGraph.GetNode(message.NodeIndex);
            var edge = message.EdgeIndex == -1 ? null : flowGraph.GetEdge(message.EdgeIndex);

            switch (message.MessageDigit)
            {
                case FordFulkersonDigits.ShowLabel:
                    var lbl = message.DataList[0] as FlowLabel;
                    node.Title = $"id = {(int)node.Tag}\n" + lbl;
                    node.TitleForeColor = Brushes.Blue;
                    node.TitleFontSize += 2;

                    await Task.Delay(TimeSleep);

                    node.TitleForeColor = Brushes.Black;
                    node.TitleFontSize -= 2;
                    break;


                case FordFulkersonDigits.PickNode:
                    node.BorderColor = Brushes.Blue;
                    node.BorderThickness = 2;
                    await Task.Delay(1);
                    return;


                case FordFulkersonDigits.PickEdge:
                    edge.LineColor = Brushes.Blue;
                    edge.LineThickness = 2;
                    break;


                case FordFulkersonDigits.DefaultEdge:
                    edge.LineColor = Brushes.Black;
                    edge.LineThickness = 1;
                    await Task.Delay(1);
                    return;


                case FordFulkersonDigits.DefaultNode:
                    node.BorderColor = Brushes.Black;
                    node.BorderThickness = 1;
                    break;


                case FordFulkersonDigits.UpdateEdge:
                    var f = (double)message.DataList[0];
                    var c = (double)message.DataList[1];
                    edge.Text = $"{f}/{c}";
                    edge.TextFontSize += 2;
                    edge.TextColor = Brushes.Blue;

                    await Task.Delay(TimeSleep);

                    edge.TextColor = Brushes.Black;
                    edge.TextFontSize -= 2;
                    break;


                case FordFulkersonDigits.ClearAllLabel:
                    for (int i = 0; i < flowGraph.CountNode; i++)
                    {
                        var _node = flowGraph.GetNode(i);
                        _node.Title = "";
                        await Task.Delay(100);
                    }
                    break;


                case FordFulkersonDigits.STCut:
                    for (int i = 0; i < flowGraph.CountNode; i++)
                    {
                        var _node = flowGraph.GetNode(i);
                        if (lablels[(int)_node.Tag].Dir == FlowLableStatus.Nothing)
                        {
                            _node.Background = Brushes.LightCoral;
                        }
                        else
                        {
                            _node.Background = Brushes.LightBlue;
                        }
                        await Task.Delay(100);
                    }
                    break;


                case FordFulkersonDigits.PushQueue:
                    this.myListViewControl.AddFirst(
                        text: message.DataList[0].ToString(),
                        textAlign: HorizontalAlignment.Center);
                    this.myListViewControl.ChangeBackgroundItem(
                        itemIndex: 0,
                        color: Brushes.LightBlue);
                    break;


                case FordFulkersonDigits.PopQueue:
                    this.myListViewControl.ChangeBackgroundItem(
                        itemIndex: 0,
                        color: Brushes.LightCoral);

                    await Task.Delay(TimeSleep);

                    this.myListViewControl.RemoveAt(0);
                    break;


                case FordFulkersonDigits.ClearAllQueue:
                    this.myListViewControl.Clear();
                    break;
            }

            await Task.Delay(TimeSleep);
        }
    }
}
