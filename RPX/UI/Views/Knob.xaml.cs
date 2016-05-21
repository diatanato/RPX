using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RPX.UI.Views
{
    using Utils;

    public partial class Knob
    {
        #region DependencyProperties

        public static DependencyProperty Template1Property     = DependencyProperty<Knob>.Register<DataTemplate>(nameof(Template1));
        public static DependencyProperty Template2Property     = DependencyProperty<Knob>.Register<DataTemplate>(nameof(Template2));

        public static DependencyProperty ValueProperty         = DependencyProperty<Knob>.Register<Object>(nameof(Value),    (UInt32)0,   x => x.OnKnobPropertyChanged);
        public static DependencyProperty MinValueProperty      = DependencyProperty<Knob>.Register<Object>(nameof(MinValue), (UInt32)0,   x => x.OnKnobPropertyChanged);
        public static DependencyProperty MaxValueProperty      = DependencyProperty<Knob>.Register<Object>(nameof(MaxValue), (UInt32)100, x => x.OnKnobPropertyChanged);

        public static DependencyProperty RotationProperty      = DependencyProperty<Knob>.Register<RotateTransform>(nameof(Rotation));
        public static DependencyProperty MinRotationProperty   = DependencyProperty<Knob>.Register<Object>(nameof(MinRotation), 0.0,   x => x.OnKnobPropertyChanged);
        public static DependencyProperty MaxRotationProperty   = DependencyProperty<Knob>.Register<Object>(nameof(MaxRotation), 360.0, x => x.OnKnobPropertyChanged);
        public static DependencyProperty RotationPointProperty = DependencyProperty<Knob>.Register(nameof(RotationPoint), new Point(), x => x.OnKnobCenterChanged);

        public DataTemplate Template1
        {
            get { return GetValue(Template1Property) as DataTemplate; }
            set { SetValue(Template1Property, value); }
        }

        public DataTemplate Template2
        {
            get { return GetValue(Template2Property) as DataTemplate; }
            set { SetValue(Template2Property, value); }
        }

        public UInt32 Value
        {
            get { return (UInt32)GetValue(ValueProperty);}
            set { SetValue(ValueProperty, Math.Min(Math.Max(value, MinValue), MaxValue)); }
        }

        public UInt32 MinValue
        {
            get { return (UInt32)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, Math.Min(value, MaxValue)); }
        }

        public UInt32 MaxValue
        {
            get { return (UInt32)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, Math.Max(value, MinValue)); }
        }

        public RotateTransform Rotation
        {
            get { return GetValue(RotationProperty) as RotateTransform; }
            private set { SetValue(RotationProperty, value); }
        }

        public double MinRotation
        {
            get { return (double)GetValue(MinRotationProperty); }
            set { SetValue(MinRotationProperty, Math.Min(value, MaxRotation)); }
        }

        public double MaxRotation
        {
            get { return (double)GetValue(MaxRotationProperty); }
            set { SetValue(MaxRotationProperty, Math.Max(value, MinRotation)); }
        }

        public Point RotationPoint
        {
            get { return (Point)GetValue(RotationPointProperty); }
            set { SetValue(RotationPointProperty, value); }
        }

        #endregion

        public Knob()
        {
            Rotation = new RotateTransform();

            InitializeComponent();
        }

        private void OnKnobPropertyChanged(DependencyPropertyChangedEventArgs<Object> e)
        {
            Rotation.Angle = (MaxRotation - MinRotation) / (MaxValue - MinValue) * (Value - MinValue) + MinRotation;
        }

        private void OnKnobCenterChanged(DependencyPropertyChangedEventArgs<Point> e)
        {
            Rotation.CenterX = RotationPoint.X;
            Rotation.CenterY = RotationPoint.Y;
        }

        private void ContentControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Value = (UInt16)Math.Max(Value + 5 * (e.Delta / Math.Abs(e.Delta)), MinValue);
        }
    }
}
