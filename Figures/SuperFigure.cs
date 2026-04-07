using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace _2_3Laba.Figures
{
    public class SuperFigure: FigureMy
    {
        public SuperFigure() {
            if (this is AllFigures) return;
            border.Stroke = Brushes.DarkCyan;
            base_init();
            canva.Children.Add(border);
            canva.Children.Add(CenterPoint);
            
        }
        public override void setScale(double new_scale)
        {
            double prev_scale = scale;
            scale = new_scale;
            double delta = scale / prev_scale;
            foreach (FigureMy ch in children)
            {
                ch.glob = getGlobal(new (getLocal(ch.glob).X * delta, getLocal(ch.glob).Y * delta));
                ch.setScale(ch.scale * delta);
            }
            base.setScale(new_scale);
        }

       

        public void AddFigure(FigureMy figure = null)
        {

            double av_x = 0;
            double av_y = 0;
            if (figure != null) {
                FigureMy par = figure.parent;
                if (par != null) par.children.Remove(figure);
                figure.parent = this;
                children.Add(figure);
                SE.UpdateHierarchy();
            }
            foreach(FigureMy ch in children)
            {
                av_x += ch.glob.X;
                av_y += ch.glob.Y;
            }
            av_x = av_x / children.Count;
            av_y = av_y / children.Count;
            glob.X = av_x;
            glob.Y = av_y;
            Update_borders();
            if (children.Count == 0) Delete();
            Move();
        }
        public override void Update_borders()
        {
            if (children.Count == 0) return;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (FigureMy ch in children)
            {
                if (ch.b_p1.X < minX) minX = ch.b_p1.X;
                if (ch.b_p1.Y < minY) minY = ch.b_p1.Y;
                if (ch.b_p2.X > maxX) maxX = ch.b_p2.X;
                if (ch.b_p2.Y > maxY) maxY = ch.b_p2.Y;
            }

            b_p1 = new Point(minX, minY);
            b_p2 = new Point(maxX, maxY);

            base.Update_borders();
        }

        private Point RotatePoint(Point p, Point center, double deltaAngle)
        {
            double rad = deltaAngle * Math.PI / 180.0;
            double s = Math.Sin(rad);
            double c = Math.Cos(rad);

            // переводим точку в систему координат центра
            double x = p.X - center.X;
            double y = p.Y - center.Y;

            // вращение
            double xNew = x * c - y * s;
            double yNew = x * s + y * c;

            return new Point(xNew + center.X, yNew + center.Y);
        }
        public void Rotate(double newAngle)
        {
            List<Point> loc_points = new List<Point>();
            foreach(FigureMy ch in children)
            {
                Point old_loc = getLocal(ch.glob);
                loc_points.Add(old_loc);
            }
            angle = newAngle;
            for(int i = 0;  i < loc_points.Count; i++)
            {
                if (children[i] is SuperFigure SF)
                {
                    SF.dop_angle = newAngle;
                    Point n_p = getGlobal(loc_points[i]);
                    double d_x = n_p.X - SF.glob.X, d_y = n_p.Y - SF.glob.Y;
                    SF.d_Move(d_x, d_y);
                    SF.Rotate(SF.angle);
                    continue;
                }
                children[i].dop_angle = angle + dop_angle;
                children[i].glob = getGlobal(loc_points[i]);
                children[i].Move();
            }
            Update_borders();
            Draw();
        }


    }
}
