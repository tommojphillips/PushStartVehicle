using MSCLoader;

using System;

using TommoJProductions.ModApi.Database;

using UnityEngine;

namespace TommoJProductions.PushStartVehicle
{
    public class PushStartVehicle : Mod
    {
        public override string ID => "PushStartVehicle"; //Your mod ID (unique)
        public override string Name => "Push Start Vehicle"; //You mod name
        public override string Author => "tommojphillips"; //Your Username
        public override string Version => VersionInfo.version; //Version
        public override string Description => "Adds push start mechanics to the satsuma." + DESCRIPTION;//Short description of your mod

        public static readonly string DESCRIPTION = "\n Latest Release: " + VersionInfo.lastestRelease +
            "\nComplied With: ModApi v" + ModApi.VersionInfo.version + " BUILD " + ModApi.VersionInfo.build + ".";

        //private Satsuma m_satsuma;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, onLoad);
        }

        private void onLoad()
        {
            Database.databaseVehicles.satsuma.gameObject.AddComponent<VehiclePushStartLogic>();

            ModConsole.Print("PushStartVehicle: Loaded");
        }
    }
}
