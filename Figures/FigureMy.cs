using _2_3Laba.Figures.Polygons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
    public class FigureMy
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Canvas canva = null;
        public string type = "figure";
        public Point glob = new Point(0, 0);
        public Brush color = Brushes.Transparent;

        public Point center_loc = new Point(0, 0);
        public double scale = 1;
        public double angle = 0;
        public double dop_angle = 0;
        public string name = "Figure";

        public Point b_p1 = new(0, 0), b_p2 = new(0, 0);
        public Rectangle border = new Rectangle()
        {
            Visibility = Visibility.Hidden,
            StrokeThickness = 2,
            Stroke = Brushes.Gray,
            Fill = Brushes.Transparent,
            IsHitTestVisible = false
        };
        public Ellipse CenterPoint = new Ellipse()
        {
            Fill = Brushes.White,
            StrokeThickness = 2,
            Stroke = Brushes.Red,
            Width = 10,
            Height = 10,
            Visibility = Visibility.Hidden,
            IsHitTestVisible = false
        };

        public FigureMy parent = null;
        public ObservableCollection<FigureMy> children = new();

        public RedWindow RW = null;
        internal bool dropping = false;
        internal Point shapeStartPosition;
        internal Point lastMousePosition;

        public virtual FigureMy Clone(FigureMy part = null, FigureMy parentCop = null)
        {
            FigureMy copy;
            if (part == null) copy = new FigureMy();
            else copy = part;
            copy.type = type;
            copy.glob = new(glob.X, glob.Y);
            copy.color = color;
            copy.center_loc = new(center_loc.X, center_loc.Y);
            copy.scale = scale;
            copy.angle = angle;
            copy.name = name;
            copy.b_p1 = new(b_p1.X, b_p1.Y);
            copy.b_p2 = new(b_p2.X, b_p2.Y);

            copy.border.Visibility = border.Visibility;
            copy.border.StrokeThickness = border.StrokeThickness;
            copy.border.Stroke = border.Stroke;
            copy.border.Fill = border.Fill;
            copy.border.IsHitTestVisible = border.IsHitTestVisible;


            copy.CenterPoint.Fill = CenterPoint.Fill;
            copy.CenterPoint.StrokeThickness = CenterPoint.StrokeThickness;
            copy.CenterPoint.Stroke = CenterPoint.Stroke;
            copy.CenterPoint.Width = CenterPoint.Width;
            copy.CenterPoint.Height = CenterPoint.Height;
            copy.CenterPoint.Visibility = CenterPoint.Visibility;
            copy.CenterPoint.IsHitTestVisible = CenterPoint.IsHitTestVisible;

            copy.parent = parentCop;
            foreach (FigureMy ch in children)
            {
                FigureMy copyCh = ch.Clone(null, copy);
                if (!(copyCh is Side))
                {
                    copy.children.Add(copyCh);
                }
            }
            return copy;
        }
        public virtual void Insert(FigureMy par = null)
        {
            FigureMy test = this;
            if (par == null)
            {
                parent = SE.Scene;
                SE.Scene.children.Add(this);
            }
            else parent = par;
            foreach (FigureMy ch in children)
            {
                ch.Insert(this);
            }
            base_init(true);
            Move();
        }

        public FigureMy(string Name = "Figure")
        {
            //name = SE.Get_nomber() + "_" + Name;
            canva = SE.canva;
        }
        public virtual void base_init(bool reinitial = false)
        {
            if (parent == null && !(this is Side) && !(this is AllFigures))
            {
                parent = SE.Scene;
                SE.Scene.children.Add(this);
            }
            canva.Children.Add(border);
            canva.Children.Add(CenterPoint);
            SE.UpdateHierarchy();

        }

        public virtual void Move(double x, double y)
        {
            Point p;
            //if (parent != null) {
            //    p = parent.getGlobal(getLocal(new(x, y)));
            //}
            //else
            p = new Point(x, y);
            glob = p;
            Draw();
            if (RW != null) RW.UpdateData();
        }
        public void Move()
        {
            Move(glob.X, glob.Y);
        }
        public void d_Move(double dx, double dy)
        {
            foreach (FigureMy ch in children)
            {
                if (!(ch is Side)) ch.d_Move(dx, dy);
            }
            Move(glob.X + dx, glob.Y + dy);
        }
        public virtual void d_Move_drag(double dx, double dy)
        {
            //if (parent != null && !(parent is AllFigures))
            //{
            //    parent.d_Move_drag(dx, dy);
            //}
            //else
            //{
            //    Move(glob.X + dx, glob.Y + dy);
            //    foreach (FigureMy ch in children) d_Move(dx, dy);
            //}
            foreach (FigureMy sel in SE.selected)
            {
                if (sel.parent is AllFigures) sel.d_Move(dx, dy);
            }
        }
        public virtual void setScale(double new_scale)
        {
            scale = new_scale;
            Draw();
        }

        public virtual void Draw()
        {
            Canvas.SetLeft(CenterPoint, glob.X - CenterPoint.Width / 2);
            Canvas.SetTop(CenterPoint, glob.Y - CenterPoint.Height / 2);

            Update_borders();
            SE.UpdateHierarchy();
        }
        public virtual void Update_borders()
        {
            double x = Math.Min(b_p1.X, b_p2.X);
            double y = Math.Min(b_p1.Y, b_p2.Y);
            double width = Math.Abs(b_p2.X - b_p1.X);
            double height = Math.Abs(b_p2.Y - b_p1.Y);

            border.Width = width;
            border.Height = height;

            Canvas.SetLeft(border, x);
            Canvas.SetTop(border, y);
            FigureMy test = this;
            if (parent != null) parent.Update_borders();
        }
        public virtual void Edit()
        {
            if (RW != null) return;
            RedWindow red = new RedWindow(this);
            RW = red;
            red.Show();
        }
        public virtual void Delete()
        {
            if (parent != null)
            {
                parent.children.Remove(this);
            }
            if (!(this is AllFigures))
            {
                if (parent != null && parent.children.Count == 0) parent.Delete();
                else if (parent != null && parent is SuperFigure SF)
                {
                    SF.AddFigure(); //здесь это как перерасчёт
                }
                parent = null;
                canva.Children.Remove(border);
                canva.Children.Remove(CenterPoint);
            }

            foreach (FigureMy ch in children.ToList())
            {
                ch.Delete();
            }
            children.Clear();
            SE.UpdateHierarchy();

        }

        public virtual void Deselect()
        {
            border.Visibility = Visibility.Hidden;
            CenterPoint.Visibility = Visibility.Hidden;
        }
        public virtual void Select()
        {
            border.Visibility = Visibility.Visible;
            CenterPoint.Visibility = Visibility.Visible;
            if (parent != null && !(parent is AllFigures)) parent.Select();
        }

        public virtual void Rejection()
        {
            if (parent != null)
            {
                dop_angle = 0;
                parent.children.Remove(this);
                if (parent is SuperFigure par) par.AddFigure();
                parent = SE.Scene;
                SE.Scene.children.Add(this);
                SE.UpdateHierarchy();
            }
        }

        public Point getGlobal(Point p)
        {

            // 1. масштаб
            double x = (p.X + center_loc.X) * scale;
            double y = (p.Y + center_loc.Y) * scale;

            // 2. поворот
            double rad = (angle + dop_angle) * Math.PI / 180.0;

            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            double xr = x * cos - y * sin;
            double yr = x * sin + y * cos;

            // 3. перенос
            return new Point(xr + glob.X, yr + glob.Y);
        }
        public Point getLocal(Point p)
        {

            double x = p.X - glob.X;
            double y = p.Y - glob.Y;


            double rad = -(angle + dop_angle) * Math.PI / 180.0;

            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            double xr = x * cos - y * sin;
            double yr = x * sin + y * cos;

            return new Point(xr / scale - center_loc.X, yr / scale - center_loc.Y);
        }

        public virtual void Load(JsonElement el)
        {
            // базовые поля уже заполнены фабрикой
        }
        public virtual void Save(FigureMy fig, Dictionary<string, object> dict)
        {
            // базовые поля уже заполнены фабрикой
        }


    }


}