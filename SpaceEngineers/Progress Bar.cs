public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    IMyTextPanel oScreen = GridTerminalSystem.GetBlockWithName("Programmers Bay: Progress Bar") as IMyTextPanel;

    oScreen.SetValue<Int64>("Content", 1);
    // Blue bg color = 0, 136, 190
    oScreen.FontColor = new Color(255, 255, 255);
    oScreen.Font = "Monospace";
    oScreen.TextPadding = 0;

    

    oScreen.WriteText(ColorToChar(oColor.R, oColor.G, oColor.B).ToString);
}

public static byte[] StringToByteArray(string hex)
{
    return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
}

const double bitSpacing = 255.0 / 7.0;
Color GetClosestColor(Color pixelColor)
{
    int R, G, B;
    R = (int)(Math.Round(pixelColor.R / bitSpacing) * bitSpacing);
    G = (int)(Math.Round(pixelColor.G / bitSpacing) * bitSpacing);
    B = (int)(Math.Round(pixelColor.B / bitSpacing) * bitSpacing);

    return new Color3(R, G, B);
}

char ColorToChar(byte r, byte g, byte b)
{
    return (char)(0xe100 + ((int)Math.Round(r / bitSpacing) << 6) + ((int)Math.Round(g / bitSpacing) << 3) + (int)Math.Round(b / bitSpacing));
}

