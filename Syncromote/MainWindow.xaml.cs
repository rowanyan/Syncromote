using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using SimpleTcp;
using ModernWpf.Controls;
using EventHook;
using System.Timers;
using NHotkey;
using NHotkey.Wpf;

namespace Syncromote
{
    //____________________________________________________________________________________________________________

    public class tcp_server
    {
        public static MainWindow reff;
        public static string ip = "";
        public static SimpleTcpServer server;

        public static void startServer(String IP)
        {
            server = new SimpleTcpServer(IP+":37664");

            server.Events.ClientConnected += ClientConnected;
            server.Events.ClientDisconnected += ClientDisconnected;
            server.Events.DataReceived += DataReceived;

            server.Start();
            //server.Send("["+ip+"]", "Hello, world!");

            //client.Send("Hello);


        }
        public static void ClientConnected(object sender, ClientConnectedEventArgs e)
        {

            //Console.WriteLine("[" + e.IpPort + "] client connected");
            
            Application.Current.Dispatcher.Invoke((Action)delegate {

                connectPremission window1 = new connectPremission(e.IpPort);


                int res = window1.showWindow().Result;

                if (res == 1)
                {
                    server.Send(e.IpPort, "p$1");
                    reff.isEstablished = true;
                    ip = e.IpPort;

                }
                else if (res == 2) {
                    server.Send(e.IpPort, "p$2");
                    server.DisconnectClient(e.IpPort);
                    
                }
            });

        }

        public static void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("[" + e.IpPort + "] client disconnected: " + e.Reason.ToString());
        }

        public static void DataReceived(object sender, DataReceivedEventArgs e)
        {
            String type = Encoding.UTF8.GetString(e.Data).Substring(0, 1);
            String data = Encoding.UTF8.GetString(e.Data).Substring(2);
            reff.receive(type, data);
            
        }

    //____________________________________________________________________________________________________________


    }
    public class tcp_client
    {
        public static MainWindow reff;
        public static SimpleTcpClient client;
       static connectPremission window1;

        public static void initiateClient(String IP)
        {
            client = new SimpleTcpClient(IP + ":37664");
            //int returnType = 0;
            // set events
            client.Events.Connected += Connected;
            client.Events.Disconnected += Disconnected;
            client.Events.DataReceived += DataReceivedClient;

            client.Connect();

            Application.Current.Dispatcher.Invoke((Action) delegate {
                 window1 = new connectPremission();
                window1.Show();
            });

        }

        public static void Connected(object sender, EventArgs e)
        {
            Console.WriteLine("*** Server connected");
        }

        public static void Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("*** Server disconnected");
        }

        public static void DataReceivedClient(object sender, DataReceivedEventArgs e)
        {
            String type = Encoding.UTF8.GetString(e.Data).Substring(0, 1);
            String data = Encoding.UTF8.GetString(e.Data).Substring(2);
           

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (type == "p")
                {
                    if (data == "1")
                    {
                        reff.isEstablished = true;
                        window1.messgaeTextBlock.Text = "Connected successfully";
                        window1.yesButton.Content = "Close";
                        window1.yesButton.Visibility = Visibility.Visible;
                        reff.connectbutton.Content = "Disconnect";
                        

                    }
                    else if (data == "2")
                    {
                        window1.messgaeTextBlock.Text = "Connection denied";
                        window1.yesButton.Content = "Close";
                        window1.yesButton.Visibility = Visibility.Visible;
                        reff.startButton.IsEnabled = true;
                    }

                }
                else
                {
                    reff.receive(type, data);
                    
                }
            });

            
        }
            
    }


    //____________________________________________________________________________________________________________

    public partial class MainWindow : Window
    {
        bool IsMouseMovingByMe = false;
        public static Notification n;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public bool isHotkeyOn = false;
        private static readonly KeyGesture IncrementGesture = new KeyGesture(Key.Q, ModifierKeys.Control);
        public bool isEstablished = false;
        int x = 0, y = 0;
        static bool srvorclt = true;
        //true = client    false=server
        //private readonly ApplicationWatcher applicationWatcher;
        //private readonly ClipboardWatcher clipboardWatcher;
        private readonly EventHookFactory eventHookFactory = new EventHookFactory();
        private readonly KeyboardWatcher keyboardWatcher;
        private readonly MouseWatcher mouseWatcher;
        private MouseWatcher mouseWatcher2;
        //private readonly PrintWatcher printWatcher;
      

        
        public void send(string message)
        {
            if (isEstablished)
            {
                Console.WriteLine("SEND:   " + message);
                if (srvorclt == true)
                {

                    tcp_client.client.Send(message);

                }
                else
                {
                    tcp_server.server.Send(tcp_server.ip, message);

                }
            }
            
        }
        public void receive(string type, string data)
        {
            Console.WriteLine("GET:   " + type+"$"+data);

            if (type == "m")
            {
                try
                {
                    if (!IsMouseMovingByMe)
                    {
                        string[] result = data.Split(',');
                        int x1 = Int32.Parse(result[0]);
                        int y1 = Int32.Parse(result[1]);
                        MouseOperations.SetCursorPosition(x1, y1);
                    }

                }
                catch (Exception)
                {

                    
                }
                
            }

            else if (type == "c")
            {
                string[] result = data.Split(',');
                int x1 = Int32.Parse(result[0]);
                int y1 = Int32.Parse(result[1]);
                MouseOperations.SetCursorPosition(x1, y1);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            }

            else if (type == "t")
            {
                if (srvorclt == true)
                {
                    messagetextBlock.Text += "\nServer: " + data;
                }
                else
                {
                    messagetextBlock.Text += "\nClient: " + data;
                }

            }
        }

        private void HotkeyManager_HotkeyAlreadyRegistered(object sender, HotkeyAlreadyRegisteredEventArgs e)
        {
            MessageBox.Show(string.Format("The hotkey {0} is already registered by another application", e.Name));
        }

        private void OnIncrement(object sender, HotkeyEventArgs e)
        {
            

            if (isEstablished )
            {
                
                if (!isHotkeyOn)
                {
                    isHotkeyOn = true;
                    if (n != null)
                    {
                        n.Close();
                    }
                    n = new Notification("Inputs are being send");
                    dispatcherTimer.Tick += dispatcherTimer_Tick;
                    dispatcherTimer.Interval = new TimeSpan(0,0, 0, 0, 200);
                    dispatcherTimer.Start();
                    

                }
                else if (isHotkeyOn)
                {
                    if (n != null)
                    {
                        n.Close();
                    }
                    n = new Notification("Stop sending inputs");
                    isHotkeyOn = false;
                    dispatcherTimer.Stop();
                }
                Console.WriteLine("Hotkey   :" + isHotkeyOn);
            }

            e.Handled = true;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs r)
        {
            
            MouseOperations.MousePoint a = MouseOperations.GetCursorPosition();
            if (x != a.X || y != a.Y)
            {
                IsMouseMovingByMe = false;
                x = a.X;
                y = a.Y;
                if (isHotkeyOn)
                {
                    send("m$" + x + "," + y);
                }

            }
            else
            {
                IsMouseMovingByMe = true;
            }

        }

        public MainWindow()
        {
            HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered;

            HotkeyManager.Current.AddOrReplace("Increment", IncrementGesture, OnIncrement);
            InitializeComponent();
            tcp_client.reff = this;
            tcp_server.reff = this;
            
            Application.Current.Exit += OnApplicationExit;

            


            mouseWatcher = eventHookFactory.GetMouseWatcher();
            mouseWatcher.Start();
            mouseWatcher.OnMouseInput += (s, e) =>
            {
                if (e.Message.ToString() == "WM_LBUTTONUP")
                {
                    if (isHotkeyOn)
                    {
                        send("c$" + e.Point.x + "," + e.Point.y);
                    }
                 
                }
                
            };

            //clipboardWatcher = eventHookFactory.GetClipboardWatcher();
            //clipboardWatcher.Start();
            //clipboardWatcher.OnClipboardModified += (s, e) =>
            //{
            //    Console.WriteLine("Clipboard updated with data '{0}' of format {1}", e.Data,
            //        e.DataFormat.ToString());
            //};
            //applicationWatcher = eventHookFactory.GetApplicationWatcher();
            //applicationWatcher.Start();
            //applicationWatcher.OnApplicationWindowChange += (s, e) =>
            //{
            //    Console.WriteLine("Application window of '{0}' with the title '{1}' was {2}",
            //        e.ApplicationData.AppName, e.ApplicationData.AppTitle, e.Event);
            //};
            //printWatcher = eventHookFactory.GetPrintWatcher();
            //printWatcher.Start();
            //printWatcher.OnPrintEvent += (s, e) =>
            //{
            //    Console.WriteLine("Printer '{0}' currently printing {1} pages.", e.EventData.PrinterName,
            //        e.EventData.Pages);
            //};

            eventHookFactory.Dispose();


        }


        private void OnApplicationExit(object sender, EventArgs e)
        {
            keyboardWatcher.Stop();
            mouseWatcher.Stop();
            //clipboardWatcher.Stop();
            //applicationWatcher.Stop();
            //printWatcher.Stop();

            eventHookFactory.Dispose();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Create
            tcp_server.startServer(ipTextBox.Text);
            srvorclt = false;
            messageTxt.Text = "Server has been started successfully";
            connectbutton.Content = "Disconnect";
            startButton.IsEnabled = false;
        }

        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ipTextBox.Text != "")
            {
                ipTextBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                ipTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void connectbutton_Click(object sender, RoutedEventArgs e)
        {
            //Connect
            
            if ((String)connectbutton.Content == "Connect")
            {
                tcp_client.initiateClient(ipTextBox.Text);
                startButton.IsEnabled = false;
            }
           else if ((String)connectbutton.Content == "Disconnect")
            {
                if (srvorclt == true)
                {
                    tcp_client.client.Disconnect();
                    connectbutton.Content = "Connect";
                    startButton.IsEnabled = true;

                }
                else if (srvorclt == false)
                {
                    messageTxt.Text = "";
                    tcp_server.server.Stop();
                    connectbutton.Content = "Connect";
                    startButton.IsEnabled = true;

                }
                
            }


        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AboutUs about = new AboutUs();
            about.Show();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            send("t$" + messagetextBox.Text);
            if (srvorclt == true)
            {
                messagetextBlock.Text += "\nClient: "+messagetextBox.Text;

            }
            else
            {
                messagetextBlock.Text += "\nServer: " + messagetextBox.Text;
            }

        }



        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string tag = args.InvokedItem.ToString();
            if(tag=="Home")
            {
                Home.Visibility = Visibility.Visible;
            } else if (tag=="About")
            {
                AboutUs about = new AboutUs();
                about.Show();

            }
            else
            {
                Home.Visibility = Visibility.Hidden;
            }
            
            
        }
    }
    //____________________________________________________________________________________________________________

    public class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public void DoMouseClick(int x2,int y2)
        {
           
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x2, y2, 0, 0);
        }
        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}


