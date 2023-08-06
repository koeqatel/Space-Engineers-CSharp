
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

    oFlightSeatScreen.ContentType = ContentType.TEXT_AND_IMAGE;
    // Blue bg color = 0, 136, 190
    oFlightSeatScreen.FontColor = new Color(255, 255, 255);
    oFlightSeatScreen.BackgroundColor = new Color(0, 136, 190);
    oFlightSeatScreen.Font = "Monospace";
    oFlightSeatScreen.FontSize = 4;
    oFlightSeatScreen.TextPadding = 32;
    oFlightSeatScreen.Alignment = TextAlignment.CENTER;

    oFlightSeatScreen.WriteText(sCoords);
}
