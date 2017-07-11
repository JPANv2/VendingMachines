using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using VendingMachines.Items;
using VendingMachines.NPCs;

namespace VendingMachines.Tiles
{
    public class VendingMachine : ModTile
    {
        public int dropID = 0;
        public override void SetDefaults()
        {
            Main.tileFrameImportant[(int)Type] = true;
           
            Main.tileNoAttach[(int)Type] = true;
            Main.tileLavaDeath[(int)Type] = false;

            VendingMachineTileStyle();

            TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(VendingMachineWorld.CheckForEmptyVendingMachine), -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(VendingMachineWorld.PlaceNewVendingMachine), -1, 0, false);
            
            TileObjectData.addTile((int)Type);
            base.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            VendingMachineMapEntry();

            dropID = mod.ItemType("VendingMachineItem");
			
            disableSmartCursor = true;
        }

        public virtual void VendingMachineTileStyle()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.Origin = new Point16(0, 3);
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16,
                16
            };

            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };

        }

        public virtual void VendingMachineMapEntry()
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Vending Machine");
            base.AddMapEntry(new Microsoft.Xna.Framework.Color(200, 200, 200), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int x = i; int y = j;
            int vmID = VendingMachineWorld.GetVendingMachineFromCoordinates(x, y);
            int k = -1;
            if (vmID >= 0 && VendingMachineWorld.vm[vmID] != null)
            {
                string id = VendingMachineWorld.vm[vmID].Clear();
                string[] idSplit = id.Split(';');
                bool itm = (idSplit[0] == "Item");
                id = idSplit[1];
                if (Main.netMode != 1)
                {
                    if (itm)
                    {
                        k = Item.NewItem(x * 16, y * 16, 48, 64, SoulOfNPC.getTypeFromItemTag(id), 1, false, 0, false, false);
                    }else
                    {
                        k = Item.NewItem(x * 16, y * 16, 48, 64, dropID, 1, false, 0, false, false);
                        VendingMachineItem vmi = Main.item[k].modItem as VendingMachineItem;
                        if (vmi != null)
                        {
                            vmi.setNPCType(SoulOfNPC.getNPCfromNPCTag(id));
                        }
                    }
                }
            }else
            {
                if (Main.netMode != 1)
                {
                    k = Item.NewItem(x * 16, y * 16, 48, 64, dropID, 1, false, 0, false, false);
                }
            }
            if (Main.netMode == 2 && k >= 0)
            {
                NetMessage.SendData(21, -1, -1, null, k, 0f, 0f, 0f, 0, 0, 0);
            }
            if (Main.netMode != 0)
            {
                ModPacket pk = mod.GetPacket();
                pk.Write((byte)2);
                pk.Write((short)vmID);
                pk.Send();
            }
        }

        public override void MouseOver(int i, int j)
        {
            MouseOverFar(i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
           // fixFrames(i, j);
            int x = i; int y = j;
            while (Main.tile[x, y].frameY%70 != 0)
            {
                y--;
            }
            while (Main.tile[x, y].frameX % 54 != 0)
            {
                x--;
            }

            int vmID = VendingMachineWorld.GetVendingMachineFromCoordinates(x, y);
            if (vmID < 0)
                return;

            Player player = Main.player[Main.myPlayer];
            if (VendingMachineWorld.vm[vmID].isItem)
            {
                Item itm = new Item();
                itm.SetDefaults(SoulOfNPC.getTypeFromItemTag(VendingMachineWorld.vm[vmID].id));
                if (itm.type != 0)
                {
                    player.showItemIconText = itm.modItem.DisplayName.GetDefault();
                }
                else
                {
                    player.showItemIconText = "Vending Machine (Empty)";
                }
            }else
            {
                NPC n = SoulOfNPC.getNPCfromNPCTag(VendingMachineWorld.vm[vmID].id);

                if (n != null && n.type != 0)
                {
                    if (n.type == NPCID.SkeletonMerchant)
                    {
                        player.showItemIconText = "Vending Machine (Skeleton Merchant)";
                    }
                    else
                    {
                        player.showItemIconText = "Vending Machine (" + Lang.GetNPCNameValue(n.netID)+ ")";
                    }
                }
                else
                {
                    player.showItemIconText = "Vending Machine (Empty)";
                }
            }
            player.noThrow = 2;
            player.showItemIcon2 = -1;
            player.showItemIcon = true;
            


        }
        public override void RightClick(int i, int j)
        {
            
            
            int x = i; int y = j;
            while (Main.tile[x, y].frameY % 70 != 0)
            {
                y--;
            }
            while (Main.tile[x, y].frameX % 54 != 0)
            {
                x--;
            }

          
            int vmID = VendingMachineWorld.GetVendingMachineFromCoordinates(x, y);
            if (vmID < 0)
            {
                Main.NewText("No vending Machine Found...");
                return;
            }

            VendingMachineData vm = VendingMachineWorld.vm[vmID];
            if (vm.isItem)
            {
                Main.player[Main.myPlayer].chest = -1;
                Main.npcChatText = "";

                int type = SoulOfNPC.getTypeFromItemTag(vm.id);
                Item itm = new Item();
                itm.SetDefaults(type);

                VendingMachineItem vendor = itm.modItem as VendingMachineItem;
                if(vendor != null)
                {
                    if (vendor.replaceRightClick)
                    {
                        vendor.machineRightClick(vm, i, j);
                    }else
                    {
                       
                        Main.npcShop = Main.MaxShopIDs - 1;
                        chooseTalkingNPC(vm);
                        Main.recBigList = false;
                        Main.playerInventory = true;
                        Main.InGuideCraftMenu = false;
                        Main.InReforgeMenu = false;
                        for (int k = 0; k< Main.instance.shop[Main.npcShop].item.Length; k++)
                        {
                            Main.instance.shop[Main.npcShop].item[k] = new Item();
                        }
                        vendor.SetupShop(Main.instance.shop[Main.npcShop].item);
                    }
                }
            }
            else
            {
                int type = SoulOfNPC.getTypeFromNPCTag(vm.id);

                Main.npcShop = NPCToShop(type);
                
                if (type == NPCID.TravellingMerchant)
                {
                    Main.player[Main.myPlayer].chest = -1;
                    Main.npcChatText = "";
                    for (int k = 0; k < 40; k++)
                    {
                        if (Main.instance.shop[Main.npcShop].item[k] == null)
                            Main.instance.shop[Main.npcShop].item[k] = new Item();
                        Main.instance.shop[Main.npcShop].item[k].SetDefaults(Main.travelShop[k]);
                    }
                    Main.InGuideCraftMenu = false;
                    Main.InReforgeMenu = false;
                    Main.recBigList = false;
                    Main.playerInventory = true;
                    chooseTalkingNPC(vm);
                }

                /*Nurse code overrides the normal vending machine shop for the behaviour of the nurse heal option.*/

                else if (type == NPCID.Nurse)
                {
                    int num5 = Main.player[Main.myPlayer].statLifeMax2 - Main.player[Main.myPlayer].statLife;
                    for (int k = 0; k < 22; k++)
                    {
                        int num6 = Main.player[Main.myPlayer].buffType[k];
                        if (Main.debuff[num6] && Main.player[Main.myPlayer].buffTime[k] > 5 && BuffLoader.CanBeCleared(num6))
                        {
                            num5 += 1000;
                        }
                    }

                    if (Main.player[Main.myPlayer].BuyItem(num5))
                    {
                        AchievementsHelper.HandleNurseService(num5);
                        Main.PlaySound(SoundID.Item4, -1, -1);
                        Main.player[Main.myPlayer].HealEffect(Main.player[Main.myPlayer].statLifeMax2 - Main.player[Main.myPlayer].statLife, true);
                        Main.player[Main.myPlayer].statLife = Main.player[Main.myPlayer].statLifeMax2;
                        for (int k = 0; k < 22; k++)
                        {
                            int num23 = Main.player[Main.myPlayer].buffType[k];
                            if (Main.debuff[num23] && Main.player[Main.myPlayer].buffTime[k] > 0 && BuffLoader.CanBeCleared(num23))
                            {
                                Main.player[Main.myPlayer].DelBuff(k);
                                k = -1;
                            }
                        }
                        return;
                    }

                }
                else
                {
                    Main.player[Main.myPlayer].chest = -1;
                    Main.npcChatText = "";
                    chooseTalkingNPC(vm);
                    Main.recBigList = false;
                    Main.playerInventory = true;
                    Main.InGuideCraftMenu = false;
                    Main.InReforgeMenu = false;
                    Main.instance.shop[Main.npcShop].SetupShop(Main.npcShop < Main.MaxShopIDs - 1 ? Main.npcShop : type);
                }
            }

        }

        public static void chooseTalkingNPC(VendingMachineData vm)
        {
            Mod mod = ModLoader.GetMod("VendingMachines");
            if (Main.player[Main.myPlayer].talkNPC < 0 || Main.player[Main.myPlayer].talkNPC >= Main.npc.Length)
            {
                for (int i = Main.npc.Length -5; i >=0; i--)
                {
                    if (Main.npc[i].active && Main.npc[i].type == mod.NPCType<InvisibleAllShopNPC>() && (Main.npc[i].ai[0] == -1 || Main.npc[i].ai[0] == Main.myPlayer))
                    {
                        Main.npc[i].ai[0] = Main.myPlayer;
                        Main.npc[i].Center = new Vector2(vm.x*16 + 24, vm.y*16 + 32);//Main.player[Main.myPlayer].Center;
                       // Main.npc[i].name = "";
                        Main.player[Main.myPlayer].talkNPC = i;
                        return;
                    }
                }
                for (int i = Main.npc.Length - 5; i >= 0; i--)
                {
                    if (!Main.npc[i].active)
                    {
                        Main.npc[i] = new Terraria.NPC();
                        Main.npc[i].SetDefaults(mod.NPCType<InvisibleAllShopNPC>());
                        Main.npc[i].ai[0] = Main.myPlayer;
                        Main.npc[i].Center = new Vector2(vm.x * 16 + 24, vm.y * 16 + 32);//Main.player[Main.myPlayer].Center;
                       // Main.npc[i].name = "";
                        Main.player[Main.myPlayer].talkNPC = i;
                        return;
                    }
                }
            }
        }

        public static int NPCToShop(int type)
        {
            switch (type)
            {
                case 17: return 1;
                case 19: return 2;
                case 20: return 3;
                case 38: return 4;
                case 54: return 5;
                case 107: return 6;
                case 108: return 7;
                case 124: return 8;
                case 142: return 9;
                case 160: return 10;
                case 178: return 11;
                case 207: return 12;
                case 208: return 13;
                case 209: return 14;
                case 227: return 15;
                case 228: return 16;
                case 229: return 17;
                case 353: return 18;
                case 368: return 19;
                case 453: return 20;
                case 550: return 21;
                default: return 22;
            }
        }

    }
}
