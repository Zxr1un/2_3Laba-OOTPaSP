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
    public class RectangleMy: PolygonMy
    {
        public RectangleMy() {
            name = SE.Get_nomber() + "_" + "Прямоугольник";

            points.Add(new(-50, -50));
            points.Add(new(-50, 50));
            points.Add(new(50, 50));
            points.Add(new(50, -50));

        }
    }
}
