using _2_3Laba.Figures;
using _2_3Laba.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _2_3Laba
{
    //Класс сцены
    public class AllFigures: SuperFigure
    {
        public AllFigures() {
            name = "Сцена";
        }
       
        public override void Delete()
        {
            base.Delete();
        }
        public override void Select()
        {
            
        }
        public override void Deselect()
        {

        }
    } 
    //Static Elements
    internal class SE
    {
        public static int counter = 0;
        public static MainWindow MW;
        public static Canvas canva = null;
        public static List<FigureMy> selected = new();
        public static AllFigures Scene = new();
        public static List<TreeView> HierarchyTreeColl = new();

        public static List<FigureMy> Saved = new();


        public static Point Get_center()
        {
            double x = canva.ActualWidth / 2;
            double y = canva.ActualHeight / 2;
            return new Point(x, y);
        }

        public static void Select(FigureMy fig, bool isRepeat = false)
        {
            if(selected.Count != 0 && !Keyboard.IsKeyDown(Key.LeftShift) && !isRepeat)
            {
                foreach (FigureMy f in selected) f.Deselect();
                selected.Clear();
                MW.Unite.IsEnabled = false;
            }
            if (fig.parent != null) {
                if(fig.parent is AllFigures)
                {
                    if (selected.Contains(fig)) return;
                    if (fig is AllFigures) return;
                    selected.Add(fig);
                    fig.Select();
                }
                else
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        if (!isRepeat)
                        {
                            foreach (FigureMy f in selected) f.Deselect();
                            selected.Clear();
                            MW.Unite.IsEnabled = false;
                        }
                        if (fig is AllFigures) return;
                        selected.Add(fig);
                        fig.Select();
                        Select(fig.parent, true);
                    }
                    else
                    {
                        SE.Select(fig.parent);
                    }
                }
                
            }
            
            
            if(selected.Count > 1) MW.Unite.IsEnabled = true;
        }
        public static void DeselectAll()
        {
            foreach (FigureMy f in selected) f.Deselect();
            selected.Clear();
        }
        public static string Get_nomber()
        {
            counter++;
            return counter.ToString();
        }

        
        public static void Register(TreeView Tree)
        {
            HierarchyTreeColl.Add(Tree);
            SE.UpdateHierarchy();
        }
        public static void UnRegister(TreeView Tree)
        {
            HierarchyTreeColl.Remove(Tree);
            SE.UpdateHierarchy();
        }
        public static void UpdateHierarchy()
        {
            foreach (TreeView HierarchyTree in HierarchyTreeColl)
            {
                if (HierarchyTree == null) return;
                // Если дерево пустое — создаём корень
                if (HierarchyTree.Items.Count == 0)
                {
                    TreeViewItem root = new TreeViewItem
                    {
                        Header = SE.Scene.name,
                        Tag = SE.Scene,
                        IsExpanded = true
                    };
                    HierarchyTree.Items.Add(root);
                }

                // Синхронизируем дерево с текущей сценой
                SyncTree((TreeViewItem)HierarchyTree.Items[0], SE.Scene);
            }
            foreach (TreeView HierarchyTree in HierarchyTreeColl)
            {
                if (HierarchyTree == null) HierarchyTreeColl.Remove(HierarchyTree);
            }
        }
        private static void SyncTree(TreeViewItem parentNode, FigureMy sceneNode)
        {
            // 1️⃣ Удаляем узлы, которых больше нет в сцене
            for (int i = parentNode.Items.Count - 1; i >= 0; i--)
            {
                if (parentNode.Items[i] is TreeViewItem childTvi)
                {
                    if (childTvi.Tag is FigureMy childFig)
                    {

                        if (!sceneNode.children.Any(c => c.Id == childFig.Id && c.name == childFig.name))
                        {
                            parentNode.Items.RemoveAt(i);
                        }
                        else
                        {
                            childTvi.Header = $"{childFig.name} ({childFig.GetType().Name})";
                            SyncTree(childTvi, childFig);
                        }
                    }
                }
            }

            // 2️⃣ Добавляем новые узлы
            foreach (var childFig in sceneNode.children)
            {
                bool exists = false;
                foreach (TreeViewItem tvi in parentNode.Items)
                {
                    if (tvi.Tag is FigureMy existingFig && existingFig.Id == childFig.Id && existingFig.name == childFig.name)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    TreeViewItem newNode = new TreeViewItem
                    {
                        Header = $"{childFig.name} ({childFig.GetType().Name})",
                        Tag = childFig
                    };
                    parentNode.Items.Add(newNode);

                    // рекурсивно добавляем поддерево
                    SyncTree(newNode, childFig);
                }
            }
        }
        public static TreeViewItem CreateNode(FigureMy fig)
        {
            TreeViewItem node = new TreeViewItem
            {
                Header = $"{fig.name} ({fig.GetType().Name})",
                Tag = fig
            };
            foreach (var child in fig.children)
            {
                node.Items.Add(CreateNode(child));
            }
            return node;
        }

        public static void SaveFigure(FigureMy fig)
        {
            Saved.Add(fig.Clone());
            UpdateSavedMenu();
        }
        public static void LoadFigure(FigureMy fig)
        {
            FigureMy fig1 = fig.Clone();
            fig1.Insert();
        }

        public static void UpdateSavedMenu()
        {
            if (MW == null || MW.SavedMenu == null) return;

            MW.SavedMenu.Items.Clear();

            foreach (var fig in Saved.ToList())
            {
                MenuItem figItem = new MenuItem
                {
                    Header = $"{fig.name} ({fig.GetType().Name})"
                };

                // ➕ Добавить
                MenuItem addItem = new MenuItem
                {
                    Header = "Добавить"
                };
                addItem.Click += (s, e) =>
                {
                    LoadFigure(fig);
                };

                // ❌ Удалить
                MenuItem deleteItem = new MenuItem
                {
                    Header = "Удалить"
                };
                deleteItem.Click += (s, e) =>
                {
                    Saved.Remove(fig);
                    UpdateSavedMenu();
                };

                figItem.Items.Add(addItem);
                figItem.Items.Add(deleteItem);

                MW.SavedMenu.Items.Add(figItem);
            }
        }
    }

}
