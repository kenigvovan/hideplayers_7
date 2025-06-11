using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;

namespace hideplayers.src
{
    [HarmonyPatch]
    public class harmPatch
    {
        public static bool Prefix_BroadcastPacket(ServerMain __instance, IServerPlayer player)
        {
            var pe = new Packet_Entities();
            
            if(__instance.Clients == null)
            {
                return false;
            }
            if(pe.Entities == null)
            {
                return true;
            }
            typeof(Packet_Entities).GetField("EntitiesLength", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(pe, __instance.Clients.Count);
            
            int num = 0;
            List<Packet_Entity> pca = new List<Packet_Entity>();//[__instance.Clients.Count];
            foreach (ConnectedClient value in __instance.Clients.Values)
            {
                if (value.Entityplayer != null && value.Entityplayer.ServerPos.DistanceTo(player.Entity.ServerPos) < hideplayers.config.DISTANCE_BETWEEN_PLAYERS_BROADCAST)
                {
                    var c = player.Entity.ServerPos.DistanceTo(value.Entityplayer.ServerPos);
                    pca.Add(ServerPackets.GetEntityPacket(value.Entityplayer));
                    num++;
                }
            }
            typeof(Packet_Entities).GetField("Entities", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(pe, pca.ToArray());
            typeof(Packet_Entities).GetField("EntitiesCount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(pe, pca.Count);
            
            var p = new Packet_Server();
            typeof(Packet_Server).GetField("Id", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(p, 40);
            typeof(Packet_Server).GetField("Entities", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(p, pe);
            __instance.SendPacket(player, p);
            return false;          
        }
        public static bool Prefix_SendInitialPlayerDataForOthers(ServerMain __instance, IServerPlayer owningPlayer, IServerPlayer toPlayer)
        {
            if (owningPlayer.Entity.ServerPos.DistanceTo(toPlayer.Entity.ServerPos) > hideplayers.config.DISTANCE_BETWEEN_PLAYERS_BROADCAST)
            {
                return false;
            }        
            return true;
        }
        /*public static void Postfix_createOrUpdateEntityFromPacket(ClientSystemEntities __instance, Packet_Entity entitypacket, ClientMain game, bool addToLoadQueue, Entity __result)
        {
            if(__result.Class.Equals("EnityPlayer"))
            {
                var f2 = 3;
            }
            var f = 3;
        }*/
    }
}
