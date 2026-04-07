using _2_3Laba.Figures;
using _2_3Laba.Figures.Polygons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _2_3Laba
{
    /// <summary>
    /// Логика взаимодействия для RedWindow.xaml
    /// </summary>
    public partial class RedWindow : Window
    {
        public bool IsPolygon = true;
        public bool IsSuperFigure = false;
        public FigureMy figure = null;

        public Circle circle = null;
        public PolygonMy polygonMy = null;
        public SuperFigure SF = null;

        private int selectedSideIndex = -1;
        private bool isUpdatingUI = false;
        public RedWindow(FigureMy figure)
        {
            this.figure = figure;
            InitializeComponent();
            
            FillColorComboBox.ItemsSource = Enum.GetValues(typeof(FigureColor));
            Circle_LineColorComboBox.ItemsSource = Enum.GetValues(typeof(FigureColor));
            SideColor.ItemsSource = Enum.GetValues(typeof(FigureColor));

            SE.Register(HierarchyTree);

            if (figure is PolygonMy polyg)
            {
                polygonMy = polyg;
                SideChoose.Items.Clear();
                for (int i = 0; i < polygonMy.points.Count; i++) SideChoose.Items.Add($"Сторона {i}");
                // ВАЖНО: сразу выбрать первую сторону
                if (SideChoose.Items.Count > 0)  SideChoose.SelectedIndex = 0;
                FillColorComboBox.IsEnabled = true;
                Circle_LineColorComboBox.IsEnabled = false;
                Circle_Thickness.IsEnabled = false;
                if (polygonMy.points.Count > 0) SideChoose.SelectedIndex = 0;
            }

            if (figure is Circle cir)
            {
                circle = cir;
                SideChoose.IsEnabled = false;
                SideColor.IsEnabled = false;
                SideThickness.IsEnabled = false;
                X1_box.IsEnabled = false;
                X2_box.IsEnabled = false;
                Y1_box.IsEnabled = false;
                Y2_box.IsEnabled = false;
                LengthBox.IsEnabled = false;
                AngleBox.IsEnabled = false;
            }
            if(figure is AllFigures scene)
            {
                X_cord_glob.IsEnabled = false;
                Y_cord_glob.IsEnabled =false;
                X_centr_loc.IsEnabled = false;
                Y_centr_loc.IsEnabled = false;
                Scale_box.IsEnabled = true;
                FillColorComboBox.IsEnabled = false;
                Circle_LineColorComboBox.IsEnabled = false;
                Circle_Thickness.IsEnabled = false;
                SideChoose.IsEnabled = false;
                SideColor.IsEnabled = false;
                SideThickness.IsEnabled = false;
                X1_box.IsEnabled = false;
                X2_box.IsEnabled = false;
                Y1_box.IsEnabled = false;
                Y2_box.IsEnabled = false;
                LengthBox.IsEnabled = false;
                AngleBox.IsEnabled = false;
            }

            UpdateData();
        }

        public void UpdateData()
        {
            Figure_Name.Text = figure.name;
            FigureMy par = figure.parent;
            Borders_Values.Content = "Границы (В.Л и Н.П.): " + $"({figure.b_p1.X.ToString("F0")},{figure.b_p1.Y.ToString("F0")}) и ({figure.b_p2.X.ToString("F0")},{figure.b_p2.Y.ToString("F0")})";
            if(par != null)
            {
                X_cord_glob.Text = par.getLocal(figure.glob).X.ToString("F0");
                Y_cord_glob.Text = par.getLocal(figure.glob).Y.ToString("F0");
            }
            else
            {
                X_cord_glob.Text = figure.glob.X.ToString("F0");
                Y_cord_glob.Text = figure.glob.Y.ToString("F0");
            }

            if(polygonMy != null)
            {
                FillColorComboBox.SelectedItem = polygonMy.color.FromBrush();
            }
                
            X_centr_loc.Text = figure.center_loc.X.ToString("F0");
            Y_centr_loc.Text = figure.center_loc.Y.ToString("F0");
            Scale_box.Text = figure.scale.ToString("F3");
            Angle_box.Text = figure.angle.ToString("F2");



            if (circle != null)
            {
                FillColorComboBox.SelectedItem = figure.color.FromBrush();
                Circle_Thickness.Text = circle.stroke_thickness_cir.ToString("F0");
                Circle_LineColorComboBox.SelectedItem = circle.stroke_cir.FromBrush();
            }
        }

        private void UpdateSideUI()
        {
            if (selectedSideIndex < 0) return;

            isUpdatingUI = true;

            int i = selectedSideIndex;
            int next = (i + 1) % polygonMy.points.Count;

            Point p1 = polygonMy.points[i];
            Point p2 = polygonMy.points[next];

            X1_box.Text = p1.X.ToString("F0");
            Y1_box.Text = p1.Y.ToString("F0");

            X2_box.Text = p2.X.ToString("F0");
            Y2_box.Text = p2.Y.ToString("F0");

            var side = polygonMy.sides[i];
            SideThickness.Text = side.thickness.ToString();
            SideColor.SelectedItem = side.color.FromBrush();

            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            LengthBox.Text = Math.Sqrt(dx * dx + dy * dy).ToString("F2");
            AngleBox.Text = (Math.Atan2(dy, dx) * 180 / Math.PI).ToString("F2");

            isUpdatingUI = false;
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (figure.parent != null)
                figure.parent.children.Remove(figure);
            SE.Scene.children.Remove(figure); // если верхнего уровня
            figure.canva.Children.Clear(); // удалить визуальные элементы
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            figure.Draw(); // просто обновляем фигуру
        }

        private void ExcludeButton_Click(object sender, RoutedEventArgs e)
        {
            if (figure.parent != null)
            {
                figure.parent.children.Remove(figure);
                SE.Scene.children.Add(figure); // поднимаем наверх
                figure.parent = null;
            }
            this.Close();
        }


        private void SideChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonMy == null) return;

            selectedSideIndex = SideChoose.SelectedIndex;
            if (selectedSideIndex < 0) return;

            HighlightSelectedSide();
            UpdateSideUI();
        }
        private void HighlightSelectedSide()
        {
            if (polygonMy == null) return;

            for (int i = 0; i < polygonMy.sides.Count; i++)
            {
                if (i == selectedSideIndex)
                    polygonMy.sides[i].Select();
                else
                    polygonMy.sides[i].Deselect();
            }
        }
        public void SelectSide(int index)
        {
            if (polygonMy == null) return;
            if (index < 0 || index >= polygonMy.sides.Count) return;

            SideChoose.SelectedIndex = index;
        }


        private void SideThickness_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingUI) return;
            if (selectedSideIndex < 0) return;

            try
            {
                polygonMy.sides[selectedSideIndex].thickness =
                    Convert.ToInt32(SideThickness.Text);

                polygonMy.Draw();
            }
            catch { }
        }
        private void SideColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdatingUI) return;
            if (selectedSideIndex < 0) return;

            if (SideColor.SelectedItem is FigureColor color)
            {
                polygonMy.sides[selectedSideIndex].color = color.ToBrush();
                polygonMy.Draw();
            }
        }

        private void Coord_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingUI) return;
            if (selectedSideIndex < 0) return;

            try
            {
                int i = selectedSideIndex;
                int next = (i + 1) % polygonMy.points.Count;

                polygonMy.points[i] = new Point(
                    Convert.ToDouble(X1_box.Text),
                    Convert.ToDouble(Y1_box.Text));

                polygonMy.points[next] = new Point(
                    Convert.ToDouble(X2_box.Text),
                    Convert.ToDouble(Y2_box.Text));

                polygonMy.Draw();
                UpdateSideUI();
            }
            catch { }
        }
        private void UpdateByPolar()
        {
            if (isUpdatingUI) return;
            if (selectedSideIndex < 0) return;

            try
            {
                
                int i = selectedSideIndex;
                int next = (i + 1) % polygonMy.points.Count;

                Point p1 = polygonMy.points[i];

                double length = Convert.ToDouble(LengthBox.Text);
                double angle = Convert.ToDouble(AngleBox.Text) * Math.PI / 180;

                polygonMy.points[next] = new Point(
                    p1.X + length * Math.Cos(angle),
                    p1.Y + length * Math.Sin(angle)
                );

                polygonMy.Draw();
                UpdateSideUI();
            }
            catch { }
        }
        private void LengthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingUI) return;
            if (selectedSideIndex < 0) return;
            UpdateByPolar();
        }
        private void AngleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingUI) return;
            if (selectedSideIndex < 0) return;
            UpdateByPolar();
        }



        private void FillColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (figure == null) return;
            if (FillColorComboBox.SelectedItem is FigureColor selectedColor)
            {
                // для круга и многоугольника
                if (figure is Circle circle) circle.color = selectedColor.ToBrush();
                else if (figure is PolygonMy poly) poly.color = selectedColor.ToBrush();

                figure.Draw();
            }
        }
        private void Circle_LineColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (figure == null) return;
            if (Circle_LineColorComboBox.SelectedItem is FigureColor selectedColor)
            {
                // для круга и многоугольника
                if (figure is Circle circle) circle.stroke_cir = selectedColor.ToBrush();
                figure.Draw();
            }
        }


        private void Figure_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Figure_Name.Text.Length > 0) {
                figure.name = Figure_Name.Text;
                SE.UpdateHierarchy();
            }
        }

        private void X_cord_glob_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                FigureMy par = figure.parent;
                double newX = Convert.ToDouble(X_cord_glob.Text);
                double dx;

                if (par != null)
                {
                    // рассчитываем смещение относительно текущей глобальной позиции
                    Point local = par.getLocal(figure.glob);
                    Point targetGlobal = par.getGlobal(new Point(newX, local.Y));
                    dx = targetGlobal.X - figure.glob.X;
                }
                else
                {
                    dx = newX - figure.glob.X;
                }

                // смещаем фигуру
                figure.d_Move(dx, 0);
            }
            catch { }
        }

        private void Y_cord_glob_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                FigureMy par = figure.parent;
                double newY = Convert.ToDouble(Y_cord_glob.Text);
                double dy;

                if (par != null)
                {
                    // рассчитываем смещение относительно текущей глобальной позиции
                    Point local = par.getLocal(figure.glob);
                    Point targetGlobal = par.getGlobal(new Point(local.X, newY));
                    dy = targetGlobal.Y - figure.glob.Y;
                }
                else
                {
                    dy = newY - figure.glob.Y;
                }

                // смещаем фигуру
                figure.d_Move(0, dy);
            }
            catch { }
        }

        private void X_centr_loc_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                figure.center_loc.X = Convert.ToInt16(X_centr_loc.Text);
                figure.Move();
            }
            catch { }
        }
        private void Y_centr_loc_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                figure.center_loc.Y = Convert.ToInt16(Y_centr_loc.Text);
                figure.Move();
            }
            catch { }
        }

        private void Scale_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Convert.ToDouble(Scale_box.Text) < 0.0001) return;
                figure.setScale(Convert.ToDouble(Scale_box.Text));
                //figure.scale = Convert.ToDouble(Scale_box.Text);
                //figure.Move();
            }
            catch { }
        }



        private void X_cord_glob_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
        private void Y_cord_glob_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
        private void X_centr_loc_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
        private void Y_centr_loc_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void Scale_box_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9,]+$");
        }
        private void Circle_Thickness_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if(figure is Circle cir)
                {
                    cir.stroke_thickness_cir = Convert.ToInt32(Circle_Thickness.Text);
                    cir.Move();
                }
                
            }
            catch {}
        }

        private void Circle_Thickness_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
        private void Angle_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (figure is SuperFigure sf)
                    sf.Rotate(Convert.ToDouble(Angle_box.Text));
                else
                {
                    figure.angle = Convert.ToDouble(Angle_box.Text);
                    figure.Move();
                }
            }
            catch { }
        }
        private void Angle_box_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9\\-]+$");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            figure.RW = null;
            if(polygonMy  != null)
            {
                foreach(Side s in polygonMy.sides)
                {
                    s.Deselect();
                }
            }
            SE.UnRegister(HierarchyTree);
        }

        private void DeleteButton_Click_1(object sender, RoutedEventArgs e)
        {
            
            Close();
            figure.Delete();
            figure.RW = null;
            if (polygonMy != null)
            {
                foreach (Side s in polygonMy.sides)
                {
                    s.Deselect();
                }
            }
        }

        private void HierarchyTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HierarchyTree.SelectedItem is TreeViewItem item)
            {
                if (item.Tag is FigureMy fig)
                {
                    fig.Edit();
                }
            }
        }

        private void RedirectToParent_Click(object sender, RoutedEventArgs e)
        {
            if (figure.parent != null) figure.parent.Edit();
        }

        private void ExitGroup_Click(object sender, RoutedEventArgs e)
        {
            figure.Rejection();
        }

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            SE.SaveFigure(figure);
        }
    }

    public enum FigureColor
    {
        Transparent,
        Black,
        White,
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Magenta,
        Gray,
        DarkGray,
        LightGray,
        Brown,
        Orange,
        Purple,
        Pink
    }

    public static class FigureColorExtensions
    {
        public static Brush ToBrush(this FigureColor color)
        {
            return (Brush)new BrushConverter().ConvertFromString(color.ToString());
        }

        public static FigureColor FromBrush(this Brush brush)
        {
            if (brush is SolidColorBrush solid)
            {
                string name = solid.Color.ToString();

                foreach (FigureColor fc in Enum.GetValues(typeof(FigureColor)))
                {
                    var fcBrush = fc.ToBrush() as SolidColorBrush;
                    if (fcBrush != null && fcBrush.Color == solid.Color)
                        return fc;
                }
            }

            return FigureColor.Transparent;
        }
    }

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FigureColor color)
            {
                if (color == FigureColor.Transparent)
                    return Brushes.Transparent;

                return color.ToBrush();
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }


}



