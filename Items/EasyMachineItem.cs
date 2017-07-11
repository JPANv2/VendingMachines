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
    public class EasyMachineItem : VendingMachineItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Easy Vending Machine");
            Tooltip.SetDefault("Sells health and mana potions.");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
           
            item.maxStack = 99;
            item.createTile = mod.TileType<EasyMachineTile>();
            hasShop = true;
            npcType = SoulOfNPC.ItemToTag(item);
        }

        public override bool SetupShop(Item[] shop)
        {
            int i = 0;
            shop[i].SetDefaults(ItemID.LesserHealingPotion);
            i++;
            shop[i].SetDefaults(ItemID.LesserManaPotion);
            i++;
            if(NPC.downedBoss3 || NPC.downedBoss2 || NPC.downedBoss1)
            {
                shop[i].SetDefaults(ItemID.HealingPotion);
                i++;
                shop[i].SetDefaults(ItemID.ManaPotion);
                i++;
            }
            if (NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3)
            {
                shop[i].SetDefaults(ItemID.GreaterHealingPotion);
                i++;
                shop[i].SetDefaults(ItemID.GreaterManaPotion);
                i++;
            }
            if (NPC.downedMoonlord)
            {
                shop[i].SetDefaults(ItemID.SuperHealingPotion);
                i++;
                shop[i].SetDefaults(ItemID.SuperManaPotion);
                i++;
            }

            if (Main.hardMode)
            {
                shop[i].SetDefaults(ItemID.HeartreachPotion);
                i++;
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 15);
            recipe.AddIngredient(ItemID.Glass, 25);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.Switch, 1);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

    }
}
