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

namespace Syncromote
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        
        public Notification()
        {
            InitializeComponent();
        }
        public Notification(string s, Brush color)
        {
            InitializeComponent();
            hotkeytextblock.Text = s;
            hotkeytextblock1.Text = s;
            hotkeytextblock2.Text = s;
            changeColor(color);
            this.Show();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 4);
            dispatcherTimer.Start();

        }
        private void dispatcherTimer_Tick(object sender, EventArgs r)
        {

            this.Close();

        }
        public void changeColor(Brush color)
        {
            hotkeytextblock.Foreground = color;
            hotkeytextblock1.Foreground = color;
            hotkeytextblock2.Foreground = color;
        }
    }
    
}
