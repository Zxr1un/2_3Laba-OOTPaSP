using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
        private double st_radius = 100;
        
        public int stroke_thickness_cir = 10;
        
        public Brush stroke_cir = Brushes.Red;
        

        public Circle()
        {
            
            name = SE.Get_nomber() + "_" + "Круг";
            type = "circle";
            canva = SE.canva;
            scale = 1;
            color = Brushes.Red;

            stroke_cir = Brushes.Blue;
            stroke_thickness_cir = 2;
            Point start_pos = SE.Get_center();

            
            canva.Children.Add(cir);
            canva.Children.Add(border);
            canva.Children.Add(CenterPoint);
            

            Move(start_pos.X, start_pos.Y);

            cir.MouseLeftButtonDown += OnLMC;
            cir.MouseLeftButtonUp += OnLMU;
            cir.MouseMove += MouseMoving;
            cir.MouseRightButtonDown += OnRMC;
            base_init();
        }

        public override FigureMy Clone(FigureMy part = null, FigureMy parentCop = null)
        {
            
            Circle copy = new Circle();
            copy.st_radius = st_radius;
            copy.stroke_thickness_cir = stroke_thickness_cir;
            copy.stroke_cir = stroke_cir;
            try
            {
                copy.canva.Children.Remove(copy.cir);
            }
            catch { }

            return base.Clone(copy, parentCop);
        }
        public override void Insert(FigureMy par = null)
        {
            canva.Children.Add(cir);
            base.Insert();
            cir.MouseLeftButtonDown += OnLMC;
            cir.MouseLeftButtonUp += OnLMU;
            cir.MouseMove += MouseMoving;
            cir.MouseRightButtonDown += OnRMC;

        }

        public override void Edit()
        {
            base.Edit();

        }

        public override void Draw()
        {
            
            double n_rad = (double)st_radius * scale;

            Canvas.SetLeft(cir, glob.X - n_rad);
            Canvas.SetTop(cir, glob.Y - n_rad);
            cir.Width = n_rad * 2;
            cir.Height = n_rad * 2;
            cir.StrokeThickness = stroke_thickness_cir;
            cir.Fill = color;
            cir.Stroke = stroke_cir;


            base.Draw();
        }
        public override void Update_borders()
        {
            b_p1 = new(glob.X - st_radius * scale, glob.Y - st_radius * scale);
            b_p2 = new(glob.X + st_radius * scale, glob.Y + st_radius * scale);
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
    }
}
