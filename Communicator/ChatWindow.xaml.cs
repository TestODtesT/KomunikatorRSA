using Communicator.Common.Entities;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Communicator
{

    public partial class ChatWindow : MetroWindow
    {
        private ClientService _clientService = new ClientService();
        private string _userName;
        public ObservableCollection<User> ConnectedUsers = new ObservableCollection<User>(); 
        public ObservableCollection<DisplayMessage> _messages = new ObservableCollection<DisplayMessage>(); 
        private bool _lostFocus = false;
        private Random _rnd = new Random();
        private List<Brush> _brushes = new List<Brush>()
        {
                Brushes.DeepPink,
                Brushes.BlueViolet,
                Brushes.LightSeaGreen,
                Brushes.Chocolate,
                Brushes.Navy,
                Brushes.Green,
                Brushes.YellowGreen,
                Brushes.RoyalBlue,
                Brushes.DarkOrange,
                Brushes.DarkSalmon,
                Brushes.DarkSeaGreen,
                Brushes.DarkSlateGray,
                Brushes.DarkSlateBlue
        };

        public ChatWindow(string userName)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen; 
            _userName = userName;

            _clientService.Connect(userName, this);
            Title = $"SKYPE - {userName}";
            listOfUsers.ItemsSource = ConnectedUsers;
            listOfMessages.ItemsSource = _messages; 
            Closing += OnWindowClosing; 
            Deactivated += new EventHandler(OnWindowDeactivated); 
            Activated += new EventHandler(OnWindowActivated); 
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            _lostFocus = true; 
        }
        
        private void OnWindowActivated(object sender, EventArgs e)
        {
            if (_lostFocus == true) 
            {
                _lostFocus = false; 
                StopHighlighting(); 
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            _clientService.SendMessage(message.Text); 
            message.Text = string.Empty; 
        }

       
        public void AddMessage(Message message)
        {
            var user = ConnectedUsers.FirstOrDefault(x => x.UserName == message.UserName);

            var displayMessage = new DisplayMessage()
            {
                UserName = message.UserName,
                Color = user != null ? user.Color : message.UserName == ">>" ? Brushes.Crimson : GetRandomBrush(),
                DateTime = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss")
            };

            foreach(var word in message.Text.Split(new[] { ' ' }))
            {
                var replaceWord = word;
                if (word.Contains("="))
                {
                    replaceWord = replaceWord.Replace("=", "&#061;");
                }
                //sometimes error - TO DO
                if (Regex.IsMatch(replaceWord, @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)(([a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$)|([a-z0-9]*(:[0-9]{1,5})?(\/.*)?$))"))
                {
                    message.Text = message.Text.Replace(replaceWord, $"<Hyperlink NavigateUri=\"{replaceWord}\">{replaceWord}</Hyperlink>");
                }
                else
                {
                    if(replaceWord.Contains(">") || replaceWord.Contains("<"))
                    {
                        message.Text = message.Text.Replace(replaceWord, replaceWord.Replace(">", "&gt;").Replace("<", "&lt;"));
                    }
                }
            }
            displayMessage.Text = message.Text; 

            if (Application.Current.Dispatcher.CheckAccess())
            {
                _messages.Add(displayMessage);
                messageScroll.ScrollToBottom(); 
                if (_lostFocus) 
                {
                    StartHighlighting(); 
                }
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() =>
                  {
                      _messages.Add(displayMessage);
                      messageScroll.ScrollToBottom();
                      if (_lostFocus) 
                      {
                          StartHighlighting(); 
                      }
                  }));
            }
        }

        public void AddUser(string value)
        {
            if (!ConnectedUsers.Any(z => z.UserName == value))
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    AddUserAndGenerateColor(value);
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                      new Action(() =>
                      {
                          AddUserAndGenerateColor(value);
                      }));
                }
            }
        }

        private void AddUserAndGenerateColor(string value)
        {
            Brush color = Brushes.DimGray;
            if (value != _userName) 
            {
                color = GetRandomBrush();
                _brushes.Remove(color);
            }
            ConnectedUsers.Add(new User() { UserName = value, Color = color });
        }

        private Brush GetRandomBrush()
        {
            var no = _rnd.Next(_brushes.Count);
            return _brushes[no];
        }

        public async void OnWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            var result = await this.ShowMessageAsync("", "Czy na pewno chcesz wyłączyć chat?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative) 
            {
                _clientService.Disconnect(); 
                Environment.Exit(0);
            }
        }

        public void RemoveUser(string value)
        {
            var existing = ConnectedUsers.FirstOrDefault(z => z.UserName == value);
            if (existing != null)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    _brushes.Add(existing.Color);
                    ConnectedUsers.Remove(existing); 
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                      new Action(() =>
                      {
                          _brushes.Add(existing.Color); 
                          ConnectedUsers.Remove(existing);
                      }));
                }
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) 
            {
                _clientService.SendMessage(message.Text); 
                message.Text = string.Empty; 
            }
        }

        public static void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(hyperlink.NavigateUri.AbsoluteUri)); 
            e.Handled = true; ;
        }


        [DllImport("user32.dll")]

        [return: MarshalAs(UnmanagedType.Bool)]

        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]

        public struct FLASHWINFO
        {

            public uint cbSize;

            public IntPtr hwnd;

            public uint dwFlags;

            public uint uCount;

            public uint dwTimeout;

        }

        public const uint FLASHW_ALL = 3;

        public void StopHighlighting()
        {
            SetHighlighting(0);
        }

        public void StartHighlighting()
        {
            SetHighlighting(3);
        }

        private void SetHighlighting(uint flag)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = new WindowInteropHelper(this).Handle;
            fInfo.dwFlags = flag;
            fInfo.uCount = uint.MaxValue;
            fInfo.dwTimeout = 0;
            FlashWindowEx(ref fInfo);
        }
    }
}
