using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using VendingMachines.Items;
using VendingMachines.Tiles;

namespace VendingMachines
{

    public class VendingMachineData
    {
        public int x = -1;
        public int y = -1;
        public string id = null;
        public bool isItem = false;

        public bool isClear()
        {
            return x < 0 || y < 0;
        }

        public string Clear()
        {
            x = -1;
            y = -1;
            string ans = (isItem? "Item;": "NPC;") + id;
            id = null;
            isItem = false;
            return ans;
        }

        public int getNPCID()
        {
            if (isItem || id == null)
                return 0;
            return  SoulOfNPC.getTypeFromNPCTag(id);
        }

        public int getItemID()
        {
            if (!isItem || id == null)
                return 0;
            return SoulOfNPC.getTypeFromItemTag(id);
        }

        public void setDefaults(int x1, int y1, string type, bool item = false)
        { 
            x = x1;
            y = y1;
            id = type;
            isItem = item;
        }

        public TagCompound Save()
        {
            TagCompound ans = new TagCompound();
            ans["x"] = x;
            ans["y"] = y;
            ans["id"] = id;
            if (isItem)
                ans["isItem"] = true;
            return ans;
        }

        public void Load(TagCompound tag)
        {
            x = (int)tag["x"];
            y = (int)tag["y"];
            if (tag.ContainsKey("npcID"))
            {
                id = (string)tag["npcID"];
            }
            else
            {
                id = (string)tag["id"];
            }
            isItem = tag.ContainsKey("isItem");    
        }
    }

    public class VendingMachineWorld : ModWorld
    {
        public static VendingMachineData[] vm = new VendingMachineData[400];


        public static int CheckForEmptyVendingMachine(int x, int y, int type, int style, int direction = 0)
        {

            /*VendingMachineItem toPlace = (Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].modItem) as VendingMachineItem;
            if (toPlace == null)
                return -1;
            */
            try
            {
                for (int i = 0; i < vm.Length; i++)
                {
                    if (vm[i] != null && vm[i].x == x && vm[i].y == y)
                    {
                        return i;
                    }
                }

                for (int i = 0; i < vm.Length; i++)
                {
                    if (vm[i] == null || vm[i].isClear())
                    {
                        return i;
                    }
                }
            }
            catch (Exception e)
            {
                Main.NewText(e.ToString());
            }
            return 399;
        }

        public static int PlaceNewVendingMachine(int x, int y, int type, int style, int direction = 0)
        {
            try
            {
                Point16 point = new Point16(x, y);
                TileObjectData.OriginToTopLeft(type, style, ref point);

                VendingMachineItem toPlace = (Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].modItem) as VendingMachineItem;
                if (toPlace == null)
                    return 399;
                if (toPlace.hasShop && (toPlace.npcType == null || toPlace.npcType == ""))
                    toPlace.npcType = SoulOfNPC.ItemToTag(toPlace.item);

                for (int i = 0; i < vm.Length; i++)
                {
                    if (vm[i] != null && vm[i].x == x && vm[i].y == y)
                    {
                        vm[i].setDefaults(point.X, point.Y, toPlace.npcType, toPlace.hasShop);
                        if (Main.netMode != 0)
                        {
                            ModPacket pk = ModLoader.GetMod("VendingMachines").GetPacket();
                            pk.Write((byte)1);
                            pk.Write((short)i);
                            TagIO.Write(vm[i].Save(), pk);
                            pk.Send();
                        }
                        return i;
                    }
                }

                for (int i = 0; i < vm.Length; i++)
                {
                    if (vm[i] == null)
                    {
                        vm[i] = new VendingMachineData();
                        vm[i].setDefaults(point.X, point.Y, toPlace.npcType, toPlace.hasShop);
                        if (Main.netMode != 0)
                        {
                            ModPacket pk = ModLoader.GetMod("VendingMachines").GetPacket();
                            pk.Write((byte)1);
                            pk.Write((short)i);
                            TagIO.Write(vm[i].Save(), pk);
                            pk.Send();
                        }
                        return i;

                    }
                    else if (vm[i].isClear())
                    {
                        vm[i].setDefaults(point.X, point.Y, toPlace.npcType, toPlace.hasShop);
                        if (Main.netMode != 0)
                        {
                            ModPacket pk = ModLoader.GetMod("VendingMachines").GetPacket();
                            pk.Write((byte)1);
                            pk.Write((short)i);
                            TagIO.Write(vm[i].Save(), pk);
                            pk.Send();
                        }
                        return i;
                    }
                }
                return 399;

            }catch(Exception e)
            {
                Main.NewText(e.ToString());
                return 399;
            }
        }

        public static int GetVendingMachineFromCoordinates(int tx, int ty)
        {
            for (int i = 0; i < vm.Length; i++)
            {
                if (vm[i] != null && !vm[i].isClear() && vm[i].x == tx && vm[i].y == ty)
                {
                    return i;
                }
            }
            return -1;
        }

        

        public override TagCompound Save()
        {
            TagCompound ans = new TagCompound();

            for(int i = 0; i< vm.Length; i++)
            {
                if(vm[i] != null && !vm[i].isClear())
                {
                    ans["vm." + i] = vm[i].Save();
                }
            }

            return ans;
        }

        public override void Load(TagCompound tag)
        {
            for (int i = 0; i < vm.Length; i++)
            {
                if (tag.ContainsKey("vm."+i))
                {
                    if (vm[i] == null)
                        vm[i] = new VendingMachineData();
                    vm[i].Load((TagCompound)tag["vm." + i]);
                }
            }
        }


        public override void NetSend(BinaryWriter writer)
        {
            TagIO.Write(Save(), writer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Load(TagIO.Read(reader));
        }

        public override void PostUpdate()
        {
            if(Main.time == 1 && Main.dayTime)
            {
                Chest.SetupTravelShop();
            }
            if(Main.time % 600 == 0)
            {
                for (int i = 0; i < vm.Length; i++)
                {
                    if (vm[i] != null && !vm[i].isClear())
                    {
                        ModTile t = TileLoader.GetTile(Main.tile[vm[i].x, vm[i].y].type);
                        if (!(t is VendingMachine))
                        {
                            vm[i].Clear();
                        }
                    }
                }
            }
        }

    }
}
