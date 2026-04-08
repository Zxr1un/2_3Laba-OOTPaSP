// HandleLineEditWindow.xaml.cs
using _2_3Laba.Figures;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _2_3Laba
{
    public partial class HandleLineEditWindow : Window
    {
        public event Action<double, double, double, double> LineChanged;
        public event Action<double, double, double, double> LineApplied;

        private bool suppressEvent = false;

        public HandlePolygon ParentPolygon { get; set; }

        public HandleLineEditWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += HandleLineEditWindow_PreviewKeyDown;
        }

        private void HandleLineEditWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // чтобы окно не обработало Space
                ParentPolygon?.FinishPolygonWithSpace();
            }
        }

        public bool IsFocusedByUser => this.IsActive;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
        }

        // Обновление значений из HandlePolygon
        public void UpdateValues(double length, double angle, double x2, double y2)
        {
            suppressEvent = true;

            // Показываем острый угол
            double displayAngle = Math.Abs(180 - Math.Abs(angle));

            LengthBox.Text = Math.Round(length, 2).ToString();
            AngleBox.Text = Math.Round(displayAngle, 2).ToString();
            XBox.Text = Math.Round(x2, 2).ToString();
            YBox.Text = Math.Round(y2, 2).ToString();

            suppressEvent = false;
        }

        private void ValueChanged(object sender, TextChangedEventArgs e)
        {
            if (suppressEvent) return;

            if (!double.TryParse(LengthBox.Text, out double len)) return;
            if (!double.TryParse(AngleBox.Text, out double ang)) return;
            if (!double.TryParse(XBox.Text, out double x)) return;
            if (!double.TryParse(YBox.Text, out double y)) return;

            // Преобразуем обратно в глобальный угол (для HandlePolygon)
            double angleForPolygon = 180 - ang;

            var box = sender as TextBox;
            if (box == LengthBox || box == AngleBox)
            {
                // Меняем длину/угол → пересчитываем конечную точку
                LineChanged?.Invoke(len, angleForPolygon, 0, 0);
            }
            else
            {
                // Меняем координаты → пересчитываем длину/угол
                LineChanged?.Invoke(0, 0, x, y);
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(LengthBox.Text, out double len)) return;
            if (!double.TryParse(AngleBox.Text, out double ang)) return;
            if (!double.TryParse(XBox.Text, out double x)) return;
            if (!double.TryParse(YBox.Text, out double y)) return;

            double angleForPolygon = 180 - ang;

            LineApplied?.Invoke(len, angleForPolygon, x, y);
        }
    }
}