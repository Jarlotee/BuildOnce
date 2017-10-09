using Microsoft.Build.Framework;
using System.IO;

namespace BuildOnce
{
    public static class FileOverrider
    {
        /// <summary>
        /// Overrides the base file
        /// </summary>
        public static void Handle(Tree<ITaskItem> tree, string outputPath, string outputType)
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

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                File.Copy(transformPath, destinationPath, true);
            }
        }
    }
}
