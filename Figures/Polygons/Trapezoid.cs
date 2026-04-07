using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2_3Laba.Figures.Polygons
{
    public class Trapezoid: PolygonMy
    {
        public Trapezoid() {
            name = SE.Get_nomber() + "_" + "Трапеция";
            points.Add(new(-50, -50));
            points.Add(new(-25, 50));
            points.Add(new(25, 50));
            points.Add(new(50, -50));

        }
    }
}
