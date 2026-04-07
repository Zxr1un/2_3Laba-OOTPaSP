using _2_3Laba.Figures.Polygons;
using _2_3Laba.Figures.Polygons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2_3Laba.Figures
{
    public class PolygonMy: FigureMy
    {

        public List<Side> sides = new(); //Список сторон
        public List<Point> points = new(); // список точек (относительный)
        public Polygon poly = null; //полигон фона
        public Brush color = Brushes.Transparent;


        public override FigureMy Clone(FigureMy part = null, FigureMy parentCop = null)
        {
            if (part is Side s1)
            {
                return base.Clone(s1, parentCop);
            }
            PolygonMy copy = new();
            foreach (Point p in points) {
                copy.points.Add(new(p.X, p.Y));
            }
            copy.poly = new Polygon()
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Green,
                StrokeThickness = 2
            };
            foreach(Point p in poly.Points) copy.poly.Points.Add(new(p.X,p.Y));

            copy.color = color;
            //copy.base_init();
            try
            {
                copy.canva.Children.Remove(copy.poly);
                foreach (Side s in copy.sides) {
                    s.canva.Children.Remove(s.poly);
                }
            }
            catch { }
            if (base.Clone(copy, parentCop) is PolygonMy pol)
            {
                copy = pol;
                foreach(Side s in copy.children)
                {
                    copy.sides.Add(s);
                }
                return copy;
            }
            
            return null;
        }
        public override void Insert(FigureMy par = null)
        {
            canva.Children.Add(poly);
            if(!(this is Side side))
            {
                poly.MouseLeftButtonDown += OnLMC;
                poly.MouseLeftButtonUp += OnLMU;
                poly.MouseMove += MouseMoving;
                poly.MouseRightButtonDown += OnRMC;
            }
            else
            {
                side.poly.MouseRightButtonDown += side.OnClick;
            }
                base.Insert(par);
        }

        public PolygonMy()
        {
            poly = new Polygon()
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Green,
                StrokeThickness = 2
            };
        }
        public override void base_init()
        {
            base.base_init();
            Point start_pos = SE.Get_center();
            glob = start_pos;
            if (points.Count <= 1)
            {
                return;
            }
            canva = SE.canva;

            canva.Children.Add(poly);
            
            foreach (Point p in points)
            {
                poly.Points.Add(getGlobal(p));
            }
            for (int i = 0; i < points.Count - 1; i++)
            {
                Side side = new Side(this, getGlobal(points[i]), getGlobal(points[i + 1]));
                side.Index = i;
                side.name = "Сторона" + i.ToString();
                sides.Add(side);
            }
            Side sideLast = new Side(this, getGlobal(points[points.Count - 1]), getGlobal(points[0]));
            sideLast.Index = points.Count - 1;
            sideLast.name = "Сторона" + sideLast.Index.ToString();
            sides.Add(sideLast);

            canva.Children.Add(border);
            canva.Children.Add(CenterPoint);
            

            poly.MouseLeftButtonDown += OnLMC;
            poly.MouseLeftButtonUp += OnLMU;
            poly.MouseMove += MouseMoving;
            poly.MouseRightButtonDown += OnRMC;
            Draw();
        }

        public override void Draw()
        {

            for (int i = 0; i < points.Count; i++)
            {
                poly.Points[i] = getGlobal(points[i]);
            }
            //верхний for -- небольшой костыль
            for(int j = 0; j < 2; j++)
            {
                for (int i = 0; i < sides.Count; i++)
                {
                    Point p1 = getGlobal(points[i]);
                    Point p2 = getGlobal((i < points.Count - 1) ? points[i + 1] : points[0]);

                    if (sides.Count > 1)
                    {
                        Side prev = (i > 0) ? sides[i - 1] : sides[sides.Count - 1];
                        Side next = (i < sides.Count - 1) ? sides[i + 1] : sides[0];

                        sides[i].UpdatePoints(p1, p2, prev, next);
                    }
                    else
                    {
                        sides[i].UpdatePoints(p1, p2);
                    }
                }
            }
            poly.Fill = color;



            base.Draw();
        }

        public override void Update_borders()
        {
            if (sides == null || sides.Count == 0)
                return;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (Side s in sides)
            {
                Point[] pts = new Point[]
                {
            s.T_P1, s.T_P2, s.H_P1, s.H_P2
                };

                foreach (Point p in pts)
                {
                    if (p.X < minX) minX = p.X;
                    if (p.Y < minY) minY = p.Y;
                    if (p.X > maxX) maxX = p.X;
                    if (p.Y > maxY) maxY = p.Y;
                }
            }

            b_p1 = new Point(minX, minY);
            b_p2 = new Point(maxX, maxY);

            base.Update_borders();
        }



        public void OnLMC(object sender, MouseEventArgs e)
        {
            dropping = true;
            // Сохраняем позицию мыши в момент нажатия
            lastMousePosition = e.GetPosition(canva);
            poly.CaptureMouse(); // Захватываем мышь для надежности
            SE.Select(this);
        }
        public void OnLMU(object sender, MouseEventArgs e) // Обработчик отпускания
        {
            dropping = false;
            poly.ReleaseMouseCapture();
        }
        public void OnRMC(object sender, RoutedEventArgs e)
        {
            SE.Select(this);
            Edit();
        }
        public void MouseMoving(object sender, MouseEventArgs e)
        {
            if (dropping)
            {
                Point currentPosition = e.GetPosition(canva);

                double offsetX = currentPosition.X - lastMousePosition.X;
                double offsetY = currentPosition.Y - lastMousePosition.Y;
                d_Move_drag(offsetX, offsetY);

                lastMousePosition = currentPosition;
            }
        }

        public override void Delete()
        {
            base.Delete();
            canva.Children.Remove(poly);
            foreach(Side s in sides)
            {
                s.Delete();
            }
            sides.Clear();
        }



    }
}
