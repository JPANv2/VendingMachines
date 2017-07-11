using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VendingMachines.Items
{
	public class SoulExtractor : ModItem
	{

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Soul Extractor");
            Tooltip.SetDefault("Kill a Town NPC with this to obtain its essence. Can only kill town npcs");
        }

        public override void SetDefaults()
		{
			item.damage = 9999;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 2;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
		}

        public override bool? CanHitNPC(Player player, NPC target)
        {
            return target.townNPC || target.type == NPCID.SkeletonMerchant || isBoundNPC(target);
        }

        public static bool isBoundNPC(NPC target)
        {
            return target.type == NPCID.BoundGoblin || target.type == NPCID.BoundMechanic || target.type == NPCID.BoundWizard || target.type == NPCID.WebbedStylist ||
                target.type == NPCID.SleepingAngler || target.type == NPCID.DemonTaxCollector || target.type == NPCID.BartenderUnconscious;
        }

        public static NPC boundToFreedNPC(NPC target)
        {
            NPC ans = new NPC();
            if(target.type == NPCID.BoundGoblin)
            {
                ans.SetDefaults(NPCID.GoblinTinkerer);
            }
            else if (target.type == NPCID.BoundMechanic)
            {
                ans.SetDefaults(NPCID.Mechanic);
            }
            else if (target.type == NPCID.BoundWizard)
            {
                ans.SetDefaults(NPCID.Wizard);
            }
            else if (target.type == NPCID.WebbedStylist)
            {
                ans.SetDefaults(NPCID.Stylist);
            }
            else if (target.type == NPCID.SleepingAngler)
            {
                ans.SetDefaults(NPCID.Angler);
            }
            else if (target.type == NPCID.DemonTaxCollector)
            {
                ans.SetDefaults(NPCID.TaxCollector);
            }
            else if (target.type == NPCID.BartenderUnconscious)
            {
                ans.SetDefaults(NPCID.DD2Bartender);
            }

            return ans;

        }

        public override bool CanHitPvp(Player player, Player target)
        {
            return false;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.townNPC || target.type == NPCID.SkeletonMerchant || isBoundNPC(target))
            {
                damage = Math.Min(damage, target.lifeMax*2);
                crit = true;
            }else
            {
                damage = 0;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if((target.townNPC || target.type == NPCID.SkeletonMerchant || isBoundNPC(target)) && target.life <= 0)
            {
                int itm = Item.NewItem(target.position, new Microsoft.Xna.Framework.Vector2(target.width, target.height), mod.ItemType("SoulOfNPC"));
                if (isBoundNPC(target))
                {
                    ((SoulOfNPC)(Main.item[itm].modItem)).setNPCType(boundToFreedNPC(target));
                }
                else
                {
                    ((SoulOfNPC)(Main.item[itm].modItem)).setNPCType(target);
                }
                if(Main.netMode != 0)
                {
                    NetMessage.SendData(21, -1, -1, null, itm, 0f, 0f, 0f, 0, 0, 0); 
                }
            }
        }

        
        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("VendingMachines:AnyGoldBar", 12);
            recipe.AddRecipeGroup("VendingMachines:AnyVoodooDoll", 1);
            recipe.AddIngredient(ItemID.Diamond, 5);
			recipe.AddTile(TileID.Hellforge);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}


	}
}
