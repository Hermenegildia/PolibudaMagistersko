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
using System.Windows.Shapes;

namespace FullTotal.ImageTransformations
{
    public class ZoomBorder: Border
    {
        private const double zoomRatioThreshold = 0.05;

        private UIElement child = null;
        private Point origin;
        private Point start;
        KinectRegion kinectRegion;
      
        HandPointer leftHandPointer;
        HandPointer rightHandPointer;
       
        public delegate void GestureDelegate();
        public event GestureDelegate OnStartStretchGestureFollowing;
        public event GestureDelegate OnEndStretchGestureFollowing;
        public event GestureDelegate OnStartRotateFestureFollowing;
        public event GestureDelegate OnEndRotateFestureFollowing;
        public delegate void MoveDelegate(string position);
        public event MoveDelegate OnMoving;

        public delegate void VectorLengthUpdate(double vectorLength);
        

        double zoomFactor = 0;
        bool wasLastGestureStretch = false;
        bool wasLastGestureRotate = false;
        DateTime lastZoomDate = DateTime.Now;
        const int timePeriodBetweenZoom = 200;
        double angle = 0;
        bool isStretchEnabled = true;

        public void AssignGestureDetectionType(bool isStretchEnabled)
        {
            this.isStretchEnabled = isStretchEnabled;
        }

        public void AssignKinectRegion(KinectRegion kinectRegion)
        {
            this.kinectRegion = kinectRegion;

            if (kinectRegion != null)
            {
                //KinectRegion.RemoveQueryInteractionStatusHandler(this.child, OnQuery); //usuń stare powiązania
                //KinectRegion.AddQueryInteractionStatusHandler(this.child, OnQuery); //obsluga medicalImage przez kinectRegion

                KinectRegion.RemoveQueryInteractionStatusHandler(this, OnQuery); //usuń stare powiązania
                KinectRegion.AddQueryInteractionStatusHandler(this, OnQuery); //obsluga ZoomBorder przez kinectRegion
                KinectRegion.RemoveHandPointerGripHandler(this.child, OnPointerGrip); //usuń stare powiązania
                KinectRegion.AddHandPointerGripHandler(this.child, OnPointerGrip);
                KinectRegion.RemoveHandPointerMoveHandler(this.child, OnPointerMove); //usuń stare powiązania
                KinectRegion.AddHandPointerMoveHandler(this.child, OnPointerMove);
                KinectRegion.RemoveHandPointerGripReleaseHandler(this.child, OnPointerGripRelease); //usuń stare powiązania
                KinectRegion.AddHandPointerGripReleaseHandler(this.child, OnPointerGripRelease);
                KinectRegion.RemoveHandPointerLeaveHandler(this, OnPointerLeave); //usuń stare powiązania
                KinectRegion.AddHandPointerLeaveHandler(this, OnPointerLeave); ////uwolnij uścisk gdy łapka schodzi z image
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
                ScaleTransform st = new ScaleTransform(1,1);
                TranslateTransform tt = new TranslateTransform(0,0);               
                //added by Ala
                RotateTransform rt = new RotateTransform(0);
                //
                group.Children.Add(rt);
                group.Children.Add(st);
                group.Children.Add(tt);
              

                child.RenderTransform = group;
                //child.RenderTransformOrigin = new Point(0.0, 0.0);
              
                this.MouseWheel += child_MouseWheel;
                this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(child_PreviewMouseRightButtonDown);

                if (kinectRegion != null)
                {
                    KinectRegion.RemoveQueryInteractionStatusHandler(this, OnQuery); //usuń stare powiązania
                    KinectRegion.AddQueryInteractionStatusHandler(this, OnQuery); //obsluga medicalImage przez kinectRegion
                    KinectRegion.RemoveHandPointerGripHandler(this.child, OnPointerGrip); //usuń stare powiązania
                    KinectRegion.AddHandPointerGripHandler(this.child, OnPointerGrip);
                    KinectRegion.RemoveHandPointerMoveHandler(this.child, OnPointerMove); //usuń stare powiązania
                    KinectRegion.AddHandPointerMoveHandler(this.child, OnPointerMove);
                    KinectRegion.RemoveHandPointerGripReleaseHandler(this.child, OnPointerGripRelease); //usuń stare powiązania
                    KinectRegion.AddHandPointerGripReleaseHandler(this.child, OnPointerGripRelease);
                    KinectRegion.RemoveHandPointerLeaveHandler(this, OnPointerLeave); //usuń stare powiązania
                    KinectRegion.AddHandPointerLeaveHandler(this, OnPointerLeave); //uwolnij uścisk gdy łapka schodzi z image
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
            e.HandPointer.IsInGripInteraction = false;
            e.HandPointer.Capture(null);
            AssignHandPointerToBuffer(e.HandPointer);

            if (OnEndRotateFestureFollowing != null)
                OnEndRotateFestureFollowing();
            if (OnEndStretchGestureFollowing != null)
                OnEndStretchGestureFollowing();

            e.Handled = true;
        }


        //grip -> move -> release = przesuwanie
        //grip prawa + lewa -> move -> release = rozciąganie
        private void OnPointerGrip(object sender, HandPointerEventArgs e)
        {
            if (rightHandPointer != null && leftHandPointer != null)
            {
                //stretching/rotating gesture begin!
                if (rightHandPointer.IsInGripInteraction && leftHandPointer.IsInGripInteraction)
                {
                    if (isStretchEnabled)
                    {

                        if (OnStartStretchGestureFollowing != null)
                            OnStartStretchGestureFollowing();
                        wasLastGestureStretch = true;
                    }
                    else
                    {
                        if (OnStartRotateFestureFollowing != null)
                            OnStartRotateFestureFollowing();
                        wasLastGestureRotate = true;
                    }
                        
                    //}
                    AssignHandPointerToBuffer(e.HandPointer);
                    e.Handled = true;
                }
            }

            //move tylko gdy tylko prawa ręka gripped
            if (!wasLastGestureStretch && e.HandPointer.HandType == HandType.Right && (leftHandPointer == null || !leftHandPointer.IsInGripInteraction))
            {
                var tt = GetTranslateTransform(child);
                start = e.HandPointer.GetPosition(this);
                origin = new Point(tt.X, tt.Y);

                e.HandPointer.Capture(child);
            }
             AssignHandPointerToBuffer(e.HandPointer);
             e.Handled = true;
        }

        private void OnPointerMove(object sender, HandPointerEventArgs e)
        {
            if (child != null) 
            {
                if (leftHandPointer == null || !leftHandPointer.IsInGripInteraction)
                {
                    //move
                    if (e.HandPointer.HandType == HandType.Right && e.HandPointer.IsInGripInteraction)
                    {
                        DoMoving(e);
                        e.Handled = true;
                    }
                    
                }
                //dwureczne --> obie rece aktywne
                else if (rightHandPointer != null && leftHandPointer != null)
                {
                    AssignHandPointerToBuffer(e.HandPointer);
                    if (rightHandPointer.IsInGripInteraction && leftHandPointer.IsInGripInteraction)
                    {
                        //do zooming
                        if (wasLastGestureStretch)
                        {
                            DoZooming();
                            e.Handled = true;
                        }

                        if (wasLastGestureRotate)
                        {
                            DoRotation();
                            e.Handled = true;
                        }
                    }
                    
                }
            }
            e.Handled = true;
        }

        private void DoRotation()
        {
            if (child != null)
            {
                var rt = GetRotateTransform(child);
                var centerX = ((Image)this.child).ActualWidth / 2;///Math.Abs((rightHandPointer.GetPosition(this).X - leftHandPointer.GetPosition(this).X));
                var centerY = ((Image)this.child).ActualHeight / 2;//Math.Abs((rightHandPointer.GetPosition(this).Y - leftHandPointer.GetPosition(this).Y));
                rt.CenterX = centerX;
                rt.CenterY = centerY;
                rt.Angle = angle;
            }
        }

        private void DoZooming()
        {
            if (DateTime.Now.Subtract(lastZoomDate).TotalMilliseconds > timePeriodBetweenZoom)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                if ((zoomFactor < 0) && (st.ScaleX < .8 || st.ScaleY < .8)) //limit oddalania
                    return;

                Point relative = new Point(this.ActualWidth / 2, this.ActualHeight / 2); //zawsze wzgledem srodka ZoomBorder
                double abosuluteX;
                double abosuluteY;

                abosuluteX = relative.X * st.ScaleX + tt.X;
                abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoomFactor;
                st.ScaleY += zoomFactor;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;

                lastZoomDate = DateTime.Now;
                zoomFactor = 0;
            }
        }

        private void DoMoving(HandPointerEventArgs e)
        {
            if (rightHandPointer == null)
                rightHandPointer = e.HandPointer;

            if (e.HandPointer.Captured == child)//zlapany obrazek
            {
                var tt = GetTranslateTransform(child);
                Vector v = start - e.HandPointer.GetPosition(this);

                tt.X = origin.X - v.X;
                tt.Y = origin.Y - v.Y;

                if (OnMoving != null)
                    OnMoving( " tt.X: " + tt.X +  " tt.Y: " + tt.Y);
            }
        }

        private void OnPointerGripRelease(object sender, HandPointerEventArgs e)
        {
            //zakoncz stretching gesture
            if (wasLastGestureStretch || wasLastGestureRotate)
            {
                AssignHandPointerToBuffer(e.HandPointer);
                if (rightHandPointer != null && leftHandPointer != null)
                {
                    if (!rightHandPointer.IsInGripInteraction && !leftHandPointer.IsInGripInteraction)
                    {
                        wasLastGestureStretch = false;
                        wasLastGestureRotate = false;
                    }

                    if (child != null)
                    {
                        e.HandPointer.Capture(null);
                        if (OnEndStretchGestureFollowing != null)
                            OnEndStretchGestureFollowing();

                        if (OnEndRotateFestureFollowing != null)
                            OnEndRotateFestureFollowing();

                    }
                }
            }
            //zakoncz move
            else if (e.HandPointer.HandType == HandType.Right)
                {
                    e.HandPointer.Capture(null);
                    AssignHandPointerToBuffer(e.HandPointer);
                }

            e.Handled = true;

        }

        private void AssignHandPointerToBuffer(HandPointer handPointer)
        {
            if (handPointer.HandType == HandType.Left)
                this.leftHandPointer = handPointer;
            else if (handPointer.HandType == HandType.Right)
                this.rightHandPointer = handPointer;
        }

        private void OnQuery(object sender, QueryInteractionStatusEventArgs e)
        {
            if (e.HandPointer.HandType == HandType.Right)
            {
                if (rightHandPointer == null)
                   rightHandPointer = e.HandPointer;

                //If a grip detected change the cursor image to grip
                if (e.HandPointer.HandEventType == HandEventType.Grip)
                {
                    e.IsInGripInteraction = true;
                }

                //If Grip Release detected change the cursor image to open
                else if (e.HandPointer.HandEventType == HandEventType.GripRelease)
                {
                    e.IsInGripInteraction = false;
                }

                //If no change in state do not change the cursor
                else if (e.HandPointer.HandEventType == HandEventType.None)
                {
                    e.IsInGripInteraction = rightHandPointer.IsInGripInteraction;
                }

                AssignHandPointerToBuffer(e.HandPointer);
            }
            else if (e.HandPointer.HandType == HandType.Left)
            {
                if (leftHandPointer == null)
                    leftHandPointer = e.HandPointer;

                if (e.HandPointer.HandEventType == HandEventType.Grip)
                {
                    e.IsInGripInteraction = true;
                }

                //If Grip Release detected change the cursor image to open
                else if (e.HandPointer.HandEventType == HandEventType.GripRelease)
                {
                    e.IsInGripInteraction = false;
                }
                //If no change in state do not change the cursor
                else if (e.HandPointer.HandEventType == HandEventType.None)
                {
                    e.IsInGripInteraction = leftHandPointer.IsInGripInteraction;
                }
                AssignHandPointerToBuffer(e.HandPointer);
            }
            e.Handled = true;
        }

        # endregion Kinect events

        #region Kinect public methods

        public void SetZoomFactor(double zoomRatio)
        {
            if (zoomRatio > zoomRatioThreshold)
                zoomFactor = 0.2;
            else if (zoomRatio < -zoomRatioThreshold)
                zoomFactor = -0.2;
            else
                zoomFactor = 0;
        }

        public void SetRotationAngle(double angleInDegrees)
        {
            if (this.angle == 0)
                this.angle = -angleInDegrees; 
            else
                this.angle -= angleInDegrees; 
        }

        #endregion Kinect public methods


        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {

                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);
                var rot = GetRotateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4)) //limit oddalania
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
            //this.Reset();
            double x = ((Image)this.child).ActualWidth / 2;
            double y = ((Image)this.child).ActualHeight / 2;
            var location = e.GetPosition(this);
            var location2 = e.GetPosition(this.child);
            RotateLeft(10, x, y);
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    //var rt = GetRotateTransform(child);
                    //var st = GetScaleTransform(child);

                    Vector v = start - e.GetPosition(this);
                    var x = (origin.X - v.X);
                    var y = (origin.Y - v.Y );
                    //var angleRadians = Math.PI * (rt.Angle) / 180;
                    tt.X = x;//x*Math.Cos(angleRadians)+y*Math.Sin(angleRadians);
                    tt.Y = y;// -x * Math.Sin(angleRadians) + y * Math.Cos(angleRadians);
                    if (OnMoving != null)
                        OnMoving("v.X: " + v.X + " tt.X: " + tt.X + " v.Y: " + v.Y + " tt.Y: " + tt.Y);
                }
            }
        }

        public void RotateLeft(double angle, double centerX, double centerY)
        {
            if (child != null)
            {
                var rt = GetRotateTransform(child);
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);
                var group = (TransformGroup)this.child.RenderTransform;
                //var x = tt.X;
                //var y = tt.Y;

                rt.CenterX = centerX * st.ScaleX +tt.X;// *st.ScaleX; group.Value.OffsetX;//
                rt.CenterY = centerY * st.ScaleY +tt.Y;// *st.ScaleY; group.Value.OffsetY;//
                //child.RenderTransformOrigin = new Point(centerX * st.ScaleX + tt.X, centerY * st.ScaleY + tt.Y);// *st.ScaleX; group.Value.OffsetX;//
                rt.Angle += angle;

               
            }
        }

        #endregion
    }
}
