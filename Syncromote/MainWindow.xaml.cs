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
using System.Windows.Media;
using System.Threading.Tasks;
using System.Reflection;
using WindowsInput;
using System.Collections.Generic;

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
            server = new SimpleTcpServer("*:37664");

            server.Events.ClientConnected += ClientConnected;
            server.Events.ClientDisconnected += ClientDisconnected;
            server.Events.DataReceived += DataReceived;

            try
            {
                server.Start();
            }
            catch (Exception e)
            {

                Console.WriteLine("Exception in Server constructor" + e.Message);
            }

            
            //server.Send("["+ip+"]", "Hello, world!");

            //client.Send("Hello);


        }
        public static void ClientConnected(object sender, ClientConnectedEventArgs e)
        {

            
            
            Application.Current.Dispatcher.Invoke((Action)delegate {

                connectPremission window1 = new connectPremission(e.IpPort);


                int res = window1.showWindow().Result;

            if (res == 1)
            {
                server.Send(e.IpPort, "p$1");
                reff.isEstablished = true;
                ip = e.IpPort;
                reff.send("|r$" + reff.selfResolution[0] + "," + reff.selfResolution[1]);

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
            reff.isEstablished = false;
        }

        public static void DataReceived(object sender, DataReceivedEventArgs e)
        {
            
            reff.receive(Encoding.UTF8.GetString(e.Data));
            
        }
    }

    //____________________________________________________________________________________________________________


    
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
            try
            {
                client.Connect();
            }
            catch (Exception e)
            {

                Console.WriteLine("Exception in Client constructor" + e.Message);
            }
            

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
                        reff.send("|r$" + reff.selfResolution[0] + "," + reff.selfResolution[1]);

                        if (Settings1.Default.ip1 == null)
                        {
                            Settings1.Default.ip1 = reff.ipTextBox.Text;
                            Settings1.Default.Save();

                        }
                        else
                        {
                            Settings1.Default.ip2 = Settings1.Default.ip1;
                            Settings1.Default.ip1 = reff.ipTextBox.Text;
                            Settings1.Default.Save();
                        }
                        reff.ipItem1.Text = Settings1.Default.ip1;
                        reff.ipItem2.Text = Settings1.Default.ip2;


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
                    reff.receive(Encoding.UTF8.GetString(e.Data));
                    
                }
            });

            
        }
            
    }


    //____________________________________________________________________________________________________________

    public partial class MainWindow : Window
    {
        
        int x_backup_l = 0, y_backup_l = 0;
        int x_backup_r = 0, y_backup_r = 0;
        bool LeftUpLock = false, LeftDownLock = false, RightUpLock = false , RightDownLock = false;
        bool keyboardDownLock = false, keyboardUpLock = false;
        bool clipboardLock = false;
        cursor cur = new cursor();
        InputSimulator keyinput = new InputSimulator();
        ClipboardWatcher clipboardWatcher;
        transWindow transWindow=new transWindow();
        public int[] selfResolution = new int[2];
        public int[] otherSideResolution = new int[2];
        bool IsMouseMovingByMe = false;
        public static Notification n;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public bool isHotkeyOn = false;
        public bool isHotkeyOnOS = false;
        private static readonly KeyGesture IncrementGesture = new KeyGesture(Key.Q, ModifierKeys.Alt);
        public bool isEstablished = false;
        int x = 0, y = 0;
        static bool srvorclt = true;
        //true = client    false=server
        // private readonly ApplicationWatcher applicationWatcher;
        
        private readonly EventHookFactory eventHookFactory = new EventHookFactory();
        private readonly KeyboardWatcher keyboardWatcher;
        private readonly MouseWatcher mouseWatcher;
        private MouseWatcher mouseWatcher2;
        //private readonly PrintWatcher printWatcher;

        public MainWindow()
        {

            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null) / 0.96;
            var dpiY = (int)dpiYProperty.GetValue(null, null) / 0.96;
            HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered;

            this.selfResolution[0] = (int)(System.Windows.SystemParameters.PrimaryScreenWidth * (double)(dpiX / 100));
            this.selfResolution[1] = (int)(System.Windows.SystemParameters.PrimaryScreenHeight * (double)(dpiX / 100));



            HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered;

            HotkeyManager.Current.AddOrReplace("Increment", IncrementGesture, OnIncrement);
            InitializeComponent();

            ipItem1.Text = Settings1.Default.ip1;
            ipItem2.Text = Settings1.Default.ip2;

            if (Settings1.Default.ip1 == null)
            {
                ipItem1.Visibility = Visibility.Collapsed;
            }
            if (Settings1.Default.ip2 == null)
            {
                ipItem2.Visibility = Visibility.Collapsed;
            }

            tcp_client.reff = this;
            tcp_server.reff = this;

            Application.Current.Exit += OnApplicationExit;




            mouseWatcher = eventHookFactory.GetMouseWatcher();
            mouseWatcher.Start();
            mouseWatcher.OnMouseInput += (s, e) =>
            {
                int[] positions = PositionConvert(e.Point.x, e.Point.y);
                if (e.Message.ToString() == "WM_LBUTTONUP")
                {
                    if (isHotkeyOn && isEstablished && LeftUpLock == false)
                    {
                        try { send("|n$" + positions[0] + "," + positions[1]); }
                        catch (Exception) { }
                    }
                    LeftUpLock = false;

                }

                else if (e.Message.ToString() == "WM_LBUTTONDOWN")
                {
                    if (isHotkeyOn && isEstablished && LeftDownLock == false)
                    {
                        try { send("|e$" + positions[0] + "," + positions[1]); }
                        catch (Exception) { }
                    }
                    LeftDownLock = false;
                }
                else if (e.Message.ToString() == "WM_RBUTTONUP")
                {
                    if (isHotkeyOn && isEstablished && RightUpLock == false)
                    {
                        try { send("|d$" + positions[0] + "," + positions[1]); }
                        catch (Exception) { }
                    }
                    RightUpLock = false;
                }
                else if (e.Message.ToString() == "WM_RBUTTONDOWN")
                {
                    if (isHotkeyOn && isEstablished && RightDownLock == false)
                    {
                        try { send("|a$" + positions[0] + "," + positions[1]); }
                        catch (Exception) { }
                    }
                    RightDownLock = false;
                }

            };

            var keyboardWatcher = eventHookFactory.GetKeyboardWatcher();
            keyboardWatcher.Start();
            keyboardWatcher.OnKeyInput += (s, e) =>
            {
                
                if (isHotkeyOn)
                {
                    if (e.KeyData.EventType.ToString() == "down" && keyboardDownLock == false)
                    {
                        send("|y$" + e.KeyData.Keyname);

                    }
                    else if (e.KeyData.EventType.ToString() == "up" && keyboardUpLock == false)
                    {
                        send("|x$" + e.KeyData.Keyname);
                    }
                    keyboardDownLock = false;
                    keyboardUpLock = false;
                }


            };



            clipboardWatcher = eventHookFactory.GetClipboardWatcher();
            clipboardWatcher.Start();
            clipboardWatcher.OnClipboardModified += (s, e) =>
            {
                if ((isHotkeyOn || isHotkeyOnOS) && clipboardLock == false)
                {
                    send("|z$" + e.Data);

                }
                clipboardLock = false;


            };


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

        public void send(string message)
        {
            try
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
            catch (Exception e)
            {

                Console.WriteLine("Exception in send method" + e.Message );
            }

        }
//        if (e.Modifiers == (Keys) Enum.Parse(typeof(Keys), "keys1", true)
//    && e.KeyCode == (Keys) Enum.Parse(typeof(Keys), "keys2", true))
//{
//    string keyPressed = e.KeyCode.ToString();
//        MessageBox.Show(keyPressed);
//}

    //? Recive
    public void receive(string message)

        {
            string[] request = message.Split('|');
            for (int i = 1; i < request.Length; i++)
            {


                String type = request[i].Substring(0, 1);
                String data = request[i].Substring(2);
                Console.WriteLine("GET:   " + type + "$" + data);

                if (type == "y" )
                {
                    keyboardDownLock = true;
                    try
                    {
                        keyinput.Keyboard.KeyDown((WindowsInput.Native.VirtualKeyCode)Keyboard.Convert(data.ToLower()));

                    }
                    catch (Exception)
                    {

                        
                    }
                    
                }

                if (type == "x" )
                {
                    try
                    {
                        keyinput.Keyboard.KeyUp((WindowsInput.Native.VirtualKeyCode)Keyboard.Convert(data.ToLower()));

                    }
                    catch (Exception)
                    {

                       
                    }
                    keyboardUpLock = true;
                    
                }

                if (type == "z" && (isHotkeyOnOS))
                {
                    clipboardLock = true;
                    Application.Current.Dispatcher.Invoke((Action)delegate {
                        Clipboard.SetText(data);
                    });
                    
                }
                if (type == "m")
                {
                    try
                    {
                        
                        
                        string[] result = data.Split(',');
                        int x1 = (Int32.Parse(result[0]) * selfResolution[0] / otherSideResolution[0]);
                        int y1 = (Int32.Parse(result[1]) * selfResolution[0] / otherSideResolution[0]);
                        MouseOperations.SetCursorPosition(x1, y1);

                    }
                    catch (Exception)
                    {


                    }

                }
                if (type == "h")
                {
                    if (data == "on")
                    {
                        isHotkeyOnOS = true;
                        if (isHotkeyOn)
                        {

                            if (n != null)
                            {
                                try { n.Close(); n = null; }
                                catch (Exception) { }
                            }
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                n = new Notification("Both hotkeys are on", Brushes.Red);
                                cur.Show();
                            });
                        }
                        else
                        {
                            if (n != null)
                            {
                                try { n.Close(); n = null; }
                                catch (Exception) { }
                            }
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                n = new Notification("The other side's hotkey is on", Brushes.White);
                                cur.Show();
                            });


                        }
                    }
                    else
                    {
                        isHotkeyOnOS = false;
                        if (n != null)
                        {
                            try { n.Close(); n = null; }
                            catch (Exception) { }
                        }
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            n = new Notification("The other side's hotkey is off", Brushes.White);
                            cur.Hide();
                        });

                    }

                }

                if (type == "n")
                {
                    LeftUpLock = true;
                    string[] result = data.Split(',');
                    int x1 = (Int32.Parse(result[0]) * selfResolution[0] / otherSideResolution[0]);
                    int y1 = (Int32.Parse(result[1]) * selfResolution[0] / otherSideResolution[0]);
                    MouseOperations.SetCursorPosition(x1, y1);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);


                }
                if (type == "e")
                {
                    MouseOperations.MousePoint mouseBackup = MouseOperations.GetCursorPosition();
                    

                    LeftDownLock = true;
                    string[] result = data.Split(',');
                    int x1 = (Int32.Parse(result[0]) * selfResolution[0] / otherSideResolution[0]);
                    int y1 = (Int32.Parse(result[1]) * selfResolution[0] / otherSideResolution[0]);

                    MouseOperations.SetCursorPosition(x1, y1);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);


                }
                if (type == "d")
                {
                    RightUpLock = true;
                    string[] result = data.Split(',');
                    int x1 = (Int32.Parse(result[0]) * selfResolution[0] / otherSideResolution[0]);
                    int y1 = (Int32.Parse(result[1]) * selfResolution[0] / otherSideResolution[0]);
                    MouseOperations.SetCursorPosition(x1, y1);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);


                }
                if (type == "a")
                {
                    MouseOperations.MousePoint mouseBackup = MouseOperations.GetCursorPosition();
                    RightDownLock = true;
                    string[] result = data.Split(',');
                    int x1 = (Int32.Parse(result[0]) * selfResolution[0] / otherSideResolution[0]);
                    int y1 = (Int32.Parse(result[1]) * selfResolution[0] / otherSideResolution[0]);
                    MouseOperations.SetCursorPosition(x1, y1);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);


                }

                if (type == "t")
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
                if (type == "r")
                    
                {
                    string[] result = data.Split(',');
                    int x1 = Int32.Parse(result[0]);
                    int y1 = Int32.Parse(result[1]);
                    otherSideResolution[0] = x1;
                    otherSideResolution[1] = y1;

                }
                
            }

        }
        
        private void HotkeyManager_HotkeyAlreadyRegistered(object sender, HotkeyAlreadyRegisteredEventArgs e)
        {
            MessageBox.Show(string.Format("The hotkey {0} is already registered by another application", e.Name));
        }

        private void OnIncrement(object sender, HotkeyEventArgs e)
        {
            //Task.Delay(2000).ContinueWith(_ =>
            //{
            //    //PerformAClick.CLICK();
            //    Keyboard.Send(Keyboard.ScanCodeShort.RETURN);
            //}
            //);

            if (isEstablished )
            {
                
                if (!isHotkeyOn)
                {
                    if ((bool)chboxskype.IsChecked|| (bool)chboxGoogleMeet .IsChecked)
                    {
                        transWindow.Show(); 
                    }
                    
                    send("|h$on");
                    isHotkeyOn = true;
                    if (n != null)
                    {
                        try { n.Close(); n = null; }
                        catch (Exception) { }
                    }
                    if (isHotkeyOnOS)
                    {
                        n = new Notification("Both hotkeys are on", Brushes.Red);
                    }
                    else
                    {
                        n = new Notification("Your hotkey is on", Brushes.White);

                    }
                    dispatcherTimer.Tick += dispatcherTimer_Tick;
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                    dispatcherTimer.Start();


                }
                else if (isHotkeyOn)
                {
                    transWindow.Hide();
                    if (n != null)
                    {
                        try { n.Close(); n = null; }
                        catch (Exception) { }
                    }
                    send("|h$off");
                    n = new Notification("Your hotkey is off", Brushes.White);
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
                int[] position = PositionConvert(x, y);
                IsMouseMovingByMe = false;
                x = a.X;
                y = a.Y;
                if (isHotkeyOn)
                {
                    send("|"+"m$" + position[0] + "," + position[1]);
                }

            }
            else
            {
                IsMouseMovingByMe = true;
            }

        }

        


        private void OnApplicationExit(object sender, EventArgs e)
        {
            //keyboardWatcher.Stop();
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
                    isEstablished = false;

                }
                else if (srvorclt == false)
                {
                    messageTxt.Text = "";
                    tcp_server.server.Stop();
                    connectbutton.Content = "Connect";
                    startButton.IsEnabled = true;
                    isEstablished = false;

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


        private int[] PositionConvert(int x, int y)
        {
            int topleftx=0;
            int toplefty=0;
            int buttomdownx=1366;
            int buttomdowny=768;

            int[] positions = new int[2];



            this.Dispatcher.Invoke(() =>
        {
            if (chboxskype.IsChecked == true)
            {
                chboxGoogleMeet.IsChecked = false;
                topleftx = 132;
                toplefty = 80;
                buttomdownx = 1232;
                buttomdowny = 698;
            }
            else if (chboxGoogleMeet.IsChecked == true)
            {
                chboxskype.IsChecked = false;
                topleftx = 30;
                toplefty = 12;
                buttomdownx = 1336;
                buttomdowny = 746;
            }
        });

           

            positions[0] = ((x - topleftx) * 1366) / (buttomdownx - topleftx);
            positions[1] = ((y - toplefty) * 1366) / (buttomdownx - topleftx);
        
            return positions;
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
public class PerformAClick
{
    [DllImport("user32.dll")]
    internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

    //main calling method
    public static void CLICK()
    {
        INPUT[] i = new INPUT[1];

        i[0].type = 0;
        i[0].U.mi.time = 0;
        i[0].U.mi.dwFlags = MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTDOWN;
        i[0].U.mi.dwExtraInfo = UIntPtr.Zero;
        i[0].U.mi.dx = 1;
        i[0].U.mi.dy = 1;

        SendInput(1, i, INPUT.Size);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        internal uint type;
        internal InputUnion U;
        internal static int Size
        {
            get { return Marshal.SizeOf(typeof(INPUT)); }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct InputUnion
    {
        [FieldOffset(0)]
        internal MOUSEINPUT mi;

    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {
        internal int dx;
        internal int dy;
        internal int mouseData;
        internal MOUSEEVENTF dwFlags;
        internal uint time;
        internal UIntPtr dwExtraInfo;
    }

    [Flags]
    internal enum MOUSEEVENTF : uint
    {
        ABSOLUTE = 0x8000,
        HWHEEL = 0x01000,
        MOVE = 0x0001,
        MOVE_NOCOALESCE = 0x2000,
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
        VIRTUALDESK = 0x4000,
        WHEEL = 0x0800,
        XDOWN = 0x0080,
        XUP = 0x0100
    }
}