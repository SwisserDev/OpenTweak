using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace OpenTweak.Helpers;

/// <summary>
/// Attached behavior that auto-scrolls a ScrollViewer to the bottom when content height changes.
/// </summary>
public static class AutoScrollBehavior
{
    public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached(
            "AutoScroll",
            typeof(bool),
            typeof(AutoScrollBehavior),
            new PropertyMetadata(false, OnAutoScrollChanged));

    // Store the previous extent height to detect content changes
    private static readonly DependencyProperty PreviousExtentHeightProperty =
        DependencyProperty.RegisterAttached(
            "PreviousExtentHeight",
            typeof(double),
            typeof(AutoScrollBehavior),
            new PropertyMetadata(0.0));

    public static bool GetAutoScroll(DependencyObject obj)
        => (bool)obj.GetValue(AutoScrollProperty);

    public static void SetAutoScroll(DependencyObject obj, bool value)
        => obj.SetValue(AutoScrollProperty, value);

    private static void OnAutoScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ScrollViewer scrollViewer)
            return;

        if ((bool)e.NewValue)
        {
            scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }
        else
        {
            scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }
    }

    private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer)
            return;

        // Only auto-scroll when content height increases (new items added)
        if (e.ExtentHeightChange > 0)
        {
            scrollViewer.ScrollToEnd();
        }
    }
}
