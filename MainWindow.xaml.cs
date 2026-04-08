using _2_3Laba.Figures;
using _2_3Laba.Figures.Polygons;
using System.Linq;
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
            this.WindowState = WindowState.Maximized;

        }


        private void CreateCircle_Click(object sender, RoutedEventArgs e)
        {
            Circle cir = new Circle();
            cir.base_init();
        }

        private void CreateTriangle_Click(object sender, RoutedEventArgs e)
        {
            Triangle tr = new Triangle();
            tr.base_init();
        }

        private void CreateSquare_Click(object sender, RoutedEventArgs e)
        {
            RectangleMy rec = new RectangleMy();
            rec.base_init();
        }

        private void CreatePolygon_Click(object sender, RoutedEventArgs e)
        {
            HandlePolygon hp = new HandlePolygon();
            hp.Start();
        }
        private void ClearScene_Click(object sender, RoutedEventArgs e)
        {
            SE.Scene.Delete();
            SE.UpdateHierarchy();
        }
        private void CreateTrapezioid_Click_Click(object sender, RoutedEventArgs e)
        {
            Trapezoid trapezoid = new Trapezoid();
            trapezoid.base_init();
        }

        private void CreatePentagon_Click(object sender, RoutedEventArgs e)
        {
            Pentagon pentagon = new Pentagon();
            pentagon.base_init();
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
                SF.base_init();
                List<FigureMy> copyList = SE.selected.ToList();
                for (int i = copyList.Count - 1; i >= 0; i--)
                {
                    for (int j = copyList.Count - 1; j >= 0; j--)
                    {
                        if (i != j && copyList[i].children.Contains(copyList[j]))
                        {
                            copyList.RemoveAt(j);
                        }
                    }
                }
                if (copyList.Count < 2)
                {
                    SF.Delete();
                    return;
                }

                foreach (FigureMy s in copyList)
                {
                    SF.AddFigure(s);
                }
                SE.DeselectAll();
                SE.Select(SF);
                
            }
        }

        private void Canva_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.OriginalSource == Canva)
            {
                SE.DeselectAll();
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        // Чтобы окно перетаскивалось за меню:
        private void Menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}