using System;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

namespace K4APointCloudVFX
{
    public class AzureKinectPointCloudSensor : UnityEngine.MonoBehaviour
    {
        private Device _kinect;

        private int _count;
        private Vector3[] _vertices;
        private Transformation _transformation;
        private int _width, _height;

        public int Count => _count;
        public Vector3[] Vertices => _vertices;
        public int Width => _width;
        public int Height => _height;

        private void Start()
        {
            InitKinect();
            var t = KinectLoop();
        }

        void InitKinect()
        {
            _kinect = Device.Open(0);
            _kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NFOV_2x2Binned,
                SynchronizedImagesOnly = true,
                CameraFPS = FPS.FPS30
            });
            _transformation = _kinect.GetCalibration().CreateTransformation();

            _width = _kinect.GetCalibration().DepthCameraCalibration.ResolutionWidth;
            _height = _kinect.GetCalibration().DepthCameraCalibration.ResolutionHeight;
            _count = _width * _height;

            _vertices = new Vector3[_count];
        }

        async Task KinectLoop()
        {
            while (true)
            {
                using (Capture capture = await Task.Run(() => _kinect.GetCapture()).ConfigureAwait(true))
                {
                    Image depthImage = _transformation.DepthImageToPointCloud(capture.Depth);
                    var depthArray = depthImage.GetPixels<Short3>().ToArray();

                    for (var i = 0; i < _count; i++)
                    {
                        _vertices[i].x = depthArray[i].X;
                        _vertices[i].y = depthArray[i].Y;
                        _vertices[i].z = depthArray[i].Z;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            _kinect.StopCameras();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                for (int i = 0; i < 100; i++)
                {
                    Debug.Log(_vertices[i]);
                }
        }
    }
}