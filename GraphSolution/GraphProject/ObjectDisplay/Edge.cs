using GraphProject.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphProject.ObjectDisplay
{
    public class Edge
    {
        #region fields
        private readonly Canvas canvas;
        private readonly Node uNode;
        private readonly Node vNode;
        private Triangle triangle;
        private TextBlock textBlock;
        private Line line;
        private object tag;
        #endregion

        #region properties
        public Node UNode
        {
            get => uNode;
        }

        public Node VNode
        {
            get => vNode;
        }

        public string Text
        {
            get => textBlock.Text;
            set
            {
                textBlock.Text = value;
                UpdateStatus();
            }
        }

        public double TextFontSize
        {
            get => textBlock.FontSize;
            set => textBlock.FontSize = value;
        }

        public Brush TextColor
        {
            get => textBlock.Foreground;
            set => textBlock.Foreground = value;
        }

        public Brush LineColor
        {
            get => line.Stroke;
            set => line.Stroke = value;
        }

        public double LineThickness
        {
            get => line.StrokeThickness;
            set => line.StrokeThickness = value;
        }

        public object Tag
        {
            get => tag;
            set => tag = value;
        }
        #endregion

        #region constructors / destructor
        public Edge(Canvas canvas, Node uNode, Node vNode, string content = "", bool isDirectedGraph = true)
        {
            this.canvas = canvas;
            this.uNode = uNode;
            this.vNode = vNode;
            this.triangle = new Triangle(canvas, uNode.CenterPoint, vNode == null ? uNode.CenterPoint : vNode.CenterPoint);
            this.textBlock = new TextBlock();
            this.line = new Line();
            this.Tag = null;
            Init(content);
            ShowArrow(isDirectedGraph);
        }
        #endregion

        #region methods
        public void Remove()
        {
            canvas.Children.Remove(line);
            this.triangle.Remove();
            canvas.Children.Remove(textBlock);
        }

        private void Init(string content)
        {
            // add controls into canvasMain
            canvas.Children.Add(line);
            canvas.Children.Add(textBlock);

            // config controls
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 1;

            textBlock.Text = content;
            textBlock.Background = canvas.Background;
            textBlock.Foreground = Brushes.Black;
            textBlock.FontSize = 13;

            // set control locate
            var uPoint = uNode.CenterPoint;
            var vPoint = vNode == null ? uPoint : vNode.CenterPoint;
            SetStartPoint(uPoint);
            SetEndPoint(vPoint);

            // events
            if (this.uNode != null)
            {
                this.uNode.NodeChangeLocate += UNode_NodeChangeLocate;
            }
            if (this.vNode != null)
            {
                this.vNode.NodeChangeLocate += VNode_NodeChangeLocate;
            }
            line.MouseLeftButtonDown += Line_MouseLeftButtonDown;
            line.MouseRightButtonDown += Line_MouseRightButtonDown;
            textBlock.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
            textBlock.MouseRightButtonDown += TextBlock_MouseRightButtonDown;
        }

        private void UpdateStatus()
        {
            var startPoint = new Point(line.X1, line.Y1);
            var endPoint = new Point(line.X2, line.Y2);

            // update triangle locate
            double delta = vNode == null ? 0 : vNode.GetNodeInnerSize().Width / 2.0;
            this.triangle.Update(startPoint, endPoint, delta);

            // update textblock (content) locate
            var centerPoint = new Point((startPoint.X + endPoint.X) / 2.0, (startPoint.Y + endPoint.Y) / 2.0);
            var textBlockSize = TextToSizeConverter.ConvertSize(textBlock.Text, textBlock.FontSize);
            Canvas.SetLeft(textBlock, centerPoint.X - textBlockSize.Width / 2.0);
            Canvas.SetTop(textBlock, centerPoint.Y - textBlockSize.Height / 2.0);
        }

        public void ShowArrow(bool isDirectedGraph)
        {
            if (isDirectedGraph)
            {
                this.triangle.SetDefaultStatus();
            }
            else
            {
                this.triangle.SetHiddenStatus();
            }
        }

        #endregion

        #region events
        private void TextBlock_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnRightClick(e);
        }

        private void Line_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnRightClick(e);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnClick(e);
        }

        private void Line_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnClick(e);
        }

        private void VNode_NodeChangeLocate(object sender, EventArgs e)
        {
            SetEndPoint((sender as Node).CenterPoint);
        }

        private void UNode_NodeChangeLocate(object sender, EventArgs e)
        {
            SetStartPoint((sender as Node).CenterPoint);
        }
        #endregion

        #region setters
        public void SetStartPoint(Point startPoint)
        {
            line.X1 = startPoint.X;
            line.Y1 = startPoint.Y;
            UpdateStatus();
        }

        public void SetEndPoint(Point endPoint)
        {
            line.X2 = endPoint.X;
            line.Y2 = endPoint.Y;
            UpdateStatus();
        }

        public void SetDefaultStatus()
        {
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 1;
        }

        public void SetSelectStatus()
        {
            line.Stroke = Brushes.Blue;
            line.StrokeThickness = 2;
        }
        #endregion

        #region fields/properties events
        #region ClickEvent
        private event EventHandler<System.Windows.Input.MouseButtonEventArgs> click;
        public event EventHandler<System.Windows.Input.MouseButtonEventArgs> Click
        {
            add
            {
                click += value;
            }
            remove
            {
                click -= value;
            }
        }
        private void OnClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            click?.Invoke(this, e);
        }
        #endregion

        #region RightClickEvent
        private event EventHandler<System.Windows.Input.MouseButtonEventArgs> rightClick;
        public event EventHandler<System.Windows.Input.MouseButtonEventArgs> RightClick
        {
            add
            {
                rightClick += value;
            }
            remove
            {
                rightClick -= value;
            }
        }
        private void OnRightClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            rightClick?.Invoke(this, e);
        }
        #endregion
        #endregion
    }
}
