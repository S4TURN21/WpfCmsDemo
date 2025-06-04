using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prime.Wpf
{
    public static class ScrollViewerAssist
    {
        public static readonly DependencyProperty IgnorePaddingProperty =
            DependencyProperty.RegisterAttached("IgnorePadding", typeof(bool), typeof(ScrollViewerAssist), new PropertyMetadata(false));

        public static void SetIgnorePadding(DependencyObject element, bool value) => element.SetValue(IgnorePaddingProperty, value);
        public static bool GetIgnorePadding(DependencyObject element) => (bool)element.GetValue(IgnorePaddingProperty);
    }
}
