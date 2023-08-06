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
        Graphics g;

        public void Main(string args)
        {
            //paste the code up here

            if (g == null) g = new Graphics(100, 100, panel); //except use an actual display panel
            g.clear();
            g.circle("fill", 10, 20, 14);
            g.print(50, 12, "hello");
            g.paint();
        }


        public void Log(string sMessage)
        {
            var oScreen = GridTerminalSystem.GetBlockWithName("Log") as IMyTextPanel;
            oScreen.SetValue<Int64>("Content", 1);
            
            List<string> aOldText = oScreen.GetText().Split('\n').ToList();

            if (aOldText.Count >= 5)
                aOldText.RemoveAt(0);

            string sNewText = String.Join(Environment.NewLine, aOldText.ToArray()) + Environment.NewLine + DateTime.Now.ToString("H:mm:ss") + ": " + sMessage;
            oScreen.WriteText(sNewText);
        }


        #region PreludeFooter
    }
}
#endregion