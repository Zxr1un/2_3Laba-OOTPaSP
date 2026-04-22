// HandlePolygon.cs
using _2_3Laba.Figures;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2_3Laba.Figures
{
    public class HandlePolygon : FigureMy
    {
        private List<Point> points = new();
        private Line currentLine = null;
        private List<Line> lines = new();
        private Canvas canvas;
        private bool isDrawing = false;
        private HandleLineEditWindow lineEditWindow = null;

        public double dop_angle = 0;       // глобальный угол последней линии
        private List<double> angles = new List<double>();
        public double angle_cur = 0;

        public HandlePolygon()
        {
            canvas = SE.canva;
        }

        public void Start()
        {
            Point start = new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2);
            points.Add(start);

            currentLine = CreateLine(start, start);
            canvas.Children.Add(currentLine);

            isDrawing = true;

            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
            canvas.KeyDown += Canvas_KeyDown;

            canvas.Focusable = true;
            canvas.Focus();

            lineEditWindow = new HandleLineEditWindow
            {
                Owner = Application.Current.MainWindow,
                Left = canvas.PointToScreen(start).X + 20,
                Top = canvas.PointToScreen(start).Y + 20
            };
            lineEditWindow = new HandleLineEditWindow
            {
                Owner = Application.Current.MainWindow,
                ParentPolygon = this, // <--- ссылка на HandlePolygon
                Left = canvas.PointToScreen(start).X + 20,
                Top = canvas.PointToScreen(start).Y + 20
            };

            lineEditWindow.LineApplied += ApplyLineFromWindow;
            lineEditWindow.LineChanged += (len, ang, x2, y2) =>
            {
                UpdateLineFromWindow(len, ang, x2, y2, fromWindow: true);
            };

            lineEditWindow.Show();
        }

        private Line CreateLine(Point p1, Point p2)
        {
            return new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y
            };
        }

        // ================= ПЕРЕСЕЧЕНИЯ =================
        private double Cross(Point a, Point b, Point c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        private bool OnSegment(Point a, Point b, Point p)
        {
            return Math.Min(a.X, b.X) <= p.X && p.X <= Math.Max(a.X, b.X) &&
                   Math.Min(a.Y, b.Y) <= p.Y && p.Y <= Math.Max(a.Y, b.Y);
        }

        private bool LinesIntersect(Point a1, Point a2, Point b1, Point b2)
        {
            double d1 = Cross(a1, a2, b1);
            double d2 = Cross(a1, a2, b2);
            double d3 = Cross(b1, b2, a1);
            double d4 = Cross(b1, b2, a2);

            if ((d1 > 0 && d2 < 0 || d1 < 0 && d2 > 0) &&
                (d3 > 0 && d4 < 0 || d3 < 0 && d4 > 0))
                return true;

            if (d1 == 0 && OnSegment(a1, a2, b1)) return true;
            if (d2 == 0 && OnSegment(a1, a2, b2)) return true;
            if (d3 == 0 && OnSegment(b1, b2, a1)) return true;
            if (d4 == 0 && OnSegment(b1, b2, a2)) return true;

            return false;
        }

        private bool HasIntersection(Point last, Point newPoint)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (i == points.Count - 2) continue;
                if (LinesIntersect(last, newPoint, points[i], points[i + 1]))
                    return true;
            }
            return false;
        }

        // ================= СОБЫТИЯ =================
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;
            Point pos = e.GetPosition(canvas);
            UpdateLineFromWindow(0, 0, pos.X, pos.Y, fromWindow: false);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing) return;

            Point pos = e.GetPosition(canvas);
            Point last = points[^1];

            if (HasIntersection(last, pos))
            {
                MessageBox.Show("Пересечение запрещено!");
                return;
            }


            points.Add(pos);
            if (points.Count > 1) lines.Add(currentLine);

            currentLine = CreateLine(pos, pos);
            canvas.Children.Add(currentLine);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (points.Count <= 1) return;

            points.RemoveAt(points.Count - 1);

            if (lines.Count > 0)
            {
                Line lastLine = lines[^1];
                canvas.Children.Remove(lastLine);
                lines.RemoveAt(lines.Count - 1);
            }

            // восстанавливаем dop_angle предыдущей линии
            if (angles.Count > 0)
            {
                dop_angle = angles[^2]; // последний сохранённый угол
                angles.RemoveAt(angles.Count - 1);
            }
            else dop_angle = 0;

            Point last = points[^1];
            currentLine.X1 = last.X;
            currentLine.Y1 = last.Y;
        }

        public void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isDrawing) return;

            if (e.Key == Key.Escape)
            {
                foreach (var line in lines) canvas.Children.Remove(line);
                canvas.Children.Remove(currentLine);
                lines.Clear();
                points.Clear();
                angles.Clear();
                isDrawing = false;
                DetachEvents();
            }
            else if (e.Key == Key.Space && points.Count > 2)
            {
                FinishPolygonWithSpace();
            }
        }

        // ================= ОБНОВЛЕНИЕ ЛИНИИ =================
        private void UpdateLineFromWindow(double length, double angle, double x2, double y2, bool fromWindow)
        {
            if (!isDrawing || points.Count == 0) return;

            Point start = points[^1];
            Point end;

            // угол последней линии
            double lastAngleRad = dop_angle * Math.PI / 180.0;

            if (!fromWindow) // мышь
            {
                end = new Point(x2, y2);

                Vector newVec = end - start;
                length = newVec.Length;

                double newAngleRad = Math.Atan2(newVec.Y, newVec.X);
                angle = (newAngleRad * 180.0 / Math.PI); // глобальный угол

                // угол относительно последней линии
                angle = angle - dop_angle;
                angle = (angle + 360) % 360; // нормализация
                if (angle > 180) angle = 360 - angle; // острый угол
            }
            else // окно
            {
                if (x2 != 0 || y2 != 0)
                {
                    end = new Point(x2, y2);

                    Vector newVec = end - start;
                    length = newVec.Length;

                    double newAngleRad = Math.Atan2(newVec.Y, newVec.X);
                    angle = (newAngleRad * 180.0 / Math.PI); // глобальный угол
                    angle = angle - dop_angle;
                    angle = (angle + 360) % 360;
                    if (angle > 180) angle = 360 - angle;
                }
                else
                {
                    double rad = (dop_angle + angle) * Math.PI / 180.0;
                    end = new Point(start.X + length * Math.Cos(rad),
                                    start.Y + length * Math.Sin(rad));
                }
            }

            currentLine.X1 = start.X;
            currentLine.Y1 = start.Y;
            currentLine.X2 = end.X;
            currentLine.Y2 = end.Y;

            lineEditWindow?.UpdateValues(length, angle, end.X, end.Y);

            angle_cur = angle; // сохраняем текущий угол для следующей точки
        }

        private void ApplyLineFromWindow(double length, double angle, double x2, double y2)
        {
            UpdateLineFromWindow(length, angle, x2, y2, fromWindow: true);


            Point end = new Point(currentLine.X2, currentLine.Y2);
            Point last = points[^1];

            if (HasIntersection(last, end))
            {
                MessageBox.Show("Пересечение запрещено!");
                return;
            }

            Vector newVec = end - last;
            dop_angle = Math.Atan2(newVec.Y, newVec.X) * 180.0 / Math.PI;

            points.Add(end);
            lines.Add(currentLine);
            angles.Add(dop_angle);
            currentLine = CreateLine(end, end);
            canvas.Children.Add(currentLine);

        }

        private void DetachEvents()
        {
            canvas.MouseMove -= Canvas_MouseMove;
            canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
            canvas.MouseRightButtonDown -= Canvas_MouseRightButtonDown;
            canvas.KeyDown -= Canvas_KeyDown;

            if (lineEditWindow != null)
            {
                lineEditWindow.Close();
                lineEditWindow = null;
            }
        }

        public void FinishPolygonWithSpace()
        {
            if (!isDrawing || points.Count <= 2) return;



            Point g_c = SE.Get_center();
            PolygonMy poly = new PolygonMy();
            poly.name = SE.Get_nomber() + "_" + "Своя фигура";

            foreach (var p in points)
                poly.points.Add(new Point(p.X - g_c.X, p.Y - g_c.Y));

            poly.base_init();

            foreach (var line in lines) canvas.Children.Remove(line);
            canvas.Children.Remove(currentLine);
            lines.Clear();
            points.Clear();
            angles.Clear();

            isDrawing = false;
            DetachEvents();
        }
    }

    
}