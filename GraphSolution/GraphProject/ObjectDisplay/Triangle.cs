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
    /*
     *         (M)      
     *          *   
     *          * *  
     *          * * *
     *          * * * *
     *      (I) * * * * *  (B)
     *          * * * * 
     *          * * *
     *          * *  
     *          * 
     *         (N)
     *         
     *   |MN| = |IB| = TriangleSize
     */
    public class Triangle
    {
        private Polyline polyline;
        private static readonly double TriangleSize = 8;
        private readonly Canvas canvas;


        public Triangle(Canvas canvas, Point startPoint, Point endPoint)
        {
            polyline = new Polyline();

            this.canvas = canvas;
            this.canvas.Children.Add(polyline);

            Init();

            Update(startPoint, endPoint);
        }

        private void Init()
        {
            polyline.Fill = Brushes.Black;
            Canvas.SetZIndex(polyline, 1);
        }

        public void Update(Point startPoint, Point endPoint, double deltaD = 0)
        {
            Point A = startPoint;
            Point B = endPoint;
            double t;

            if (deltaD != 0)
            {
                t = deltaD / Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
                B = new Point(B.X - (B.X - A.X) * t, B.Y - (B.Y - A.Y) * t);
            }

            //t = Constants.TriangleSize / Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
            t = TriangleSize / Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
            Point I = new Point(B.X - (B.X - A.X) * t, B.Y - (B.Y - A.Y) * t);

            //t = Constants.TriangleSize / 2.0 / Math.Sqrt(Math.Pow(A.Y - B.Y, 2) + Math.Pow(B.X - A.X, 2));
            t = TriangleSize / 2.0 / Math.Sqrt(Math.Pow(A.Y - B.Y, 2) + Math.Pow(B.X - A.X, 2));
            Point M = new Point(I.X + (B.Y - A.Y) * t, I.Y + (A.X - B.X) * t);
            Point N = new Point(I.X - (B.Y - A.Y) * t, I.Y - (A.X - B.X) * t);

            polyline.Points.Clear();
            polyline.Points.Add(B);
            polyline.Points.Add(M);
            polyline.Points.Add(N);
            polyline.Points.Add(B);
        }

        public void SetDefaultStatus()
        {
            polyline.Fill = Brushes.Black;
        }

        public void SetHiddenStatus()
        {
            polyline.Fill = Brushes.Transparent;
        }

        public void Remove()
        {
            canvas.Children.Remove(polyline);
        }
    }
}
