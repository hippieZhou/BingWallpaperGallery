using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinggoWallpapers.WinUI.UserControls;
public sealed partial class InAppNotification : UserControl
{
    public StackedNotificationsBehavior NotificationQueue => StackedNotifications;

    public InAppNotification()
    {
        InitializeComponent();
    }
}
