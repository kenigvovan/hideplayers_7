using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace hideplayers.src
{
    public class hideplayers: ModSystem
    {
        public static Harmony harmonyInstance;
        public static ICoreServerAPI sapi;
        public const string harmonyID = "hideplayers.Patches";
        public override void StartServerSide(ICoreServerAPI api)
        {
            harmonyInstance = new Harmony(harmonyID);
            sapi = api;
            doPatches();
        }
        public static void doPatches()
        {
            harmonyInstance.Patch(typeof(Vintagestory.Server.ServerMain).GetMethod("SendPlayerEntities", BindingFlags.NonPublic | BindingFlags.Instance), prefix: new HarmonyMethod(typeof(harmPatch).GetMethod("Prefix_BroadcastPacket")));
            harmonyInstance.Patch(typeof(Vintagestory.Server.ServerMain).GetMethod("SendInitialPlayerDataForOthers"), prefix: new HarmonyMethod(typeof(harmPatch).GetMethod("Prefix_SendInitialPlayerDataForOthers")));
        }
        public override void Dispose()
        {
            base.Dispose();
            harmonyInstance.UnpatchAll();
        }
    }
}
