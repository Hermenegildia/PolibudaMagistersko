/*
 * 
 *  Copyright (c) 2012 Jarrett Webb & James Ashley
 * 
 *  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 *  documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 *  and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 *  TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 *  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 *  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 *  IN THE SOFTWARE.
 * 
 * 
 */


using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for UcSkeletonViewer.xaml
    /// </summary>
    public partial class UcSkeletonViewer : UserControl
    {

        #region Member Variables
        private const float FeetPerMeters = 3.2808399f;
        private readonly Brush[] _SkeletonBrushes = new Brush[] { Brushes.Black, Brushes.Crimson, Brushes.Indigo, Brushes.DodgerBlue, Brushes.Purple, Brushes.Pink };
        private Skeleton[] _FrameSkeletons;
        #endregion Member Variables

        public UcSkeletonViewer()
        {
            InitializeComponent();
        }

        private void KinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonsPanel.Children.Clear();
            JointInfoPanel.Children.Clear();

            if (this.IsEnabled)
            {
                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame != null)
                    { 
                        frame.CopySkeletonDataTo(this._FrameSkeletons);
                        for (int i = 0; i < this._FrameSkeletons.Length; i++)
                        {
                            DrawSkeleton(this._FrameSkeletons[i], this._SkeletonBrushes[i]);

                            TrackJoint(this._FrameSkeletons[i].Joints[JointType.HandLeft], Brushes.YellowGreen);  //this._SkeletonBrushes[i]);
                            TrackJoint(this._FrameSkeletons[i].Joints[JointType.HandRight], Brushes.Turquoise);//this._SkeletonBrushes[i]);
                            TrackJoint(this._FrameSkeletons[i].Joints[JointType.HandLeft], this._FrameSkeletons[i].Joints[JointType.HandRight], this._SkeletonBrushes[i]);
                            
                        }
                        
                    }
                }
            }

        }

        private void DrawSkeleton(Skeleton skeleton, Brush brush)
        {
            if (skeleton != null && skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                //Draw head and torso
                Polyline figure = CreateFigure(skeleton, brush, new[] { JointType.Head, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.Spine,
                                                                             JointType.ShoulderRight, JointType.ShoulderCenter, JointType.HipCenter});
                SkeletonsPanel.Children.Add(figure);

                figure = CreateFigure(skeleton, brush, new[] { JointType.HipLeft, JointType.HipRight });
                SkeletonsPanel.Children.Add(figure);

                //Draw left leg
                figure = CreateFigure(skeleton, brush, new[] { JointType.HipCenter, JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft });
                SkeletonsPanel.Children.Add(figure);

                //Draw right leg
                figure = CreateFigure(skeleton, brush, new[] { JointType.HipCenter, JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight });
                SkeletonsPanel.Children.Add(figure);

                //Draw left arm
                figure = CreateFigure(skeleton, brush, new[] { JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft });
                SkeletonsPanel.Children.Add(figure);

                //Draw right arm
                figure = CreateFigure(skeleton, brush, new[] { JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight });
                SkeletonsPanel.Children.Add(figure);
            }
        }

        private Polyline CreateFigure(Skeleton skeleton, Brush brush, JointType[] joints)
        {
            Polyline figure = new Polyline();

            figure.StrokeThickness = 18;
            figure.Stroke = brush;

            for (int i = 0; i < joints.Length; i++)
            {
                figure.Points.Add(GetJointPoint(skeleton.Joints[joints[i]]));
            }

            return figure;
        }


        private Point GetJointPoint(Joint joint)
        {
            //DepthImagePoint point = this.KinectDevice.MapSkeletonPointToDepth(joint.Position, this.KinectDevice.DepthStream.Format);
            CoordinateMapper cm = new CoordinateMapper(this.KinectDevice);
            DepthImagePoint point = cm.MapSkeletonPointToDepthPoint(joint.Position, this.KinectDevice.DepthStream.Format);
            point.X *= (int)this.LayoutRoot.ActualWidth / KinectDevice.DepthStream.FrameWidth;
            point.Y *= (int)this.LayoutRoot.ActualHeight / KinectDevice.DepthStream.FrameHeight;

            return new Point(point.X, point.Y);
        }

        private void TrackJoint(Joint joint, Brush brush)
        {
            if (joint.TrackingState != JointTrackingState.NotTracked)
            {
                Canvas container = new Canvas();
                Point jointPoint = GetJointPoint(joint);

                //FeetPerMeters is a class constant of 3.2808399f;
                double z = joint.Position.Z;// *FeetPerMeters;

                Ellipse element = new Ellipse();
                element.Height = 15;
                element.Width = 15;
                element.Fill = brush;
                Canvas.SetLeft(element, 0 - (element.Width / 2));
                Canvas.SetTop(element, 0 - (element.Height / 2));
                container.Children.Add(element);

                TextBlock positionText = new TextBlock();
                positionText.Text = string.Format("<{0:0.00}, {1:0.00}, {2:0.00}>", jointPoint.X, jointPoint.Y, z);
                positionText.Foreground = brush;
                positionText.FontSize = 24;
                positionText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(positionText, 35);
                Canvas.SetTop(positionText, 15);
                container.Children.Add(positionText);

                Canvas.SetLeft(container, jointPoint.X);
                Canvas.SetTop(container, jointPoint.Y);

                JointInfoPanel.Children.Add(container);
            }
        }                       

        //Ala helper function
        private void TrackJoint(Joint leftHand, Joint rightHand, Brush brush)
        {
            if (leftHand.TrackingState != JointTrackingState.NotTracked && rightHand.TrackingState != JointTrackingState.NotTracked)
            {
                Canvas container = new Canvas();
                Point leftHandPoint = GetJointPoint(leftHand);
                Point rightHandPoint = GetJointPoint(rightHand);

                //FeetPerMeters is a class constant of 3.2808399f;
                //double z = joint.Position.Z;// *FeetPerMeters;

                double pythagoras1 = Math.Pow(rightHandPoint.X - leftHandPoint.X, 2);
                double pythagoras2 = Math.Pow(leftHandPoint.Y - rightHandPoint.Y, 2);
                double vec_length = Math.Sqrt(pythagoras1 + pythagoras2);

                pythagoras1 = Math.Pow(rightHand.Position.X - leftHand.Position.X, 2);
                pythagoras2 = Math.Pow(leftHand.Position.Y - rightHand .Position.Y, 2);
                double real_vec_length = Math.Sqrt(pythagoras1 + pythagoras2);

                
                //Ellipse element = new Ellipse();
                //element.Height = 15;
                //element.Width = 15;
                //element.Fill = brush;
                //Canvas.SetLeft(element, 0 - (element.Width / 2));
                //Canvas.SetTop(element, 0 - (element.Height / 2));
                //container.Children.Add(element);

                TextBlock positionText = new TextBlock();
                positionText.Text = string.Format("odległość przeskalowana: {0:0.00}", vec_length);
                positionText.Text = positionText.Text + string.Format(" odległość real: {0:0.00}", real_vec_length);
                positionText.Foreground = brush;
                positionText.FontSize = 44;
                positionText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(positionText, 35);
                Canvas.SetTop(positionText, 15);
                container.Children.Add(positionText);

                //Canvas.SetLeft(container, jointPoint.X);
                //Canvas.SetTop(container, jointPoint.Y);

                JointInfoPanel.Children.Add(container);
            }
        }

        private bool CheckHandElbowPosition(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        #region KinectDevice
        protected const string KinectDevicePropertyName = "KinectDevice";
        public static readonly DependencyProperty KinectDeviceProperty =
            DependencyProperty.Register(KinectDevicePropertyName, typeof(KinectSensor), typeof(UcSkeletonViewer), new PropertyMetadata(null, KinectDeviceChanged));

        private static void KinectDeviceChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            UcSkeletonViewer viewer = (UcSkeletonViewer)owner;
            if (e.OldValue != null)
            {
                ((KinectSensor)e.OldValue).SkeletonFrameReady -= viewer.KinectDevice_SkeletonFrameReady;
                viewer._FrameSkeletons = null;
            }

            if (e.NewValue != null)
            {
                viewer.KinectDevice = (KinectSensor)e.NewValue;
                viewer.KinectDevice.SkeletonFrameReady += viewer.KinectDevice_SkeletonFrameReady;
                viewer._FrameSkeletons = new Skeleton[viewer.KinectDevice.SkeletonStream.FrameSkeletonArrayLength];
            }    
        }

        public KinectSensor KinectDevice
        {
            get { return (KinectSensor)GetValue(KinectDeviceProperty); }
            set { SetValue(KinectDeviceProperty, value); }
        }
        #endregion KinectDevice
    }
}
