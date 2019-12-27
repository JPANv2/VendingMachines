using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using VendingMachines.Items;

namespace VendingMachines.Tiles
{
    public class ATMTile : VendingMachine
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            dropID = ModContent.ItemType<ATMItem>();
        }

        public override void VendingMachineTileStyle()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16
            };

            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
        }

        public override void VendingMachineMapEntry()
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("ATM");
            base.AddMapEntry(new Microsoft.Xna.Framework.Color(200, 200, 200), name);
        }
    }
}
