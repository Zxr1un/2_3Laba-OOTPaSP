using _2_3Laba.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;



namespace _2_3Laba.Figures.Polygons
{
    
    public class Side: PolygonMy
    {
        public int Index;

        public Vector vector;
        public Point glob_2;
        public Point T_P1, T_P2, H_P1, H_P2;
        public Brush color = Brushes.Red;
        public int thickness = 10;

        public override FigureMy Clone(FigureMy part = null, FigureMy parentCop = null)
        {
            Side clone = null;
            if (parentCop is PolygonMy polClone)
            {
                clone = new(polClone, new(glob.X, glob.Y), new(glob_2.X, glob_2.Y));
                clone.color = color;
                clone.thickness = thickness;
                clone.Index = Index;
            }
            else return null;
            return base.Clone(clone, parentCop);
        }

        public Side(PolygonMy par,Point p1, Point p2) {
            parent = par;
            glob = new Point(p1.X, p1.Y);
            glob_2 = new Point(p2.X, p2.Y);

        }
        public override void base_init(bool reinitial = false)
        {
            canva = SE.canva;
            if (!parent.children.Contains(this)) parent.children.Add(this);
            poly = new Polygon()
            {
                Stroke = Brushes.Transparent,
                Fill = color,
                StrokeThickness = 1
            };
            
            for (int i = 0; i < 4; i++)
            {
                poly.Points.Add(glob);
            }
            canva.Children.Add(poly);
            UpdatePoints(glob, glob_2);
            poly.MouseRightButtonDown += OnClick;
            if (parent is PolygonMy pol)
            {
                poly.MouseLeftButtonDown += pol.OnLMC;
                poly.MouseLeftButtonUp += pol.OnLMU;
                poly.MouseMove += pol.MouseMoving;
            }
            base.base_init(reinitial);
        }

        public override void Draw()
        {
            poly.Fill = color;
        }

        public void UpdatePoints(Point p1, Point p2, Side prev = null, Side next = null)
        {
            glob = p1;
            glob_2 = p2;
            vector = (Vector)p2 - (Vector)p1;
            vector.Normalize();
            Calculate_Connection(prev, next);
            Draw();
        }


        public void Calculate_Connection(Side prev = null, Side next = null)
        {
            if (vector.Length < 0.0001) return;

            Vector dir = vector;
            dir.Normalize();

            Vector normal = new Vector(dir.Y, -dir.X); // твоя ориентация
            double t = thickness / 2.0;

            // Базовые точки (если нет соседей)
            Point b1 = glob + normal * t;
            Point b2 = glob - normal * t;
            Point e1 = glob_2 + normal * t;
            Point e2 = glob_2 - normal * t;

            // --- СТЫК С PREV ---
            if (prev != null && prev.vector.Length > 0.0001)
            {
                Vector dirPrev = prev.vector;
                dirPrev.Normalize();

                Vector normalPrev = new Vector(dirPrev.Y, -dirPrev.X);
                double tPrev = prev.thickness / 2.0;

                Point pb1 = prev.glob + normalPrev * tPrev;
                Point pb2 = prev.glob - normalPrev * tPrev;
                Point pe1 = prev.glob_2 + normalPrev * tPrev;
                Point pe2 = prev.glob_2 - normalPrev * tPrev;

                var inter1 = LineIntersection(pb1, pe1, b1, e1);
                var inter2 = LineIntersection(pb2, pe2, b2, e2);

                if (inter1.HasValue) b1 = inter1.Value;
                if (inter2.HasValue) b2 = inter2.Value;
            }

            // --- СТЫК С NEXT ---
            if (next != null && next.vector.Length > 0.0001)
            {
                Vector dirNext = next.vector;
                dirNext.Normalize();

                Vector normalNext = new Vector(dirNext.Y, -dirNext.X);
                double tNext = next.thickness / 2.0;

                Point nb1 = next.glob + normalNext * tNext;
                Point nb2 = next.glob - normalNext * tNext;
                Point ne1 = next.glob_2 + normalNext * tNext;
                Point ne2 = next.glob_2 - normalNext * tNext;

                var inter1 = LineIntersection(b1, e1, nb1, ne1);
                var inter2 = LineIntersection(b2, e2, nb2, ne2);

                if (inter1.HasValue) e1 = inter1.Value;
                if (inter2.HasValue) e2 = inter2.Value;
            }

            // сохраняем
            T_P1 = b1;
            T_P2 = b2;
            H_P1 = e2;
            H_P2 = e1;

            // применяем к полигону
            poly.Points[0] = T_P1;
            poly.Points[1] = T_P2;
            poly.Points[2] = H_P1;
            poly.Points[3] = H_P2;
        }
        private Point? LineIntersection(Point p1, Point p2, Point q1, Point q2)
        {
            double A1 = p2.Y - p1.Y;
            double B1 = p1.X - p2.X;
            double C1 = A1 * p1.X + B1 * p1.Y;

            double A2 = q2.Y - q1.Y;
            double B2 = q1.X - q2.X;
            double C2 = A2 * q1.X + B2 * q1.Y;

            double det = A1 * B2 - A2 * B1;

            if (Math.Abs(det) < 0.0001) return null; // параллельные

            double x = (B2 * C1 - B1 * C2) / det;
            double y = (A1 * C2 - A2 * C1) / det;

            return new Point(x, y);
        }

        public override void Edit()
        {
            if (parent is PolygonMy polyParent)
            {
                int index = Index;

                // если окно уже открыто — просто переключаем сторону
                if (polyParent.RW != null)
                {
                    polyParent.RW.SelectSide(index);
                }
                else
                {
                    // открываем редактор
                    polyParent.Edit();

                    // после открытия — выбрать сторону
                    if (polyParent.RW != null) polyParent.RW.SelectSide(index);
                }
            }
        }
        internal void OnClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Edit();
            e.Handled = true;
        }
        public override void Select()
        {
            if (poly.Fill != Brushes.Black) poly.Stroke = Brushes.Black;
            else poly.Stroke = Brushes.Red;
            poly.StrokeThickness = 1;
        }
        public override void Deselect()
        {
            poly.Stroke = Brushes.Transparent;
        }

        public override void Delete()
        {
            if (parent != null)
            {
                parent.children.Remove(this);
            }

            canva.Children.Remove(poly);
            canva.Children.Remove(border);
            canva.Children.Remove(CenterPoint);
            parent = null;

        }


    }
}
