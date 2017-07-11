using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using VendingMachines.Items;

namespace VendingMachines.Tiles
{
    public class ReforgeMachineTile : VendingMachine
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            dropID = mod.ItemType<ReforgeMachineItem>();
        }

        public override void VendingMachineMapEntry()
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Reforging Machine");
            base.AddMapEntry(new Microsoft.Xna.Framework.Color(200, 200, 200), name);
        }
    }
}
