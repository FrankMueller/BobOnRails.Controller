using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using BobOnRails.Controller.Communication;
using BobOnRails.Controller.MVVM;
using BobOnRails.Controller.Physics;
using HelixToolkit.Wpf;

namespace BobOnRails.Controller.GUI
{
    /// <summary>
    /// Provides a ViewModel for the Main window.
    /// </summary>
    public class MainViewModel : BaseViewModel, IDisposable
    {
        private GeometryModel3D bob;
        private CommunicationHandler communicationHandler;
        private bool isMeasuring;
        private MotionTracker motionTracker;
        private DispatcherTimer motionUpdateTimer;

        private RelayCommand connectDeviceCommand;
        private RelayCommand startMeasurementCommand;
        private RelayCommand stopMeasurementCommand;


        /// <summary>
        /// Gets or sets the IP address of the virtual device.
        /// </summary>
        public string DeviceIP { get; set; }

        /// <summary>
        /// Gets or sets the port of the virtual device.
        /// </summary>
        public int DevicePort { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public Model3D Model { get; set; }

        /// <summary>
        /// Gets or sets the 3D line path representing the path the device travelled along.
        /// </summary>
        public Point3DCollection PathLine { get; set; }

        /// <summary>
        /// Gets the command to execute to generate a path for the virtual device.
        /// </summary>
        public RelayCommand ConnectDeviceCommand
        {
            get
            {
                if (connectDeviceCommand == null)
                {
                    connectDeviceCommand = new RelayCommand(
                        (o) => { communicationHandler = new CommunicationHandler(DeviceIP, DevicePort);  },
                        (o) => { return communicationHandler == null; });
                }

                return connectDeviceCommand;
            }
        }

        /// <summary>
        /// Gets the command to execute to start the virtual device.
        /// </summary>
        public RelayCommand StartMeasurementCommand
        {
            get
            {
                if (startMeasurementCommand == null)
                {
                    startMeasurementCommand = new RelayCommand(
                        (o) => { communicationHandler.StartMeasuring(); isMeasuring = true; },
                        (o) => { return communicationHandler != null && !isMeasuring; });
                }

                return startMeasurementCommand;
            }
        }

        /// <summary>
        /// Gets the command to execute to stop the virtual device.
        /// </summary>
        public RelayCommand StopMeasurementCommand
        {
            get
            {
                if (stopMeasurementCommand == null)
                {
                    stopMeasurementCommand = new RelayCommand(
                        (o) => { communicationHandler.StopMeasuring(); isMeasuring = false; },
                        (o) => { return communicationHandler != null && isMeasuring; });
                }

                return stopMeasurementCommand;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            DeviceIP = "127.0.0.1";
            DevicePort = 5006;

            InitializeScene();

            motionTracker = new MotionTracker();

            motionUpdateTimer = new DispatcherTimer();
            motionUpdateTimer.Tick += OnTimerTick;
            motionUpdateTimer.Interval = TimeSpan.FromMilliseconds(20);
            motionUpdateTimer.Start();
        }

        /// <inheritdoc cref="IDisposable.Dispose()" />
        public void Dispose()
        {
            motionUpdateTimer.Stop();

            if (communicationHandler != null)
                communicationHandler.Dispose();
        }

        private void InitializeScene()
        {
            //Create some materials
            var redMaterial = MaterialHelper.CreateMaterial(Colors.Red);
            var whiteMaterial = MaterialHelper.CreateMaterial(Colors.White);
            var blackMaterial = MaterialHelper.CreateMaterial(Colors.Black);

            //Create a mesh geometry representing Bob
            var bobLength = 0.12;
            var bobWidth = 0.02;
            var meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddBox(new Rect3D(0, 0, 0.010, bobWidth, bobLength, 0.004));
            bob = new GeometryModel3D { Geometry = meshBuilder.ToMesh(true), Material = redMaterial };

            meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddBox(new Rect3D(0, 0, 0.014, bobWidth, bobLength, 0.0125));
            meshBuilder.AddBox(new Rect3D(0, 0, 0.002, bobWidth, bobLength, 0.008));
            var bobTop = new GeometryModel3D { Geometry = meshBuilder.ToMesh(true), Material = whiteMaterial };

            meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddCylinder(new Point3D(-0.001, bobLength / 8, 0.001), new Point3D(0.000, bobLength / 8, 0.001), 0.005, 32, true, true);
            var bobWheels = new GeometryModel3D { Geometry = meshBuilder.ToMesh(true), Material = blackMaterial, BackMaterial = blackMaterial };


            //Create a geometry representing the path Bob travelled
            PathLine = new Point3DCollection();

            var grid = new GridLinesVisual3D()
            {
                Width = 1,
                Length = 1,
                MinorDistance = 0.01,
                MajorDistance = 0.1,
                Thickness = 0.001,
                Material = MaterialHelper.CreateMaterial(Colors.Blue)
            };

            //Create a model group
            var modelGroup = new Model3DGroup();
            modelGroup.Children.Add(bob);
            //modelGroup.Children.Add(bobTop);
            //modelGroup.Children.Add(bobWheels);
            modelGroup.Children.Add(grid.Model);

            Model = modelGroup;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (communicationHandler != null)
            {
                var measurements = communicationHandler.GrabAccelerometerMeasurements();

                foreach (var measurement in measurements)
                    motionTracker.AppendMotion(measurement.Acceleration, measurement.TimeStamp);

                for (int i = PathLine.Count; i < motionTracker.Path.Count; i++)
                {
                    var position = motionTracker.Path[i].Position;
                    var linePoint = new Point3D(position.X, position.Y, position.Z);
                    PathLine.Add(linePoint);
                }

                var currentPosition = motionTracker.CurrentPosition.Position;
                bob.Transform = new TranslateTransform3D(currentPosition.X, currentPosition.Y, currentPosition.Z);
            }
        }
    }
}
