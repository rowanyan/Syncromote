using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for connectPremission.xaml
    /// </summary>
    public partial class connectPremission : Window
    {
        public int returnType = 0;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public connectPremission(String IpPort)
        {
            InitializeComponent();
            messgaeTextBlock.Text = "Will you allow the connection from " + IpPort + " UwU?";

            SoundPlayer playSound = new SoundPlayer(Syncromote.Properties.Resources.tuturu);
            playSound.Play();
            ShowInTaskbar = false;

        }

        public connectPremission()
        {
            InitializeComponent();
            messgaeTextBlock.Text = "Wait for the server to accept the connection";

            noButton.Visibility = Visibility.Hidden;
            yesButton.Visibility = Visibility.Hidden;



            SoundPlayer playSound = new SoundPlayer(Syncromote.Properties.Resources.tuturu);
            playSound.Play();
            ShowInTaskbar = false;


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Yes
            returnType = 1;
            tcs.SetResult(true);
            this.Close();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // No
            returnType = 2;
            tcs.SetResult(true);
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public async Task<int> showWindow()
        {

            this.ShowDialog();
            await tcs.Task;
            
            return returnType;
        }

    }
}
