public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

float fWidth = 5;
bool bHasBeenOne = false;

public void Main(string argument, UpdateType updateSource)
{
    var aSensorsGroup = GridTerminalSystem.GetBlockGroupWithName("Alignment Sensors");
    List<IMyTerminalBlock> aSensors = new List<IMyTerminalBlock>();
    aSensorsGroup.GetBlocks(aSensors, null);

    int iActiveSensors = 0;

    IMyPistonBase oPiston = GridTerminalSystem.GetBlockWithName("Piston Right") as IMyPistonBase;
    IMyLandingGear oLandingGear = GridTerminalSystem.GetBlockWithName("Landing Gear Right") as IMyLandingGear;

    foreach (IMySensorBlock oSensor in aSensors)
    {
        if (oSensor.IsActive)
            iActiveSensors++;
    }

    if (!bHasBeenOne)
        bHasBeenOne = iActiveSensors == 1;

    if (iActiveSensors == 1)
        oPiston.Velocity = (float) 0.1;

    if (iActiveSensors == 2)
        fWidth = fWidth - (float)0.1;

    // Echo(bHasBeenOne.ToString());

    if (bHasBeenOne && iActiveSensors == 0)
    {
        fWidth = (float)5;
        oLandingGear.ApplyAction("SwitchLock");
        oPiston.Velocity = (float)-5;
        bHasBeenOne = false;
    }

    SetSensors(aSensors, fWidth, fWidth, 10);
}

void SetSensors(List<IMyTerminalBlock> aSensors, float LeftExtend = 0, float RightExtend = 0, float FrontExtend = 0)
{
    foreach (IMySensorBlock oSensor in aSensors)
    {
        oSensor.LeftExtend = LeftExtend;
        oSensor.RightExtend = RightExtend;
        oSensor.BottomExtend = 0;
        oSensor.TopExtend = 0;
        oSensor.BackExtend = 0;
        oSensor.FrontExtend = FrontExtend;

        oSensor.DetectAsteroids = false;
        oSensor.DetectFloatingObjects = false;
        oSensor.DetectLargeShips = true;
        oSensor.DetectPlayers = true;
        oSensor.DetectSmallShips = false;
        oSensor.DetectStations = false;

        oSensor.DetectOwner = true;
        oSensor.DetectFriendly = true;
        oSensor.DetectEnemy = false;
        oSensor.DetectNeutral = false;
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