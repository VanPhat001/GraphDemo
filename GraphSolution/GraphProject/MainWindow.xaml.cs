using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphProject.AlgorithmsDemo.BellmanFord;
using GraphProject.AlgorithmsDemo.BFS;
using GraphProject.AlgorithmsDemo.DFS;
using GraphProject.AlgorithmsDemo.Dijkstra;
using GraphProject.AlgorithmsDemo.Kruskal;
using GraphProject.AlgorithmsDemo.Prim;
using GraphProject.AlgorithmsDemo.Tarjan;
using GraphProject.ObjectDisplay;
using Microsoft.Win32;

namespace GraphProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region fields
        private Node selectedNode;
        private Edge selectedEdge;
        private bool isDrawing;
        private List<Node> nodeList;
        private List<Edge> edgeList;
        private bool isDirectedGraph;
        public static int TimeSleep = 750;
        #endregion

        #region properties
        public Node SelectedNode
        {
            get => selectedNode;
            set
            {
                if (selectedNode != null)
                {
                    selectedNode.SetDefaultStatus();
                }
                selectedNode = value;
                if (selectedNode != null)
                {
                    selectedNode.SetSelectStatus();
                }
            }
        }

        public Edge SelectedEdge
        {
            get => selectedEdge;
            set
            {
                if (selectedEdge != null)
                {
                    selectedEdge.SetDefaultStatus();
                }
                selectedEdge = value;
                if (selectedEdge != null)
                {
                    selectedEdge.SetSelectStatus();
                }
            }

        }
        #endregion

        #region constructors / destructor
        public MainWindow()
        {
            InitializeComponent();

            SelectedNode = null;
            SelectedEdge = null;
            isDrawing = false;
            nodeList = new List<Node>();
            edgeList = new List<Edge>();
            isDirectedGraph = true;
        }
        #endregion

        #region methods
        private void DeleteNode(Node node)
        {
            if (SelectedNode == node)
            {
                SelectedNode = null;
            }
            node.Remove();
            this.nodeList.Remove(node);

            for (int i = edgeList.Count - 1; i >= 0; i--)
            {
                var edge = edgeList[i];
                if (edge.UNode == node || edge.VNode == node)
                {
                    edge.Remove();
                    this.edgeList.RemoveAt(i);
                }
            }
        }

        private void DeleteEdge(Edge edge)
        {
            this.edgeList.Remove(edge);
            edge.Remove();
            if (SelectedEdge == edge)
            {
                SelectedEdge = null;
            }
        }

        private void FinishDrawing()
        {
            if (isDrawing)
            {
                selectedEdge.Remove();
                isDrawing = false;
                SelectedEdge = null;

                var mousePosition = Mouse.GetPosition(canvasMain);
                foreach (var node in nodeList)
                {
                    if (node.CheckPointIn(mousePosition))
                    {
                        Edge edge = new Edge(canvasMain, SelectedNode, node, isDirectedGraph: isDirectedGraph);
                        edge.Click += Edge_Click_SelectEdge;
                        edge.RightClick += Edge_RightClick_DeleteEdge;
                        this.edgeList.Add(edge);
                        SelectedNode = null;
                        break;
                    }
                }
            }
        }

        private string ActionWindow_Keydown(Key key, string selectObjectText)
        {
            if (key == Key.Space)
            {
                selectObjectText += " ";
            }
            else if (key == Key.Delete)
            {
                // TODO: delete node
                if (SelectedNode != null)
                {
                    DeleteNode(SelectedNode);
                }
                else if (SelectedEdge != null)
                {
                    DeleteEdge(SelectedEdge);
                }
            }
            else if (key == Key.Back)
            {
                var text = selectObjectText;
                selectObjectText = text.Substring(0, Math.Max(text.Length - 1, 0));
            }
            else if (Key.A <= key && key <= Key.Z)
            {
                selectObjectText += (char)(key - Key.A + 'a');
            }
            else if (Key.D0 <= key && key <= Key.D9)
            {
                selectObjectText += (char)(key - Key.D0 + '0');
            }
            else if (Key.NumPad0 <= key && key <= Key.NumPad9)
            {
                selectObjectText += (char)(key - Key.NumPad0 + '0');
            }

            return selectObjectText;
        }

        public void ShowNodeTitle(bool isShow)
        {
            foreach (var node in nodeList)
            {
                if (isShow)
                {
                    node.TitleForeColor = Brushes.Black;
                }
                else
                {
                    node.TitleForeColor = Brushes.Transparent;
                }
            }
        }

        public void SetAllNodeDefault()
        {
            foreach (var node in nodeList)
            {
                node.SetDefaultStatus();
                node.Title = "";
            }
        }

        public void SetAllEdgeDefault()
        {
            foreach (var edge in edgeList)
            {
                edge.SetDefaultStatus();
            }
        }

        public void SetAllControlDefault()
        {
            SetAllNodeDefault();
            SetAllEdgeDefault();
        }

        public void ClearGraph()
        {
            foreach (var edge in edgeList)
            {
                edge.Remove();
                Debug.WriteLine("remove edge");
            }


            foreach (var node in nodeList)
            {
                node.Remove();
                Debug.WriteLine("remove node");
            }

            edgeList.Clear();
            nodeList.Clear();
            Debug.WriteLine("clear graph!!!");
        }

        private void ReadGraphFromText(string[] lines, string sourceName = "")
        {
            int countNode;
            if (!Int32.TryParse(lines[0], out countNode))
            {
                MessageBox.Show($"Dòng 1 trong {sourceName} sai cú pháp! - [Số đỉnh không hợp lệ]");
                return;
            }

            Tuple<int, int, int>[] info = new Tuple<int, int, int>[lines.Length - 1];
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                var items = line.Split(' ');

                int uNode, vNode, edgeValue = 0;
                if (items.Length < 2 || 3 < items.Length)
                {
                    MessageBox.Show($"Dòng { i + 1} trong { sourceName} sai cú pháp! - [Cú pháp: UNode VNode Weight]\nhoặc [Cú pháp: Unode Vnode]");
                    return;
                }
                if (!Int32.TryParse(items[0], out uNode))
                {
                    MessageBox.Show($"Dòng {i + 1} trong {sourceName} sai cú pháp! - [Không thể convert uNode]");
                    return;
                }
                if (!Int32.TryParse(items[1], out vNode))
                {
                    MessageBox.Show($"Dòng {i + 1} trong {sourceName} sai cú pháp! - [Không thể convert vNode]");
                    return;
                }
                if (items.Length == 3)
                {
                    if (!Int32.TryParse(items[2], out edgeValue))
                    {
                        MessageBox.Show($"Dòng {i + 1} trong {sourceName} sai cú pháp! - [Không thể convert edgeValue]");
                        return;
                    }
                }

                info[i - 1] = new Tuple<int, int, int>(uNode, vNode, edgeValue);
            }


            ClearGraph();

            Random rand = new Random();
            for (int i = 0; i < countNode; i++)
            {
                var node = new Node(canvasMain, (i + 1).ToString());
                int width = rand.Next((int)(this.Width - 100));
                int height = rand.Next((int)this.Height - 100);
                var locate = new Point(width, height);
                node.SetLocate(locate);
                node.Click += Node_Click_SelectNode;
                node.RightClick += Node_RightClick_DeleteNode;

                this.nodeList.Add(node);
                Debug.WriteLine("add node");
            }

            foreach (var item in info)
            {
                var uNode = nodeList[item.Item1 - 1];
                var vNode = nodeList[item.Item2 - 1];
                string content = item.Item3.ToString();
                var edge = new Edge(canvasMain, uNode, vNode, content, isDirectedGraph: isDirectedGraph);
                edge.Click += Edge_Click_SelectEdge;
                edge.RightClick += Edge_RightClick_DeleteEdge;

                this.edgeList.Add(edge);
                Debug.WriteLine("add edge");
            }

            Debug.WriteLine("read graph done!!!");
        }
        #endregion

        #region events
        #region window events
        private void Window_Loaded_Init(object sender, RoutedEventArgs e)
        {
            // init test case
            //btnOpen.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            var fileName = @"D:\data.txt";
            string[] lines = File.ReadAllLines(fileName);
            ReadGraphFromText(lines, "file " + fileName);
        }

        private void Window_MouseDoubleClick_CreateNode(object sender, MouseButtonEventArgs e)
        {
            Node node = new Node(canvasMain, this.nodeList.Count.ToString());

            this.nodeList.Add(node);

            node.SetLocate(e.GetPosition(canvasMain));
            node.Click += Node_Click_SelectNode;
            node.RightClick += Node_RightClick_DeleteNode;
        }

        private void Window_MouseLeftButtonDown_SetAllControlDefault(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(canvasMain);
            //Debug.WriteLine($"window click ({mousePosition.X},{mousePosition.Y})");

            SelectedNode = null;
            SelectedEdge = null;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.Text = ActionWindow_Keydown(e.Key, SelectedNode.Text);
            }
            else if (SelectedEdge != null)
            {
                SelectedEdge.Text = ActionWindow_Keydown(e.Key, SelectedEdge.Text);
            }
        }

        private void Window_KeyUp_CancelDraw(object sender, KeyEventArgs e)
        {
            FinishDrawing();
        }

        private void Window_MouseLeftButtonUp_CancelDraw(object sender, MouseButtonEventArgs e)
        {
            FinishDrawing();
        }
        #endregion

        #region canvas events
        private void canvasMain_MouseMove_DrawEdge(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(canvasMain);
            //Debug.WriteLine($"canvas mouse move ({mousePosition.X}, {mousePosition.Y})");

            if (SelectedNode != null && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (isDrawing && Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if (SelectedEdge == null)
                    {
                        SelectedEdge = new Edge(canvasMain, SelectedNode, null, isDirectedGraph: isDirectedGraph);
                    }
                    SelectedEdge.SetEndPoint(mousePosition);
                    Debug.WriteLine($"draw node ({mousePosition.X},{mousePosition.Y})");
                }
                else if (SelectedNode.CheckPointIn(mousePosition))
                {
                    SelectedNode.SetLocate(mousePosition);
                }
            }
        }
        #endregion

        #region node events
        private void Node_RightClick_DeleteNode(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var node = sender as Node;
            DeleteNode(node);
        }

        private void Node_Click_SelectNode(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var mousePosition = e.GetPosition(canvasMain);
            //Debug.WriteLine($"node click ({mousePosition.X},{mousePosition.Y})");
            SelectedNode = (sender as Node);
            SelectedNode.SetSelectStatus();
            SelectedEdge = null;

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                isDrawing = true;
            }
        }
        #endregion

        #region edge events
        private void Edge_RightClick_DeleteEdge(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var edge = sender as Edge;
            DeleteEdge(edge);
        }

        private void Edge_Click_SelectEdge(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            SelectedEdge = sender as Edge;
            SelectedNode = null;
        }
        #endregion
        #endregion

        #region events from xaml file
        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                string[] lines = File.ReadAllLines(ofd.FileName);
                ReadGraphFromText(lines, "file " + ofd.FileName);
            }
        }

        private void ButtonChangeLocation_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            for (int i = 0; i < nodeList.Count; i++)
            {
                var node = nodeList[i];
                int width = rand.Next((int)(this.Width - 100));
                int height = rand.Next((int)this.Height - 100);
                node.SetLocate(new Point(width, height));
            }
        }

        private void ButtonDirected_Click(object sender, RoutedEventArgs e)
        {
            if (isDirectedGraph == false)
            {
                isDirectedGraph = true;
                foreach (var edge in edgeList)
                {
                    edge.ShowArrow(isDirectedGraph);
                }
            }
        }

        private void ButtonUndirected_Click(object sender, RoutedEventArgs e)
        {
            if (isDirectedGraph == true)
            {
                isDirectedGraph = false;
                foreach (var edge in edgeList)
                {
                    edge.ShowArrow(isDirectedGraph);
                }
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Do you want to clear this graph?", "Clear graph", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ClearGraph();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private async void ButtonBFS_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlDefault();
            ShowNodeTitle(true);
            tlbOption.IsEnabled = false;

            #region main code
            BFSDemo bfs = new BFSDemo(nodeList, edgeList, isDirectedGraph);
            await bfs.Run();

            while (bfs.IsRunning)
            {
                Task.Delay(200).Wait();
            }
            #endregion

            MessageBox.Show("finish");
            tlbOption.IsEnabled = true;
            ShowNodeTitle(false);
            SetAllControlDefault();
        }

        private async void ButtonDFSStack_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlDefault();
            ShowNodeTitle(true);
            tlbOption.IsEnabled = false;

            #region main code
            DFSDemo dfs = new DFSDemo(nodeList, edgeList, isDirectedGraph);
            await dfs.Run(useStack: true);

            while (dfs.IsRunning)
            {
                Task.Delay(200).Wait();
            }
            #endregion

            MessageBox.Show("finish");
            tlbOption.IsEnabled = true;
            ShowNodeTitle(false);
            SetAllControlDefault();
        }

        private async void ButtonDFSRecursion_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlDefault();
            ShowNodeTitle(true);
            tlbOption.IsEnabled = false;

            #region main code
            DFSDemo dfs = new DFSDemo(nodeList, edgeList, isDirectedGraph);
            await dfs.Run(useStack: false);

            while (dfs.IsRunning)
            {
                Task.Delay(200).Wait();
            }
            #endregion

            MessageBox.Show("finish");
            tlbOption.IsEnabled = true;
            ShowNodeTitle(false);
            SetAllControlDefault();
        }

        private async void ButtonTarjan_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlDefault();
            ShowNodeTitle(true);
            tlbOption.IsEnabled = false;

            #region main code
            if (isDirectedGraph == false)
            {
                MessageBox.Show("Can't run algorithm with undirected graph");
            }
            else
            {
                TarjanDemo tarjan = new TarjanDemo(nodeList, edgeList, isDirectedGraph);
                await tarjan.Run();

                while (tarjan.IsRunning)
                {
                    Task.Delay(200).Wait();
                }

                MessageBox.Show("finish");
            }
            #endregion

            tlbOption.IsEnabled = true;
            ShowNodeTitle(false);
            SetAllControlDefault();
        }

        private async void ButtonDijkstra_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlDefault();
            ShowNodeTitle(true);
            tlbOption.IsEnabled = false;

            #region main code
            if (SelectedNode == null)
            {
                MessageBox.Show("Select one node before run Dijkstra algorithm");
            }
            else
            {
                DijkstraDemo dijkstra = new DijkstraDemo(nodeList, edgeList, isDirectedGraph);
                await dijkstra.Run(SelectedNode);

                while (dijkstra.IsRunning)
                {
                    Task.Delay(200).Wait();
                }

                MessageBox.Show("finish");
            }
            #endregion

            tlbOption.IsEnabled = true;
            ShowNodeTitle(false);
            SetAllControlDefault();
        }

        private async void ButtonBellmanFord_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlDefault();
            ShowNodeTitle(true);
            tlbOption.IsEnabled = false;

            #region main code
            if (SelectedNode == null)
            {
                MessageBox.Show("Select one node before run Dijkstra algorithm");
            }
            else
            {
                var myListViewControl = new MyListViewControl(lsbStatus);
                BellmanFordDemo bellmanFord = new BellmanFordDemo(nodeList, edgeList, isDirectedGraph, myListViewControl);
                await bellmanFord.Run(SelectedNode);

                while (bellmanFord.IsRunning)
                {
                    Task.Delay(200).Wait();
                }

                MessageBox.Show("finish");
                myListViewControl.SetHidden();
            }
            #endregion

            tlbOption.IsEnabled = true;
            ShowNodeTitle(false);
            SetAllControlDefault();
        }

        private async void ButtonKruskal_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlDefault();
            ShowNodeTitle(true);
            tlbOption.IsEnabled = false;

            #region main code
            if (isDirectedGraph)
            {
                MessageBox.Show("Can't run algorithm with directed graph");
            }
            else
            {
                var myListViewControl = new MyListViewControl(lsbStatus);
                KruskalDemo kruskal = new KruskalDemo(nodeList, edgeList, isDirectedGraph, myListViewControl);
                await kruskal.Run();

                while (kruskal.IsRunning)
                {
                    Task.Delay(200).Wait();
                }

                MessageBox.Show("finish");
                myListViewControl.SetHidden();
            }
            #endregion

            tlbOption.IsEnabled = true;
            ShowNodeTitle(false);
            SetAllControlDefault();
        }

        private async void ButtonPrim_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlDefault();
            ShowNodeTitle(true);
            tlbOption.IsEnabled = false;

            #region main code
            if (isDirectedGraph)
            {
                MessageBox.Show("Can't run algorithm with directed graph");
            }
            else
            {
                PrimDemo prim = new PrimDemo(nodeList, edgeList, isDirectedGraph);
                await prim.Run();

                while (prim.IsRunning)
                {
                    Task.Delay(200).Wait();
                }

                MessageBox.Show("finish");
            }
            #endregion

            tlbOption.IsEnabled = true;
            ShowNodeTitle(false);
            SetAllControlDefault();
        }
        #endregion

    }
}
