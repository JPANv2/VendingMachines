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
    public class ReforgeMachineItem : VendingMachineItem
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Reforge Machine");
            Tooltip.SetDefault("Allows you to Reforge weapons and accessories");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            item.maxStack = 99;
            item.createTile = ModContent.TileType<ReforgeMachineTile>();
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
            Main.InGuideCraftMenu = false;
            Main.InReforgeMenu = true;
        }

        public class ReforgeMachineRecipe : ModRecipe
        {
            int goblinSoulSlot = -1;
            public ReforgeMachineRecipe(Mod mod) : base(mod)
            {

            }

            public override bool RecipeAvailable()
            {
                goblinSoulSlot = -1;
                for (int i = 0; i < 58; i++)
                {
                    if (Main.player[Main.myPlayer].inventory[i].type == ModContent.ItemType<SoulOfNPC>())
                    {
                        if (((SoulOfNPC)(Main.player[Main.myPlayer].inventory[i].modItem)).npcType == "" + NPCID.GoblinTinkerer)
                        {
                            goblinSoulSlot = i;
                            break;
                        }
                    }
                }
                return goblinSoulSlot > -1;
            }

            public override int ConsumeItem(int type, int numRequired)
            {
                if (type == ModContent.ItemType<SoulOfNPC>())
                {
                    return 0;
                }
                return numRequired;
            }

            public override void OnCraft(Item item)
            {
                Main.player[Main.myPlayer].inventory[goblinSoulSlot].SetDefaults(0);
                Main.player[Main.myPlayer].inventory[goblinSoulSlot].stack = 0;
                goblinSoulSlot = -1;
            }
        }

        public override void AddRecipes()
        {
            ReforgeMachineRecipe recipe = new ReforgeMachineRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 15);
            recipe.AddIngredient(ItemID.Glass, 25);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.Switch, 1);
            Item itm = new Item();
            itm.SetDefaults(ModContent.ItemType<SoulOfNPC>());
            (itm.modItem as SoulOfNPC).npcType = "" + NPCID.GoblinTinkerer;
            recipe.AddIngredient(itm.modItem);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }



    }
}
