public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
public Dictionary<float, float> aInfo = new Dictionary<float, float>();
public float iCounter = -1.4;
public float iLastMax = 0;


public void Main(string argument, UpdateType updateSource)
{
    IMyMotorStator oRotor = GridTerminalSystem.GetBlockWithName("Rotor") as IMyMotorStator;
    IMySolarPanel oSolarPanel = GridTerminalSystem.GetBlockWithName("Solar Panel") as IMySolarPanel;

    oRotor.SetValue<float>("UpperLimit", this.iCounter);

    if (this.iLastMax != oSolarPanel.MaxOutput)
    {
        aInfo.Add(oRotor.GetValue<float>("UpperLimit"), oSolarPanel.MaxOutput);
        this.iLastMax = oSolarPanel.MaxOutput;
        this.iCounter += 0.1f;
    }

    IMyTextPanel oScreen = PreparePanel();
    oScreen.WriteText(string.Join(Environment.NewLine, aInfo));
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

    if (aOldText.Count >= 0)
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