public void Main(string argument, UpdateType updateSource)
{
    IMyMotorAdvancedStator oRotor = GridTerminalSystem.GetBlockWithName("Drill Rotor") as IMyMotorAdvancedStator;

    oRotor.TargetVelocityRPM = -oRotor.Angle * 4;

    if (Math.Round(oRotor.Angle, 2) == (double)0.000)
    {
        oRotor.TargetVelocityRPM = 0;
        IMyTimerBlock oTimer = GridTerminalSystem.GetBlockWithName("Rotor Timer") as IMyTimerBlock;
        oTimer.ApplyAction("OnOff_Off");
    }
    else
    {
        this.Log(oRotor.Angle.ToString());
    }
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