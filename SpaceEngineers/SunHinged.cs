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
}