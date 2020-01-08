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
    public partial class Popup2 : Window
    {
        public HistoryItem SelectedItem { get; private set; }

        public Popup2()
        {
            InitializeComponent();
        }
        
        private void Window_Activated(object sender, EventArgs e)
        {
            if (ItemList0.Items.Count > 0) { 
                ItemList0.SelectedIndex = 0;
                ((ListBoxItem)ItemList0.ItemContainerGenerator.ContainerFromIndex(0)).Focus();
            }
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if(this.IsVisible) this.ClosePopup(null);
        }
        private void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ClosePopup((HistoryItem)((ListBox)sender).SelectedItem);
            else if (e.Key == Key.Back || e.Key == Key.Delete)
                ClosePopup(null);
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
        private void ClosePopup(HistoryItem selected)
        {
            this.SelectedItem = selected;
            this.Close();
        }
        
        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ListBox)sender).SelectedItem != null)
                setToolTip(((HistoryItem)((ListBox)sender).SelectedItem).Text);
        }
        
        private void Item_DoubleClick(object sender, MouseButtonEventArgs e) {
            var item = (sender as ListBoxItem).DataContext as HistoryItem;
            ClosePopup(item);
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
