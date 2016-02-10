﻿using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Linq;

namespace BuildOnce
{
    public class TransformConfigTask : Task
    {
        [Required]
        public ITaskItem[] Content { get; set; }
        [Required]
        public ITaskItem[] None { get; set; }
        [Required]
        public string OutputPath { get; set; }

        public Tree<ITaskItem> Tree { get; set; }

        public TransformConfigTask()
        {
            Tree = new Tree<ITaskItem>();
        }

        public override bool Execute()
        {
            var isSuccesful = true;
            var NoneList = None.ToList();
            var ContentList = Content.ToList();

            Log.LogMessage("Config Dependency Tree");

            NoneList.ForEach(t =>
            {
                var dependentMeta = t.GetMetadata("DependentUpon");

                if (!string.IsNullOrWhiteSpace(dependentMeta))
                {
                    if (!string.IsNullOrWhiteSpace(t.GetMetadata("RelativeDir")))
                    {
                        dependentMeta = t.GetMetadata("RelativeDir") + dependentMeta;
                    }

                    var dependency = ContentList
                        .Where(c => c.ItemSpec.Equals(dependentMeta, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (dependency == null)
                    {
                        Log.LogError("Unable to find parent item {0} for child item {1}", dependentMeta, t.ItemSpec);
                        isSuccesful = false;
                    }

                    var item = Tree.Where(i => i.Key == dependency).FirstOrDefault();

                    if (item == null)
                    {
                        item = new Tree<ITaskItem> { Key = dependency };
                        Tree.Add(item);
                    }

                    item.Add(new Tree<ITaskItem> { Key = t });
                }
            });

            Log.LogMessage(Tree.ToString(0));

            var xmlTransformer = new XmlTransfomer();

            foreach (var item in Tree)
            {
                if (item.Key.GetMetadata("Extension").Equals(".config", StringComparison.OrdinalIgnoreCase) ||
                    item.Key.GetMetadata("Extension").Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    xmlTransformer.Transform(item, OutputPath);
                }
            }
            
            if(isSuccesful)
            {
                Log.LogMessage(MessageImportance.High, "BuildOnce transforms were generated successfully and sent to '{0}'", OutputPath);           
            }

            return isSuccesful;
        }
    }
}