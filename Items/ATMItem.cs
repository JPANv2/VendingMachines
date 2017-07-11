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
    public class ATMItem : VendingMachineItem
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("ATM");
            Tooltip.SetDefault("Allows you to collect Taxes (if available)");
            
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            item.maxStack = 99;
            item.createTile = mod.TileType<ATMTile>();
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
            if (Main.player[Main.myPlayer].taxMoney > 0)
            {
                int l = Main.player[Main.myPlayer].taxMoney;
                while (l > 0)
                {
                    if (l > 1000000)
                    {
                        int num26 = l / 1000000;
                        l -= 1000000 * num26;
                        int number = Item.NewItem((int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, Main.player[Main.myPlayer].width, Main.player[Main.myPlayer].height, 74, num26, false, 0, false, false);
                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData(21, -1, -1, null, number, 1f, 0f, 0f, 0, 0, 0);
                        }
                    }
                    else if (l > 10000)
                    {
                        int num27 = l / 10000;
                        l -= 10000 * num27;
                        int number2 = Item.NewItem((int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, Main.player[Main.myPlayer].width, Main.player[Main.myPlayer].height, 73, num27, false, 0, false, false);
                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData(21, -1, -1, null, number2, 1f, 0f, 0f, 0, 0, 0);
                        }
                    }
                    else if (l > 100)
                    {
                        int num28 = l / 100;
                        l -= 100 * num28;
                        int number3 = Item.NewItem((int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, Main.player[Main.myPlayer].width, Main.player[Main.myPlayer].height, 72, num28, false, 0, false, false);
                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData(21, -1, -1, null, number3, 1f, 0f, 0f, 0, 0, 0);
                        }
                    }
                    else
                    {
                        int num29 = l;
                        if (num29 < 1)
                        {
                            num29 = 1;
                        }
                        l -= num29;
                        int number4 = Item.NewItem((int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, Main.player[Main.myPlayer].width, Main.player[Main.myPlayer].height, 71, num29, false, 0, false, false);
                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData(21, -1, -1, null, number4, 1f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
               // Main.npcChatText = Lang.dialog(Main.rand.Next(380, 382), false);
                Main.player[Main.myPlayer].taxMoney = 0;
                return;
            }
        }

        public class TaxMachineRecipe : ModRecipe
        {
            int taxSoulSlot = -1;
            public TaxMachineRecipe(Mod mod) : base(mod)
            {

            }

            public override bool RecipeAvailable()
            {
                taxSoulSlot = -1;
                for (int i = 0; i < 58; i++)
                {
                    if (Main.player[Main.myPlayer].inventory[i].type == mod.ItemType<SoulOfNPC>())
                    {
                        if (((SoulOfNPC)(Main.player[Main.myPlayer].inventory[i].modItem)).npcType == "" + NPCID.TaxCollector)
                        {
                            taxSoulSlot = i;
                            break;
                        }
                    }
                }
                return taxSoulSlot > -1;
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
                Main.player[Main.myPlayer].inventory[taxSoulSlot].SetDefaults(0);
                Main.player[Main.myPlayer].inventory[taxSoulSlot].stack = 0;
                taxSoulSlot = -1;
            }
        }

        public override void AddRecipes()
        {
            TaxMachineRecipe recipe = new TaxMachineRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 15);
            recipe.AddIngredient(ItemID.Glass, 25);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.Switch, 1);
            Item itm = new Item();
            itm.SetDefaults(mod.ItemType<SoulOfNPC>());
            (itm.modItem as SoulOfNPC).npcType = "" + NPCID.TaxCollector;
            recipe.AddIngredient(itm.modItem);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }



    }
}
