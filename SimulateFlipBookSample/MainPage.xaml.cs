using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using SimulateFlipBookToolkit;

namespace SimulateFlipBookSample
{
    public partial class MainPage
    {

        private WriteableBitmapTransformer transformer;

        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var source = new WriteableBitmap(Source, null);
            source.Invalidate();
            transformer = new WriteableBitmapTransformer(source);
            UpdateTarget(transformer.GenerateTransformedWriteableBitmap(1, 0, -LayoutRoot.ActualWidth));
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        private Point startPoint;

        private void Target_OnMouseEnter(object sender, MouseEventArgs e)
        {
            startPoint = e.GetPosition(Target);
            startPoint = new Point(startPoint.X, Target.ActualHeight - startPoint.Y);
            if (startPoint.Y < Target.ActualHeight / 3)
            {
                startPoint = new Point(Target.ActualWidth, 0);
            }
            else if (startPoint.Y <= Target.ActualHeight *2 / 3)
            {
                startPoint = new Point(Target.ActualWidth, Target.ActualHeight/2);
            }
            else
            {
                startPoint = new Point(Target.ActualWidth, Target.ActualHeight);
            }
        }

        private void Target_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (transformer != null)
            {
                var p1 = e.GetPosition(Target);
                p1 = new Point(p1.X, Target.ActualHeight - p1.Y);
                var p2 = startPoint;
                UpdateTarget(p2.Y == p1.Y ? transformer.GenerateTransformedWriteableBitmap(1, 0, (p1.X - p2.X) / 2 - p1.X) : transformer.GenerateTransformedWriteableBitmap((p2.X - p1.X) / (p1.Y - p2.Y), -1, (p1.Y + p2.Y) / 2 + ((p2.X - p1.X) / (p2.Y - p1.Y) * (p1.X + p2.X) / 2)));
            }
        }

        private void Target_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (transformer != null)
            {
                UpdateTarget(transformer.GenerateTransformedWriteableBitmap(1, 0, -Target.ActualWidth));
            }
        }

        void UpdateTarget(ImageSource source)
        {
            Target.Source = source;
        }

    }
}