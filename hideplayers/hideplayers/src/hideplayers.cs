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
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace hideplayers.src
{
    public class hideplayers: ModSystem
    {
        public static Harmony harmonyInstance;
        public static ICoreServerAPI sapi;
        public static Config config;
        public const string harmonyID = "hideplayers.Patches";
        public override void StartServerSide(ICoreServerAPI api)
        {
            harmonyInstance = new Harmony(harmonyID);
            sapi = api;
            loadConfig(api);
            doPatches();
        }
       /* public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            harmonyInstance = new Harmony(harmonyID);
            harmonyInstance.Patch(typeof(ClientSystemEntities).GetMethod("createOrUpdateEntityFromPacket", BindingFlags.Public | BindingFlags.Static), postfix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_createOrUpdateEntityFromPacket")));
        }*/
        public static void doPatches()
        {
            harmonyInstance.Patch(typeof(Vintagestory.Server.ServerMain).GetMethod("SendPlayerEntities", BindingFlags.NonPublic | BindingFlags.Instance), prefix: new HarmonyMethod(typeof(harmPatch).GetMethod("Prefix_BroadcastPacket")));
            harmonyInstance.Patch(typeof(Vintagestory.Server.ServerMain).GetMethod("SendInitialPlayerDataForOthers"), prefix: new HarmonyMethod(typeof(harmPatch).GetMethod("Prefix_SendInitialPlayerDataForOthers")));
        }
        public override void Dispose()
        {
            base.Dispose();
            harmonyInstance.UnpatchAll(harmonyID);
            config = null;
        }
        private void loadConfig(ICoreAPI api)
        {
            config = api.LoadModConfig<Config>(this.Mod.Info.ModID + ".json");
            if (config == null)
            {
                config = new Config();
                api.StoreModConfig<Config>(config, this.Mod.Info.ModID + ".json");
                return;
            }
        }
    }
}
