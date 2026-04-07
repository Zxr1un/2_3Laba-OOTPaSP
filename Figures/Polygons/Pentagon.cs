using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace _2_3Laba.Figures.Polygons
{
    internal class Pentagon: PolygonMy
    {
        public Pentagon()
        {
            name = SE.Get_nomber() + "_" + "Пятиугольник";
            scale = 1.0;
            poly = new Polygon()
            {
                Stroke = Brushes.Transparent,
                Fill = Brushes.Transparent,
                StrokeThickness = 0
            };
            points.Add(new(0, -85.065));
            points.Add(new(80.90, -26.30));
            points.Add(new(50, 68.82));
            points.Add(new(-50, 68.82));
            points.Add(new(-80.9, -26.3));
            base_init();
        }
    }
}
