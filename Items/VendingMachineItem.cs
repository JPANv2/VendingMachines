using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using VendingMachines.Tiles;

namespace VendingMachines.Items
{

    class VendingMachineRecipe : ModRecipe
    {
        string npcType = null;
        public VendingMachineRecipe(Mod mod) : base(mod)
        {
            
        }



        public override int ConsumeItem(int type, int numRequired)
        {
            if(type == mod.ItemType<SoulOfNPC>())
            {
                for(int i = 0; i < 58; i++)
                {
                    if(Main.player[Main.myPlayer].inventory[i].type == type)
                    {
                        npcType = ((SoulOfNPC)(Main.player[Main.myPlayer].inventory[i].modItem)).npcType;
                        return numRequired;
                    }
                }
            }
            return numRequired;
        }

        public override void OnCraft(Item item)
        {
            
            NPC n = SoulOfNPC.getNPCfromNPCTag(npcType);
            ((VendingMachineItem)(item.modItem)).setNPCType(n);
            npcType = null;
        }
    }

    public class VendingMachineItem: ModItem
    {
        public bool hasShop = false;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Vending Machine (Empty)");
            Tooltip.SetDefault("Sells no items.");
        }

        public override void SetDefaults()
        {
            item.value = Item.sellPrice(0, 5, 0, 0);
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.maxStack = 1;
            item.consumable = true;

            item.width = 20;
            item.height = 20;

            item.createTile = mod.TileType<VendingMachine>();
            //item.placeStyle = 0;
            item.rare = 3;
        }

        public override void AddRecipes()
        {
            VendingMachineRecipe recipe = new VendingMachineRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 15);
            recipe.AddIngredient(ItemID.Glass, 25);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.Switch, 1);
            recipe.AddIngredient(mod, "SoulOfNPC", 1);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
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
            hasShop = false;
            if (npc == null || npc.type == 0)
            {
                item.SetNameOverride("Vending Machine (Empty)");
                Tooltip.SetDefault("Sells no items.");
               
            }else if (npc.type == NPCID.SkeletonMerchant)
            {
                item.SetNameOverride("Vending Machine (Skeleton Merchant)");
                Tooltip.SetDefault("Sells the items the Skeleton Merchant would sell.");
            }
            else
            {
                item.SetNameOverride("Vending Machine (" + Lang.GetNPCNameValue(npc.netID) + ")");
                Tooltip.SetDefault("Sells the items " + Lang.GetNPCNameValue(npc.netID) + " would sell.");
                
            }
        }

        public override TagCompound Save()
        {
            TagCompound ans = new TagCompound();
            if(!hasShop)
                ans["npc"] = npcType;

            return ans;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("npc"))
            {
                setNPCType(SoulOfNPC.getNPCfromNPCTag((string)tag["npc"]));
                hasShop = false;
            }

        }

        public override void NetSend(BinaryWriter writer)
        {
            TagIO.Write(Save(), writer);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            Load(TagIO.Read(reader));
        }


        public virtual bool replaceRightClick {
            get { return false; }
        }

        public virtual void machineRightClick(VendingMachineData vm, int i, int j)
        {

        }

        public virtual bool SetupShop(Chest shop)
        {
            return false;
        }

    }
}
