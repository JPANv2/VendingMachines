using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;
using VendingMachines.Items;

namespace VendingMachines.Tiles
{
    public class DyeBallMachineTile : VendingMachine
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            dropID = ModContent.ItemType<DyeBallMachineItem>();
        }

        public override void VendingMachineMapEntry()
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Dye Ball Machine");
            base.AddMapEntry(new Microsoft.Xna.Framework.Color(200, 200, 200), name);
        }
    }
}
