using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using UnityEngine;
using Windows.Kinect;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Bitmap = System.Drawing.Bitmap;
//using Vector = Windows.Vector;

namespace ElementSimulator
{
    public class RectangleDetector 
    {
        /// <summary>
        /// // Event that fires when a new depth frame is available
        /// </summary>
        public event EventHandler<RectangleDetectedArgs> RectangleDetected;
        /// <summary>
        /// Argument holding a bitmap
        /// </summary>
        public class RectangleDetectedArgs : EventArgs
        {
            public List<RotatedRect> Rectangles;
            public RectangleDetectedArgs(List<RotatedRect> rectangles)
            {
                Rectangles = rectangles;
            }
        }

        /// <summary>
        /// // Event that fires when a new depth frame is available
        /// </summary>
        public event EventHandler<NewFrameArgs> NewFrame;
        /// <summary>
        /// Argument holding a bitmap
        /// </summary>
        public class NewFrameArgs : EventArgs
        {
            public Bitmap DepthImage;
            public Bitmap Above;
            public Bitmap SeenBefore;
            public Bitmap Objects;
            public List<PointF[]> RectangleCorners;
            public NewFrameArgs(Bitmap depthImage,Bitmap above, Bitmap seenBefore, Bitmap objects, List<PointF[]> rectangleCorners)
            {
                DepthImage = depthImage;
                Above = above;
                SeenBefore = seenBefore;
                Objects = objects;
                RectangleCorners = rectangleCorners;
            }
        }

        #region Private Variables
        //KinectSensor Sensor; // Kinect sensor object
        //DepthImageFormat Resolution; // Current resolution of the depthimage

        ushort[] DepthArray; // Current depth intensities
        Image<Gray, short> DepthImage; // Current image(depth)
        Image<Gray, byte> Thresh;
        Image<Gray, short> DepthImagePrev; // Current image(depth)

        double[] BackgroundArray; // Accumulated sum of background intensities
        double[] BackgroundCounter; // Counter for each pixel used for background modeling.
        Image<Gray, short> BackgroundImage; // Current Backgroundimage(depth)
        Image<Gray, byte> Objects;

        Image<Gray, byte> SeenBeforeImage; // Image describing the number of times a pixel is "seen" before (after threshold)
        bool UpdateBackground = false; // Indicate if we should update the background model
        Gray OffsetMM_gray = new Gray(60); // Offset as Gray
        
        Gray One_gray = new Gray(1); // Gray value of one

        Gray FrameMemory_gray = new Gray(30); // The number of frames before a pixel is labeled as true object-pixel

        Matrix<double> Tdp; // Depth-to-Projector Transformationmatrix

        int Count = 0; // Number of frames used for calibration so far
        #endregion

        #region Protected Variables
        protected int N;
        protected int Width;
        protected int Height;
        protected Image<Gray, byte> DepthMask;
        #endregion

        #region Public Variables
        public int OffsetMM_int // Offset as int
        {
            get { return (int)OffsetMM_gray.Intensity; }
            set { OffsetMM_gray = new Gray(value); }
        }
        public int FrameMemory_int
        {
            get { return (int)FrameMemory_gray.Intensity;}
            set { FrameMemory_gray = new Gray(value); }
        }
        public int HumanBorderL = 20;
        public int HumanBorderR = 20;
        public int HumanBorderT = 20;
        public int HumanBorderD = 20;

        public bool DoSplit = false;
        public double SplitRatio = 0.4; // Big = Smaller triangles
        public int minArea = 300;
        public int nCalibrationFrames = 30;
        #endregion
        
        public short[] GetBG()
        {
            return BackgroundImage.Data.Cast<short>().ToArray();
        }

        public short[] GetDepth()
        {
            return DepthImage.Data.Cast<short>().ToArray();
        }

        public byte[] GetThreshold()
        {
            return Thresh.Data.Cast<byte>().ToArray();
        }

        public byte[] GetRects()
        {
            return Objects.Data.Cast<byte>().ToArray();
        }
        /// <summary>
        /// Contructor
        /// </summary>

        //public void LoadCalibration(string CalibFile = "C:/Users/hdtv-user/Desktop/virtual_table/Calibrate/Tdp.xml")
        //{
        //    Tdp = new Matrix<double>(3, 3);
        //    Tdp.SetIdentity();
        //    if (System.IO.File.Exists(CalibFile))
        //    {
        //        Matrix<double> TdpInv = new Matrix<double>(3, 3);
        //        IntPtr fs = CvInvoke.cvOpenFileStorage(CalibFile, IntPtr.Zero, Emgu.CV.CvEnum.STORAGE_OP.READ);
        //        IntPtr mat = CvInvoke.cvReadByName(fs, IntPtr.Zero, "Tdp");
        //        IntPtr matInv = CvInvoke.cvCreateMat(3, 3, Emgu.CV.CvEnum.MAT_DEPTH.CV_64F);

        //        if (mat != IntPtr.Zero)
        //        {
        //            CvInvoke.cvInvert(mat, matInv, Emgu.CV.CvEnum.SOLVE_METHOD.CV_LU);
                    
        //            CvInvoke.cvCopy(mat, Tdp, IntPtr.Zero);
        //            CvInvoke.cvCopy(matInv, TdpInv, IntPtr.Zero);

        //            CvInvoke.cvReleaseMat(ref mat);
        //            CvInvoke.cvReleaseMat(ref matInv);

        //            //return;
        //            PointF TopLeft_p = new PointF(0, 0);
        //            PointF BottomRight_p = new PointF(1920, 1080);
        //            PointF TopRight_p = new PointF(1920, 0);
        //            PointF BottomLeft_p = new PointF(0, 1080);


        //            PointF[] Corners = new PointF[]{TopLeft_p, TopRight_p, BottomRight_p,BottomLeft_p};
        //            Point[] ps = Corners.Select(c => TdpInv.Transform(c).ToPoint()).ToArray();
        //            DepthMask = new Image<Gray, short>(Width, Height);
        //            DepthMask.FillConvexPoly(ps, One_gray);
        //            MaskContour = DepthMask.Convert<Gray, byte>().FindContours();
        //        }
        //    }
        //}
        VectorOfPoint MaskContour;
        /// <summary>
        /// Tell the sensor to update the background model
        /// </summary>
        public void Calibrate()
        {
            Array.Clear(BackgroundArray, 0, BackgroundArray.Length);
            Array.Clear(BackgroundCounter, 0, BackgroundCounter.Length);
            Count = 0;
            UpdateBackground = true;
        }

        /// <summary>
        /// Set the resolution of the depth image
        /// </summary>
        /// <param name="resolution">Desired resolution</param>
        public void SetResolution(int _Width, int _Height)
        {
            Width = _Width;
            Height = _Height;
           
            N = Width * Height;
            DepthImage = new Image<Gray, short>(Width, Height);
            DepthMask = new Image<Gray, byte>(Width, Height, One_gray);

            VectorOfVectorOfPoint contoursDetected = new VectorOfVectorOfPoint();
            System.Drawing.Point offset = new System.Drawing.Point(0, 0);
            CvInvoke.FindContours(DepthMask, contoursDetected, null, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple, offset);
            MaskContour = contoursDetected[0];

            DepthImagePrev = new Image<Gray, short>(Width, Height);
            DepthArray = new ushort[N];
            BackgroundImage = new Image<Gray, short>(Width, Height);
            BackgroundCounter = new double[Width*Height];
            BackgroundArray = new double[Width * Height];
            SeenBeforeImage = new Image<Gray, byte>(Width, Height);
        }

        /// <summary>
        /// Connect the Kinect
        /// </summary>
        //public void Connect()
        //{
        //    foreach (KinectSensor sensor in KinectSensor.KinectSensors)
        //        if (sensor.Status == KinectStatus.Connected)
        //        {
        //            Sensor = sensor;
        //            break;
        //        }
        //    if (Sensor != null)
        //    {
        //        Sensor.DepthStream.Enable(Resolution);
        //        Sensor.DepthStream.Range = DepthRange.Near;
        //        Sensor.DepthFrameReady += Sensor_DepthFrameReady;
        //        Sensor.Start();
        //    }
        //}

        /// <summary>
        /// Disconnect the Kinect
        /// </summary>
        //public void Disconnect()
        //{
        //    if (Sensor != null)
        //        Sensor.Stop();
        //}

        /// <summary>
        /// Method handling the depth image and doing the rectangle detection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public List<RotatedRect> DepthReady(ushort[] depth)
        {
            List<RotatedRect> rectangles = new List<RotatedRect>();

            DepthArray = depth;
            //short[] CorrectDepthArray = new short[DepthArray.Length];
            //using (DepthImageFrame dframe = e.OpenDepthImageFrame())
            //    if (dframe != null)
            //        dframe.CopyPixelDataTo(DepthArray);
            //    else
            //        return;

            //DepthArray = DepthArray.Select(d => (short)(d >> DepthImageFrame.PlayerIndexBitmaskWidth)).ToArray();
            Buffer.BlockCopy(DepthArray, 0, DepthImage.Data, 0, Buffer.ByteLength(DepthArray));
            
            /*if (DepthMask != null)
                DepthImage._Mul(DepthMask);*/
            if (UpdateBackground && Count++ < nCalibrationFrames)
            {
                for (int i = 0; i < BackgroundArray.Length; i++)
                { 
                    BackgroundArray[i] += (double)DepthArray[i];
                    BackgroundCounter[i] += DepthArray[i] != 0 ? 1 : 0;
                }

                if (Count == nCalibrationFrames)
                {
                    for (int i = 0; i < BackgroundArray.Length; i++)
                        if (BackgroundCounter[i] > 0)
                            BackgroundArray[i] /= BackgroundCounter[i];
                        else
                            BackgroundArray[i] = 0;
                    short[] BackgroundArrayShort = BackgroundArray.Select(ba => (short)ba).ToArray();
                    Buffer.BlockCopy(BackgroundArrayShort, 0, BackgroundImage.Data, 0, Buffer.ByteLength(BackgroundArrayShort));
                    //BackgroundImage._Mul(DepthMask);
                    UpdateBackground = false;
                }
            }
            else
            {
                DepthImage = DepthImagePrev.Mul(0.9).Add(DepthImage.Mul(0.1));
                DepthImagePrev = DepthImage.Copy();
                Image<Gray, byte> DepthAbove = BackgroundImage.Sub(DepthImage).ThresholdBinary(OffsetMM_gray,One_gray).Convert<Gray, byte>();
                SeenBeforeImage = SeenBeforeImage.Mul(DepthAbove).Add(DepthAbove);
                
                Objects = SeenBeforeImage.ThresholdBinary(FrameMemory_gray,new Gray(255));

                Objects._Erode(3);
                Objects._Dilate(6);
                Objects._Erode(3);

                
                VectorOfVectorOfPoint contoursDetected = new VectorOfVectorOfPoint();
                System.Drawing.Point offset = new System.Drawing.Point(0, 0);

                var t  = Objects.Clone();
                Thresh = Objects.Clone();

                CvInvoke.FindContours(t, contoursDetected, null, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple, offset);
                Objects.SetZero();
                for (int i = 0; i < contoursDetected.Size; i++)
                {
                    System.Drawing.Point[] contour = contoursDetected[i].ToArray();

                    int XMax = contour.Max(c => c.X);
                    int XMin = contour.Min(c => c.X);
                    int YMax = contour.Max(c => c.Y);
                    int YMin = contour.Min(c => c.Y);
                    double area = CvInvoke.ContourArea(contoursDetected[i]);

                    if (area > minArea && XMin > HumanBorderR && YMin > HumanBorderD && XMax < Width - HumanBorderL && YMax < Height - HumanBorderT)
                    {
                        RotatedRect MinimumBox = CvInvoke.MinAreaRect(contoursDetected[i]);
                        rectangles.Add(MinimumBox);
//                        Debug.Log("Found rectangle: " + MinimumBox.Center);
                        Objects.Draw(MinimumBox, new Gray(255), 2);
                    }
                    
                }

                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(HumanBorderR, HumanBorderD, Width - HumanBorderR - HumanBorderL, Height - HumanBorderT - HumanBorderD);
                Objects.Draw(rect, new Gray(255), 2);
                //Debug.Log("Found rectangles: " + rectangles.Count);
                //if (rectangles.Count > 0 && RectangleDetected != null)
                //    RectangleDetected(this, new RectangleDetectedArgs(rectangles));
            }
            return rectangles;
        }

        System.Drawing.Rectangle[] SubDivide(System.Drawing.Rectangle rectangle)
        {
            System.Drawing.Rectangle[] rectangles = new System.Drawing.Rectangle[4];
            rectangles[0] = new System.Drawing.Rectangle(rectangle.X, rectangle.Y, rectangle.Width / 2, rectangle.Height / 2);
            rectangles[1] = new System.Drawing.Rectangle(rectangle.X + rectangle.Width / 2, rectangle.Y, rectangle.Width / 2, rectangle.Height / 2);
            rectangles[2] = new System.Drawing.Rectangle(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2, rectangle.Width / 2, rectangle.Height / 2);
            rectangles[3] = new System.Drawing.Rectangle(rectangle.X, rectangle.Y + rectangle.Height / 2, rectangle.Width / 2, rectangle.Height / 2);
            return rectangles;
        }

        //PointF NormalizeNew(PointF pOld)
        //{
        //    return new PointF(pOld.X / Width, pOld.Y / Width);
        //}
        //PointF Normalize(PointF pOld)
        //{
        //    return new PointF(pOld.X / Width, pOld.Y / Height);
        //}
        //PointF UnNormalize(PointF pOld)
        //{
        //    return new PointF(pOld.X * Width, pOld.Y * Height);
        //}
    }
}
