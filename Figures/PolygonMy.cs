using _2_3Laba.Figures.Polygons;
using _2_3Laba.Figures.Polygons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
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
        public Polygon poly = new Polygon()
        {
            Stroke = Brushes.Transparent,
            Fill = Brushes.Green,
            StrokeThickness = 2
        }; //полигон фона


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
            foreach(Point p in poly.Points) copy.poly.Points.Add(new(p.X,p.Y));
            copy.color = color;

            if (base.Clone(copy, parentCop) is PolygonMy pol)
            {
                copy = pol;
                foreach (Side s in copy.children)
                {
                    copy.sides.Add(s);
                }
                return copy;
            }

            return null;
        }
        public override void Insert(FigureMy par = null)
        {
            base.Insert(par);
        }

        public PolygonMy()
        {
            type = "polygon";
        }
        public override void base_init(bool reinitial = false)
        {
            if(this is Side side1)
            {
                base.base_init(reinitial);
                return;
            }
            canva = SE.canva;
            if (!reinitial)
            {
                Point start_pos = SE.Get_center();
                glob = start_pos;
            }
            canva.Children.Add(poly);
            poly.Points.Clear();
            foreach (Point p in points)
            {
                poly.Points.Add(getGlobal(p));
            }
            if (children.Count == 0)
            {
                if (points.Count <= 1)
                {
                    return;
                }
                poly.Points.Clear();
                foreach (Point p in points)
                {
                    poly.Points.Add(getGlobal(p));
                }
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Side side = new Side(this, getGlobal(points[i]), getGlobal(points[i + 1]));
                    side.base_init();
                    side.Index = i;
                    side.name = "Сторона" + i.ToString();
                    sides.Add(side);
                }
                Side sideLast = new Side(this, getGlobal(points[points.Count - 1]), getGlobal(points[0]));
                sideLast.base_init();
                sideLast.Index = points.Count - 1;
                sideLast.name = "Сторона" + sideLast.Index.ToString();
                sides.Add(sideLast);
            }
            if(sides.Count == 0)
            {
                foreach(Side side in children)
                {
                    sides.Add(side);
                }
            }
            int minZIndex = 200;
            foreach (Side side in sides)
            {
                int zIndex = Panel.GetZIndex(side.poly);
                if (zIndex < minZIndex)
                    minZIndex = zIndex;
            }
            Panel.SetZIndex(poly, minZIndex - 1);




            poly.MouseLeftButtonDown += OnLMC;
            poly.MouseLeftButtonUp += OnLMU;
            poly.MouseMove += MouseMoving;
            poly.MouseRightButtonDown += OnRMC;
            
            base.base_init(reinitial);
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
            if (this is Side) return;
            canva.Children.Remove(poly);
            foreach(Side s in sides)
            {
                s.Delete();
            }
            sides.Clear();
        }


        public override void Load(JsonElement el)
        {
            if (el.TryGetProperty("points", out var arr))
            {
                points = new();

                foreach (var p in arr.EnumerateArray())
                {
                    points.Add(new Point(
                        p.GetProperty("X").GetDouble(),
                        p.GetProperty("Y").GetDouble()
                    ));
                }

            }
            
            
        }

        public override void Save(FigureMy fig, Dictionary<string, object> dict)
        {
            dict["points"] = points;
        }
    }
}
