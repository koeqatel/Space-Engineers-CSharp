public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public class Colors
{
    //non-monospace fonts ugh
    public static string Red = "";
    public static string RedDark = "";
    public static string RedDarker = "";
    public static string RedDarkest = "";
    public static string Orange = "";
    public static string Yellow = "";
    public static string YellowGreen = "";
    public static string Green = "";
    public static string GreenDark = "";
    public static string GreenDarker = "";
    public static string GreenDarkest = "";
    public static string Cyan = "";
    public static string DarkCyan = "";
    public static string Black = "";
    public static string LightGray = "";
    public static string Gray = "";
    public static string White = "";
}
public class Graphics
{
    public int width;
    public int height;
    IMyTextPanel console;
    private string[] screen;
    public string bg;
    public string fg;

    public Graphics(int w, int h, IMyTextPanel c)
    {
        width = w;
        height = h;
        console = c;
        fg = Colors.Red;
        bg = Colors.Gray;
        screen = new string[height * width];
        clear();
    }
    public void paint()
    {
        string[] s = new string[height];
        for (int o = 0; o < height; o++)
        {
            string[] r = new string[width];
            for (int x = 0; x < width; x++)
            {
                r[x] = screen[o * width + x];
            }
            string d = string.Concat(r) + "\n";
            s[o] = d;// + d; //add an extra row for textpanels for better squareness
        }
        console.WritePublicText(string.Concat(s));
        console.ShowTextureOnScreen();
        console.ShowPublicTextOnScreen();
    }
    private static void FillArray<T>(T[] arrayToFill, T fillVal)
    {
        T[] fillValue = new T[] { fillVal };
        arrayToFill[0] = fillVal;
        int arrayToFillHalfLength = arrayToFill.Length / 2;
        for (int i = 1; i < arrayToFill.Length; i *= 2)
        {
            int copyLength = i;
            if (i > arrayToFillHalfLength)
            {
                copyLength = arrayToFill.Length - i;
            }
            Array.Copy(arrayToFill, 0, arrayToFill, i, copyLength);
        }
    }
    public void clear()
    {
        FillArray(screen, bg);
    }
    public void pixel(int x, int y)
    {
        if (x > 0 && x < width && y > 0 && y < height)
        {
            screen[width * y + x] = fg;
        }
    }
    public void line(int x0, int y0, int x1, int y1)
    {
        if (x0 == x1)
        {
            for (int y = y0; y <= y1; y++)
            {
                pixel(x0, y);
            }
        }
        else if (y0 == y1)
        {
            for (int x = x0; x <= x1; x++)
            {
                pixel(x, y0);
            }
        }
        else
        { //add a case for 45 degree lines at some point
            double dy = y1 - y0;
            double err = 0;
            double slope = Math.Abs(dy / (x1 - x0));
            int ndy = Math.Sign(dy);
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                pixel(x, y);
                err = err + slope;
                while (err >= 0.5)
                {
                    pixel(x, y);
                    y += ndy;
                    err--;
                }
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
    public void circle(string m, int cx, int cy, int r)
    {
        double rSquared = r * r;
        if (m == "fill")
        { //i should really do this properly at some point
            for (int t = 0; t <= r; t++)
            {
                int c = (int)Math.Round(Math.Sqrt(rSquared - t * t), 0);
                line(cx - c, cy + t, cx + c, cy + t);
                line(cx - c, cy - t, cx + c, cy - t);
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
    public void print(int x, int y, string s)
    { //this doesn't really do anything
        for (int i = 0; i < s.Length; i++)
        {
            if (x > 0 && x < width && y > 0 && y < height)
            {
                screen[y * width + x + i] = "" + s[i];
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
        IMyTextPanel oOxyMap = GridTerminalSystem.GetBlockWithName("Cockpit: OxyMap") as IMyTextPanel;
        oOxyMap.SetValue<Int64>("Content", 1);
        // Blue bg color = 0, 136, 190
        oOxyMap.FontColor = new Color(255, 255, 255);
        oOxyMap.Font = "Monospace";
        oOxyMap.FontSize = (float) 0.15;
        oOxyMap.Alignment = TextAlignment.CENTER;
        oOxyMap.TextPadding = 0;

        this.Log(oOxyMap);

        //LCD fontsize = 0.158, ration = 20:11
        graphics = new Graphics(236, 119, (IMyTextPanel)oOxyMap);
    }
    counter++;
    graphics.bg = Colors.Black;
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
                graphics.fg = Colors.Red;
                CloseDoors(name);
                Lights(name, ListVents[i].GetOxygenLevel());
            }
            else if (ListVents[i].GetOxygenLevel() == 1)
            {
                graphics.fg = Colors.Green;
                Lights(name, ListVents[i].GetOxygenLevel());
            }
            else
            {
                graphics.fg = Colors.Yellow;
                Lights(name, ListVents[i].GetOxygenLevel());
            }

            //ShowStrips(name, ListVents[i].GetOxygenLevel());
        }
    }

    graphics.rect("fill", x, y, w, h);
    graphics.fg = Colors.Gray;
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