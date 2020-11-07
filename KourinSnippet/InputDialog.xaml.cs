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
    /// InputDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            X_Box1.Focus();
        }

        public string Label1 {
            set {
                X_Label1.Text = value;
                X_Label1.Visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
                X_Box1.Visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string Label2 {
            set {
                X_Label2.Text = value;
                X_Label2.Visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
                X_Box2.Visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string Label3 {
            set {
                X_Label3.Text = value;
                X_Label3.Visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
                X_Box3.Visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string Text1 {
            get { return this.X_Box1.Text; }
        }
        public string Text2 {
            get { return this.X_Box2.Text; }
        }
        public string Text3 {
            get { return this.X_Box3.Text; }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) {
                this.Close();
            }
        }
    }
}
