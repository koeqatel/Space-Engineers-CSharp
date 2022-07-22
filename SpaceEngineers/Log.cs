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
        public void Main(string argument, UpdateType updateSource)
        {
            IMyTerminalBlock oSeat = GridTerminalSystem.GetBlockWithName("Cockpit: Flight Seat") as IMyCockpit;            
            this.Log(oSeat);
        }

        IMyTextPanel PreparePanel()
        {
            IMyTextPanel oScreen = GridTerminalSystem.GetBlockWithName("Log") as IMyTextPanel;
            oScreen.SetValue<Int64>("Content", 1);
            oScreen.SetValue<Color>("FontColor", new Color(100, 100, 100));

            return oScreen;
        }

        void Log(string sMessage)
        {
            IMyTextPanel oScreen = PreparePanel();

            List<string> aOldText = oScreen.GetText().Split('\n').ToList();

            if (aOldText.Count >= 17)
                aOldText.RemoveAt(0);

            string sNewText = String.Join(Environment.NewLine, aOldText.ToArray()) + Environment.NewLine + DateTime.Now.ToString("H:mm:ss") + ": " + sMessage;
            oScreen.WriteText(sNewText);
        }

        void Log(IMyTerminalBlock aMessage)
        {
            IMyTextPanel oScreen = PreparePanel();

            //Get Actions
            List<ITerminalAction> aActions = new List<ITerminalAction>();
            aMessage.GetActions(aActions);

            string sActions = "Actions: " + Environment.NewLine;

            for (int i = 0; i < aActions.Count; i++)
                sActions += aActions[i].Name + Environment.NewLine;

            //Get Properties
            List<ITerminalProperty> aProperties = new List<ITerminalProperty>();
            aMessage.GetProperties(aProperties);

            string sProperties = "Properties: " + Environment.NewLine;

            for (int i = 0; i < aProperties.Count; i++)
                sProperties += aProperties[i].Id + Environment.NewLine;

            oScreen.WriteText(sActions + Environment.NewLine + sProperties);
        }


        #region PreludeFooter
    }
}
#endregion