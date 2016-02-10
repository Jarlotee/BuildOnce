using Microsoft.Build.Framework;
using Microsoft.Web.XmlTransform;
using System;
using System.IO;
using System.Linq;

namespace BuildOnce
{
    public class XmlTransfomer
    {
        /// <summary>
        /// Executes transforms for all configuration detected
        /// </summary>
        public void Transform(Tree<ITaskItem> tree, string outputPath)
        {
            foreach (var branch in tree)
            {
                var targetTransform = branch.Key.GetMetadata("Filename").Replace(tree.Key.GetMetadata("Filename") + ".", "");

                var sourcePath = tree.Key.GetMetadata("FullPath");
                var transformPath = branch.Key.GetMetadata("FullPath");
                var destinationPath = string.Format("{0}//{1}//{2}{3}{4}",
                    outputPath,
                    targetTransform,
                    tree.Key.GetMetadata("RelativeDir"),
                    tree.Key.GetMetadata("Filename"),
                    tree.Key.GetMetadata("Extension"));

                TransformXML(sourcePath, transformPath, destinationPath);
            }
        }

        /// <summary>
        /// Executes transforms for a single configuration
        /// </summary>
        public void Transform(Tree<ITaskItem> tree, string outputPath, string targetConfiguration)
        {
            var target = string.Format("{0}{1}.{2}{3}",
                    tree.Key.GetMetadata("RelativeDir"),
                    tree.Key.GetMetadata("Filename"),
                    targetConfiguration,
                    tree.Key.GetMetadata("Extension"));


            var transformItem = tree.Where(k => k.Key.ItemSpec.Equals(target, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (transformItem != null)
            {
                var sourcePath = tree.Key.GetMetadata("FullPath");
                var transformPath = transformItem.Key.GetMetadata("FullPath");
                var destinationPath = string.Format("{0}{1}{2}{3}",
                    outputPath,
                    tree.Key.GetMetadata("RelativeDir"),
                    tree.Key.GetMetadata("Filename"),
                    tree.Key.GetMetadata("Extension"));

                TransformXML(sourcePath, transformPath, destinationPath);
            }
        }

        private void TransformXML(string sourcePath, string transformPath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException("File to transform not found", sourcePath);
            }
            if (!File.Exists(transformPath))
            {
                throw new FileNotFoundException("Transform file not found", transformPath);
            }

            using (XmlTransformableDocument document = new XmlTransformableDocument())
            using (XmlTransformation transformation = new XmlTransformation(transformPath))
            {
                document.PreserveWhitespace = true;
                document.Load(sourcePath);

                var success = transformation.Apply(document);

                if (!success)
                {
                    string message = string.Format(
                        "There was an unknown error trying while trying to apply the transform. Source file='{0}',Transform='{1}', Destination='{2}'",
                        sourcePath, transformPath, destinationPath);
                    throw new Exception(message);
                }

                var destinationInfo = new FileInfo(destinationPath);

                if (!destinationInfo.Directory.Exists)
                {
                    destinationInfo.Directory.Create();
                }

                document.Save(destinationPath);
            }
        }
    }
}
