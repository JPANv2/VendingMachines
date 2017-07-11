using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace VendingMachines.Items
{
    public class SoulOfNPC : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of NPC");
            Tooltip.SetDefault("The essence of some Town NPC");
        }


        public override void SetDefaults()
        {
            Item refItem = new Item();
            refItem.SetDefaults(ItemID.SoulofSight);
            item.width = refItem.width;
            item.height = refItem.height;
            item.maxStack = 1;
            item.value = 0;
            item.rare = 3;
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[item.type] = true;
            ItemID.Sets.ItemIconPulse[item.type] = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }


        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
        }

        public override bool CloneNewInstances
        {
            get { return true; }
        }

        public string npcType = "";

        public override ModItem Clone()
        {
            return (ModItem)this.MemberwiseClone();
        }
        public void setNPCType(NPC npc)
        {
            npcType = SoulOfNPC.NPCToTag(npc);
            if (npc.type == NPCID.SkeletonMerchant)
            {
                item.SetNameOverride("Soul of the Skeleton Merchant");
                Tooltip.SetDefault("The essence of the Skeleton Merchant");
            }
            else
            {
                item.SetNameOverride("Soul of the " + Lang.GetNPCNameValue(npc.netID));
                Tooltip.SetDefault("The essence of the " + Lang.GetNPCNameValue(npc.netID));
            }
        }

        public override TagCompound Save()
        {
            return new TagCompound() { { "npc" , npcType } };
        }

        public override void Load(TagCompound tag)
        {
            setNPCType(getNPCfromNPCTag((string)tag["npc"]));
        }

        public override void NetSend(BinaryWriter writer)
        {
            TagIO.Write(Save(), writer);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            Load(TagIO.Read(reader));
        }


        public static string NPCIDToTag(int type)
        {
            NPC n = new NPC();
            n.SetDefaults(type);
            return NPCToTag(n);
        }

        public static string NPCToTag(NPC npc)
        {
            if(npc.modNPC == null)
            {
                return "" + npc.type;
            }else
            {
                if (npc.modNPC.mod.GetNPC(npc.modNPC.GetType().Name) != null)
                {
                    return npc.modNPC.mod.Name + ":" + npc.modNPC.GetType().Name;
                }else
                {
                    return npc.modNPC.mod.Name + ":" + npc.modNPC.Name;
                }
            }
        }

        public static int getTypeFromNPCTag(string tag)
        {
            tag = tag.Trim(' ');
            int type = 0;
            if (!Int32.TryParse(tag, out type)) {
                Mod m = ModLoader.GetMod(tag.Split(':')[0]);
                if (m != null)
                    type = m.NPCType(tag.Split(':')[1]);
            }
        
            return type;
        }

        public static NPC getNPCfromNPCTag(string tag)
        {
            NPC ans = new NPC();
            ans.SetDefaults(getTypeFromNPCTag(tag));
            return ans;
        }


        public static int getTypeFromItemTag(string tag)
        {
            tag = tag.Trim(' ');
            int type = 0;
            if (!Int32.TryParse(tag, out type))
            {
                Mod m = ModLoader.GetMod(tag.Split(':')[0]);
                if (m != null)
                    type = m.ItemType(tag.Split(':')[1]);
            }
            return type;
        }

        public static string ItemToTag(Item item)
        {
            if (item.modItem == null)
            {
                return "" + item.type;
            }
            else
            {
                return item.modItem.mod.Name + ":" + item.modItem.GetType().Name;
            }
        }



    }

    
}
