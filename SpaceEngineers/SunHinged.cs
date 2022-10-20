public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public float fLastMax = 0;
public int iLowerCount = 0;

public void Main(string argument, UpdateType updateSource)
{
    IMySolarPanel oSolarPanel = GridTerminalSystem.GetBlockWithName("Solar Panel") as IMySolarPanel;
    IMyMotorStator oHinge = GridTerminalSystem.GetBlockWithName("Hinge 2") as IMyMotorStator;

    if (oSolarPanel.MaxOutput == 0)
        oHinge.SetValue<float>("UpperLimit", 89);

    if (this.iLowerCount >= 3)
    {
        oHinge.SetValue<float>("UpperLimit", oHinge.GetValue<float>("UpperLimit") - 1);
        this.iLowerCount = 0;
    }

    if (oSolarPanel.MaxOutput < this.fLastMax)
        this.iLowerCount++;

    if (oSolarPanel.MaxOutput > this.fLastMax)
        this.iLowerCount = 0;

    this.fLastMax = oSolarPanel.MaxOutput;

    this.Log(oHinge.GetValue<float>("UpperLimit") + "°: " + Math.Round(this.fLastMax * 1000, 2).ToString() + ": " + Math.Round(oSolarPanel.MaxOutput * 1000, 2).ToString());
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

    List<string> aOldText = oScreen.GetText().Split('\n').ToList();

    if (aOldText.Count >= 1)
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