using System;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BobOnRails.Controller.MVVM;
using BobOnRails.Controller.Physics;
using HelixToolkit.Wpf;

namespace BobOnRails.Controller.DeviceSimulator
{
    /// <summary>
    /// Provides a ViewModel for the Main window.
    /// </summary>
    public class VirtualDeviceViewModel : BaseViewModel, IDisposable
    {
        private VirtualDevice virtualDevice;
        private Path virtualPath;

        private RelayCommand generatePathCommand;
        private RelayCommand startDeviceCommand;
        private RelayCommand stopDeviceCommand;

        /// <summary>
        /// Gets or sets the IP address of the virtual device.
        /// </summary>
        public string DeviceIP { get; set; }

        /// <summary>
        /// Gets or sets the port of the virtual device.
        /// </summary>
        public int DevicePort { get; set; }

        /// <summary>
        /// Gets or sets the 3D model to display.
        /// </summary>
        public Model3D Model { get; set; }

        /// <summary>
        /// Gets or sets the 3D line path representing the <see cref="VirtualPath"/>.
        /// </summary>
        public Point3DCollection PathLine { get; private set; }

        /// <summary>
        /// Gets or sets the path the virtual device should move along.
        /// </summary>
        public Path VirtualPath
        {
            get { return virtualPath; }
            set
            {
                virtualPath = value;

                PathLine.Clear();
                foreach (var pathPoint in virtualPath)
                    PathLine.Add(new Point3D(pathPoint.Position.X, pathPoint.Position.Y, pathPoint.Position.Z));
            }
        }

        /// <summary>
        /// Gets the command to execute to generate a path for the virtual device.
        /// </summary>
        public RelayCommand GeneratePathCommand
        {
            get
            {
                if (generatePathCommand == null)
                {
                    generatePathCommand = new RelayCommand(
                        (o) => { VirtualPath = MotionSimulator.GenerateSinusSpeedCirclePath(0.353, 2.0 * 1000 / 3600, 50, 10, 6); },
                        //(o) => { VirtualPath = MotionSimulator.GenerateSinusSpeedLinearPath(2.0 * 1000 / 3600, 50, 10, 5); },
                        (o) => { return virtualDevice == null; });
                }

                return generatePathCommand;
            }
        }

        /// <summary>
        /// Gets the command to execute to start the virtual device.
        /// </summary>
        public RelayCommand StartDeviceCommand
        {
            get
            {
                if (startDeviceCommand == null)
                {
                    startDeviceCommand = new RelayCommand(
                        (o) => { virtualDevice = new VirtualDevice(new IPEndPoint(IPAddress.Parse(DeviceIP), DevicePort), VirtualPath); },
                        (o) => { return virtualDevice == null && VirtualPath != null; });
                }

                return startDeviceCommand;
            }
        }

        /// <summary>
        /// Gets the command to execute to stop the virtual device.
        /// </summary>
        public RelayCommand StopDeviceCommand
        {
            get
            {
                if (stopDeviceCommand == null)
                {
                    stopDeviceCommand = new RelayCommand(
                        (o) => { /*virtualDevice.Dispose();*/ virtualDevice = null; },
                        (o) => { return virtualDevice != null; });
                }

                return stopDeviceCommand;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDeviceViewModel"/> class.
        /// </summary>
        public VirtualDeviceViewModel()
        {
            DeviceIP = "127.0.0.1";
            DevicePort = 5006;

            InitializeScene();
        }

        /// <inheritdoc cref="IDisposable.Dispose()" />
        public void Dispose()
        {
            if (virtualDevice != null)
                virtualDevice.Dispose();
        }

        private void InitializeScene()
        {
            //Create a geometry representing the path Bob travelled
            PathLine = new Point3DCollection();

            var grid = new GridLinesVisual3D()
            {
                Width = 1,
                Length = 1,
                MinorDistance = 0.01,
                MajorDistance = 0.1,
                Thickness = 0.001,
                Material = MaterialHelper.CreateMaterial(Colors.DarkGreen)
            };

            //Create a model group
            var modelGroup = new Model3DGroup();
            modelGroup.Children.Add(grid.Model);

            Model = modelGroup;
        }
    }
}
