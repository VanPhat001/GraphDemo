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
    public class Node
    {
        #region fields
        private readonly Canvas canvas;
        private Grid grid;
        private Ellipse ellipse;
        private TextBlock textBlockContent;
        private TextBlock textBlockTitle;
        private object tag;
        #endregion

        #region properties
        public string Title
        {
            get => textBlockTitle.Text;
            set
            {
                textBlockTitle.Text = value;

                Size nodeSize = GetNodeOutterSize();
                Size textBlockTitleSize = TextToSizeConverter.ConvertSize(value, textBlockTitle.FontSize);
                Canvas.SetLeft(textBlockTitle, CenterPoint.X - textBlockTitleSize.Width / 2.0);
                Canvas.SetTop(textBlockTitle, Canvas.GetTop(grid) + nodeSize.Height);
            }
        }

        public double TitleFontSize
        {
            get => textBlockTitle.FontSize;
            set => textBlockTitle.FontSize = value;
        }

        public string Text
        {
            get => textBlockContent.Text;
            set => textBlockContent.Text = value;
        }

        public Object Tag
        {
            get => tag;
            set => tag = value;
        }

        public Brush TitleForeColor
        {
            get => textBlockTitle.Foreground;
            set => textBlockTitle.Foreground = value;
        }

        public Brush ContentForeColor
        {
            get => this.textBlockContent.Foreground;
            set => this.textBlockContent.Foreground = value;
        }

        public Brush Background
        {
            get => this.ellipse.Fill;
            set => this.ellipse.Fill = value;
        }

        public Brush BorderColor
        {
            get => this.ellipse.Stroke;
            set => this.ellipse.Stroke = value;
        }

        public double BorderThickness
        {
            get => this.ellipse.StrokeThickness;
            set => this.ellipse.StrokeThickness = value;
        }

        public Point CenterPoint
        {
            get
            {
                var topLeft = GetTopLeftPoint();
                var nodeSize = GetNodeInnerSize();
                return new Point(topLeft.X + nodeSize.Width / 2.0, topLeft.Y + nodeSize.Height / 2.0);
            }
        }
        #endregion

        #region constructors / destructor
        public Node(Canvas canvas, string text)
        {
            this.canvas = canvas;
            this.grid = new Grid();
            this.ellipse = new Ellipse();
            this.textBlockContent = new TextBlock();
            this.textBlockTitle = new TextBlock();

            this.Text = text;
            this.Tag = null;

            Init();
        }
        #endregion

        #region methods
        private void Init()
        {
            // add controls into canvasMain
            canvas.Children.Add(grid);
            canvas.Children.Add(textBlockTitle);

            // add controls into grid (Node wrapper)
            grid.Children.Add(ellipse);
            grid.Children.Add(textBlockContent);

            // config controls
            Canvas.SetZIndex(grid, 1);

            ellipse.Fill = Brushes.White;
            ellipse.Width = ellipse.Height = 35;
            ellipse.Stroke = Brushes.Black;
            ellipse.StrokeThickness = 1;

            textBlockContent.MinWidth = 15;
            textBlockContent.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            textBlockContent.HorizontalAlignment = HorizontalAlignment.Center;
            textBlockContent.TextAlignment = TextAlignment.Center;
            textBlockContent.Background = Brushes.Transparent;
            textBlockContent.Foreground = Brushes.Black;

            textBlockTitle.Background = Brushes.Transparent;
            textBlockTitle.Foreground = Brushes.Transparent;
            textBlockTitle.TextAlignment = TextAlignment.Center;

            // events
            grid.MouseLeftButtonDown += Node_Click;
            grid.MouseRightButtonDown += Node_RightClick;
        }

        public void Remove()
        {
            canvas.Children.Remove(grid);
            canvas.Children.Remove(textBlockTitle);
        }

        public bool CheckPointIn(Point point)
        {
            Size nodeSize = GetNodeOutterSize();
            Point topLeftPoint = GetTopLeftPoint();

            return topLeftPoint.X <= point.X && point.X <= topLeftPoint.X + nodeSize.Width
                && topLeftPoint.Y <= point.Y && point.Y <= topLeftPoint.Y + nodeSize.Height;
        }
        #endregion

        #region setters
        public void SetDefaultStatus()
        {
            ellipse.Fill = Brushes.White;
            ellipse.Stroke = Brushes.Black;
            ellipse.StrokeThickness = 1;
            textBlockContent.Foreground = Brushes.Black;
        }

        public void SetSelectStatus()
        {
            ellipse.Fill = Brushes.LightBlue;
            ellipse.StrokeThickness = 2;
            textBlockContent.Foreground = Brushes.Red;
        }

        public void SetLocate(Point centerPoint)
        {
            Size nodeSize = GetNodeOutterSize();
            Canvas.SetLeft(grid, centerPoint.X - nodeSize.Width / 2.0);
            Canvas.SetTop(grid, centerPoint.Y - nodeSize.Height / 2.0);

            Size textBlockTitleSize = TextToSizeConverter.ConvertSize(textBlockTitle.Text, textBlockTitle.FontSize);
            Canvas.SetLeft(textBlockTitle, centerPoint.X - textBlockTitleSize.Width / 2.0);
            Canvas.SetTop(textBlockTitle, Canvas.GetTop(grid) + nodeSize.Height);

            OnNodeChangeLocate();
        }
        #endregion

        #region getters
        // get ellipse size
        public Size GetNodeInnerSize()
        {
            return new Size(ellipse.Width, ellipse.Height);
        }

        // get grid size
        private Size GetNodeOutterSize()
        {
            Size textBlockSize = TextToSizeConverter.ConvertSize(textBlockContent.Text, textBlockContent.FontSize);
            return new Size(Math.Max(ellipse.Width, textBlockSize.Width), ellipse.Height);
        }

        private Point GetTopLeftPoint()
        {
            return new Point(Canvas.GetLeft(grid), Canvas.GetTop(grid));
        }
        #endregion

        #region events
        private void Node_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnClick(e);
        }

        private void Node_RightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnRightClick(e);
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

        #region NodeChangeLocateEvent
        private event EventHandler nodeChangeLocate;

        public event EventHandler NodeChangeLocate
        {
            add
            {
                nodeChangeLocate += value;
            }
            remove
            {
                nodeChangeLocate -= value;
            }
        }

        private void OnNodeChangeLocate()
        {
            nodeChangeLocate?.Invoke(this, new EventArgs());
        }
        #endregion
        #endregion
    }
}
