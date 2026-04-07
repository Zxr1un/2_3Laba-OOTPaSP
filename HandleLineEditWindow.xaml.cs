// HandleLineEditWindow.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;

namespace _2_3Laba
{
    public partial class HandleLineEditWindow : Window
    {
        public event Action<double, double, double, double> LineChanged;
        public event Action<double, double, double, double> LineApplied;

        private bool suppressEvent = false;

        public HandleLineEditWindow()
        {
            InitializeComponent();
        }

        public bool IsFocusedByUser => this.IsActive;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            // когда окно активно, флаг можно использовать
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
        }

        // Обновление значений из HandlePolygon
        public void UpdateValues(double length, double angle, double x2, double y2)
        {
            suppressEvent = true;
            LengthBox.Text = Math.Round(length, 2).ToString();
            AngleBox.Text = Math.Round(angle, 2).ToString();
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

            // Определяем источник изменения
            var box = sender as TextBox;
            if (box == LengthBox || box == AngleBox)
            {
                // Меняем длину/угол → пересчитываем конечную точку
                LineChanged?.Invoke(len, ang, 0, 0);
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

            LineApplied?.Invoke(len, ang, x, y);
        }
    }
}