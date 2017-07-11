using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace VendingMachines
{
	class VendingMachines : Mod
	{
        public VendingMachines()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup rec = new RecipeGroup(() => "Any Gold Bar", new int[]
            {
                19,
                706
            });
            RecipeGroup.RegisterGroup("VendingMachines:AnyGoldBar", rec);
            rec = new RecipeGroup(() =>  "Any Voodoo Doll", new int[]
            {
                ItemID.GuideVoodooDoll,
                ItemID.ClothierVoodooDoll
            });
            RecipeGroup.RegisterGroup("VendingMachines:AnyVoodooDoll", rec);
        }


        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            int type = reader.ReadByte();
            if(type == 1)
            {
                int vmID = reader.ReadInt16();
                if(VendingMachineWorld.vm[vmID] == null)
                {
                    VendingMachineWorld.vm[vmID] = new VendingMachineData();
                    //VendingMachineWorld.vm[vmID].Load(TagIO.Read(reader));
                }
                TagCompound vmTag = TagIO.Read(reader);
                VendingMachineWorld.vm[vmID].Load(vmTag);
                if (Main.netMode == 2)
                {
                    ModPacket p = this.GetPacket();
                    p.Write((byte)1);
                    p.Write((short)vmID);
                    TagIO.Write(vmTag, p);
                    p.Send();
                }
            }
            if (type == 2)
            {
                int vmID = reader.ReadInt16();
                if (VendingMachineWorld.vm[vmID] != null && !VendingMachineWorld.vm[vmID].isClear()) {
                    WorldGen.KillTile(VendingMachineWorld.vm[vmID].x, VendingMachineWorld.vm[vmID].y);
                    //VendingMachineWorld.vm[vmID] = null;
                }

                if (Main.netMode == 2)
                {
                    ModPacket p = this.GetPacket();
                    p.Write((byte)2);
                    p.Write((short)vmID);
                    p.Send();
                }
            }
        }

       
	}
}
