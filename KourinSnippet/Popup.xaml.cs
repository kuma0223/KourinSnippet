using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KourinSnippet
{
    /// <summary>
    /// Popup.xaml の相互作用ロジック
    /// </summary>
    public partial class Popup : Window
    {
        public SnippetItem SelectedItem { get; private set; }

        private ListBox[] listboxs = new ListBox[5];

        public Popup()
        {
            InitializeComponent();
            listboxs[0] = this.ItemList0;
            listboxs[1] = this.ItemList1;
            listboxs[2] = this.ItemList2;
            listboxs[3] = this.ItemList3;
            listboxs[4] = this.ItemList4;
        }
        
        private void Window_Activated(object sender, EventArgs e)
        {
            for (var i = 0; i < ItemList0.Items.Count; i++)
                if (((SnippetItem)ItemList0.Items[i]).Type != SnippetItem.ItemType.Command)
                {
                    ItemList0.SelectedIndex = i;
                    ((ListBoxItem)ItemList0.ItemContainerGenerator.ContainerFromIndex(i)).Focus();
                    break;
                }
            
            var z = System.Windows.SystemParameters.WorkArea;
            var k = z;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if(this.IsVisible) this.ClosePopup(null);
        }

        private void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter){
                var item = (SnippetItem)((ListBox)sender).SelectedItem;
                if(item != null && item.Type == SnippetItem.ItemType.Directory)
                    OpenItem((ListBox)sender, ((ListBox)sender).SelectedIndex);
                else
                    ClosePopup((SnippetItem)((ListBox)sender).SelectedItem);
            }
            else if (e.Key == Key.Back || e.Key == Key.Delete)
                ClosePopup(null);
            else if (e.Key == Key.Right)
                OpenItem((ListBox)sender, ((ListBox)sender).SelectedIndex);
            else if (e.Key == Key.Left)
                CloseItem((ListBox)sender);
            else if (e.Key == Key.Up) {
                var list = (ListBox)sender;
                if (list.SelectedIndex == 0) {
                    list.SelectedIndex = list.Items.Count - 1;
                    ((ListBoxItem)list.ItemContainerGenerator.ContainerFromIndex(list.Items.Count - 1)).Focus();
                    e.Handled = true;
                }
            } else if (e.Key == Key.Down) {
                var list = (ListBox)sender;
                if (list.SelectedIndex == list.Items.Count - 1) {
                    list.SelectedIndex = 0;
                    ((ListBoxItem)list.ItemContainerGenerator.ContainerFromIndex(0)).Focus();
                    e.Handled = true;
                }
            }
        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ListboxIndex(sender);
            for(var i=index+1; i < listboxs.Length; i++) listboxs[i].Visibility = Visibility.Collapsed;

            setToolTip((SnippetItem)((ListBox)sender).SelectedItem);
        }

        private void Item_Click(object sender, MouseButtonEventArgs e) {
            var item = (sender as ListBoxItem).DataContext as SnippetItem;

            if (item.Type == SnippetItem.ItemType.Directory) {
                var lb = ParentListbox(item);
                OpenItem(lb, lb.SelectedIndex);
                e.Handled = true;
            }
        }

        private void Item_DoubleClick(object sender, MouseButtonEventArgs e) {
            var item = (sender as ListBoxItem).DataContext as SnippetItem;
            
            if (item.Type != SnippetItem.ItemType.Directory) {
                ClosePopup(item);
                e.Handled = true;
            }
        }
        
        private void ClosePopup(SnippetItem selected)
        {
            this.SelectedItem = selected;
            this.Close();
        }

        private void OpenItem(ListBox sender, int itemIndex)
        {
            if(itemIndex<0) return;

            var item = (SnippetItem)((ListBox)sender).Items[itemIndex];
            if (item.Type == SnippetItem.ItemType.Directory && item.Children.Count>0)
            {
                var index = ListboxIndex(sender);
                if (index < listboxs.Length - 1) { 
                    var nextbox = listboxs[index+1];
                    nextbox.ItemsSource = item.Children;
                    nextbox.Visibility = Visibility.Visible;
                    nextbox.SelectedIndex = 0;
                    nextbox.ScrollIntoView(nextbox.SelectedItem);
                    nextbox.UpdateLayout();
                    ((ListBoxItem)nextbox.ItemContainerGenerator.ContainerFromIndex(0)).Focus();
                    setToolTip((SnippetItem)nextbox.SelectedItem);
                }
            }
        }
        private void CloseItem(ListBox sender)
        {
            var index = ListboxIndex(sender);
            if (index > 0) { 
                listboxs[index].Visibility = Visibility.Collapsed;
                var nextbox = listboxs[index-1];
                ((ListBoxItem)nextbox.ItemContainerGenerator.ContainerFromIndex(nextbox.SelectedIndex)).Focus();
            }
            setToolTip("");
        }

        private int ListboxIndex(object control)
        {
            return int.Parse(((Control)control).Name.Replace("ItemList", ""));
        }

        private ListBox ParentListbox(SnippetItem item) {
            foreach(var x in listboxs) {
                if(x.Items.Contains(item)) return x;
            }
            return null;
        }

        private void setToolTip(SnippetItem item)
        {
            if(item == null) setToolTip("");
            else setToolTip(item.Text);
        }
        private void setToolTip(string text)
        {
            x_TooltipText.Text = text;
            x_Tooltip.IsOpen = (text != null && text != "");
        }

        private void SetPosition()
        {
            var vt = System.Windows.SystemParameters.VirtualScreenTop;
            var vl = System.Windows.SystemParameters.VirtualScreenLeft;
            var vh = System.Windows.SystemParameters.VirtualScreenHeight;
            var vw = System.Windows.SystemParameters.VirtualScreenWidth;
            var vr = vl + vw;
            var vb = vt + vh;

            if(this.Left + this.ActualWidth > vr) this.Left = vr - this.ActualWidth;
            if(this.Top + this.ActualHeight > vb) this.Top = vb - this.ActualHeight;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPosition();
        }
    }
}
