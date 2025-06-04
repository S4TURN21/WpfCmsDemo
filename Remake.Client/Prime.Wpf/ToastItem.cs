using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Prime.Wpf
{
    public class ToastItemCloseEventArgs : EventArgs
    {
        public Message? Message { get; }
        public ToastItemCloseEventArgs(Message? message)
        {
            this.Message = message;
        }
    }

    [TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
    public class ToastItem : Control
    {
        private const string PART_CloseButton = "PART_CloseButton";

        #region Message
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(Message), typeof(ToastItem));
        public Message? Message
        {
            get
            {
                return (Message)GetValue(MessageProperty);
            }

            set
            {
                SetValue(MessageProperty, value);
            }
        }
        #endregion

        #region Show Animation
        public static readonly DependencyProperty ShowAnimationProperty = DependencyProperty.Register("ShowAnimation", typeof(Storyboard), typeof(ToastItem));
        public Storyboard? ShowAnimation
        {
            get
            {
                return (Storyboard)GetValue(ShowAnimationProperty);
            }

            set
            {
                SetValue(ShowAnimationProperty, value);
            }
        }
        #endregion

        #region Hide Animation
        public static readonly DependencyProperty HideAnimationProperty = DependencyProperty.Register("HideAnimation", typeof(Storyboard), typeof(ToastItem));
        public Storyboard? HideAnimation
        {
            get
            {
                return (Storyboard)GetValue(HideAnimationProperty);
            }

            set
            {
                SetValue(HideAnimationProperty, value);
            }
        }
        #endregion

        static ToastItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToastItem), new FrameworkPropertyMetadata(typeof(ToastItem)));
        }

        public event EventHandler<ToastItemCloseEventArgs>? OnClosed;

        private ButtonBase? _closeButton;
        private CancellationTokenSource? _cancelationTokenSource;
        private DispatcherOperation<Task>? _timeOut;

        private ButtonBase? CloseButton
        {
            get
            {
                return _closeButton;
            }
            set
            {
                if (_closeButton != null)
                {
                    _closeButton.Click -= OnCloseButtonClick;
                }

                _closeButton = value;

                if (_closeButton != null)
                {
                    _closeButton.Click += OnCloseButtonClick;
                }
            }
        }

        public ToastItem()
        {
            this.Loaded += ToastItem_Loaded;

        }

        private void ToastItem_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitTimeout();

            var renderGroup = new TransformGroup();
            renderGroup.Children.Add(new TranslateTransform());
            renderGroup.Children.Add(new ScaleTransform());

            this.RenderTransform = renderGroup;

            var layoutGroup = new TransformGroup();
            layoutGroup.Children.Add(new TranslateTransform());
            layoutGroup.Children.Add(new ScaleTransform());

            this.LayoutTransform = layoutGroup;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation animY = new DoubleAnimation
            {
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                From = this.ActualHeight,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                AccelerationRatio = 0.25,
                DecelerationRatio = 0.1
            };

            Storyboard.SetTargetProperty(animY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
            Storyboard.SetTarget(animY, this);

            DoubleAnimation animOpacity = new DoubleAnimation
            {
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3),
                AccelerationRatio = 0.25,
                DecelerationRatio = 0.1
            };
            Storyboard.SetTargetProperty(animOpacity, new PropertyPath(UIElement.OpacityProperty));
            Storyboard.SetTarget(animOpacity, this);

            storyboard.Children.Add(animOpacity);
            storyboard.Children.Add(animY);
            storyboard.Begin();

            ShowAnimation?.Begin(this);
        }

        private void InitTimeout()
        {
            if (this.Message?.Sticky != true)
            {
                _cancelationTokenSource = new CancellationTokenSource();
                var token = _cancelationTokenSource.Token;
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await Task.Delay(Message?.Life ?? 3000, token);

                    this.OnClosed?.Invoke(this, new ToastItemCloseEventArgs(Message));
                }, DispatcherPriority.Normal, token);
            }

            //this.timeOut = Task.Run(() =>
            //{
            //    //await Task.Delay(Message?.Life ?? 3000);

            //    this.OnClosed?.Invoke(this, new ToastItemCloseEventArgs(Message));
            //});
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            CloseButton = GetTemplateChild(PART_CloseButton) as ButtonBase;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.ClearTimeout();

            this.OnClosed?.Invoke(this, new ToastItemCloseEventArgs(Message));
        }

        private void ClearTimeout()
        {
            //_timeOut?.Abort();
            if (_cancelationTokenSource != null)
            {
                _cancelationTokenSource.Cancel();
                _cancelationTokenSource = null;
            }
        }
    }

    public class Toast : ItemsControl
    {
        static Toast()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Toast), new FrameworkPropertyMetadata(typeof(Toast)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var toastItem = new ToastItem();
            toastItem.OnClosed += ToastItem_OnClosed;
            return toastItem;
        }

        private void ToastItem_OnClosed(object? sender, ToastItemCloseEventArgs e)
        {
            if (sender is ToastItem toastItem && e.Message is Message message)
            {
                var itemsSource = this.ItemsSource as IList;

                if (itemsSource != null && itemsSource.Contains(message))
                {
                    var renderGroup = new TransformGroup();
                    renderGroup.Children.Add(new TranslateTransform());
                    renderGroup.Children.Add(new ScaleTransform());

                    toastItem.RenderTransform = renderGroup;

                    var layoutGroup = new TransformGroup();
                    layoutGroup.Children.Add(new TranslateTransform());
                    layoutGroup.Children.Add(new ScaleTransform());

                    toastItem.LayoutTransform = layoutGroup;

                    Storyboard storyboard = new Storyboard();

                    DoubleAnimation animY = new DoubleAnimation
                    {
                        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                        To = -72,
                        Duration = TimeSpan.FromSeconds(0.25),
                        AccelerationRatio = 0.25,
                        DecelerationRatio = 0.1
                    };

                    Storyboard.SetTargetProperty(animY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
                    Storyboard.SetTarget(animY, toastItem);

                    DoubleAnimation animOpacity = new DoubleAnimation
                    {
                        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.25),
                        AccelerationRatio = 0.25,
                        DecelerationRatio = 0.1
                    };
                    Storyboard.SetTargetProperty(animOpacity, new PropertyPath(UIElement.OpacityProperty));
                    Storyboard.SetTarget(animOpacity, toastItem);

                    DoubleAnimation animHeight = new DoubleAnimation
                    {
                        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.25),
                        AccelerationRatio = 0.25,
                        DecelerationRatio = 0.1
                    };

                    Storyboard.SetTargetProperty(animHeight, new PropertyPath("(FrameworkElement.LayoutTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
                    Storyboard.SetTarget(animHeight, toastItem);

                    storyboard.Children.Add(animOpacity);
                    storyboard.Children.Add(animY);
                    storyboard.Children.Add(animHeight);

                    storyboard.Completed += (o, e) =>
                    {
                        itemsSource.Remove(message);
                    };

                    storyboard.Begin();
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ToastItem;
        }
    }
}
