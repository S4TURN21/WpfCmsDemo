using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Remake.Wpf.Controls
{
    [TemplatePart(Name = CurrentContentPresenterPartName)]
    [TemplatePart(Name = PreviousContentPresenterPartName)]
    public class Transitioner : Control
    {
        public const string CurrentContentPresenterPartName = "PART_CurrentContentPresenter";
        public const string PreviousContentPresenterPartName = "PART_PreviousContentPresenter";

        private ContentPresenter? _currentContentPresenter;
        private ContentPresenter? _previousContentPresenter;

        public static readonly DependencyProperty CurrentPageProperty;
        public object? CurrentPage
        {
            get => GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        static Transitioner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Transitioner), new FrameworkPropertyMetadata(typeof(Transitioner)));

            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(object), typeof(Transitioner), new PropertyMetadata(null, CurrentPageChangedCallback));
        }
  
        private static void CurrentPageChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Transitioner transitioner = (Transitioner)d;

            var previousPage = e.OldValue;
            var currentPage = e.NewValue;

            var currentContentPresenter = transitioner._currentContentPresenter;
            var previousContentPresenter = transitioner._previousContentPresenter;

            if (currentContentPresenter == null || previousContentPresenter == null) return;

            previousContentPresenter.Content = null;
            currentContentPresenter.Content = currentPage;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _currentContentPresenter = Template.FindName(CurrentContentPresenterPartName, this) as ContentPresenter;
            _previousContentPresenter = Template.FindName(PreviousContentPresenterPartName, this) as ContentPresenter;

            if (_currentContentPresenter != null)
            {
                _currentContentPresenter.Content = CurrentPage;
            }
        }
    }
}
