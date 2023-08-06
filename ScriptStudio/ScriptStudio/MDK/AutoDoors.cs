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
namespace SpaceEngineers.Auto.Doors
{
    public sealed class AutoDoors : MyGridProgram
    {
        // Your code goes between the next #endregion and #region
        #endregion

        public Program()
        {
        }

        public void Main(string args)
        {
            var oReactor = GridTerminalSystem.GetBlockWithName("Small Reactor") as IMyReactor;
            var vPos = oReactor.GetPosition();

            var vRelativePos = this.GetRelativePosition(vPos.X, vPos.Y, vPos.Z);
            // var bX = Math.Round(vRelativePos.X, 5).ToString();
            // var bY = Math.Round(vRelativePos.Y, 5).ToString();
            // var bZ = Math.Round(vRelativePos.Z, 5).ToString();

            // this.PositionShip(vRelativePos);

            // this.Log(Me.CustomName + "(" + bX + ", " + bY + ", " + bZ + ")");
            
            var division = vRelativePos.Y / vRelativePos.X;
            var hoek = Math.Round(1 / division, 3);

            this.Log("Hoek: " + hoek + Environment.NewLine + " Reactor pos: " + vPos.X + Environment.NewLine + " Prog blk pos: " + Me.GetPosition().X + Environment.NewLine);
        }

        public void Log(string sMessage)
        {
            var oScreen = GridTerminalSystem.GetBlockWithName("Log") as IMyTextPanel;
            List<string> aOldText = oScreen.GetText().Split('\n').ToList();

            if (aOldText.Count >= 10)
                aOldText.RemoveAt(0);

            string sNewText = String.Join(Environment.NewLine, aOldText.ToArray()) + Environment.NewLine + DateTime.Now.ToString("H:mm:ss") + ": " + sMessage;
            oScreen.WriteText(sNewText);
        }

        Vector3D vLastRelativePos = new Vector3D(0, 0, 0);

        public void PositionShip(Vector3D vRelativePos)
        {
            if (vRelativePos.X != (float)0)
                this.Position(vLastRelativePos.X, vRelativePos.X);

            if (vRelativePos.Y != (float)0)
                this.Position(vLastRelativePos.Y, vRelativePos.Y);

            if (vRelativePos.Z != (float)0)
                this.Position(vLastRelativePos.Z, vRelativePos.Z);

        }

        public void Position(double iLastRelative, double iCurrent)
        {
            var oGyro = GridTerminalSystem.GetBlockWithName("Gyroscope") as IMyGyro;
            oGyro.GyroOverride = true;

            if (iCurrent < 0)
                oGyro.Yaw = (float)0.1;
            else if (iCurrent > 0)
                oGyro.Yaw = (float)-0.1;
            else if (iCurrent == 0)
                oGyro.Yaw = (float)0;

            if (iCurrent < 0)
                oGyro.Pitch = (float)0.1;
            else if (iCurrent > 0)
                oGyro.Pitch = -(float)-0.1;
            else if (iCurrent == 0)
                oGyro.Pitch = (float)0;

            if (iCurrent < 0)
                oGyro.Roll = (float)0.1;
            else if (iCurrent > 0)
                oGyro.Roll = (float)-0.1;
            else if (iCurrent == 0)
                oGyro.Roll = (float)0;
        }

        public Vector3D GetRelativePosition(double dX, double dY, double dZ)
        {
            var vPos = Me.GetPosition();

            double dRelX = dX - vPos.X;
            double dRelY = dY - vPos.Y;
            double dRelZ = dZ - vPos.Z;

            return new Vector3D(dRelX, dRelY, dRelZ);
        }

        public void GetClosestDoor()
        {

        }

        #region PreludeFooter
    }
}
#endregion
