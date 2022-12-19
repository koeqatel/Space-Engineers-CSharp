
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}
public void Main(string args)
{
    IMyCockpit oFlightSeat = GridTerminalSystem.GetBlockWithName("Cockpit: Flight Seat") as IMyCockpit;
    string Matrix1 = Math.Round(oFlightSeat.WorldMatrix.M11, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M12, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M13, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M14, 3).ToString();
    string Matrix2 = Math.Round(oFlightSeat.WorldMatrix.M21, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M22, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M23, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M24, 3).ToString();
    string Matrix3 = Math.Round(oFlightSeat.WorldMatrix.M31, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M32, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M33, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M34, 3).ToString();
    string Matrix4 = Math.Round(oFlightSeat.WorldMatrix.M41, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M42, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M43, 3).ToString() + ": " + Math.Round(oFlightSeat.WorldMatrix.M44, 3).ToString();

    IMyTextSurface oFlightSeatScreen = oFlightSeat.GetSurface(0);

    oFlightSeatScreen.ContentType = ContentType.TEXT_AND_IMAGE;
    // Blue bg color = 0, 136, 190
    oFlightSeatScreen.FontColor = new Color(255, 255, 255);
    oFlightSeatScreen.BackgroundColor = new Color(0, 136, 190);
    oFlightSeatScreen.Font = "Monospace";
    oFlightSeatScreen.FontSize = 3;
    oFlightSeatScreen.TextPadding = 0;
    oFlightSeatScreen.Alignment = TextAlignment.CENTER;

    oFlightSeatScreen.WriteText(Matrix1 + Environment.NewLine + Matrix2 + Environment.NewLine + Matrix3 + Environment.NewLine + Matrix4);
}

IMyTextPanel PreparePanel()
{
    IMyTextPanel oScreen = GridTerminalSystem.GetBlockWithName("Log") as IMyTextPanel;
    oScreen.SetValue<Int64>("Content", 1);
    // Blue bg color = 0, 136, 190
    oScreen.SetValue<Color>("FontColor", new Color(100, 100, 100));

    return oScreen;
}

void Log(string sMessage)
{
    IMyTextPanel oScreen = PreparePanel();

    string sNewText = sMessage;
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