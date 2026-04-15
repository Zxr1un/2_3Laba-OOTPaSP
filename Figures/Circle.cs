using System;
using System.Collections.Generic;
using System.Linq;
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
    public class Circle: FigureMy
    {
        public Ellipse cir = new Ellipse();
        public double st_radius = 100;
        public double e = 1.0;
        
        public int stroke_thickness_cir = 10;
        
        public Brush stroke_cir = Brushes.Red;
        


        public override void base_init(bool reinitial = false)
        {
            if(name == "Figure") name = SE.Get_nomber() + "_" + "Круг";
            type = "circle";
            canva = SE.canva;
            if (!reinitial)
            {
                Point start_pos = SE.Get_center();
                Move(start_pos.X, start_pos.Y);
            }
            canva.Children.Add(cir);

            

            cir.MouseLeftButtonDown += OnLMC;
            cir.MouseLeftButtonUp += OnLMU;
            cir.MouseMove += MouseMoving;
            cir.MouseRightButtonDown += OnRMC;
            base.base_init(reinitial);
        }

        public override FigureMy Clone(FigureMy part = null, FigureMy parentCop = null)
        {
            
            Circle copy = new Circle();
            copy.st_radius = st_radius;
            copy.stroke_thickness_cir = stroke_thickness_cir;
            copy.stroke_cir = stroke_cir;
            copy.e = e;
            return base.Clone(copy, parentCop);
        }
        public override void Insert(FigureMy par = null)
        {
            base.Insert(par);

        }

        public override void Edit()
        {
            base.Edit();

        }

        public override void Draw()
        {

            double n_rad_x = (double)st_radius * scale * e;  
            double n_rad_y = (double)st_radius * scale;

            Canvas.SetLeft(cir, glob.X + center_loc.X - n_rad_x);
            Canvas.SetTop(cir, glob.Y + center_loc.Y - n_rad_y);
            cir.Width = n_rad_x * 2;
            cir.Height = n_rad_y * 2;
            cir.StrokeThickness = stroke_thickness_cir;
            cir.Fill = color;
            cir.Stroke = stroke_cir;

            cir.RenderTransformOrigin = new Point(0.5, 0.5);

            // Теперь вращаем вокруг центра эллипса
            RotateTransform rotateTransform = new RotateTransform(angle + dop_angle);
            cir.RenderTransform = rotateTransform;

            base.Draw();
        }
        public override void Update_borders()
        {
            double a = st_radius * scale * e; 
            double b = st_radius * scale; 

            Point center = new Point(glob.X + center_loc.X, glob.Y + center_loc.Y);

            double rad = (angle + dop_angle) * Math.PI / 180.0;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            double width_rotated = 2 * Math.Sqrt(a * a * cos * cos + b * b * sin * sin);
            double height_rotated = 2 * Math.Sqrt(a * a * sin * sin + b * b * cos * cos);

            b_p1 = new Point(center.X - width_rotated / 2, center.Y - height_rotated / 2);
            b_p2 = new Point(center.X + width_rotated / 2, center.Y + height_rotated / 2);

            base.Update_borders();
        }

        public void OnLMC(object sender, MouseEventArgs e)
        {
            dropping = true;
            // Сохраняем позицию мыши в момент нажатия
            lastMousePosition = e.GetPosition(canva);
            cir.CaptureMouse(); // Захватываем мышь для надежности
            SE.Select(this);
        }
        public void OnLMU(object sender, MouseEventArgs e) // Обработчик отпускания
        {
            dropping = false;
            cir.ReleaseMouseCapture();
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
            canva.Children.Remove(cir);
        }

        public override void Load(JsonElement el)
        {
            if (el.TryGetProperty("radius", out var r)) st_radius = r.GetDouble();
            if (el.TryGetProperty("stroke_thickness_cir", out var th)) stroke_thickness_cir = (int)th.GetDouble();
            if (el.TryGetProperty("e", out var ec)) e = ec.GetDouble();
            string color = FigureFactory.GetString(el, "stroke_cir");
            if (!string.IsNullOrEmpty(color))
            {
                try
                {
                    stroke_cir = (Brush)new BrushConverter().ConvertFromString(color);
                }
                catch
                {
                    stroke_cir = Brushes.Red;
                }
            }
        }

        public override void Save(FigureMy fig, Dictionary<string, object> dict)
        {
            dict["radius"] = st_radius;
            dict["stroke_thickness_cir"] = stroke_thickness_cir;
            dict["stroke_cir"] = FigureFactory.BrushToString(stroke_cir);
            dict["e"] = e;
        }
    }
}
