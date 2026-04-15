using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.IO;
using _2_3Laba.Figures.Polygons;

namespace _2_3Laba.Figures
{
    public static class FigureFactory
    {

        public static FigureMy FromJson(string json)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return ParseFigure(doc.RootElement, null);
        }
        private static FigureMy ParseFigure(JsonElement el, FigureMy parent)
        {
            string type = GetString(el, "type");

            FigureMy fig = CreateByType(type);

            fig.parent = parent;

            // Стандартное
            fig.type = type;
            fig.name = GetString(el, "name");

            fig.glob = GetPoint(el, "glob");
            fig.center_loc = GetPoint(el, "center_loc");

            fig.scale = GetDouble(el, "scale", 1);
            fig.angle = GetDouble(el, "angle", 0);
            fig.dop_angle = GetDouble(el, "dop_angle", 0);

            string color = GetString(el, "color");
            if (!string.IsNullOrEmpty(color))
            {
                try
                {
                    fig.color = (Brush)new BrushConverter().ConvertFromString(color);
                }
                catch
                {
                    fig.color = Brushes.Transparent;
                }
            }
            fig.Load(el);

            if (el.TryGetProperty("children", out var children) &&
                children.ValueKind == JsonValueKind.Array)
            {
                foreach (var child in children.EnumerateArray())
                {
                    var ch = ParseFigure(child, fig);
                    fig.children.Add(ch);
                    ch.parent = fig;
                }
            }

            return fig;
        }
        private static FigureMy CreateByType(string type)
        {
            return type switch
            {
                "circle" => new Circle(),
                "polygon" => new PolygonMy(),
                "superfigure" => new SuperFigure(),
                "side" => new Side(null, new(0,0), new(0,0)),
                "scene" => new AllFigures(),
                _ => new FigureMy()
            };
        }


        // Вспомогательное
        public static string GetString(JsonElement el, string name)
        {
            return el.TryGetProperty(name, out var v)
                ? v.GetString()
                : null;
        }

        public static double GetDouble(JsonElement el, string name, double def = 0)
        {
            return el.TryGetProperty(name, out var v)
                ? v.GetDouble()
                : def;
        }

        public static Point GetPoint(JsonElement el, string name)
        {
            if (!el.TryGetProperty(name, out var v))
                return new Point(0, 0);

            double x = v.TryGetProperty("X", out var px) ? px.GetDouble() : 0;
            double y = v.TryGetProperty("Y", out var py) ? py.GetDouble() : 0;

            return new Point(x, y);
        }

        public static string BrushToString(Brush brush)
        {
            if (brush is SolidColorBrush scb)
                return scb.Color.ToString();

            return "Transparent";
        }


        public static FigureMy Load(string json)
        {
            FigureMy fig = FromJson(json);
            if(!(fig is AllFigures)) fig.Insert();
            return fig;
        }




        public static void SaveToFile(FigureMy root, string path)
        {
            var obj = SerializeFigure(root);

            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }

        private static object SerializeFigure(FigureMy fig)
        {
            var dict = new Dictionary<string, object>();

            dict["type"] = fig.type;
            dict["name"] = fig.name;

            dict["glob"] = new { fig.glob.X, fig.glob.Y };
            dict["center_loc"] = new { fig.center_loc.X, fig.center_loc.Y };

            dict["scale"] = fig.scale;
            dict["angle"] = fig.angle;
            dict["dop_angle"] = fig.dop_angle;

            dict["color"] = BrushToString(fig.color);

            // 🔥 ВАЖНО: специфичные поля
            FillCustomFields(fig, dict);

            // children
            var children = new List<object>();
            foreach (var ch in fig.children)
                children.Add(SerializeFigure(ch));

            dict["children"] = children;

            return dict;
        }

        private static void FillCustomFields(FigureMy fig, Dictionary<string, object> dict)
        {
            if (fig is Circle c)
            {
                c.Save(fig, dict);
                //dict["radius"] = c.st_radius;
                //dict["stroke_thickness_cir"] = c.stroke_thickness_cir;
                //dict["stroke_cir"] = BrushToString(c.stroke_cir);
                //dict["e"] = c.e;
            }

            if (fig is Side s)
            {
                s.Save(fig, dict);
                //dict["index"] = s.Index;

                //dict["glob_2"] = new { s.glob_2.X, s.glob_2.Y };

                //dict["thickness"] = s.thickness;
                //dict["color_s"] = BrushToString(s.color_s);
            }

            else if (fig is PolygonMy p)
            {
                p.Save(fig, dict);
                //dict["points"] = p.points;
            }

            

            if (fig is SuperFigure)
            {
                // пока пусто
            }
        }

    }

}