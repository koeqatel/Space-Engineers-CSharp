#region Prelude
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;

// Change this namespace for each script you create.
namespace SpaceEngineers.UWBlockPrograms.BatteryMonitor
{
    public sealed class Program : MyGridProgram
    {
        // Your code goes between the next #endregion and #region
        #endregion

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
        public void Main(string args)
        {
            IMyCockpit oFlightSeat = GridTerminalSystem.GetBlockWithName("Cockpit: Flight Seat") as IMyCockpit;

            Vector3D vCoords = oFlightSeat.GetPosition();
            string sCoords = "(" + Math.Round(vCoords.X, 3) + ", " + Math.Round(vCoords.Y, 3) + ", " + Math.Round(vCoords.Z, 3) + ")";

            IMyTextSurface oFlightSeatScreen = oFlightSeat.GetSurface(0);
            oFlightSeatScreen.WriteText(sCoords);
        }

        #region PreludeFooter
    }
}
#endregion