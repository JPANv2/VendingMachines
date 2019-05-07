using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace VendingMachines.NPCs
{
    public class InvisibleAllShopNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {

            //npc.immortal = true;
            npc.friendly = true;
            //npc.townNPC = true;
            npc.height = 1;//64;
            npc.width = 1;// 48;
            npc.lifeMax = 1;
            npc.trapImmune = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.behindTiles = true;
            npc.alpha = 0;
            npc.hide = true;
            npc.aiStyle = -1;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return false;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false;
        }

        public override void AI()
        {
            if(npc.ai[0] > 0)
            {
                if(npc.ai[0] == Main.myPlayer)
                {
                    if (!Main.playerInventory)
                    {
                        npc.ai[0] = -1;
                        npc.StrikeNPCNoInteraction(9999, 0, 1);
                    }
                }
            }
           // npc.velocity.Y = -10f;
        }

    }
}
