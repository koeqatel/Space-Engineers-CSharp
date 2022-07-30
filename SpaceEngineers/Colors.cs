public void Main(string argument, UpdateType updateSource)
{
    IMyTextPanel oScreen = PreparePanel();
    oScreen.WriteText(GetColorChar(new Color(255, 0, 0)));
}

IMyTextPanel PreparePanel()
{
    IMyTextPanel oScreen = GridTerminalSystem.GetBlockWithName("Programmers Bay: LCD Colors") as IMyTextPanel;

    oScreen.SetValue<Int64>("Content", 1);
    // Blue bg color = 0, 136, 190
    oScreen.FontColor = new Color(255, 255, 255);
    oScreen.Font = "Monospace";
    oScreen.TextPadding = 0;

    return oScreen;
}

const double bitSpacing = 255.0 / 7.0;
static Color GetClosestColor(Color pixelColor)
{
    int R, G, B;
    R = (int)(Math.Round(pixelColor.R / bitSpacing) * bitSpacing);
    G = (int)(Math.Round(pixelColor.G / bitSpacing) * bitSpacing);
    B = (int)(Math.Round(pixelColor.B / bitSpacing) * bitSpacing);

    return new Color(R, G, B);
}

static char ColorToChar(byte r, byte g, byte b)
{
    return (char)(0xe100 + ((int)Math.Round(r / bitSpacing) << 6) + ((int)Math.Round(g / bitSpacing) << 3) + (int)Math.Round(b / bitSpacing));
}

public static string GetColorChar(Color pixelColor)
{
    Color oColor = GetClosestColor(pixelColor);
    return ColorToChar(oColor.R, oColor.G, oColor.B).ToString();
}