using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Prime.Wpf
{
    [TemplatePart(Name = PART_SkeletonGradient, Type = typeof(UIElement))]
    public class Skeleton : Control
    {
        private const string PART_SkeletonGradient = "PART_SkeletonGradient";
        private TranslateTransform? _translateTransform;
        private UIElement? SkeletonGradient;

        static Skeleton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Skeleton), new FrameworkPropertyMetadata(typeof(Skeleton)));
        }

        public Skeleton()
        {
            Loaded += Skeleton_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SkeletonGradient = GetTemplateChild(PART_SkeletonGradient) as UIElement;
            if (SkeletonGradient != null)
            {
                SkeletonGradient.RenderTransform = new TranslateTransform();
            }
        }

        private void Skeleton_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => StartAnimation()));
        }

        private void StartAnimation()
        {
            if (SkeletonGradient != null)
            {
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = -this.ActualWidth,
                    To = this.ActualWidth,
                    Duration = TimeSpan.FromSeconds(1.2),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                SkeletonGradient.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
            }
        }
    }
}
