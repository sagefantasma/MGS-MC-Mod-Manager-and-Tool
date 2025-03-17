using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

// Aliases to resolve Vector3D ambiguity
using AssimpVector3D = Assimp.Vector3D;
using WpfVector3D = System.Windows.Media.Media3D.Vector3D;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class ModelViewerControl : UserControl
    {
        public ModelViewerControl()
        {
            InitializeComponent(); // This line is crucial so HelixViewport is recognized
        }

        /// <summary>
        /// Clears any current model/scene from the Helix viewport.
        /// Call this before overwriting texture files to ensure Helix
        /// is no longer holding a file lock.
        /// </summary>
        public void ClearModel()
        {
            HelixViewport.Children.Clear();
        }

        /// <summary>
        /// Loads a model from the given file path and adds custom lights,
        /// clearing any previously loaded model.
        /// </summary>
        /// <param name="modelPath">Path to .obj / .fbx / .dae etc.</param>
        public void LoadModel(string modelPath)
        {
            var importer = new ModelImporter();

            try
            {
                // Load the model
                Model3D model = importer.Load(modelPath);

                // Clear any existing models/scene objects
                HelixViewport.Children.Clear();

                // Add custom lights
                AddCustomLights();

                // Create a ModelVisual3D to contain the model
                var modelVisual = new ModelVisual3D { Content = model };

                // Add the model to the viewport
                HelixViewport.Children.Add(modelVisual);

                // Adjust the camera to fit the model
                HelixViewport.ZoomExtents();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading model: " + ex.Message,
                    "Model Loading Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Adds directional + ambient lights to the scene.
        /// </summary>
        private void AddCustomLights()
        {
            var lights = new Model3DGroup();

            // Example directional light
            var direction1 = CalculateDirection(300, 15);
            var intensity1 = 1.0;
            var color1 = MultiplyColor(Colors.White, intensity1);
            var directionalLight1 = new DirectionalLight(color1, direction1);
            lights.Children.Add(directionalLight1);

            // Ambient Light
            var ambientLight = new AmbientLight(Color.FromScRgb(1.0f, 0.3f, 0.3f, 0.3f));
            lights.Children.Add(ambientLight);

            // Add the lights to the viewport
            HelixViewport.Children.Add(new ModelVisual3D { Content = lights });
        }

        /// <summary>
        /// Creates a normalized Vector3D from spherical coords, e.g. for lighting angles.
        /// </summary>
        private WpfVector3D CalculateDirection(double angleHorizontalDegrees, double angleVerticalDegrees)
        {
            double angleHorizontalRad = Math.PI * angleHorizontalDegrees / 180.0;
            double angleVerticalRad = Math.PI * angleVerticalDegrees / 180.0;

            double x = Math.Cos(angleVerticalRad) * Math.Sin(angleHorizontalRad);
            double y = Math.Sin(angleVerticalRad);
            double z = Math.Cos(angleVerticalRad) * Math.Cos(angleHorizontalRad);

            var direction = new WpfVector3D(x, y, z);
            direction.Normalize();
            return direction;
        }

        /// <summary>
        /// Multiplies a color's intensity, clamping within [0,1].
        /// </summary>
        private Color MultiplyColor(Color color, double intensity)
        {
            float scR = (float)Math.Min(1.0, color.ScR * intensity);
            float scG = (float)Math.Min(1.0, color.ScG * intensity);
            float scB = (float)Math.Min(1.0, color.ScB * intensity);
            return Color.FromScRgb(1.0f, scR, scG, scB);
        }

        /// <summary>
        /// Utility to iterate geometry in a Model3D or Model3DGroup.
        /// </summary>
        private IEnumerable<GeometryModel3D> GetGeometryModels(Model3D model)
        {
            if (model is GeometryModel3D geometryModel)
            {
                yield return geometryModel;
            }
            else if (model is Model3DGroup modelGroup)
            {
                foreach (var child in modelGroup.Children)
                {
                    foreach (var gm in GetGeometryModels(child))
                    {
                        yield return gm;
                    }
                }
            }
        }

        #region Extra/Optional Methods

        // Example of overriding materials for quick debugging
        private void OverrideMaterials(Model3D model)
        {
            var simpleMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));

            foreach (var geometryModel in GetGeometryModels(model))
            {
                geometryModel.Material = simpleMaterial;
                geometryModel.BackMaterial = simpleMaterial;
            }
        }

        // Example of visualizing normals (for debugging)
        private void VisualizeNormals(Model3D model)
        {
            var lines = new LinesVisual3D
            {
                Color = Colors.Blue,
                Thickness = 1
            };

            foreach (var geometryModel in GetGeometryModels(model))
            {
                if (geometryModel.Geometry is MeshGeometry3D mesh &&
                    mesh.Normals != null)
                {
                    for (int i = 0; i < mesh.Positions.Count; i++)
                    {
                        var position = mesh.Positions[i];
                        var normal = mesh.Normals[i];

                        lines.Points.Add(position);
                        lines.Points.Add(position + normal * 0.1);
                    }
                }
            }

            HelixViewport.Children.Add(lines);
        }

        // Example of rendering a simple wireframe overlay
        private void RenderWireframe(Model3D model)
        {
            var wireframe = new LinesVisual3D
            {
                Color = Colors.Red,
                Thickness = 1
            };

            foreach (var geometryModel in GetGeometryModels(model))
            {
                if (geometryModel.Geometry is MeshGeometry3D mesh)
                {
                    for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
                    {
                        var p0 = mesh.Positions[mesh.TriangleIndices[i]];
                        var p1 = mesh.Positions[mesh.TriangleIndices[i + 1]];
                        var p2 = mesh.Positions[mesh.TriangleIndices[i + 2]];

                        wireframe.Points.Add(p0); wireframe.Points.Add(p1);
                        wireframe.Points.Add(p1); wireframe.Points.Add(p2);
                        wireframe.Points.Add(p2); wireframe.Points.Add(p0);
                    }
                }
            }

            HelixViewport.Children.Add(wireframe);
        }

        #endregion
    }
}
