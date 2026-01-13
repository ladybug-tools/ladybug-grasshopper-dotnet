using System;
using System.Drawing;
using Grasshopper.Kernel;
using GH = Grasshopper;

namespace LadybugGrasshopper
{
    public class LadybugGrasshopperInfo : GH_AssemblyInfo
    {
        public override string Name => "LadybugGrasshopper";
        public override Bitmap Icon => Resources.Ladybug;
        public override string Description => "Ladybug Grasshopper component";
        public override Guid Id => new Guid("a44d4635-75e4-4019-89be-e0005630d465");
        public override string AuthorName => "Ladybug Tools";
        public override string AuthorContact => "info@ladybug.tools";
    }

    public class CategoryIcon : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            GH.Instances.ComponentServer.AddCategoryIcon("Ladybug", Resources.Ladybug);
            GH.Instances.ComponentServer.AddCategoryIcon("Honeybee", Resources.Honeybee);
            GH.Instances.ComponentServer.AddCategoryIcon("Dragonfly", Resources.Dragonfly);
            GH.Instances.ComponentServer.AddCategoryIcon("HB-Radiance", Resources.HB_Radiance);
            GH.Instances.ComponentServer.AddCategoryIcon("HB-Energy", Resources.HB_Energy);
            GH.Instances.ComponentServer.AddCategoryIcon("Fairyfly", Resources.fairyfly);

            GH.Instances.ComponentServer.AddCategoryShortName("Ladybug", "LB");
            GH.Instances.ComponentServer.AddCategorySymbolName("Ladybug", 'L');

            GH.Instances.ComponentServer.AddCategoryShortName("Honeybee", "HB");
            GH.Instances.ComponentServer.AddCategorySymbolName("Honeybee", 'H');

            GH.Instances.ComponentServer.AddCategoryShortName("Dragonfly", "DF");
            GH.Instances.ComponentServer.AddCategorySymbolName("Dragonfly", 'D');

            GH.Instances.ComponentServer.AddCategoryShortName("HB-Radiance", "HB-R");
            GH.Instances.ComponentServer.AddCategorySymbolName("HB-Radiance", 'R');

            GH.Instances.ComponentServer.AddCategoryShortName("HB-Energy", "HB-E");
            GH.Instances.ComponentServer.AddCategorySymbolName("HB-Energy", 'E');

            GH.Instances.ComponentServer.AddCategoryShortName("Fairyfly", "FF");
            GH.Instances.ComponentServer.AddCategorySymbolName("Fairyfly", 'F');
            return GH_LoadingInstruction.Proceed;
        }
    }
}
