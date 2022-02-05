using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Alice梦幻桌面
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 指向 Program Manager 窗口句柄
        private IntPtr programIntPtr = IntPtr.Zero;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void Init()
        {
            // 通过类名查找一个窗口，返回窗口句柄。
            programIntPtr = Win32.FindWindow("Progman", null);

            // 窗口句柄有效
            if (programIntPtr != IntPtr.Zero)
            {

                IntPtr result = IntPtr.Zero;

                // 向 Program Manager 窗口发送 0x52c 的一个消息，超时设置为0x3e8（1秒）。
                Win32.SendMessageTimeout(programIntPtr, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 0x3e8, result);

                // 遍历顶级窗口
                Win32.EnumWindows((hwnd, lParam) =>
                {
                    // 找到包含 SHELLDLL_DefView 这个窗口句柄的 WorkerW
                    if (Win32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                    {
                        // 找到当前 WorkerW 窗口的，后一个 WorkerW 窗口。 
                        IntPtr tempHwnd = Win32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);

                        // 隐藏这个窗口
                        Win32.ShowWindow(tempHwnd, 0);
                    }
                    return true;
                }, IntPtr.Zero);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo folder = new DirectoryInfo(Environment.CurrentDirectory + @"\MP4");//定位到应用程序位置
            try
            {
                foreach (FileInfo file in folder.GetFiles("*.mp4"))//在MP4文件夹下寻找并过滤视频文件
                {
                    MediaElement.Source = new Uri(file.FullName);
                    break;
                }
            }
            catch(ArgumentNullException)
            {
                MessageBox.Show("MP4文件夹下无MP4媒体文件");
            }
            catch(DirectoryNotFoundException)
            {
                MessageBox.Show("请在目录下添加MP4文件夹");
            }
            // 初始化桌面窗口
            Init();

            // 窗口置父，设置背景窗口的父窗口为 Program Manager 窗口
            IntPtr hwnd2 = new WindowInteropHelper(window).Handle;
            Win32.SetParent(hwnd2, programIntPtr);
        }

        private void MediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            window.WindowState = WindowState.Maximized;//窗口最大化
            (sender as MediaElement).Play();//视频播放
        }
        /// <summary>
        /// 视频循环播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            (sender as MediaElement).Stop();
            (sender as MediaElement).Play();
        }
    }
}
