using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;

namespace Prime.Wpf
{
    public class BoxShadowCollection : Collection<BoxShadow> { }

    public static class BoxShadowAssist
    {
        public static readonly DependencyProperty BoxShadowProperty;

        static BoxShadowAssist()
        {
            BoxShadowProperty = DependencyProperty.RegisterAttached("BoxShadow", typeof(BoxShadowCollection), typeof(BoxShadowAssist));
        }

        public static BoxShadowCollection GetBoxShadow(DependencyObject element) =>
            (BoxShadowCollection)element.GetValue(BoxShadowProperty);
        public static void SetBoxShadow(DependencyObject element, BoxShadowCollection value) =>
            element.SetValue(BoxShadowProperty, value);
    }

    public class BoxShadow : Decorator
    {
        public static readonly DependencyProperty ColorProperty;
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty ShadowDepthProperty;
        public static readonly DependencyProperty BlurProperty;
        public static readonly DependencyProperty XProperty;
        public static readonly DependencyProperty YProperty;
        public static readonly DependencyProperty ShowBehindTransparentProperty;

        static BoxShadow()
        {
            ShowBehindTransparentProperty = DependencyProperty.Register("ShowBehindTransparent", typeof(bool), typeof(BoxShadow),
               new FrameworkPropertyMetadata(false,
                   FrameworkPropertyMetadataOptions.AffectsRender));

            XProperty = DependencyProperty.Register("X", typeof(double), typeof(BoxShadow),
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsRender));
            YProperty = DependencyProperty.Register("Y", typeof(double), typeof(BoxShadow),
                new FrameworkPropertyMetadata(4d,
                    FrameworkPropertyMetadataOptions.AffectsRender));

            ShadowDepthProperty = DependencyProperty.Register("ShadowDepth", typeof(double), typeof(BoxShadow),
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsRender));

            BlurProperty = DependencyProperty.Register("Blur", typeof(double), typeof(BoxShadow),
                new FrameworkPropertyMetadata(4d,
                    FrameworkPropertyMetadataOptions.AffectsRender));

            ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(BoxShadow),
               new FrameworkPropertyMetadata(Color.FromArgb(64, 0, 0, 0),
                   FrameworkPropertyMetadataOptions.AffectsRender));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BoxShadow),
                new FrameworkPropertyMetadata(new CornerRadius(),
                    FrameworkPropertyMetadataOptions.AffectsRender));
        }

        public static Geometry DrawRoundedRectangle(Rect rect, CornerRadius cornerRadius)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                bool isStroked = false;
                const bool isSmoothJoin = true;

                context.BeginFigure(rect.TopLeft + new Vector(0, cornerRadius.TopLeft), true, true);
                context.ArcTo(new Point(rect.TopLeft.X + cornerRadius.TopLeft, rect.TopLeft.Y),
                    new Size(cornerRadius.TopLeft, cornerRadius.TopLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.TopRight - new Vector(cornerRadius.TopRight, 0), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.TopRight.X, rect.TopRight.Y + cornerRadius.TopRight),
                    new Size(cornerRadius.TopRight, cornerRadius.TopRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.BottomRight - new Vector(0, cornerRadius.BottomRight), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.BottomRight.X - cornerRadius.BottomRight, rect.BottomRight.Y),
                    new Size(cornerRadius.BottomRight, cornerRadius.BottomRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.BottomLeft + new Vector(cornerRadius.BottomLeft, 0), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.BottomLeft.X, rect.BottomLeft.Y - cornerRadius.BottomLeft),
                    new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);

                context.Close();
            }

            return geometry;
        }

        public bool ShowBehindTransparent
        {
            get { return (bool)GetValue(ShowBehindTransparentProperty); }
            set { SetValue(ShowBehindTransparentProperty, value); }
        }

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public double ShadowDepth
        {
            get { return (double)GetValue(ShadowDepthProperty); }
            set { SetValue(ShadowDepthProperty, value); }
        }
        public double Blur
        {
            get { return (double)GetValue(BlurProperty); }
            set { SetValue(BlurProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double x = X;
            double y = Y;
            Color color = Color;
            CornerRadius cornerRadius = CornerRadius;

            double spread = ShadowDepth;
            double blur = Blur;

            Rect bounds = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
            var inside = new Rect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);

            bounds.Offset(x, y);

            double boundsTop = bounds.Top;
            double boundsLeft = bounds.Left;
            double boundsRight = bounds.Right;
            double boundsBottom = bounds.Bottom;

            double[] guidelineSetX = new double[]
            {
                boundsLeft- (spread - blur),
                boundsLeft + Math.Max(cornerRadius.TopLeft, blur-spread),
                boundsRight - Math.Max(cornerRadius.TopRight, blur - spread),
                boundsLeft + Math.Max(cornerRadius.BottomLeft, blur-spread),
                boundsRight - Math.Max(cornerRadius.BottomRight, blur - spread),
                boundsRight + (spread-blur)
            };

            double[] guidelineSetY = new double[]
            {
                boundsTop - (spread - blur),
                boundsTop + Math.Max(cornerRadius.TopLeft, blur - spread),
                boundsTop + Math.Max(cornerRadius.TopRight, blur - spread),
                boundsBottom - Math.Max(cornerRadius.BottomLeft, blur - spread),
                boundsBottom - Math.Max(cornerRadius.BottomRight, blur - spread),
                boundsBottom + (spread-blur),
            };

            Brush[] brushes = GetBrushes(color, cornerRadius, spread, blur);

            drawingContext.PushGuidelineSet(new GuidelineSet(guidelineSetX, guidelineSetY));

            if (!ShowBehindTransparent)
            {

                var outside = new Rect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
                outside.Inflate(spread + blur, spread + blur);

                var clip = new GeometryGroup();
                clip.Children.Add(new RectangleGeometry(outside)); // outside
                clip.Children.Add(DrawRoundedRectangle(inside, cornerRadius)); // inside
                drawingContext.PushClip(clip);
            }

            Rect topLeft = new Rect(boundsLeft - (blur + spread), boundsTop - (blur + spread), (blur + spread) + Math.Max(cornerRadius.TopLeft, blur - spread), (blur + spread) + Math.Max(cornerRadius.TopLeft, blur - spread));
            drawingContext.DrawRectangle(brushes[TopLeft], null, topLeft);

            double topWidth = guidelineSetX[2] - guidelineSetX[1];
            if (topWidth > 0)
            {
                Rect top = new Rect(guidelineSetX[1], boundsTop - (blur + spread), topWidth, 2.0d * blur);
                drawingContext.DrawRectangle(brushes[Top], null, top);
            }

            Rect topRight = new Rect(guidelineSetX[2], boundsTop - (blur + spread), (blur + spread) + Math.Max(cornerRadius.TopRight, blur - spread), (blur + spread) + Math.Max(cornerRadius.TopRight, blur - spread));
            drawingContext.DrawRectangle(brushes[TopRight], null, topRight);

            double leftHeight = guidelineSetY[3] - guidelineSetY[1];
            if (leftHeight > 0)
            {
                Rect left = new Rect(boundsLeft - (blur + spread), guidelineSetY[1], 2.0d * blur, leftHeight);
                drawingContext.DrawRectangle(brushes[Left], null, left);
            }

            double rightHeight = guidelineSetY[4] - guidelineSetY[2];
            if (rightHeight > 0)
            {
                Rect right = new Rect(guidelineSetX[5], guidelineSetY[2], 2.0d * blur, rightHeight);
                drawingContext.DrawRectangle(brushes[Right], null, right);
            }

            Rect bottomLeft = new Rect(boundsLeft - (blur + spread), guidelineSetY[3], (blur + spread) + Math.Max(cornerRadius.BottomLeft, blur - spread), (blur + spread) + Math.Max(cornerRadius.BottomLeft, blur - spread));
            drawingContext.DrawRectangle(brushes[BottomLeft], null, bottomLeft);

            double bottomWidth = guidelineSetX[4] - guidelineSetX[3];
            if (bottomWidth > 0)
            {
                Rect bottom = new Rect(guidelineSetX[3], guidelineSetY[5], bottomWidth, 2.0d * blur);
                drawingContext.DrawRectangle(brushes[Bottom], null, bottom);
            }

            Rect bottomRight = new Rect(guidelineSetX[4], guidelineSetY[4], (blur + spread) + Math.Max(cornerRadius.BottomRight, blur - spread), (blur + spread) + Math.Max(cornerRadius.BottomRight, blur - spread));
            drawingContext.DrawRectangle(brushes[BottomRight], null, bottomRight);

            if (cornerRadius.TopLeft == ShadowDepth &&
                    cornerRadius.TopLeft == cornerRadius.TopRight &&
                    cornerRadius.TopLeft == cornerRadius.BottomLeft &&
                    cornerRadius.TopLeft == cornerRadius.BottomRight)
            {
                // All corners of target are 0, render one large rectangle
                //Rect center = new Rect(guidelineSetX[0], guidelineSetY[0], centerWidth, centerHeight);
                //drawingContext.DrawRectangle(brushes[Center], null, center);
            }
            else
            {
                // If the corner radius is TL=2, TR=1, BL=0, BR=2 the following shows the shape that needs to be created.
                //             _________________
                //            |                 |_
                //         _ _|                   |
                //        |                       |
                //        |                    _ _|
                //        |                   |   
                //        |___________________| 
                // The missing corners of the shape are filled with the radial gradients drawn above

                // Define shape counter clockwise
                PathFigure figure = new PathFigure();


                //if (cornerRadius.TopLeft > ShadowDepth)
                //{
                figure.StartPoint = new Point(guidelineSetX[1], guidelineSetY[0]);
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[1], guidelineSetY[1]), true));
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[0], guidelineSetY[1]), true));
                //}
                //else
                //{
                //    figure.StartPoint = new Point(guidelineSetX[0], guidelineSetY[0]);
                //}

                //if (cornerRadius.BottomLeft > ShadowDepth)
                //{
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[0], guidelineSetY[3]), true));
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[3], guidelineSetY[3]), true));
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[3], guidelineSetY[5]), true));
                //}
                //else
                //{
                //    figure.Segments.Add(new LineSegment(new Point(guidelineSetX[0], guidelineSetY[5]), true));
                //}

                //if (cornerRadius.BottomRight > ShadowDepth)
                //{
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[4], guidelineSetY[5]), true));
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[4], guidelineSetY[4]), true));
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[5], guidelineSetY[4]), true));
                //}
                //else
                //{
                //    figure.Segments.Add(new LineSegment(new Point(guidelineSetX[5], guidelineSetY[5]), true));
                //}


                //if (cornerRadius.TopRight > ShadowDepth)
                //{
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[5], guidelineSetY[2]), true));
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[2], guidelineSetY[2]), true));
                figure.Segments.Add(new LineSegment(new Point(guidelineSetX[2], guidelineSetY[0]), true));
                //}
                //else
                //{
                //    figure.Segments.Add(new LineSegment(new Point(guidelineSetX[5], guidelineSetY[0]), true));
                //}

                figure.IsClosed = true;
                figure.Freeze();

                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                geometry.Freeze();

                drawingContext.DrawGeometry(brushes[Center], null, geometry);
            }

            drawingContext.Pop();
        }

        private const int steps = 10;

        private static GradientStopCollection CreateSideStops(Color c, double shadowDepth, double blur)
        {
            Color stopColor = c;

            EasingFunctionBase ease = new QuadraticEase();
            ease.EasingMode = EasingMode.EaseInOut;

            GradientStopCollection gsc = new GradientStopCollection();

            gsc.Add(new GradientStop(c, 0));

            for (int i = steps - 1; i >= 0; i--)
            {
                double t = i / (double)steps;

                stopColor.A = (byte)(ease.Ease(t) * c.A);

                gsc.Add(new GradientStop(stopColor, 1 - t));
            }

            gsc.Freeze();

            return gsc;
        }

        private static GradientStopCollection CreateCornerStops(Color c, double cornerRadius, double shadowDepth, double blur)
        {
            Color stopColor = c;

            EasingFunctionBase ease = new QuadraticEase();
            ease.EasingMode = EasingMode.EaseInOut;

            double gradientScale = 1 / (cornerRadius + blur + shadowDepth);

            GradientStopCollection gsc = new GradientStopCollection();
            double start = (Math.Max(cornerRadius, blur - shadowDepth) + shadowDepth - blur) * gradientScale;

            gsc.Add(new GradientStop(c, start));

            for (int i = steps - 1; i >= 0; i--)
            {
                double t = i / (double)steps;
                var offset = start + (1 - start) * t;
                stopColor.A = (byte)(ease.Ease(1 - t) * c.A);

                gsc.Add(new GradientStop(stopColor, offset));
            }
            stopColor.A = 0;
            gsc.Add(new GradientStop(stopColor, 1));

            gsc.Freeze();

            return gsc;
        }

        private Brush[] GetBrushes(Color c, CornerRadius cornerRadius, double shadowDepth, double blur)
        {
            Brush[] brushes = new Brush[9];

            brushes[Center] = new SolidColorBrush(c);
            brushes[Center].Freeze();

            GradientStopCollection sideStops = CreateSideStops(c, shadowDepth, blur);

            LinearGradientBrush top = new LinearGradientBrush(sideStops, new Point(0, 1), new Point(0, 0));
            top.Freeze();
            brushes[Top] = top;

            LinearGradientBrush left = new LinearGradientBrush(sideStops, new Point(1, 0), new Point(0, 0));
            left.Freeze();
            brushes[Left] = left;

            LinearGradientBrush right = new LinearGradientBrush(sideStops, new Point(0, 0), new Point(1, 0));
            right.Freeze();
            brushes[Right] = right;

            LinearGradientBrush bottom = new LinearGradientBrush(sideStops, new Point(0, 0), new Point(0, 1));
            bottom.Freeze();
            brushes[Bottom] = bottom;

            GradientStopCollection topLeftStops;
            if (cornerRadius.TopLeft == 0)
                topLeftStops = sideStops;
            else
                topLeftStops = CreateCornerStops(c, cornerRadius.TopLeft, shadowDepth, blur);

            RadialGradientBrush topLeft = new RadialGradientBrush(topLeftStops);
            topLeft.RadiusX = 1;
            topLeft.RadiusY = 1;
            topLeft.Center = new Point(1, 1);
            topLeft.GradientOrigin = new Point(1, 1);
            topLeft.Freeze();
            brushes[TopLeft] = topLeft;

            GradientStopCollection topRightStops;
            if (cornerRadius.TopRight == 0)
                topRightStops = sideStops;
            else if (cornerRadius.TopRight == cornerRadius.TopLeft)
                topRightStops = topLeftStops;
            else
                topRightStops = CreateCornerStops(c, cornerRadius.TopRight, shadowDepth, blur);

            RadialGradientBrush topRight = new RadialGradientBrush(topRightStops);
            topRight.RadiusX = 1;
            topRight.RadiusY = 1;
            topRight.Center = new Point(0, 1);
            topRight.GradientOrigin = new Point(0, 1);
            topRight.Freeze();
            brushes[TopRight] = topRight;

            GradientStopCollection bottomLeftStops;
            if (cornerRadius.BottomLeft == 0)
                bottomLeftStops = sideStops;
            else if (cornerRadius.BottomLeft == cornerRadius.TopLeft)
                bottomLeftStops = topLeftStops;
            else if (cornerRadius.BottomLeft == cornerRadius.TopRight)
                bottomLeftStops = topRightStops;
            else
                bottomLeftStops = CreateCornerStops(c, cornerRadius.BottomLeft, shadowDepth, blur);

            RadialGradientBrush bottomLeft = new RadialGradientBrush(bottomLeftStops);
            bottomLeft.RadiusX = 1;
            bottomLeft.RadiusY = 1;
            bottomLeft.Center = new Point(1, 0);
            bottomLeft.GradientOrigin = new Point(1, 0);
            bottomLeft.Freeze();
            brushes[BottomLeft] = bottomLeft;

            GradientStopCollection bottomRightStops;

            if (cornerRadius.BottomRight == 0)
                bottomRightStops = sideStops;
            else if (cornerRadius.BottomRight == cornerRadius.TopLeft)
                bottomRightStops = topLeftStops;
            else if (cornerRadius.BottomRight == cornerRadius.TopRight)
                bottomRightStops = topRightStops;
            else if (cornerRadius.BottomRight == cornerRadius.BottomLeft)
                bottomRightStops = bottomLeftStops;
            else
                bottomRightStops = CreateCornerStops(c, cornerRadius.BottomRight, shadowDepth, blur);

            RadialGradientBrush bottomRight = new RadialGradientBrush(bottomRightStops);

            bottomRight.RadiusX = 1;
            bottomRight.RadiusY = 1;
            bottomRight.Center = new Point(0, 0);
            bottomRight.GradientOrigin = new Point(0, 0);
            bottomRight.Freeze();
            brushes[BottomRight] = bottomRight;

            return brushes;
        }

        private const int TopLeft = 0;
        private const int Top = 1;
        private const int TopRight = 2;
        private const int Left = 3;
        private const int Center = 4;
        private const int Right = 5;
        private const int BottomLeft = 6;
        private const int Bottom = 7;
        private const int BottomRight = 8;
    }
}
