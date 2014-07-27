using Kinect.Toolbox;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FullTotal.ImageTransformations
{
    public class ZoomBorder: Border
    {
        private UIElement child = null;
        private Point origin;
        private Point start;
        KinectRegion kinectRegion;
        bool isRightGripInteraction = false;
        bool isLeftGripInteraction = false;
       
        public delegate void GestureDelegate(string gestureName);
        public event GestureDelegate StartStretchGestureFollowing;
        public event GestureDelegate EndStretchGestureFollowing;
        

        public void AssignKinectRegion(KinectRegion kinectRegion)
        {
            this.kinectRegion = kinectRegion;

            if (kinectRegion != null)
            {
                KinectRegion.RemoveQueryInteractionStatusHandler(this.child, OnQuery); //usuń stare powiązania
                KinectRegion.AddQueryInteractionStatusHandler(this.child, OnQuery); //obsluga medicalImage przez kinectRegion
                KinectRegion.RemoveQueryInteractionStatusHandler(this, OnQuery); //usuń stare powiązania
                KinectRegion.AddQueryInteractionStatusHandler(this, OnQuery); //obsluga ZoomBorder przez kinectRegion
                KinectRegion.RemoveHandPointerGripHandler(this.child, OnPointerGrip); //usuń stare powiązania
                KinectRegion.AddHandPointerGripHandler(this.child, OnPointerGrip);
                KinectRegion.RemoveHandPointerMoveHandler(this.child, OnPointerMove); //usuń stare powiązania
                KinectRegion.AddHandPointerMoveHandler(this.child, OnPointerMove);
                KinectRegion.RemoveHandPointerGripReleaseHandler(this.child, OnPointerGripRelease); //usuń stare powiązania
                KinectRegion.AddHandPointerGripReleaseHandler(this.child, OnPointerGripRelease);
                KinectRegion.RemoveHandPointerLeaveHandler(this.child, OnPointerLeave); //usuń stare powiązania
                KinectRegion.AddHandPointerLeaveHandler(this.child, OnPointerLeave); ////uwolnij uścisk gdy łapka schodzi z image
            }
        }



        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        private RotateTransform GetRotateTransform(UIElement element)
        {
            return (RotateTransform)((TransformGroup)element.RenderTransform)
                .Children.First(tr => tr is RotateTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                //added by Ala
                RotateTransform rt = new RotateTransform();
                group.Children.Add(rt);
                //
                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);
              
                this.MouseWheel += child_MouseWheel;
                this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(child_PreviewMouseRightButtonDown);

                if (kinectRegion != null)
            {
                KinectRegion.RemoveQueryInteractionStatusHandler(this.child, OnQuery); //usuń stare powiązania
                KinectRegion.AddQueryInteractionStatusHandler(this.child, OnQuery); //obsluga medicalImage przez kinectRegion
                KinectRegion.RemoveHandPointerGripHandler(this.child, OnPointerGrip); //usuń stare powiązania
                KinectRegion.AddHandPointerGripHandler(this.child, OnPointerGrip);
                KinectRegion.RemoveHandPointerMoveHandler(this.child, OnPointerMove); //usuń stare powiązania
                KinectRegion.AddHandPointerMoveHandler(this.child, OnPointerMove);
                KinectRegion.RemoveHandPointerGripReleaseHandler(this.child, OnPointerGripRelease); //usuń stare powiązania
                KinectRegion.AddHandPointerGripReleaseHandler(this.child, OnPointerGripRelease);
                KinectRegion.RemoveHandPointerLeaveHandler(this.child, OnPointerLeave); //usuń stare powiązania
                KinectRegion.AddHandPointerLeaveHandler(this.child, OnPointerLeave); //uwolnij uścisk gdy łapka schodzi z image
                }
            }
        }
        
        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;

                //reset rotation
                var rt = GetRotateTransform(child);
                rt.CenterX = 0.0;
                rt.CenterY = 0.0;
                rt.Angle = 0;
            }
        }


        #region Kinect events
      
        //rozluźnij łapkę, gdy wyjazd za foto
        private void OnPointerLeave(object sender, HandPointerEventArgs e)
        {
            if (e.HandPointer.HandType == HandType.Right)
                isRightGripInteraction = false;
            else
                isLeftGripInteraction = false;
            e.HandPointer.IsInGripInteraction = false;
        }


        //grip -> move -> release = przesuwanie
        private void OnPointerGrip(object sender, HandPointerEventArgs e)
        {
            if (e.HandPointer.HandType == HandType.Right)
            {   
                var tt = GetTranslateTransform(child);
                start = e.HandPointer.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                e.HandPointer.Capture(child);
            }
        }

        private void OnPointerMove(object sender, HandPointerEventArgs e)
        {
            if (e.HandPointer.HandType == HandType.Right && !isLeftGripInteraction)
            {
                if (child != null)
                {
                    if (e.HandPointer.Captured == child)
                    {
                        var tt = GetTranslateTransform(child);
                        Vector v = start - e.HandPointer.GetPosition(this);
                        tt.X = origin.X - v.X;
                        tt.Y = origin.Y - v.Y;
                    }
                }
            }
            else if (isRightGripInteraction && isLeftGripInteraction)
            {
                if (child != null)
                {
                    if (e.HandPointer.Captured == null)
                    {
                        if (StartStretchGestureFollowing != null)
                            StartStretchGestureFollowing("StretchGesture");
                    }
                }
            }
        }

        private void OnPointerGripRelease(object sender, HandPointerEventArgs e)
        {
            if (child != null)
            {
                e.HandPointer.Capture(null);
                if (EndStretchGestureFollowing != null)
                    EndStretchGestureFollowing("StretchGesture");
            }
        }

        private void OnQuery(object sender, QueryInteractionStatusEventArgs e)
        {
            if (e.HandPointer.HandType == HandType.Right)
            {
                //If a grip detected change the cursor image to grip
                if (e.HandPointer.HandEventType == HandEventType.Grip)
                {
                    isRightGripInteraction = true;
                    e.IsInGripInteraction = true;
                }

                //If Grip Release detected change the cursor image to open
                else if (e.HandPointer.HandEventType == HandEventType.GripRelease)
                {
                    isRightGripInteraction = false;
                    e.IsInGripInteraction = false;
                }

                //If no change in state do not change the cursor
                else if (e.HandPointer.HandEventType == HandEventType.None)
                {
                    e.IsInGripInteraction = isRightGripInteraction;

                }

            }
            else if (e.HandPointer.HandType == HandType.Left)
            {
                if (e.HandPointer.HandEventType == HandEventType.Grip)
                {
                    isLeftGripInteraction = true;
                    e.IsInGripInteraction = true;
                }

                //If Grip Release detected change the cursor image to open
                else if (e.HandPointer.HandEventType == HandEventType.GripRelease)
                {
                    isLeftGripInteraction = false;
                    e.IsInGripInteraction = false;

                }
                //If no change in state do not change the cursor
                else if (e.HandPointer.HandEventType == HandEventType.None)
                {
                    e.IsInGripInteraction = isLeftGripInteraction;

                }
            }
            e.Handled = true;
        }

        # endregion Kinect events


      

        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(child);
                double abosuluteX;
                double abosuluteY;

                abosuluteX = relative.X * st.ScaleX + tt.X;
                abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                var tt = GetTranslateTransform(child);
                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                child.CaptureMouse();
            }
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                }
            }
        }

        public void RotateLeft(double angle, double centerX, double centerY)
        {
            if (child != null)
            {
                var rt = GetRotateTransform(child);
                rt.CenterX = centerX;
                rt.CenterY = centerY;
                rt.Angle += angle;
            }
        }

        #endregion
    }
}
