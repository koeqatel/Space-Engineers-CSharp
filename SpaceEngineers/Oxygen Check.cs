public class SEFix
{
    public static T[] arr<T>(params T[] arg)
    {
        return arg; //becuse SE is stupid
    }
}
public class ColorUtils
{
    private static double oo64 = 1.0 / 64.0;
    private static double[][] map = new double[][] {
        new double[] { 0*oo64, 48*oo64, 12*oo64, 60*oo64,  3*oo64, 51*oo64, 15*oo64, 63*oo64},
        new double[] {32*oo64, 16*oo64, 44*oo64, 28*oo64, 35*oo64, 19*oo64, 47*oo64, 31*oo64},
        new double[] { 8*oo64, 56*oo64,  4*oo64, 52*oo64, 11*oo64, 59*oo64,  7*oo64, 55*oo64},
        new double[] {40*oo64, 24*oo64, 36*oo64, 20*oo64, 43*oo64, 27*oo64, 39*oo64, 23*oo64},
        new double[] { 2*oo64, 50*oo64, 14*oo64, 62*oo64,  1*oo64, 49*oo64, 13*oo64, 61*oo64},
        new double[] {34*oo64, 18*oo64, 46*oo64, 30*oo64, 33*oo64, 17*oo64, 45*oo64, 29*oo64},
        new double[] {10*oo64, 58*oo64,  6*oo64, 54*oo64,  9*oo64, 57*oo64,  5*oo64, 53*oo64},
        new double[] {42*oo64, 26*oo64, 38*oo64, 22*oo64, 41*oo64, 25*oo64, 37*oo64, 21*oo64}
    };

    private static int[][] palette = new int[][] {
        SEFix.arr( 255, 255, 0),
        SEFix.arr( 255, 0, 0),
        SEFix.arr( 0, 0, 255),
        SEFix.arr( 0, 255, 0),
        SEFix.arr( 255, 255, 255),
        SEFix.arr( 97, 97, 97),
        SEFix.arr( 0, 0, 0)
    };
    private static string[] colorStrings = new string[] {
        "\uE004", //oh but it works fine with *strings* -_-
        "\uE003", //red
        "\uE002", //yellow
        "\uE001", //green
        "\uE007\u0458", //white
        "\uE00D", //gray
        "\u2014\u0060" //black
    };

    private static int redC = 300;
    private static int greenC = 540;
    private static int blueC = 150;

    private static double compareColors(int r1, int g1, int b1, int r2, int g2, int b2)
    {
        double dl = ((r1 * redC + g1 * greenC + b1 * blueC) - (r2 * redC + g2 * greenC + b2 * blueC)) / 255000.0;
        double dr = (r1 - r2) / 255.0, dg = (g1 - g2) / 255.0, db = (b1 - b2) / 255.0;
        return ((dr * dr * redC + dg * dg * greenC + db * db * blueC) * 0.0075 + dl * dl);
    }
    private static double calcError(int r, int g, int b, int r0, int g0, int b0, int[] color1, int[] color2, double ratio)
    {
        return compareColors(r, g, b, r0, g0, b0) +
            compareColors(color1[0], color1[1], color1[2], color2[0], color2[1], color2[2]) * 0.03 * (Math.Abs(ratio - 0.5) + 0.5) *
            (1 + (color1[0] == color1[1] && color1[0] == color1[2] && color1[0] == color2[0] &&
             color1[0] == color2[1] && color1[0] == color2[2] ? 0.03 : 0));
    }
    private static int makeRatio(int r, int g, int b, int[] c1, int[] c2)
    {
        int ratio = 32;
        if (c1[0] != c2[0] || c1[1] != c2[1] || c1[2] != c2[2])
        {
            ratio =
                ((c2[0] != c1[0] ? redC * 64 * (r - c1[0]) / (c2[0] - c1[0]) : 0) +
                 (c2[1] != c1[1] ? greenC * 64 * (g - c1[1]) / (c2[1] - c1[1]) : 0) +
                 (c1[2] != c2[2] ? blueC * 64 * (b - c1[2]) / (c2[2] - c1[2]) : 0)) /
                ((c2[0] != c1[0] ? redC : 0) +
                 (c2[1] != c1[1] ? greenC : 0) +
                 (c2[2] != c1[2] ? blueC : 0));
            if (ratio < 0)
                ratio = 0;
            else if (ratio > 63)
                ratio = 63;
        }
        return ratio;
    }
    private static int[] createMix(int r, int g, int b)
    {
        int[] result = SEFix.arr(0, 0, 32);
        double minPenalty = Single.MaxValue;
        for (int i = 0; i < palette.Length; i++)
        {
            for (int j = i; j < palette.Length; j++)
            {
                int ratio = makeRatio(r, g, b, palette[i], palette[j]);
                double penalty = calcError(
                    r, g, b,
                    palette[i][0] + ratio * (palette[j][0] - palette[i][0]) / 64,
                    palette[i][1] + ratio * (palette[j][1] - palette[i][1]) / 64,
                    palette[i][2] + ratio * (palette[j][2] - palette[i][2]) / 64,
                    palette[i], palette[j],
                    (double)ratio / 64.0);
                if (penalty < minPenalty)
                {
                    minPenalty = penalty;
                    result[0] = i;
                    result[1] = j;
                    result[2] = ratio;
                }
            }
        }
        return result;
    }
    public static string[][] genDitherPattern(int r, int g, int b)
    {
        int[] mix = createMix(r, g, b);
        string[][] dithered = new string[8][];
        for (int x = 0; x < 8; x++)
        {
            dithered[x] = new string[8];
            for (int y = 0; y < 8; y++)
            {
                double mapValue = map[y & 7][x & 7];
                double ratio = mix[2] / 64.0;
                dithered[x][y] = colorStrings[mix[mapValue < ratio ? 1 : 0]];
            }
        }
        return dithered;
    }
}
public class Ascii
{
    private static int offset = 0x21;
    // binary numbers represent bitmap glyphs, three bits to a line
    // eg 9346 == 010 010 010 000 010 == !
    //    5265 == 001 010 010 010 001 == (
    private static short[] glyphs = SEFix.arr<short>(
        9346, 23040, 24445, 15602,
        17057, 10923, 9216, 5265,
        17556, 21824, 1488, 20,
        448, 2, 672, 31599,
        11415, 25255, 29326, 23497,
        31118, 10666, 29370, 10922,
        10954, 1040, 1044, 5393,
        3640, 17492, 25218, 15203,
        11245, 27566, 14627, 27502,
        31143, 31140, 14827, 23533,
        29847, 12906, 23469, 18727,
        24557, 27501, 11114, 27556,
        11131, 27565, 14478, 29842,
        23403, 23378, 23549, 23213,
        23186, 29351, 13459, 2184,
        25750, 10752, 7, 17408,
        239, 18862, 227, 4843,
        1395, 14756, 1886, 18861,
        8595, 4302, 18805, 25745,
        509, 429, 170, 1396,
        1369, 228, 1934, 18851,
        363, 362, 383, 341,
        2766, 3671, 5521, 9234,
        17620, 1920
    );
    public static short getGlyph(char code)
    {
        return glyphs[code - offset];
    }
}

public class Graphics
{
    public readonly int width;
    public readonly int height;
    private IMyTextPanel console;
    private string[] screen;
    private string[] screenLines;
    private string[][] fgDither;
    private string[][] bgDither;
    private int[] clip = new int[4];
    private Dictionary<string, string[][]> oldPatterns = new Dictionary<string, string[][]>();
    public Graphics(int w, int h, IMyTextPanel c)
    {
        width = w;
        height = h;
        screen = new string[height * width];
        screenLines = new string[width * height + height - 1];
        console = c;
        mask();
        setFG(255, 255, 255);
        setBG(0, 0, 0);
    }
    public void setFG(int r, int g, int b)
    {
        string k = r + ":" + g + ":" + b;
        if (!oldPatterns.TryGetValue(k, out fgDither))
        {
            fgDither = ColorUtils.genDitherPattern(r, g, b);
            oldPatterns[k] = fgDither;
        }
    }
    public void setBG(int r, int g, int b)
    {
        string k = r + ":" + g + ":" + b;
        if (!oldPatterns.TryGetValue(k, out bgDither))
        {
            bgDither = ColorUtils.genDitherPattern(r, g, b);
            oldPatterns[k] = bgDither;
        }
    }
    public void mask(int x1, int y1, int x2, int y2)
    {
        clip[0] = x1;
        clip[1] = y1;
        clip[2] = x2;
        clip[3] = y2;
    }
    public void mask()
    {
        clip[0] = 0;
        clip[1] = 0;
        clip[2] = width - 1;
        clip[3] = height - 1;
    }
    public void paint()
    {
        for (int i = 0; i < height; i++)
        {
            screenLines[i] = string.Join(null, screen, i * width, width) + "\n";
        }
        console.WriteText(string.Concat(screenLines));
        console.ShowTextureOnScreen();
        console.ShowPublicTextOnScreen();
    }
    public void clear()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < width; j += 8)
            {
                Array.Copy(bgDither[i], 0, screen, i * width + j, 8);
            }
        }
        int size = width * height;
        int half = width * height >> 1;
        for (int i = width * 8; i < size; i *= 2)
        {
            int copyLength = i;
            if (i > half)
            {
                copyLength = size - i;
            }
            Array.Copy(screen, 0, screen, i, copyLength);
        }
    }
    public void pixel(int x, int y)
    {
        if (x >= clip[0] && x <= clip[2] && y >= clip[1] && y <= clip[3])
        {
            screen[width * y + x] = fgDither[y & 7][x & 7];
        }
    }
    public void line(int x0, int y0, int x1, int y1)
    {
        if (x0 == x1)
        {
            int high = Math.Max(y1, y0);
            for (int y = Math.Min(y1, y0); y <= high; y++)
            {
                pixel(x0, y);
            }
        }
        else if (y0 == y1)
        {
            int high = Math.Max(x1, x0);
            for (int x = Math.Min(x1, x0); x <= high; x++)
            {
                pixel(x, y0);
            }
        }
        else
        {
            bool yLonger = false;
            int incrementVal, endVal;
            int shortLen = y1 - y0;
            int longLen = x1 - x0;
            if (Math.Abs(shortLen) > Math.Abs(longLen))
            {
                int swap = shortLen;
                shortLen = longLen;
                longLen = swap;
                yLonger = true;
            }
            endVal = longLen;
            if (longLen < 0)
            {
                incrementVal = -1;
                longLen = -longLen;
            }
            else incrementVal = 1;
            int decInc;
            if (longLen == 0) decInc = 0;
            else decInc = (shortLen << 16) / longLen;
            int j = 0;
            if (yLonger)
            {
                for (int i = 0; i - incrementVal != endVal; i += incrementVal)
                {
                    pixel(x0 + (j >> 16), y0 + i);
                    j += decInc;
                }
            }
            else
            {
                for (int i = 0; i - incrementVal != endVal; i += incrementVal)
                {
                    pixel(x0 + i, y0 + (j >> 16));
                    j += decInc;
                }
            }
        }
    }
    private void flatBottom(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        float invslope1 = (float)(x2 - x1) / (y2 - y1);
        float invslope2 = (float)(x3 - x1) / (y3 - y1);
        float curx1 = x1;
        float curx2 = x1;
        for (int scanlineY = y1; scanlineY <= y2; scanlineY++)
        {
            line((int)curx1, scanlineY, (int)curx2, scanlineY);
            curx1 += invslope1;
            curx2 += invslope2;
        }
    }
    private void flatTop(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        float invslope1 = (float)(x3 - x1) / (y3 - y1);
        float invslope2 = (float)(x3 - x2) / (y3 - y2);
        float curx1 = x3;
        float curx2 = x3;
        for (int scanlineY = y3; scanlineY > y1; scanlineY--)
        {
            curx1 -= invslope1;
            curx2 -= invslope2;
            line((int)curx1, scanlineY, (int)curx2, scanlineY);
        }
    }
    private void swap(ref int a, ref int b)
    {
        int c = a;
        a = b;
        b = c;
    }
    public void tri(string m, int x1, int y1, int x2, int y2, int x3, int y3)
    {
        if (m == "line")
        {
            line(x1, y1, x2, y2);
            line(x2, y2, x3, y3);
            line(x3, y3, x1, y1);
        }
        else if (m == "fill")
        {
            if (y1 > y3)
            {
                swap(ref y1, ref y3);
                swap(ref x1, ref x3);
            }
            if (y1 > y2)
            {
                swap(ref y1, ref y2);
                swap(ref x1, ref x2);
            }
            if (y2 > y3)
            {
                swap(ref y2, ref y3);
                swap(ref x2, ref x3);
            }
            if (y2 == y3)
            {
                flatBottom(x1, y1, x2, y2, x3, y3);
            }
            else if (y1 == y2)
            {
                flatTop(x1, y1, x2, y2, x3, y3);
            }
            else
            {
                int x4 = (int)(x1 + ((float)(y2 - y1) / (float)(y3 - y1)) * (x3 - x1));
                flatBottom(x1, y1, x2, y2, x4, y2);
                flatTop(x2, y2, x4, y2, x3, y3);
            }
        }
    }
    public void rect(string m, int xb, int yb, int w, int h)
    {
        if (m == "line")
        {
            line(xb, yb, xb, yb + h - 1);
            line(xb, yb, xb + w - 1, yb);
            line(xb + w - 1, yb, xb + w - 1, yb + h - 1);
            line(xb, yb + h - 1, xb + w - 1, yb + h - 1);
        }
        else if (m == "fill")
        {
            for (int x = xb; x < xb + w; x++)
            {
                for (int y = yb; y < yb + h; y++)
                {
                    pixel(x, y);
                }
            }
        }
    }
    public void ellipse(string m, int cx, int cy, int rx, int ry)
    {
        int rx2 = rx * rx;
        int ry2 = ry * ry;
        if (m == "fill")
        {
            int rxsys = rx2 * ry2;
            pixel(cx, cy);
            for (int i = 1; i < rx * ry; i++)
            {
                int x = i % rx;
                int y = i / rx;
                if (ry2 * x * x + rx2 * y * y <= rxsys)
                {
                    pixel(cx + x, cy + y);
                    pixel(cx - x, cy - y);
                    //if (x && y) { //unnecessary (prevents overdrawing pixels)
                    pixel(cx + x, cy - y);
                    pixel(cx - x, cy + y);
                    //}
                }
            }
        }
        else if (m == "line")
        {
            int frx2 = 4 * rx2;
            int fry2 = 4 * ry2;
            int s = 2 * ry2 + rx2 * (1 - 2 * ry);
            int y = ry;
            for (int x = 0; ry2 * x <= rx2 * y; x++)
            {
                pixel(cx + x, cy + y);
                pixel(cx - x, cy + y);
                pixel(cx + x, cy - y);
                pixel(cx - x, cy - y);
                if (s >= 0)
                {
                    s += frx2 * (1 - y);
                    y--;
                }
                s += ry2 * ((4 * x) + 6);
            }
            y = 0;
            s = 2 * rx2 + ry2 * (1 - 2 * rx);
            for (int x = rx; rx2 * y <= ry2 * x; y++)
            {
                pixel(cx + x, cy + y);
                pixel(cx - x, cy + y);
                pixel(cx + x, cy - y);
                pixel(cx - x, cy - y);
                if (s >= 0)
                {
                    s += fry2 * (1 - x);
                    x--;
                }
                s += rx2 * ((4 * y) + 6);
            }
        }
    }
    public void circle(string m, int cx, int cy, int r)
    {
        if (m == "fill")
        {
            int rr = r * r;
            pixel(cx, cy);
            for (int i = 1; i < r * r; i++)
            {
                int x = i % r;
                int y = i / r;
                if (x * x + y * y < rr)
                {
                    pixel(cx + x, cy + y);
                    pixel(cx - x, cy - y);
                    if (x > 0 && y > 0)
                    {
                        pixel(cx + x, cy - y);
                        pixel(cx - x, cy + y);
                    }
                }
            }
        }
        else if (m == "line")
        {
            int x = r;
            int y = 0;
            int do2 = 1 - x;
            while (y <= x)
            {
                pixel(cx + x, cy + y);
                pixel(cx + y, cy + x);
                pixel(cx - x, cy + y);
                pixel(cx - y, cy + x);
                pixel(cx - x, cy - y);
                pixel(cx - y, cy - x);
                pixel(cx + x, cy - y);
                pixel(cx + y, cy - x);
                y++;
                if (do2 <= 0)
                {
                    do2 += 2 * y + 1;
                }
                else
                {
                    do2 += 2 * (y - --x) + 1;
                }
            }
        }
    }
    public void print(int x, int y, string text)
    {
        int x1 = x;
        int y1 = y;
        for (int i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '\n':
                    y1 += 6;
                    x1 = x;
                    break;
                case ' ':
                    x1 += 4;
                    break;
                default:
                    short glyph = Ascii.getGlyph(text[i]);
                    int j = 14;
                    do
                    {
                        if ((glyph & 1) != 0)
                        {
                            pixel(x1 + j % 3, y1 - 4 + j / 3);
                        }
                        glyph >>= 1;
                        j--;
                    } while (glyph > 0);
                    x1 += 4;
                    break;
            }
        }
    }
}

Graphics graphics;
int counter;
void Main(string argument)
{
    if (counter == 0)
    {
        IMyTerminalBlock oOxyMap = GridTerminalSystem.GetBlockWithName("Cockpit: OxyMap") as IMyTextPanel;
        //LCD fontsize = 0.158, ration = 20:11
        graphics = new Graphics(180, 110, (IMyTextPanel)oOxyMap);
    }
    counter++;
    graphics.setBG(97, 97, 97);
    graphics.clear();

    this.PaintRoom("Cargo", 80, 55, 5, 7);
    this.PaintRoom("Cockpit", 70, 55, 12, 7);

    graphics.paint();
}

void PaintRoom(string name, int x, int y, int w, int h)
{
    List<IMyAirVent> ListVents = new List<IMyAirVent>();
    GridTerminalSystem.GetBlocksOfType<IMyAirVent>(ListVents);

    for (int i = 0; i < ListVents.Count; i++)
    {
        if (ListVents[i].CustomName.Contains(name))
        {
            if (ListVents[i].GetOxygenLevel() == 0)
            {
                graphics.setFG(255, 0, 0);
                CloseDoors(name);
                Lights(name, ListVents[i].GetOxygenLevel());
            }
            else if (ListVents[i].GetOxygenLevel() == 1)
            {
                graphics.setFG(0, 255, 0);
                Lights(name, ListVents[i].GetOxygenLevel());
            }
            else
            {
                graphics.setFG(255, 255, 0);
                Lights(name, ListVents[i].GetOxygenLevel());
            }

            //ShowStrips(name, ListVents[i].GetOxygenLevel());
        }
    }

    graphics.rect("fill", x, y, w, h);
    graphics.setFG(0, 0, 0);
    graphics.rect("line", x, y, w, h);
}

void CloseDoors(string name)
{
    List<IMyDoor> Doors = new List<IMyDoor>();
    GridTerminalSystem.GetBlocksOfType<IMyDoor>(Doors);

    for (int i = 0; i < Doors.Count; i++)
    {
        if (Doors[i].CustomName.Contains(name))
            Doors[i].GetActionWithName("Open_Off").Apply(Doors[i]);
    }
}

void Lights(string name, double oxygenLevel)
{
    List<IMyInteriorLight> Lights = new List<IMyInteriorLight>();
    GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(Lights);

    for (int i = 0; i < Lights.Count; i++)
    {
        if (Lights[i].CustomName.Contains(name))
        {
            if (oxygenLevel == 0)
                Lights[i].Color = VRageMath.Color.Red;

            else if (oxygenLevel == 1)
                Lights[i].Color = VRageMath.Color.White;

            else if (oxygenLevel <= 1)
                Lights[i].Color = VRageMath.Color.White;
        }
    }
}

void ShowStrips(string name, double oxygenLevel)
{
    List<IMyTextPanel> DoorLCDs = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(DoorLCDs);

    for (int i = 0; i < DoorLCDs.Count; i++)
    {
        DoorLCDs[i].WriteText(" ");

        if (DoorLCDs[i].CustomName.Contains(name) && DoorLCDs[i].CustomName.Contains("Door-LCD"))
        {
            string status = " Empty";
            if (oxygenLevel == 0)
            {
                status = "Depressurized";
                DoorLCDs[i].FontColor = VRageMath.Color.Red;
            }

            else if (oxygenLevel < 1)
            {
                status = "Pressurizing: " + Math.Round((oxygenLevel * 100), 2) + "%";
                DoorLCDs[i].FontColor = VRageMath.Color.Yellow;
            }

            else if (oxygenLevel == 1)
            {
                status = "Pressurized";
                DoorLCDs[i].FontColor = VRageMath.Color.Green;
            }

            DoorLCDs[i].FontSize = 8;
            DoorLCDs[i].WriteText(status);
        }
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