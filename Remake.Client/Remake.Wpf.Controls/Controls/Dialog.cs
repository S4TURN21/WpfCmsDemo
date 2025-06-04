using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Remake.Wpf.Controls
{
    [TemplatePart(Name = CurrentContentPresenterPartName)]
    public class Dialog : Control
    {
        public const string CurrentContentPresenterPartName = "PART_CurrentContentPresenter";

        private ContentPresenter? _currentContentPresenter;

        public static readonly DependencyProperty CurrentPageProperty;
        public object? CurrentPage
        {
            get => GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        static Dialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Dialog), new FrameworkPropertyMetadata(typeof(Dialog)));

            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(object), typeof(Dialog), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CurrentPageChangedCallback));
        }

        private static void CurrentPageChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Dialog dialog = (Dialog)d;

            var newContent = e.NewValue;

            var currentContentPresenter = dialog._currentContentPresenter;

            if (currentContentPresenter == null) return;


            if (newContent != null)
            {
                currentContentPresenter.Content = newContent;

                dialog.Visibility = Visibility.Visible;
                //dialog.Background = new SolidColorBrush(Colors.Black);

                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.15),
                    EasingFunction = new CubicEase(),
                    AccelerationRatio = 0.25,
                    DecelerationRatio = 0.1
                };
                dialog.BeginAnimation(OpacityProperty, anim);
                //dialog.Background.BeginAnimation(Brush.OpacityProperty, anim);

                DoubleAnimation anim2 = new DoubleAnimation
                {
                    From = 0.7,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.15),
                    EasingFunction = new CubicEase(),
                    AccelerationRatio = 0.25,
                    DecelerationRatio = 0.1
                };
                currentContentPresenter.RenderTransform = new ScaleTransform();
                currentContentPresenter.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim2);
                currentContentPresenter.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim2);
            }
            else
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.15),
                    EasingFunction = new CubicEase(),
                    AccelerationRatio = 0.25,
                    DecelerationRatio = 0.1
                };
                anim.Completed += (o, e) =>
                {
                    currentContentPresenter.Content = null;
                    dialog.Visibility = Visibility.Hidden;
                };


                DoubleAnimation anim2 = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.7,
                    Duration = TimeSpan.FromSeconds(0.15),
                    EasingFunction = new CubicEase(),
                    AccelerationRatio = 0.25,
                    DecelerationRatio = 0.1
                };
                currentContentPresenter.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim2);
                currentContentPresenter.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim2);

                dialog.BeginAnimation(OpacityProperty, anim);
            }
        }

        public Dialog()
        {
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
        }

        //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonDown(e);
        //    if (e.Source != CurrentPage)
        //    {
        //        CurrentPage = null;
        //    }
        //}

        //protected override void OnContentChanged(object oldContent, object newContent)
        //{
        //    if (newContent != null)
        //    {
        //        Visibility = Visibility.Visible;
        //        this.Background = new SolidColorBrush(Colors.Black);

        //        //ColorAnimation anim = new ColorAnimation
        //        //{
        //        //    From = Color.FromArgb(0, 0, 0, 0),
        //        //    To = Color.FromArgb(102, 0, 0, 0),
        //        //    Duration = TimeSpan.FromSeconds(0.2),
        //        //    EasingFunction = new CubicEase(),
        //        //    AccelerationRatio = 0.25,
        //        //    DecelerationRatio = 0.1
        //        //};
        //        //this.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);

        //        DoubleAnimation anim = new DoubleAnimation
        //        {
        //            From = 0,
        //            To = 0.4,
        //            Duration = TimeSpan.FromSeconds(0.2),
        //            EasingFunction = new CubicEase(),
        //            AccelerationRatio = 0.25,
        //            DecelerationRatio = 0.1
        //        };
        //        this.Background.BeginAnimation(Brush.OpacityProperty, anim);
        //    }
        //    else
        //    {

        //        //ColorAnimation anim = new ColorAnimation
        //        //{
        //        //    From = Color.FromArgb(102, 0, 0, 0),
        //        //    To = Color.FromArgb(0, 0, 0, 0),
        //        //    Duration = TimeSpan.FromSeconds(0.2),
        //        //    EasingFunction = new CubicEase(),
        //        //    AccelerationRatio = 0.25,
        //        //    DecelerationRatio = 0.1
        //        //};
        //        //this.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);

        //        DoubleAnimation anim = new DoubleAnimation
        //        {
        //            From = 0.4,
        //            To = 0,
        //            Duration = TimeSpan.FromSeconds(0.2),
        //            EasingFunction = new CubicEase(),
        //            AccelerationRatio = 0.25,
        //            DecelerationRatio = 0.1
        //        };
        //        anim.Completed += (o, e) => Visibility = Visibility.Hidden;

        //        this.Background.BeginAnimation(Brush.OpacityProperty, anim);
        //        this.Background = null;
        //    }
        //}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _currentContentPresenter = Template.FindName(CurrentContentPresenterPartName, this) as ContentPresenter;
        }
    }
}
