using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Collections;

namespace Prime.Wpf
{
    public class TransitionConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan duration = (TimeSpan)parameter;

            if (targetType == typeof(Brush) && (values[0] is SolidColorBrush || values[1] is SolidColorBrush))
            {
                var brushNew = values[0] as SolidColorBrush;
                var brushPrev = values[1] as SolidColorBrush;

                if (brushNew == null)
                {
                    return null;
                }

                var brushCore = brushPrev;

                if (brushCore == null)
                {
                    brushCore = new SolidColorBrush(brushNew.Color);
                }

                // 1. set TO opacity
                // 2. animate opacity

                //if(brushCore.Color == Colors.Transparent)
                //{
                //    brushCore.Opacity = 0;
                //}

                //brushNew.Color = Color.FromArgb((byte)Math.Floor(brushNew.Opacity >= 1.0 ? 255 : brushNew.Opacity * 256.0), brushNew.Color.R, brushNew.Color.G, brushNew.Color.B);

                ColorAnimation colorAnim = new ColorAnimation();
                colorAnim.Duration = duration;
                colorAnim.To = brushNew.Color;
                colorAnim.AccelerationRatio = 0.25f;
                colorAnim.DecelerationRatio = 0.25f;

                brushCore.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                //brushCore.BeginAnimation(SolidColorBrush.OpacityProperty, opacityAnim);

                return brushCore;
            }
            else if (targetType == typeof(IEnumerable) && (values[0] is BoxShadowCollection || values[1] is BoxShadowCollection))
            {
                var shadowsNew = values[0] as BoxShadowCollection;
                var shadowsPrev = values[1] as BoxShadowCollection;

                var shadowsNewLength = shadowsNew != null ? shadowsNew.Count : 0;
                var shadowsPrevLength = shadowsPrev != null ? shadowsPrev.Count : 0;

                var length = Math.Max(shadowsNewLength, shadowsPrevLength);

                //var core = new BoxShadowCollection();

                //for (int i = 0; i < length; i++)
                //{
                //    core.Add(new BoxShadow())
                //}

                //foreach (var item in shadowsPrev)
                //{

                //}

                var shadowsCore = new BoxShadowCollection();

                for (int i = 0; i < length; i++)
                {
                    if (shadowsNew != null)
                    {
                        var shadowCore = new BoxShadow();
                        shadowCore.SetValue(BoxShadow.ColorProperty, shadowsPrev != null && i < shadowsPrev.Count ? shadowsPrev[i].Color : Colors.Transparent);
                        shadowCore.SetValue(BoxShadow.XProperty, shadowsNew[i].X);
                        shadowCore.SetValue(BoxShadow.YProperty, shadowsNew[i].Y);
                        shadowCore.SetValue(BoxShadow.CornerRadiusProperty, shadowsNew[i].CornerRadius);
                        shadowCore.SetValue(BoxShadow.ShadowDepthProperty, shadowsNew[i].ShadowDepth);
                        shadowCore.SetValue(BoxShadow.BlurProperty, shadowsNew[i].Blur);
                        shadowCore.SetValue(BoxShadow.ShowBehindTransparentProperty, shadowsNew[i].ShowBehindTransparent);
                        shadowsCore.Add(shadowCore);

                        ColorAnimation colorAnim = new ColorAnimation();
                        colorAnim.Duration = duration;
                        colorAnim.To = i < shadowsNew.Count ? shadowsNew[i].Color : Colors.Transparent;
                        colorAnim.AccelerationRatio = 0.25f;
                        colorAnim.DecelerationRatio = 0.25f;
                        shadowCore.BeginAnimation(BoxShadow.ColorProperty, colorAnim);
                    }
                }

                return shadowsCore;//shadowsNew;
            }

            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TransitionExtension : MultiBinding
    {
        private static readonly IMultiValueConverter converter = new TransitionConverter();
        public TransitionExtension(DependencyProperty property, float duration)
        {
            this.Converter = converter;
            this.ConverterParameter = TimeSpan.FromSeconds(duration);

            this.Bindings.Add(new Binding()
            {
                Path = new PropertyPath(property),
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent)
            });

            this.Bindings.Add(new Binding()
            {
                Path = new PropertyPath(property),
                RelativeSource = new RelativeSource(RelativeSourceMode.Self)
            });
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Data;
//using System.Windows.Media.Animation;
//using System.Windows.Media;

//namespace Prime.Wpf
//{
//    public class TransitionConverter : IMultiValueConverter
//    {
//        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//        {
//            SolidColorBrush? brushNew = values[0] as SolidColorBrush;
//            SolidColorBrush? brushPrev = values[1] as SolidColorBrush;

//            TimeSpan duration = (TimeSpan)parameter;

//            if (brushNew == null)
//            {
//                return null;
//            }

//            var brushCore = brushPrev;

//            if (brushCore == null)
//            {
//                brushCore = new SolidColorBrush(brushNew.Color);
//            }

//            ColorAnimation anim = new ColorAnimation();
//            anim.Duration = duration;
//            anim.To = brushNew.Color;
//            anim.AccelerationRatio = 0.25f;
//            anim.DecelerationRatio = 0.25f;

//            brushCore.BeginAnimation(SolidColorBrush.ColorProperty, anim);

//            return brushCore;
//        }

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class TransitionExtension : MultiBinding
//    {
//        private static readonly IMultiValueConverter converter = new TransitionConverter();
//        public TransitionExtension(DependencyProperty property, float duration)
//        {
//            this.Converter = converter;
//            this.ConverterParameter = TimeSpan.FromSeconds(duration);

//            this.Bindings.Add(new Binding()
//            {
//                Path = new PropertyPath(property),
//                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent)
//            });

//            this.Bindings.Add(new Binding()
//            {
//                Path = new PropertyPath(property),
//                RelativeSource = new RelativeSource(RelativeSourceMode.Self)
//            });
//        }
//    }
//}
