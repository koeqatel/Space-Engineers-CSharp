public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public float fRotorVelocityPrev = 3;
public float fRotorSolarPanelOutputPrev = 0;

public void SetBestSolarAngle(IMyMotorStator oRotor, IMySolarPanel oPanel)
{
    if (oPanel.MaxOutput < fRotorSolarPanelOutputPrev)
    {
        fRotorVelocityPrev = fRotorVelocityPrev * -1 * 0.5f;
    }

    fRotorSolarPanelOutputPrev = oPanel.MaxOutput;

    oRotor.SetValue<float>("Velocity", fRotorVelocityPrev);
}

public float fHingeSolarPanelOutputPrev = 0;
public int iTimesDecreased = 0;
public float fOldMaxOutput = 0;

public void Main(string argument, UpdateType updateSource)
{
    IMySolarPanel oSolarPanel = GridTerminalSystem.GetBlockWithName("Solar Panel") as IMySolarPanel;
    IMyMotorStator oHinge = GridTerminalSystem.GetBlockWithName("Hinge") as IMyMotorStator;
    IMyMotorStator oRotor = GridTerminalSystem.GetBlockWithName("Rotor") as IMyMotorStator;

    if (oSolarPanel.MaxOutput == 0)
    {
        oHinge.SetValue<float>("UpperLimit", 88);
        fRotorVelocityPrev = 1;
        return;
    }

Log(oHinge);
    Echo (oHinge.Angle.ToString());

    if (fRotorVelocityPrev > 0.000001 || fRotorVelocityPrev < -0.000001 || fOldMaxOutput > oSolarPanel.MaxOutput + 0.0005)
    {
        SetBestSolarAngle(oRotor, oSolarPanel);
        fOldMaxOutput = oSolarPanel.MaxOutput;
    }
    else
    {
        oRotor.SetValue<float>("Velocity", 0);
        if (iTimesDecreased >= 3)
        {
            oHinge.SetValue<float>("UpperLimit", oHinge.GetValue<float>("UpperLimit") - 1);
            iTimesDecreased = 0;
        }

        if (oSolarPanel.MaxOutput < fHingeSolarPanelOutputPrev)
            iTimesDecreased++;

        if (oSolarPanel.MaxOutput > fHingeSolarPanelOutputPrev)
            iTimesDecreased = 0;

        fHingeSolarPanelOutputPrev = oSolarPanel.MaxOutput;
    }
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