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
namespace SpaceEngineers.UWBlockPrograms.BatteryMonitor {
    public sealed class Program : MyGridProgram {
    // Your code goes between the next #endregion and #region
#endregion

void Main(string argument)
{
    this.ShowStrips("Cargo");
    this.ShowStrips("Cockpit");
}

void ShowStrips(string name)
{
    List<IMyTextPanel> DoorLCDs = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(DoorLCDs);

    for (int i = 0; i < DoorLCDs.Count; i++)
    {
        if (DoorLCDs[i].CustomName.Contains(name) && DoorLCDs[i].CustomName.Contains("Door-LCD"))
        {
            string status = " Empty";           

            DoorLCDs[i].SetValue<Int64>("Content", 1);
            DoorLCDs[i].FontSize = 8;
            DoorLCDs[i].WriteText(status);
        }
    }
}

#region PreludeFooter
    }
}
#endregion