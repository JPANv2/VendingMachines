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
    public class DyeBallMachineItem : VendingMachineItem
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Dye Ball Machine");
            Tooltip.SetDefault("Insert Strange Plant, out comes rare Dye!");
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            item.maxStack = 99;
            item.createTile = mod.TileType<DyeBallMachineTile>();
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
            Main.PlaySound(12, -1, -1, 1, 1f, 0f);
            int num30 = Main.player[Main.myPlayer].FindItem(ItemID.Sets.ExoticPlantsForDyeTrade);
            if (num30 != -1)
            {
                Main.player[Main.myPlayer].inventory[num30].stack--;
                if (Main.player[Main.myPlayer].inventory[num30].stack <= 0)
                {
                    Main.player[Main.myPlayer].inventory[num30] = new Item();
                }
                Main.PlaySound(24, -1, -1, 1, 1f, 0f);
                Main.player[Main.myPlayer].GetDyeTraderReward();
            }
            return;
        }

        public class DyeMachineRecipe : ModRecipe
        {
            int dyeSoulSlot = -1;
            public DyeMachineRecipe(Mod mod) : base(mod)
            {

            }

            public override bool RecipeAvailable()
            {
                dyeSoulSlot = -1;
                for (int i = 0; i < 58; i++)
                {
                    if (Main.player[Main.myPlayer].inventory[i].type == mod.ItemType<SoulOfNPC>())
                    {
                        if (((SoulOfNPC)(Main.player[Main.myPlayer].inventory[i].modItem)).npcType == "" + NPCID.DyeTrader)
                        {
                            dyeSoulSlot = i;
                            break;
                        }
                    }
                }
                return dyeSoulSlot > -1;
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
                Main.player[Main.myPlayer].inventory[dyeSoulSlot].SetDefaults(0);
                Main.player[Main.myPlayer].inventory[dyeSoulSlot].stack = 0;
                dyeSoulSlot = -1;
            }
        }


        public override void AddRecipes()
        {
            DyeMachineRecipe recipe = new DyeMachineRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 15);
            recipe.AddIngredient(ItemID.Glass, 25);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.Switch, 1);
            Item itm = new Item();
            itm.SetDefaults(mod.ItemType<SoulOfNPC>());
            (itm.modItem as SoulOfNPC).npcType = "" + NPCID.DyeTrader;
            recipe.AddIngredient(itm.modItem);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }



    }
}
