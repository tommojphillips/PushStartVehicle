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
        public override string Version => "0.1"; //Version
        public override string Description => ""; //Short description of your mod

        private Satsuma m_satsuma;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, onLoad);
        }

        private void onLoad()
        {
            m_satsuma = Database.databaseVehicles.satsuma;
            m_satsuma.gameObject.AddComponent<VehiclePushStartLogic>();

            ModConsole.Print("PushStartVehicle: Loaded");
        }
    }
}
