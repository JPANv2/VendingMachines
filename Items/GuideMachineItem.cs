using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VendingMachines.Tiles;

namespace VendingMachines.Items
{
    public class GuideMachineItem : VendingMachineItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Guide's Crafting Help Machine");
            Tooltip.SetDefault("Allows you to consult the Recipe Book");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            
            item.maxStack = 99;
            item.createTile = mod.TileType<GuideMachineTile>();
            hasShop = true;
            npcType = SoulOfNPC.ItemToTag(item);
        }

        public override bool replaceRightClick
        {
            get
            {
                return true;
            }
        }

        public override void machineRightClick(VendingMachineData vm, int i, int j)
        {
            Main.player[Main.myPlayer].chest = -1;
            Main.npcChatText = "";
            VendingMachine.chooseTalkingNPC(vm);
            Main.recBigList = false;
            Main.playerInventory = true;
            Main.InGuideCraftMenu = true;
            Main.InReforgeMenu = false;
        }

        public class GuideMachineRecipe : ModRecipe
        {
            int guideSoulSlot = -1;
            public GuideMachineRecipe(Mod mod) : base(mod)
            {

            }

            public override bool RecipeAvailable()
            {
                guideSoulSlot = -1;
                for (int i = 0; i < 58; i++)
                {
                    if (Main.player[Main.myPlayer].inventory[i].type == mod.ItemType<SoulOfNPC>())
                    {
                        if (((SoulOfNPC)(Main.player[Main.myPlayer].inventory[i].modItem)).npcType == "" + NPCID.Guide)
                        {
                            guideSoulSlot = i;
                            break;
                        }
                    }
                }
                return guideSoulSlot > -1;
            }

            public override int ConsumeItem(int type, int numRequired)
            {
                if (type == mod.ItemType<SoulOfNPC>())
                {
                    return 0;
                }
                return numRequired;
            }

            public override void OnCraft(Item item)
            {
                Main.player[Main.myPlayer].inventory[guideSoulSlot].SetDefaults(0);
                Main.player[Main.myPlayer].inventory[guideSoulSlot].stack = 0;
                guideSoulSlot = -1;
            }
        }

        public override void AddRecipes()
        {
            GuideMachineRecipe recipe = new GuideMachineRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 15);
            recipe.AddIngredient(ItemID.Glass, 25);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.Switch, 1);
            Item itm = new Item();
            itm.SetDefaults(mod.ItemType<SoulOfNPC>());
            (itm.modItem as SoulOfNPC).npcType = "" + NPCID.Guide;
            recipe.AddIngredient(itm.modItem);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }



    }
}
