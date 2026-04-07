using _2_3Laba.Figures;
using _2_3Laba.Figures.Polygons;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2_3Laba
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SE.MW = this;
            InitializeComponent();
            SE.canva = Canva;
            SE.Register(HierarchyTree);
            HierarchyTree.MouseDoubleClick += HierarchyTree_MouseDoubleClick;

        }


        private void CreateCircle_Click(object sender, RoutedEventArgs e)
        {
            Circle cir = new Circle();
        }

        private void CreateTriangle_Click(object sender, RoutedEventArgs e)
        {
            Triangle tr = new Triangle();
        }

        private void CreateSquare_Click(object sender, RoutedEventArgs e)
        {
            RectangleMy rec = new RectangleMy();
        }

        private void CreatePolygon_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ClearScene_Click(object sender, RoutedEventArgs e)
        {
            SE.Scene.Delete();
            SE.UpdateHierarchy();
        }
        private void CreateTrapezioid_Click_Click(object sender, RoutedEventArgs e)
        {
            Trapezoid trapezoid = new Trapezoid();
        }

        private void CreatePentagon_Click(object sender, RoutedEventArgs e)
        {
            Pentagon pentagon = new Pentagon();
        }

        private void SaveScene_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadScene_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HierarchyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (HierarchyTree.SelectedItem is TreeViewItem item &&
                item.Tag is FigureMy fig)
            {
                SE.Select(fig);
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

        private void Unite_Click(object sender, RoutedEventArgs e)
        {
            if(SE.selected.Count > 1)
            {
                SuperFigure SF = new();
                foreach (FigureMy s in SE.selected.ToList())
                {
                    SF.AddFigure(s);
                }
                SF.name = SE.Get_nomber() + "_" + "Объединение";
                SE.DeselectAll();
                SE.Select(SF);
                
            }
        }
    }
}