using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace _2_3Laba.Figures.Polygons
{
    public class Triangle: PolygonMy
    {
        public Triangle() {
            name = SE.Get_nomber() + "_" + "Треугольник";

            points.Add(new(-50, -28.87));
            points.Add(new(50, -28.87));
            points.Add(new(0, 57.74));

        }
    }
}
