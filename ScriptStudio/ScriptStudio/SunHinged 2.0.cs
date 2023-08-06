public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public float DetermineRotorDirection()
{
    IMySolarPanel oSolarPanelLeft = GridTerminalSystem.GetBlockWithName("Solar Panel: Bot-Left") as IMySolarPanel;
    IMySolarPanel oSolarPanelRight = GridTerminalSystem.GetBlockWithName("Solar Panel: Bot-Right") as IMySolarPanel;

    Echo("Left:    " + oSolarPanelLeft.MaxOutput);
    Echo("Right: " + oSolarPanelRight.MaxOutput);

    return -0.1f;
}

public float DetermineHingeDirection()
{
    return 0f;
}

public void Main(string argument, UpdateType updateSource)
{
    IMyMotorStator oRotor = GridTerminalSystem.GetBlockWithName("Rotor") as IMyMotorStator;
    IMySolarPanel oSolarPanel = GridTerminalSystem.GetBlockWithName("Solar Panel") as IMySolarPanel;
    IMyMotorStator oHinge = GridTerminalSystem.GetBlockWithName("Hinge") as IMyMotorStator;

    DetermineRotorDirection();
    DetermineHingeDirection();
}