public void Main(string argument, UpdateType updateSource)
{
    IMyTextPanel oScreen = GridTerminalSystem.GetBlockWithName("Colors") as IMyTextPanel;

    string sRainbow = ""; // Grays
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";

    sRainbow += Environment.NewLine; // Reds
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";

    sRainbow += Environment.NewLine; // Yellows
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";

    sRainbow += Environment.NewLine; // Greens
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";

    sRainbow += Environment.NewLine; // Cyans
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";
    sRainbow += "";


    sRainbow += Environment.NewLine; // Clean
    sRainbow += "";


    sRainbow += Environment.NewLine;

    oScreen.WriteText(sRainbow);
}