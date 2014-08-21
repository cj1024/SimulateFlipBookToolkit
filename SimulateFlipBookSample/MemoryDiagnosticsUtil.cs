using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Info;

namespace SimulateFlipBookSample
{
    public static class MemoryDiagnosticsUtil
    {
        private static Popup popup;
        private static TextBlock currentMemoryBlock;
        private static TextBlock appUsageMemoryBlock;
        private static DispatcherTimer timer;

        public static void Start()
        {
            CreatePopup();
            CreateTimer();
            ShowPopup();
            StartTimer();
        }

        private static void ShowPopup()
        {
            popup.IsOpen = true;
        }

        private static void StartTimer()
        {
            timer.Start();
        }

        private static void CreateTimer()
        {
            if (timer != null)
                return;
            timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(300)};
            timer.Tick += timer_Tick;
        }

        private static void timer_Tick(object sender, EventArgs e)
        {
            appUsageMemoryBlock.Text = (((double) DeviceStatus.ApplicationCurrentMemoryUsage)/
                 DeviceStatus.ApplicationMemoryUsageLimit).ToString("F4");
            currentMemoryBlock.Text = ((double)DeviceStatus.ApplicationCurrentMemoryUsage / (1024 * 1024)).ToString("F4");
        }

        private static void CreatePopup()
        {
            if (popup != null)
                return;

            popup = new Popup();
            double fontSize = (double) Application.Current.Resources["PhoneFontSizeSmall"] - 2;
            var foreground = (Brush) Application.Current.Resources["PhoneForegroundBrush"];
            var sp = new StackPanel {Orientation = Orientation.Horizontal, Background = (Brush) Application.Current.Resources["PhoneSemitransparentBrush"]};
            currentMemoryBlock = new TextBlock {Text = "---", FontSize = fontSize, Foreground = foreground};
            appUsageMemoryBlock = new TextBlock {Text = "---", FontSize = fontSize, Foreground = foreground};
            sp.Children.Add(new TextBlock {Text = "Mem(M): ", FontSize = fontSize, Foreground = foreground});
            sp.Children.Add(currentMemoryBlock);
            sp.Children.Add(new TextBlock {Text = "---Rate: ", FontSize = fontSize, Foreground = foreground});
            sp.Children.Add(appUsageMemoryBlock);
            sp.RenderTransform = new CompositeTransform {Rotation = 90, TranslateX = 480, TranslateY = 420, CenterX = 0, CenterY = 0};
            popup.Child = sp;
        }

        public static void Stop()
        {
            HidePopup();
            StopTimer();
        }

        private static void StopTimer()
        {
            timer.Stop();
        }

        private static void HidePopup()
        {
            popup.IsOpen = false;
        }
    }
}
