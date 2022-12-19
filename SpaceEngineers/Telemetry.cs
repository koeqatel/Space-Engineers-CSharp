        /*  -= Telemetry PLUS =-

        = How to use?
         1) Place a PB in your ship and load this code;
         2) Place a PB in your base and load this code;
         3) Go to the Custom Data in the PB of your base and set flag collector under Telemetry to true;
         4) Add the TAG [Telemetry] to a text panel in your base;
         5) Every ship telemetry with this PB will show up in the panel provided they are in antenna range;
         6) Add the TAG [VTelemetry] to a text panel or console block (projector) in your base;
         7) Add the TAG [TelemetryAlert] to a text panel in your base to see only ships broadcasting 'alert' status
        */

        public static readonly string version = "TelemetryPlus v1.0.0";
        /* What's New?
         * v1.2.0 - last release known to be supported by original author (published on Steam Workshop)
         * vTC.1.0 - first release supported by TechCoder (with all due credit and respect to Magistrator!)
         *          + Cargo percent full (NOTE: This number includes all 'Cargo Containers' other than "Lockers" and "Weapons", Collectors, Drills, Grinders and Connectors)
         *          + Damage percent (NOTE: this returns damage to blocks you have "Show in Terminal" turned on!)
         *          + "<1%" to percent readings (required for damage to not display 0%)
         *          + ability to set the font size on the visual display name (set visual_text_size - 0.35f or 0.45f seems to be nice 'small' font setting {Telemetry original setting was 0.75f})
         *          + ******* SHIP DAMAGE ******* status shown on ships that have damage to blocks and/or core systems
         *          + ******* CORE FAULT DETECTED ******* status shown on ships that have damage to 'core' systems (batteries, thrusters, tanks, etc.)
         *          + ALERT LOG (LCD with the [TelemetryAlert] TAG) shows only ships that have alert status (makes it much easier to spot them) and include the GPS with the Alert (makes it easier to find them, too!)
         *          + on visual display, ships with MAYDAY status show as dark orange (a 'warning' of sorts), ships with CORE issues show as crimson
         *          + Telemetry and Alert LCDs now show data as Scrolling - easier to see more ship data
         */

        // What follows are the Custom Data settings and defaults that can be configured;
        private void RegisterSettings(ref Section s)
        {
            s.RegisterBool("collector", false); // Set to true to enable this PB as a Telemetry collection node;
            s.RegisterString("visual_tag", "VTelemetry"); // Text pannels or projectors with this [tag] will display the visual telemetry;
            s.RegisterString("panel_tag", "Telemetry"); // Text pannels with this [tag] will display the telemetry;
            s.RegisterString("alert_panel_tag", "TelemetryAlert"); // Text pannels with this [tag] will display the telemetry;
            s.RegisterBool("subgrid_panels", false); // Set to true to enable this PB to write to pannels in other subgrids;
            s.RegisterInt("channel", 0); // Send telemetry via specific channels, use tag [TELEMETRY_0] in a text panel to get a specific channel;
            s.RegisterString("security_key", "TELEMETRY"); // Change to avoid other factions from seeing your telemetry;
            s.RegisterFloat("visual_text_size", 0.45f);  // Change to set the font size on the visual screen (size of ship names)
        }

        private void RegisterCommands(ref CmdHandler c)
        {
            c.NewCmd("SCALE", "Saves data to PB Storage", ToggleScale);
            c.NewCmd("SAVE", "Saves data to PB Storage", SaveCmd);
            c.NewCmd("CLEAR", "Clears existing data", ResetCmd);
            c.NewCmd("ORBIT", "Dumps orbit GPS in CustomData", CalculateOrbit);
        }
        static string visual_tag = "VTelemetry"; static string panel_tag = "Telemetry"; static string alert_panel_tag = "TelemetryAlert"; static string security_key = "TELEMETRY"; static
        int channel = 0; static bool subgrid_panels = false; static float visual_text_size = 0.75f; void ToggleScale(ref string arg)
        {
            Interface.ToggleScale(); Interface.Print(
        this, ref recordManager);
        }
        void ResetCmd(ref string arg) { recordManager.records.Clear(); }
        void SaveCmd(ref string arg) { Save(); }
        void CalculateOrbit(ref string arg)
        {
            double altitude; if (!double.TryParse(arg, out altitude))
            {
                Logger.Err(
        "Unable to parse altitude in meters. Example: \"ORBIT 11000\""); return;
            }
            Vector3D orbit; try { orbit = GetOrbit(this, altitude); }
            catch (Exception e)
            {
                Logger.Err("Unable to calculate orbit: " +
        e.Message); return;
            }
            Logger.Info("Orbit set in Custom Data"); settingsSection.RegisterString("orbit", GenerateGPS("orbit",
        orbit)); return;
        }
        void UpdateAdvertise(ref string _) { if (!collector) { new Record().GetData(this).Advertise(this); } }
        void
        UpdateCollector(ref string _)
        {
            string summary = string.Format("{0} {1} {2}\n", Animation.Curve(), version, Animation.Curve()); Animation.Curve
        (); if (collector)
            {
                try
                {
                    recordManager.CollectRecords(this); summary += recordManager.Stats(); recordManager.WriteToPannels(this)
        ; Interface.Print(this, ref recordManager);
                }
                catch (Exception e) { Logger.Err("Collector: " + e.Message); }
            }
            Echo(summary + "\n" +
        Logger.PrintBuffer() + "\n" + arbiter.Stats());
        }
        static bool collector = false; void UpdateSettings(ref string _)
        {
            IMyTextSurface pbdisplay = Me.GetSurface(0);
            pbdisplay.ContentType = ContentType.TEXT_AND_IMAGE;
            pbdisplay.FontSize = 1.5f;
            pbdisplay.TextPadding = 40f;
            pbdisplay.Font = "DEBUG";
            pbdisplay.ClearImagesFromSelection();
            pbdisplay.WriteText(version);
            Me.CustomData =
        settings.UpdateSettings(Me.CustomData); if (subgrid_panels != settingsSection.GetBool("subgrid_panels"))
            {
                subgrid_panels = !
        subgrid_panels; Logger.Info("Writing to subgrid panels " + (subgrid_panels ? "enabled" : "disabled"));
            }
            if (security_key != settingsSection.
        GetString("security_key"))
            {
                IGC.DisableBroadcastListener(IGC.RegisterBroadcastListener(security_key)); security_key = settingsSection
        .GetString("security_key"); IGC.RegisterBroadcastListener(security_key); Logger.Info("Security key changed to: " +
        security_key);
            }
            if (collector != settingsSection.GetBool("collector"))
            {
                collector = !collector; Logger.Info("Collector mode " + (collector ?
        "enabled" : "disabled")); var listener = IGC.RegisterBroadcastListener(security_key); if (!collector)
                {
                    IGC.DisableBroadcastListener(
        listener);
                }
            }
            if (channel != settingsSection.GetInt("channel"))
            {
                channel = settingsSection.GetInt("channel"); Logger.Info(
        "Channel changed to: " + channel.ToString());
            }
            if (panel_tag != settingsSection.GetString("panel_tag"))
            {
                panel_tag = settingsSection.GetString(
        "panel_tag"); Logger.Info("Pannel tag changed to: " + panel_tag);
            }
            if (visual_tag != settingsSection.GetString("visual_tag"))
            {
                visual_tag =
        settingsSection.GetString("visual_tag"); Logger.Info("Visual pannel tag changed to: " + visual_tag);
            }
            if (visual_text_size != settingsSection.GetFloat("visual_text_size"))
            {
                visual_text_size = settingsSection.GetFloat("visual_text_size"); Logger.Info("Visual text size changed to: " + visual_text_size);
            }
            if (alert_panel_tag != settingsSection.GetString("alert_panel_tag"))
            {
                alert_panel_tag = settingsSection.GetString("alert_panel_tag"); Logger.Info("Alert panel tag changed to: " + alert_panel_tag);
            }
        }
        void Main(string argument, UpdateType
        updateSource)
        {
            try { arbiter.HandleUpdate(updateSource, ref argument); }
            catch (Exception exception)
            {
                Echo("Main exception: " + exception.
        Message);
            }
        }
        Settings settings; Section settingsSection; Arbiter arbiter; RecordManager recordManager; CmdHandler cmdHandler; string
        empty_string = ""; Program()
        {
            settings = new Settings(); settingsSection = settings.NewSection("Telemetry"); RegisterSettings(ref
        settingsSection); UpdateSettings(ref empty_string); recordManager = new RecordManager(); cmdHandler = new CmdHandler(); RegisterCommands(ref
        cmdHandler); Load(); arbiter = new Arbiter(this); arbiter.RegisterMiliSecond(750, UpdateCollector); arbiter.RegisterSecond(3,
        UpdateSettings); arbiter.RegisterSecond(5, UpdateAdvertise); arbiter.RegisterDefault(cmdHandler.HandleCmd); Logger.Info("Started...");
        }
        void Save() { try { this.Storage = recordManager.ToSerial(); } catch (Exception e) { Logger.Err("Saving error: " + e.Message); } }
        void Load
        ()
        {
            if (this.Storage == "") { return; }
            try { recordManager.FromSerial(this.Storage); }
            catch (Exception e)
            {
                this.Storage = ""; Logger.Err
        ("Loading error: " + e.Message);
            }
        }

        static class Animation
        {
            private static string[] ROTATOR = new string[] { "|", "/", "-", "\\" };
            private static int rotatorCount = 0; private static string[] SCORE = new string[] { "_", "-", "¯", "-" }; private static int scoreCount = 0;
            private static string[] CURVE = new string[] { ">", "<" }; private static int curveCount = 0; public static string Rotator()
            {
                if (++
            rotatorCount > ROTATOR.Length - 1) { rotatorCount = 0; }
                return ROTATOR[rotatorCount];
            }
            public static string Score()
            {
                if (++scoreCount > SCORE.
            Length - 1) { scoreCount = 0; }
                return SCORE[scoreCount];
            }
            public static string Curve()
            {
                if (++curveCount > CURVE.Length - 1) { curveCount = 0; }
                return CURVE[curveCount];
            }
        }
        class Arbiter
        {
            private float MAX_INSTRUCTION_QUOTA = 0.85f; private int instructionOverruns = 0; public
                delegate void Updater(ref string msg); public LoadStats loadStats = new LoadStats(); private Program program; private struct
                UpdaterInfo
            { public string Name; public Updater updater; }
            private Dictionary<UpdateType, UpdaterInfo> updateRegister = new Dictionary<
                UpdateType, UpdaterInfo>(); private Dictionary<Rater, Updater> updateRater = new Dictionary<Rater, Updater>(); public Arbiter(Program p)
            {
                program = p;
            }
            public void RegisterDefault(Updater updater)
            {
                var defaultUpdate = UpdateType.Terminal | UpdateType.Mod | UpdateType.Script |
                UpdateType.Trigger; updateRegister[defaultUpdate] = new UpdaterInfo() { Name = "Default", updater = updater };
            }
            public void Register(
                UpdateType updateType, Updater updater)
            {
                updateRegister[updateType] = new UpdaterInfo() { Name = updateType.ToString(), updater = updater };
                switch (updateType)
                {
                    case UpdateType.Update1: program.Runtime.UpdateFrequency |= UpdateFrequency.Update1; break;
                    case UpdateType.
                Update10:
                        program.Runtime.UpdateFrequency |= UpdateFrequency.Update10; break;
                    case UpdateType.Update100:
                        program.Runtime.
                UpdateFrequency |= UpdateFrequency.Update100; break;
                }
            }
            public void RegisterMiliSecond(int updateMiliSeconds, Updater updater)
            {
                updateRater[
                new Rater(TimeSpan.FromMilliseconds(updateMiliSeconds))] = updater; program.Runtime.UpdateFrequency |= UpdateFrequency.Update1;
            }
            public void RegisterSecond(int updateSeconds, Updater updater)
            {
                updateRater[new Rater(TimeSpan.FromSeconds(updateSeconds))] =
            updater; program.Runtime.UpdateFrequency |= UpdateFrequency.Update1;
            }
            private float InstructionQuotaUsage(float starting = 0)
            {
                return (
            float)(program.Runtime.CurrentInstructionCount - starting) / (float)program.Runtime.MaxInstructionCount;
            }
            public bool
            InstructionOveruse()
            { var check = InstructionQuotaUsage() >= MAX_INSTRUCTION_QUOTA; if (check) { ++instructionOverruns; } return check; }
            public void
            InnerHandleUpdate(UpdateType updateType, ref string msg)
            {
                foreach (var pair in updateRegister)
                {
                    if ((pair.Key & updateType) != 0)
                    {
                        Run(pair.Value.
            updater, ref msg, pair.Value.Name);
                    }
                    if (InstructionOveruse()) { return; }
                }
                if (updateRater.Count != 0)
                {
                    foreach (var rater in updateRater)
                    {
                        if (rater.Key.Elapsed()) { Run(rater.Value, ref msg, rater.Key.Name); }
                        if (InstructionOveruse()) { return; }
                    }
                }
            }
            public void
                        HandleUpdate(UpdateType updateType, ref string msg)
            {
                InnerHandleUpdate(updateType, ref msg); loadStats.Measure(InstructionQuotaUsage(),
                        "Total");
            }
            private void Run(Updater updater, ref string msg, string name)
            {
                var cic = program.Runtime.CurrentInstructionCount; try
                {
                    updater(ref msg);
                }
                catch (Exception e) { Logger.Err(string.Format("Run({0}): {1}", updater.Method.Name, e.Message)); }
                loadStats.
                    Measure(InstructionQuotaUsage(cic), name);
            }
            public string Stats() { return loadStats + "Overruns: " + instructionOverruns; }
        }
        delegate
                    void CmdAction(ref string arg); class Cmd
        {
            public string help; public CmdAction cmdAction; public Cmd(string h, CmdAction a)
            {
                help
                    = h; cmdAction = a;
            }
        }
        class CmdHandler
        {
            public static char[] CMD_SEPARATOR = new char[] { ' ' }; public Dictionary<string, Cmd> cmdDict;
            public CmdHandler() { cmdDict = new Dictionary<string, Cmd>(); }
            public void NewCmd(string cmd, string help, CmdAction action)
            {
                cmdDict[
            cmd] = new Cmd(help, action);
            }
            public void HandleCmd(ref string arg)
            {
                arg = arg.Trim(); if (arg == "") { return; }
                string[] parts = arg.Trim(
            ).Split(CMD_SEPARATOR, 2); string arg0 = parts[0].ToUpper(); string arg1 = parts.ElementAtOrDefault(1); arg1 = String.IsNullOrEmpty
            (arg1) ? "" : arg1; if (!cmdDict.ContainsKey(arg0)) { return; }
                cmdDict[arg0].cmdAction(ref arg1); Logger.Info("Executed: " + arg0);
            }
        }
        class PbDefenseWrapper
        {
            private IMyTerminalBlock _block; public bool noDefenseData; private Func<IMyTerminalBlock, float>
        _getShieldPercent; private Func<IMyEntity, IMyTerminalBlock> _getShieldBlock; private Func<IMyTerminalBlock, bool> _isShieldBlock; public void
        SetActiveShield(IMyTerminalBlock block) => _block = block; public PbDefenseWrapper(IMyTerminalBlock block)
            {
                _block = block; var delegates = _block
        .GetProperty("DefenseSystemsPbAPI")?.As<Dictionary<string, Delegate>>().GetValue(_block); if (delegates == null)
                {
                    noDefenseData
        = true; return;
                }
                _getShieldPercent = (Func<IMyTerminalBlock, float>)delegates["GetShieldPercent"]; _getShieldBlock = (Func<
        IMyEntity, IMyTerminalBlock>)delegates["GetShieldBlock"]; _isShieldBlock = (Func<IMyTerminalBlock, bool>)delegates["IsShieldBlock"]; if
        (!IsShieldBlock()) _block = GetShieldBlock(_block.CubeGrid) ?? _block;
            }
            public float GetShieldPercent() => _getShieldPercent?.
        Invoke(_block) ?? -1; public IMyTerminalBlock GetShieldBlock(IMyEntity entity) => _getShieldBlock?.Invoke(entity) ?? null; public bool
        IsShieldBlock() => _isShieldBlock?.Invoke(_block) ?? false;
        }
        static class Interface
        {
            private static Color bgColor = Color.Black; private
        static Color fgColor = Color.Cyan.Alpha(0.1f); private static Color fgActiveColor = Color.Red.Alpha(0.1f); private static float
        dot_size = 0.25f; private static Color dotColor = Color.DarkGreen; private static Color textColor = Color.DarkGreen; private static float
        text_size = visual_text_size; private static class Sprite
            {
                private static MySprite sprite_dot = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = "•",
                    Alignment = TextAlignment.CENTER,
                    FontId = "White"
                }; private static Vector2 sprite_dot_offset = new Vector2(0.0f, 31.0f / 2.0f); private
                    static Vector2 sprite_dot_size = new Vector2(41.0f, 31.0f); public static MySprite Dot(Vector2 pos, Color color, float scale = 1.0f)
                {
                    sprite_dot.Color = color; sprite_dot.RotationOrScale = scale; sprite_dot.Size = scale * sprite_dot_size; sprite_dot.Position = pos - scale *
                    sprite_dot_offset; return sprite_dot;
                }
                private static MySprite sprite_cross = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = "X",
                    Alignment =
                    TextAlignment.CENTER,
                    FontId = "White"
                }; private static Vector2 sprite_cross_offset = new Vector2(0.0f, 35.0f / 2.0f); private static Vector2
                    sprite_cross_size = new Vector2(45.0f, 35.0f); public static MySprite Cross(Vector2 pos, Color color, float scale = 1.0f)
                {
                    sprite_cross.Color =
                    color; sprite_cross.RotationOrScale = scale; sprite_cross.Size = scale * sprite_cross_size; sprite_cross.Position = pos - scale *
                    sprite_cross_offset; return sprite_cross;
                }
                private static MySprite sprite_text = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.
                    LEFT,
                    FontId = "White"
                }; public static MySprite Text(string text, Vector2 pos, Color color, Vector2 size, float scale = 1.0f)
                {
                    sprite_text.Data = text; sprite_text.Color = color; sprite_text.RotationOrScale = scale; sprite_text.Size = size; sprite_text.Position = pos;
                    return sprite_text;
                }
                private static MySprite sprite_circle_hollow = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "CircleHollow",
                    Alignment = TextAlignment.CENTER,
                }; public static MySprite CircleHollow(Vector2 pos, float size, Color color)
                {
                    sprite_circle_hollow.
                    Color = color; sprite_circle_hollow.Size = new Vector2(size); sprite_circle_hollow.Position = pos; return sprite_circle_hollow;
                }
                private static MySprite sprite_circle = new MySprite() { Type = SpriteType.TEXTURE, Data = "Circle", Alignment = TextAlignment.CENTER, };
                public static MySprite Circle(Vector2 pos, float size, Color color)
                {
                    sprite_circle.Color = color; sprite_circle.Size = new Vector2(
                size); sprite_circle.Position = pos; return sprite_circle;
                }
            }
            public static float scale = 1.0f; private static List<float>
                scale_levels = new List<float>() { 0.1f, 1.0f, 10.0f, 100.0f, 1000.0f }; public static void ToggleScale()
            {
                var index = scale_levels.IndexOf(scale
                ); if (--index < 0) { scale = scale_levels.Last(); } else { scale = scale_levels[index]; }
            }
            public static void Print(Program p, ref
                RecordManager rm)
            {
                var tagRegex = TagMatchRegex(Program.visual_tag); List<IMyTextPanel> panels = new List<IMyTextPanel>(); p.
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels); foreach (IMyTextPanel panel in panels)
                {
                    if (!tagRegex.IsMatch(panel.CustomName))
                    {
                        continue;
                    }
                    Print(panel, ref rm, panel.GetPosition(), panel.WorldMatrix.Down, panel.WorldMatrix.Right);
                }
                List<IMyProjector> projectors =
                        new List<IMyProjector>(); p.GridTerminalSystem.GetBlocksOfType<IMyProjector>(projectors); foreach (IMyProjector projector in
                        projectors)
                {
                    if (!tagRegex.IsMatch(projector.CustomName)) { continue; }
                    Print(projector.GetSurface(0), ref rm, projector.GetPosition(),
                        projector.WorldMatrix.Left, projector.WorldMatrix.Backward);
                }
            }
            private static void Print(IMyTextSurface surface, ref RecordManager
                        rm, Vector3D thisPos, Vector3D thisUp, Vector3D thisRight)
            {
                surface.ContentType = ContentType.SCRIPT; surface.Script = ""; surface.
                        ScriptBackgroundColor = Color.Black; surface.ScriptForegroundColor = Color.Black; RectangleF viewport = new RectangleF((surface.TextureSize - surface.
                        SurfaceSize) / 2.0f, surface.SurfaceSize); MySpriteDrawFrame frame = surface.DrawFrame(); var maxSize = (float)Math.Sqrt(Math.Pow(viewport.
                        Size.X, 2.0f) + Math.Pow(viewport.Size.Y, 2.0f)); var legend = false; var legend_location = 0.0f; var circle_diamater_scale_step = 200.0f
                        ; for (float circle_size_diameter = (float)Math.Ceiling(maxSize / circle_diamater_scale_step) * circle_diamater_scale_step;
                        circle_size_diameter > 0; circle_size_diameter -= circle_diamater_scale_step)
                {
                    var setColor = fgColor; if (!legend)
                    {
                        if (circle_size_diameter < viewport.
                        Width) { legend_location = circle_size_diameter; legend = true; setColor = fgActiveColor; }
                    }
                    frame.Add(Sprite.CircleHollow(viewport.
                        Center, circle_size_diameter, setColor)); frame.Add(Sprite.Circle(viewport.Center, circle_size_diameter - 1, bgColor));
                }
                var show =
                        string.Format("\n {0}", ToMetric(scale * legend_location / 2.0f)); frame.Add(Sprite.Text(show, new Vector2(viewport.Center.X -
                        legend_location / 2.0f, viewport.Center.Y - 45.0f * 0.75f), fgActiveColor, viewport.Size, 0.75f)); frame.Add(Sprite.Dot(viewport.Center,
                        fgActiveColor, 0.25f)); foreach (var record in rm.records.Values)
                {
                    var relativePos = record.position - thisPos; var v2pos = new Vector2((float)
                        Vector3D.Dot(relativePos, thisRight), (float)Vector3D.Dot(relativePos, thisUp)); var text = string.Format("{0}\n{1}", record.name,
                        ToMetric((float)relativePos.Length())); var drawingPos = (v2pos / scale) + viewport.Center;
                    dotColor = record.ship_lost ? Color.Yellow : (record.ship_core_fault ? Color.Crimson : (record.ship_mayday ? Color.DarkOrange : Color.DarkGreen));
                    textColor = record.ship_lost ? Color.Yellow : (record.ship_core_fault ? Color.Crimson : (record.ship_mayday ? Color.DarkOrange : Color.DarkGreen));
                    frame.Add(Sprite.Cross(drawingPos, dotColor, dot_size)); frame.Add(Sprite.Text(text, drawingPos, textColor.Alpha(0.1f), viewport.Size, visual_text_size));
                }
                frame.Dispose();
            }
        }
        class
                        LoadStats
        {
            private class InstructionLoad
            {
                public float lowest; public float average; public float highest; public int calls; public
                        InstructionLoad(float currentLoad)
                { highest = average = lowest = currentLoad; calls = 1; }
                public void Measure(float currentLoad)
                {
                    lowest = Math.Min(
                        lowest, currentLoad); highest = Math.Max(highest, currentLoad); average = 0.9f * average + 0.1f * currentLoad; ++calls;
                }
            }
            private Dictionary<
                        string, InstructionLoad> instructionStats = new Dictionary<string, InstructionLoad>(); public void Measure(float currentLoad, string
                        updateType)
            {
                if (!instructionStats.ContainsKey(updateType)) { instructionStats[updateType] = new InstructionLoad(currentLoad); return; }
                instructionStats[updateType].Measure(currentLoad);
            }
            public override string ToString()
            {
                var str = "%Min - %Avg - %Max :Source\n"; foreach (var
                load in instructionStats)
                {
                    str += String.Format("{1:F0} - {2:F0} - {3:F0} :{0}\n", load.Key, load.Value.lowest * 100.0f, load.Value.
                average * 100.0f, load.Value.highest * 100.0f);
                }
                return str;
            }
        }
        static partial class Logger
        {
            public enum LogType { I, W, E, D }
            private class
                LogEntry
            { public string entry; public int count; public LogType logType; }
            private static int MAX_LOG_ENTRIES = 8; private static List<
                LogEntry> logger = new List<LogEntry>(); public static void Log(string line, LogType logType)
            {
                if (logger.Count >= 1 && line == logger[0].
                entry && logType == logger[0].logType) { ++logger[0].count; return; }
                logger.Insert(0, new LogEntry { entry = line, count = 1, logType = logType }
                ); if (logger.Count() > MAX_LOG_ENTRIES) { logger.RemoveAt(0); }
            }
            public static void Clear() { logger.Clear(); }
            public static void D(string line) { Log(line, LogType.D); }
            public static void Info(string line) { Log(line, LogType.I); }
            public static
            void Warn(string line)
            { Log(line, LogType.W); }
            public static void Err(string line) { Log(line, LogType.E); }
            public static string
            PrintBuffer()
            {
                var str = ""; foreach (var logEntry in logger)
                {
                    str += logEntry.logType.ToString() + ": " + (logEntry.count != 1 ? "(" + logEntry.
            count.ToString() + ") " : "") + logEntry.entry + "\n";
                }
                return str;
            }
        }
        class Rater
        {
            private TimeSpan interval; private DateTime
            last_update = DateTime.MinValue; public string Name; public Rater(TimeSpan interval)
            {
                this.interval = interval; this.Name = "Update" + ((
            interval.TotalSeconds >= 1.0) ? (interval.TotalSeconds.ToString() + "s") : (interval.TotalMilliseconds.ToString() + "ms"));
            }
            public bool
            Elapsed()
            { if (DateTime.UtcNow - this.last_update < interval) { return false; } last_update = DateTime.UtcNow; return true; }
        }
        public static int ship_blocks = 0; public static int tank_blocks = 0; public static int battery_blocks = 0; public static int thruster_blocks = 0;
        class Record :
            Serialize
        {
            public long grid_id = 0; public string name = ""; public bool ship_mayday = false; public bool ship_core_fault = false; public bool ship_lost = false;
            public float ship_damage = 0.0f;
            public int check_ship_blocks = 0; public int check_tank_blocks = 0; public int check_battery_blocks = 0; public int check_thruster_blocks = 0;
            public float battery_charge = 0.0f; public float cargo = 0.0f; public float hydrogen_level = 0.0f; public
            float speed = 0.0f; public int altitude = 0; public Vector3D position = new Vector3D(); public string docked_grid = ""; public DateTime
            updated = DateTime.UtcNow; public float shield_percent = -1.0f; public int channel = 0; public float oxygen_level = 0.0f; public Record
            GetData(Program p)
            {
                GetGridData(p); GetDefenseData(p); GetShipLost(p); GetShipDamage(p); GetBatteryCharge(p); GetCargoLevels(p); GetTankLevels(p); GetRCData(p); GetConnectedGrid(p);
                return this;
            }
            public void Pack()
            {
                Pack(grid_id); Pack(name); Pack(ship_mayday); Pack(ship_core_fault); Pack(ship_damage); Pack(battery_charge); Pack(cargo); Pack(hydrogen_level); Pack(speed); Pack(altitude);
                Pack(position); Pack(docked_grid); Pack(updated); Pack(shield_percent); Pack(channel); Pack(oxygen_level); Pack(ship_lost);
            }
            public string ToSerial
                ()
            { InitPack(); Pack(); return FinishPack(); }
            public void Unpack()
            {
                Unpack(ref grid_id); Unpack(ref name); Unpack(ref ship_mayday); Unpack(ref ship_core_fault); Unpack(ref ship_damage); Unpack(ref battery_charge); Unpack(ref cargo); Unpack(ref hydrogen_level); Unpack(ref speed); Unpack(ref altitude); Unpack(ref position); Unpack(ref docked_grid); Unpack(
                ref updated); try { Unpack(ref shield_percent); Unpack(ref channel); Unpack(ref oxygen_level); Unpack(ref ship_lost); }
                catch
                {
                    Logger.Warn("Unable to decode some data. Update ship scripts to the latest version.");
                }
            }
            public Record FromSerial(string s)
            {
                InitUnpack(s); Unpack(); if (!FinishUnpack())
                {
                    Logger.Warn("Some data not decoded. Update script to the latest version.");
                }
                return this;
            }
            public void Advertise(Program p)
            {
                p.IGC.SendBroadcastMessage<string>(Program.security_key, this.ToSerial()
                );
            }
            private void GetGridData(Program p)
            {
                this.grid_id = p.Me.CubeGrid.EntityId; this.name = p.Me.CubeGrid.CustomName; this.
                channel = Program.channel;
            }
            private void GetDefenseData(Program p)
            {
                var shield = new PbDefenseWrapper(p.Me); if (shield.noDefenseData)
                {
                    return;
                }
                shield_percent = shield.GetShieldPercent();
            }
            private void DefineCore(Program p)
            {
                // get original count (when PB is initialized, which means after updates it must be recompiled)
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                p.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks);
                ship_blocks = (ship_blocks == 0) ? blocks.Count : ship_blocks;
                List<IMyGasTank> tanks = new List<IMyGasTank>();
                p.GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks);
                tank_blocks = (tank_blocks == 0) ? tanks.Count : tank_blocks;
                List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
                p.GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries);
                battery_blocks = (battery_blocks == 0) ? batteries.Count : battery_blocks;
                List<IMyThrust> thrusters = new List<IMyThrust>();
                p.GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters);
                thruster_blocks = (thruster_blocks == 0) ? thrusters.Count : thruster_blocks;
            }
            private void GetShipLost(Program p)
            {
                TimeSpan timeSpan = DateTime.UtcNow - updated;
                ship_lost = timeSpan.Minutes > 5;
            }
            private void GetShipDamage(Program p)
            {
                //  Logger.Clear();
                DefineCore(p);
                float integrity = -1.0f;
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                p.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks);
                if (blocks.Count != ship_blocks)
                {
                    // this is not enough damage to cause an alert by itself - it is handled in ship_damage below - this is just a quick check (could be deleted?)
                    // Logger.Info("damage to blocks COUNT NOT EQUAL");
                }
                // check damage to core 
                List<IMyGasTank> check_tanks = new List<IMyGasTank>();
                p.GridTerminalSystem.GetBlocksOfType<IMyGasTank>(check_tanks);
                ship_mayday = check_tanks.Count != tank_blocks;

                ship_core_fault = ship_mayday;
                if (!ship_mayday)
                {
                    List<IMyBatteryBlock> check_batteries = new List<IMyBatteryBlock>();
                    p.GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(check_batteries);
                    ship_mayday = check_batteries.Count != battery_blocks;
                    ship_core_fault = ship_mayday;
                }
                if (!ship_mayday)
                {
                    List<IMyThrust> check_thrusters = new List<IMyThrust>();
                    check_thrusters.Clear();
                    p.GridTerminalSystem.GetBlocksOfType<IMyThrust>(check_thrusters);
                    ship_mayday = check_thrusters.Count != thruster_blocks;
                    ship_core_fault = ship_mayday;
                }
                if (!ship_core_fault)
                {
                    foreach (var block in blocks)
                    {
                        IMySlimBlock slimblock = block.CubeGrid.GetCubeBlock(block.Position);
                        float MaxIntegrity = slimblock.MaxIntegrity;
                        float BuildIntegrity = slimblock.BuildIntegrity;
                        float CurrentDamage = slimblock.CurrentDamage;
                        integrity += ((BuildIntegrity != MaxIntegrity) || CurrentDamage != 0) ? (BuildIntegrity - CurrentDamage) / MaxIntegrity : 1.0f;
                    }
                    ship_damage = (((ship_blocks - 1) / integrity) - 1) != 0 ? ((ship_blocks - 1) / integrity) - 1 : -1.0f;
                    ship_mayday = ship_mayday ? ship_mayday : (ship_damage != -1.0f);
                }
            }
            private void GetBatteryCharge(Program p)
            {
                float charge = -1.0f; List<
                    IMyBatteryBlock> batteries = new List<IMyBatteryBlock>(); p.GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries); foreach (
                    IMyBatteryBlock battery in batteries)
                {
                    if (!battery.IsSameConstructAs(p.Me)) { continue; }
                    charge = (charge == -1.0f) ? (battery.CurrentStoredPower
                    / battery.MaxStoredPower) : ((charge + (battery.CurrentStoredPower / battery.MaxStoredPower)) / 2.0f);
                }
                battery_charge = charge;
            }
            private void GetCargoLevels(Program p)
            {
                float c_level = -1.0f;
                List<IMyTerminalBlock> storage = new List<IMyTerminalBlock>();
                List<IMyTerminalBlock> temp = new List<IMyTerminalBlock>();
                List<IMyCargoContainer> containers = new List<IMyCargoContainer>();
                p.GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers); foreach (IMyCargoContainer container in containers)
                {
                    if (!container.IsSameConstructAs(p.Me) || container.BlockDefinition.SubtypeId.Contains("Locker") || container.BlockDefinition.SubtypeId.Contains("Weapon")) { continue; }
                    p.GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(temp, blk => blk.CubeGrid.IsSameConstructAs(p.Me.CubeGrid));
                    storage.AddRange(temp);
                }
                p.GridTerminalSystem.GetBlocksOfType<IMyCollector>(temp, blk => blk.CubeGrid.IsSameConstructAs(p.Me.CubeGrid));
                storage.AddRange(temp);
                p.GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(temp, blk => blk.CubeGrid.IsSameConstructAs(p.Me.CubeGrid));
                storage.AddRange(temp);
                p.GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(temp, blk => blk.CubeGrid.IsSameConstructAs(p.Me.CubeGrid));
                storage.AddRange(temp);
                p.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(temp, blk => blk.CubeGrid.IsSameConstructAs(p.Me.CubeGrid));
                storage.AddRange(temp);
                float current = 0;
                float max = 0;
                for (int i = 0; i < storage.Count; i++)
                {
                    var inv = (storage[i]).GetInventory(0);
                    if (inv == null)
                    {
                        Logger.Warn("No Storage found");
                        return;
                    }
                    current += (float)inv.CurrentVolume;
                    max += (float)inv.MaxVolume;
                }
                cargo = (current == 0) ? c_level : current / max;
            }
            private void GetTankLevels(Program p)
            {
                float h_level = -1.0f; float o_level = -1.0f; List<IMyGasTank> tanks = new List<IMyGasTank>(); p.
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks); foreach (IMyGasTank tank in tanks)
                {
                    if (!tank.IsSameConstructAs(p.Me)) { continue; }
                    if (
            tank.BlockDefinition.SubtypeId.Contains("Hydro"))
                    {
                        h_level = (h_level == -1.0f) ? (float)tank.FilledRatio : (h_level + (float)tank.
            FilledRatio) / 2.0f;
                    }
                    else { o_level = (o_level == -1.0f) ? (float)tank.FilledRatio : (o_level + (float)tank.FilledRatio) / 2.0f; }
                }
                this.
            hydrogen_level = h_level; this.oxygen_level = o_level;
            }
            private void GetRCData(Program p)
            {
                List<IMyShipController> shipControllers = new List<
            IMyShipController>(); p.GridTerminalSystem.GetBlocksOfType<IMyShipController>(shipControllers);
                foreach (IMyShipController shipController in shipControllers)
                {
                    if (!shipController.IsSameConstructAs(p.Me)) { continue; }
                    double elevation;
                    if (shipController.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevation)) { altitude = (int)elevation; }
                    speed = (float)shipController.GetShipVelocities().LinearVelocity.Length();
                    position = shipController.GetPosition(); return;
                }
            }
            private void GetConnectedGrid(Program p)
            {
                List<IMyShipConnector>
            connectors = new List<IMyShipConnector>(); p.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors);
                foreach (IMyShipConnector connector in connectors)
                {
                    if (!connector.IsSameConstructAs(p.Me)) { continue; }
                    if (connector.Status != MyShipConnectorStatus.Connected) { continue; }
                    docked_grid = connector.OtherConnector.CubeGrid.CustomName; return;
                }
                docked_grid = "";
            }
            private string space = "   ";
            private List<string> list_data = new List<string>();
            private void AddParam(string s, float f)
            {
                if (f != -1.0)
                {
                    if (s == "D:{0:P0}")
                    {
                        if (f < 0.01 & f != 0 & s.Contains("D:{0:P0}"))
                        {
                            list_data.Add(string.Format(s, "<1 %"));
                        }
                    }
                    else
                    {
                        list_data.Add(string.Format(s, f));
                    }
                }
            }
            public override string ToString()
            {
                list_data.Clear();
                TimeSpan timeSpan = DateTime.UtcNow - updated;
                ship_lost = timeSpan.Minutes > 5;
                if (ship_lost)
                {
                    list_data.Add("*******  SHIP NOT REPORTING IN  *******\n");
                }
                if (ship_mayday)
                {
                    list_data.Add("*******  SHIP DAMAGE  *******\n");
                }
                if (ship_core_fault) { list_data.Add("*******  CORE FAULT DETECTED *******\n"); }
                AddParam("B:{0:P0}", battery_charge); AddParam("C:{0:P0}", cargo); AddParam("H:{0:P0}", hydrogen_level); AddParam("O:{0:P0}", oxygen_level); AddParam("S:{0:F0}", shield_percent); AddParam("D:{0:P0}", ship_damage);
                var s = string.Format("{0}\n {1}\n", name, string.Join(space, list_data)); if (docked_grid != "") { s += string.Format(" dock: {0}\n", docked_grid); }
                else
                {
                    list_data.Clear(); AddParam("v:{0:F1} m/s", speed); AddParam("a:{0:F1} m", altitude); s += string.Format(" {0}\n", string.Join(space, list_data));
                }
                s += string.Format(" updated: {0} ago\n", TimeSpanToString((DateTime.UtcNow - updated).Duration())); return s;
            }
        }
        class RecordManager : Serialize
        {
            public Dictionary<long
            , Record> records = new Dictionary<long, Record>(); public string Stats()
            {
                if (records.Count == 0) { return "No records.\n"; }
                if (records.Count == 1) { return records.Values.First().ToString(); }
                HashSet<int> channels = new HashSet<int>(); DateTime oldest = DateTime.MaxValue; DateTime newest = DateTime.MinValue; foreach (var record in records.Values)
                {
                    channels.Add(record.channel); oldest = (oldest < record.updated) ? oldest : record.updated; newest = (newest > record.updated) ? newest : record.updated;
                }
                return string.Format("Stats --\n" + " entries: {0}\n" + " channels: {1}\n" + " newest: {2} ago\n" + " oldest: {3} ago\n", records.Count(), string.Join(", ", channels),
            TimeSpanToString((DateTime.UtcNow - newest).Duration()), TimeSpanToString((DateTime.UtcNow - oldest).Duration()));
            }
            public void CollectRecords
            (Program p)
            {
                IMyBroadcastListener listener = p.IGC.RegisterBroadcastListener(Program.security_key); while (listener.HasPendingMessage)
                {
                    var igcData = listener.AcceptMessage(); try { Record r = new Record().FromSerial((string)igcData.Data); records[r.grid_id] = r; }
                    catch (Exception exception) { Logger.Err("Receive record: " + exception.Message); }
                }
            }
            public string GetChannelRecords(Program p, int channel)
            {
                string sum = ""; string gps = ""; foreach (var record in records.Values)
                {
                    if (channel == -1 || record.channel == channel)
                    {
                        sum += record.ToString(); gps += GenerateGPS(record.name, record.position) + "\n";
                        EvaluateRecord(p, record);
                    }
                }
                return ScrollLoop(sum, 1, "***** TELEMETRY ****\n", "\n" + gps);
            }

            public void EvaluateRecord(Program p, Record record)
            {
                string sum = ""; string gps = "";
                if (record.ship_mayday || record.ship_core_fault || record.ship_lost)
                {
                    sum = record.ToString(); gps += GenerateGPS(record.name, record.position, "FF00FF", false) + "\n";
                    alert_buffer[record.name] = sum + "\n" + gps;
                }
            }
            private Dictionary<int, string> channel_buffer = new Dictionary<int, string>(); private TimeSpan interval = new TimeSpan(0, 0, 2);
            private DateTime last_update = DateTime.MinValue; private DateTime alert_last_update = DateTime.MinValue;
            public Dictionary<string, string> alert_buffer = new Dictionary<string, string>(); public void WriteToPannels(Program p)
            {
                channel_buffer.Clear(); alert_buffer.Clear(); var tagRegex = TagNumberMatchRegex(Program.panel_tag); var alerttagRegex = TagNumberMatchRegex(Program.alert_panel_tag);
                List<IMyTextPanel> panels = new List<IMyTextPanel>(); p.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels);
                foreach (IMyTextPanel panel in panels)
                {
                    if (!Program.subgrid_panels && !panel.IsSameConstructAs(p.Me)) { continue; }
                    System.Text.RegularExpressions.Match match = tagRegex.Match(panel.CustomName);
                    if (match.Success)
                    {
                        intLinesPerScreen = (int)(17.6 / panel.FontSize);
                        panel.ContentType = ContentType.TEXT_AND_IMAGE; panel.ClearImagesFromSelection(); var ch = -1;
                        if (match.Groups[3].Value != "")
                        {
                            ch = int.Parse(match.Groups[3].Value);
                        }
                        if (!channel_buffer.ContainsKey(ch)) { channel_buffer[ch] = GetChannelRecords(p, ch); }
                        if (DateTime.UtcNow - this.last_update >= interval)
                        {
                            last_update = DateTime.UtcNow;
                            panel.WriteText(channel_buffer[ch]);
                        }
                    }
                    match = alerttagRegex.Match(panel.CustomName);
                    if (match.Success)
                    {
                        intLinesPerScreen = (int)(17.6 / panel.FontSize);
                        panel.ContentType = ContentType.TEXT_AND_IMAGE;
                        panel.ClearImagesFromSelection();
                        var alerts = new StringBuilder();
                        foreach (KeyValuePair<string, string> alert in alert_buffer)
                        {
                            alerts.Append(alert.Value + "\n-----------------------------\n");
                        }
                        if (DateTime.UtcNow - this.alert_last_update >= interval)
                        {
                            alert_last_update = DateTime.UtcNow;
                            panel.WriteText(ScrollLoop(alerts.ToString(), 2, "******* ALERTS *******\n"));
                        }
                    }
                }
            }
            public static int intLinesPerScreen = 1;

            public static int intScrollLoopOffset = 0;

            public string ScrollLoop(string strData, int intScrollLines = 1, string strHeader = null, string strFooter = null)
            {
                int intDisplayLines = intLinesPerScreen;
                string strReturn = "";
                string[] arrData = strData.Split('\n');
                int intDataLength = arrData.Length;
                int x = 0;
                if (strHeader != null)
                {
                    strReturn += strHeader + "\n";
                    intDisplayLines -= 1;
                }
                if (strFooter != null)
                {
                    string[] footerData = strFooter.Split('\n'); intDisplayLines = intDisplayLines - footerData.Length;
                }
                if (intDisplayLines < intDataLength)
                {
                    for (int i = 0; i < intDisplayLines; i++)
                    {
                        if (i + intScrollLoopOffset < intDataLength)
                        {
                            strReturn += arrData[i + intScrollLoopOffset] + "\n";
                            x = 0;
                        }
                        else
                        {
                            strReturn += arrData[x] + "\n";
                            x++;
                        }
                    }
                }
                else
                {
                    strReturn = strData;
                }
                if (strFooter != null)
                    strReturn += strFooter + "\n";
                intScrollLoopOffset += intScrollLines;
                if ((intDataLength) - 1 < intScrollLoopOffset)
                    intScrollLoopOffset = 0;
                else if (intScrollLoopOffset < 0)
                    intScrollLoopOffset = intDataLength - 1;
                return strReturn;
            }
            public string ToSerial()
            {
                InitPack(); Pack(records.Count); foreach (KeyValuePair<long, Record> p in records)
                {
                    Pack(p.Key); p.Value.
            serialized = this.serialized; p.Value.Pack();
                }
                return FinishPack();
            }
            public void FromSerial(string s)
            {
                InitUnpack(s); int count = 0; Unpack(
            ref count); for (var i = 0; i < count; i++)
                {
                    long key = 0; Unpack(ref key); Record record = new Record(); record.serialized = this.serialized
            ; record.Unpack(); records[key] = record;
                }
            }
        }
        class Serialize
        {
            private string[] separator = new string[] { "\n" }; public Queue<string>
            serialized = new Queue<string>(); public Serialize InitPack() { serialized.Clear(); return this; }
            public string FinishPack()
            {
                return
            string.Join(separator[0], serialized);
            }
            public Serialize InitUnpack(string str)
            {
                serialized = new Queue<string>(str.Split(separator
            , StringSplitOptions.None)); return this;
            }
            public bool FinishUnpack() { return serialized.Count == 0; }
            public Serialize Pack(
            string str)
            { serialized.Enqueue(str.Replace(separator[0], " ")); return this; }
            public Serialize Pack(int val)
            {
                serialized.Enqueue(
            val.ToString()); return this;
            }
            public Serialize Pack(long val) { serialized.Enqueue(val.ToString()); return this; }
            public
            Serialize Pack(float val)
            { serialized.Enqueue(val.ToString()); return this; }
            public Serialize Pack(double val)
            {
                serialized.Enqueue(
            val.ToString()); return this;
            }
            public Serialize Pack(bool val) { serialized.Enqueue(val ? "1" : "0"); return this; }
            public Serialize
            Pack(VRage.Game.MyCubeSize val)
            { Pack((int)val); return this; }
            public Serialize Pack(Vector3D val)
            {
                Pack(val.X); Pack(val.Y); Pack
            (val.Z); return this;
            }
            public Serialize Pack(List<Vector3D> val)
            {
                Pack(val.Count); foreach (Vector3D v in val) { Pack(v); }
                return
            this;
            }
            public Serialize Pack(List<int> val) { Pack(val.Count); foreach (int v in val) { Pack(v); } return this; }
            public Serialize Pack(
            DateTime t)
            { Pack(t.ToBinary()); return this; }
            public Serialize Unpack(ref string val) { val = serialized.Dequeue(); return this; }
            public
            Serialize Unpack(ref int val)
            { val = int.Parse(serialized.Dequeue()); return this; }
            public Serialize Unpack(ref long val)
            {
                val = long.
            Parse(serialized.Dequeue()); return this;
            }
            public Serialize Unpack(ref float val)
            {
                val = float.Parse(serialized.Dequeue()); return
            this;
            }
            public Serialize Unpack(ref double val) { val = double.Parse(serialized.Dequeue()); return this; }
            public Serialize Unpack(
            ref bool val)
            { val = serialized.Dequeue() == "1"; return this; }
            public Serialize Unpack(ref MyCubeSize val)
            {
                val = (MyCubeSize)int.
            Parse(serialized.Dequeue()); return this;
            }
            public Serialize Unpack(ref Vector3D val)
            {
                Unpack(ref val.X).Unpack(ref val.Y).Unpack
            (ref val.Z); return this;
            }
            public Serialize Unpack(ref List<Vector3D> list)
            {
                int count = 0; Unpack(ref count); for (int i = 0; i <
            count; i++) { Vector3D v3d = new Vector3D(); Unpack(ref v3d); list.Add(v3d); }
                return this;
            }
            public Serialize Unpack(ref List<int> val)
            {
                int count = 0; Unpack(ref count); for (int i = 0; i < count; i++) { int integer = 0; Unpack(ref integer); val.Add(integer); }
                return this;
            }
            public Serialize Unpack(ref DateTime t) { t = DateTime.FromBinary(long.Parse(serialized.Dequeue())); return this; }
        }
        class Settings
        {
            public MyIni ini = new MyIni(); public List<Section> sections = new List<Section>(); public Settings() { }
            public Section NewSection(
            string name)
            { var s = new Section(name); sections.Add(s); return s; }
            public string UpdateSettings(string iniString)
            {
                ini.Clear(); ini.
            TryParse(iniString); foreach (var s in this.sections) { s.UpdateSection(ref ini); }
                return ini.ToString();
            }
        }
        class Setting
        {
            public enum
            Content
            { INT, FLOAT, STRING, BOOL }; public object setting; public Content content; public void IniSet(ref MyIni ini, string sectionName
            , string keyName)
            {
                switch (this.content)
                {
                    case Setting.Content.BOOL: ini.Set(sectionName, keyName, (bool)this.setting); break;
                    case Setting.Content.INT: ini.Set(sectionName, keyName, (int)this.setting); break;
                    case Setting.Content.FLOAT:
                        ini.Set(sectionName
                    , keyName, (float)this.setting); break;
                    case Setting.Content.STRING: ini.Set(sectionName, keyName, (string)this.setting); break;
                }
            }
            public bool IniGet(ref MyIni ini, string sectionName, string keyName)
            {
                switch (this.content)
                {
                    case Setting.Content.BOOL:
                        bool
            boolVal; if (!bool.TryParse(ini.Get(sectionName, keyName).ToString(), out boolVal)) { return false; }
                        setting = boolVal; break;
                    case
            Setting.Content.INT:
                        int intVal; if (!int.TryParse(ini.Get(sectionName, keyName).ToString(), out intVal)) { return false; }
                        setting =
            intVal; break;
                    case Setting.Content.FLOAT:
                        float floatVal; if (!float.TryParse(ini.Get(sectionName, keyName).ToString(), out floatVal
            )) { return false; }
                        setting = floatVal; break;
                    case Setting.Content.STRING:
                        setting = ini.Get(sectionName, keyName).ToString(); break
            ;
                }
                return true;
            }
        }
        class Section
        {
            public Dictionary<string, Setting> settings = new Dictionary<string, Setting>(); public string
            sectionName = ""; public Section(string sectionName) { this.sectionName = sectionName; }
            public void RegisterString(string name, string
            defaultValue)
            { settings[name] = new Setting { content = Setting.Content.STRING, setting = defaultValue }; }
            public void RegisterFloat(string name
            , float defaultValue)
            { settings[name] = new Setting { content = Setting.Content.FLOAT, setting = defaultValue }; }
            public void
            RegisterBool(string name, bool defaultValue)
            { settings[name] = new Setting { content = Setting.Content.BOOL, setting = defaultValue }; }
            public
            void RegisterInt(string name, int defaultValue)
            {
                settings[name] = new Setting { content = Setting.Content.INT, setting = defaultValue };
            }
            public string GetString(string name) { return (string)settings[name].setting; }
            public float GetFloat(string name)
            {
                return (
            float)settings[name].setting;
            }
            public bool GetBool(string name) { return (bool)settings[name].setting; }
            public int GetInt(string
            name)
            { return (int)settings[name].setting; }
            public void UpdateSection(ref MyIni ini)
            {
                if (!ini.ContainsSection(this.sectionName))
                { SaveSection(ref ini); }
                else { LoadSection(ref ini); }
            }
            public void SaveSection(ref MyIni ini)
            {
                foreach (var pair in settings)
                {
                    pair.Value.IniSet(ref ini, sectionName, pair.Key);
                }
            }
            public void LoadSection(ref MyIni ini)
            {
                List<MyIniKey> iniKeys = new List<
                    MyIniKey>(); ini.GetKeys(sectionName, iniKeys); foreach (var invalidKey in iniKeys.Where(k => !settings.Keys.Contains(k.Name)))
                {
                    ini.
                    Delete(invalidKey);
                }
                foreach (var pair in settings)
                {
                    if (ini.ContainsKey(sectionName, pair.Key))
                    {
                        if (pair.Value.IniGet(ref ini,
                    sectionName, pair.Key)) { continue; }
                    }
                    settings[pair.Key].IniSet(ref ini, sectionName, pair.Key);
                }
            }
        }
        static System.Text.RegularExpressions.
                    Regex tagFormatRegex = new System.Text.RegularExpressions.Regex("\\[([^\\[\\]]+)\\]"); static System.Text.RegularExpressions.
                    Regex TagMatchRegex(string tags)
        { return new System.Text.RegularExpressions.Regex(@"\[(" + tags.Replace(",", "|") + @")\]"); }
        static
                    System.Text.RegularExpressions.Regex TagNumberMatchRegex(string tags)
        {
            return new System.Text.RegularExpressions.Regex(@"\[(" +
                    tags.Replace(",", "|") + @")(_(\d+))?\]");
        }
        static System.Text.RegularExpressions.Regex TagWordMatchRegex(string tags)
        {
            return
                    new System.Text.RegularExpressions.Regex(@"\[(" + tags.Replace(",", "|") + @")(_([a-zA-Z]+))?\]");
        }
        static System.Text.
                    RegularExpressions.Regex TagActionMatchRegex(string tags)
        {
            return new System.Text.RegularExpressions.Regex("\\[(" + tags.Replace(",", "|") +
                    ")_([vxyzVXYZ])\\]");
        }
        static string ToMetric(float n)
        {
            if (n > 1000000000.0f) { return string.Format("{0:F1}Gm", n / 1000000000.0f); }
            else if (n >
                    1000000.0f) { return string.Format("{0:F1}Mm", n / 1000000.0f); }
            else if (n > 1000.0f) { return string.Format("{0:F1}km", n / 1000.0f); }
            return
                    string.Format("{0:F0}m", n);
        }
        static string TimeSpanToString(TimeSpan ts)
        {
            string s; if (ts.TotalMinutes < 1.0)
            {
                s = String.Format(
                    "{0}s", ts.Seconds);
            }
            else if (ts.TotalHours < 1.0) { s = String.Format("{0}m:{1:D2}s", ts.Minutes, ts.Seconds); }
            else
            {
                s = String.Format(
                    "{0}h:{1:D2}m:{2:D2}s", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
            }
            return s;
        }
        static string GenerateGPS(string name, Vector3D v, string color =
                    "FF00FF", bool full = true)
        { return string.Format("GPS:{0}:{1}:{2}:{3}:#{4}:", name, full ? v.X : Math.Round(v.X, 4), full ? v.Y : Math.Round(v.Y, 4), full ? v.Z : Math.Round(v.Z, 4), color); }
        static Vector3D GetOrbit(Program p, double
                    altitude)
        {
            List<IMyShipController> shipControllers = new List<IMyShipController>(); p.GridTerminalSystem.GetBlocksOfType<
                    IMyShipController>(shipControllers); foreach (IMyShipController shipController in shipControllers)
            {
                if (!shipController.IsSameConstructAs(p.
                    Me)) { continue; }
                Vector3D planetCenter; if (!shipController.TryGetPlanetPosition(out planetCenter))
                {
                    throw new Exception(
                    "Not in gravity?");
                }
                double currentAltitude; if (!shipController.TryGetPlanetElevation(MyPlanetElevation.Surface, out currentAltitude))
                {
                    throw
                    new Exception("Not in gravity?");
                }
                Vector3D currentPosition = shipController.GetPosition(); return (altitude - currentAltitude) *
                    Vector3D.Normalize(currentPosition - planetCenter) + currentPosition;
            }

            throw new Exception("No RCs or Cockpits?");
        }
        // Jturp's Text Utility Class
        public struct FontFunctions
        {
            public bool Debug { get; set; }
            private Program _p;
            private Dictionary<char, int> _fontD;
            private Dictionary<MyCubeSize, Dictionary<string, PanelSize>> _panelD;
            private FontType _font;
            private PanelSize _lcdRes;
            private enum FontType { Default, Monospace };
            private List<string> _tempList, _wrapList, _justList, _centList, _rightList, _scrList;
            private StringBuilder _strings;
            private const float _wRes = 18944f / 28.8f;
            private const float _wResWD = _wRes * 2;
            private const float _hRes = _wRes * 0.99375f;
            private int _num, _lcdRatio;
            private double _width, _cWidth, _padWidth;
            private float _lcdWidth, _fontRatio;
            private string _padding;
            private string[] _words, _temp;
            private char[] _chars;

            public FontFunctions(Program p)
            {
                _p = p;
                _tempList = _wrapList = _justList = _centList = _rightList = _scrList = new List<string>();
                _strings = new StringBuilder();
                _font = FontType.Default;
                _lcdRes = new PanelSize(_wRes, _hRes);
                _width = _padWidth = _cWidth = 0;
                _lcdWidth = 0;
                _fontRatio = 1.0f;
                _lcdRatio = 1;
                _num = 0;
                _padding = "";
                _words = _temp = null;
                _chars = null;
                Debug = false;

                _panelD = new Dictionary<MyCubeSize, Dictionary<string, PanelSize>>()
        {
          { MyCubeSize.Large, new Dictionary<string, PanelSize> {
            { "LargeLCDPanel", new PanelSize(_wRes, _hRes  ) },
            { "LargeLCDPanelWide", new PanelSize(_wResWD, _hRes ) },
            { "LargeLCDPanel2x2", new PanelSize(_wRes, _hRes ) },
            { "LargeBlockCorner_LCD_1", new PanelSize(_wRes, 98.667f ) },
            { "LargeBlockCorner_LCD_2", new PanelSize(_wRes, 98.667f ) },
            { "LargeBlockCorner_LCD_Flat_1", new PanelSize(_wRes, 111.0f ) },
            { "LargeBlockCorner_LCD_Flat_2", new PanelSize(_wRes, 111.0f ) },
            { "FlightLCD", new PanelSize(655f, 240.5f) },
            { "LargeLCDPanelSlope2Base4", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlope2Base3", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlope2Base2", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlope2Base1", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlope2Tip4", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlope2Tip3", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlope2Tip2", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlope2Tip1", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlopeV", new PanelSize(_wRes, _hRes) },
            { "LargeLCDPanelSlopeH", new PanelSize(_wRes, _hRes) }}
          },
          { MyCubeSize.Small, new Dictionary<string, PanelSize> {
            { "SmallTextPanel", new PanelSize(_wRes, _hRes) },
            { "SmallLCDPanel", new PanelSize(_wRes, _hRes) },
            { "SmallLCDPanelWide", new PanelSize(_wResWD, _hRes) },
            { "SmallBlockCorner_LCD_1", new PanelSize(_wRes, 98.667f ) },
            { "SmallBlockCorner_LCD_2", new PanelSize(_wRes, 98.667f ) },
            { "SmallBlockCorner_LCD_Flat_1", new PanelSize(_wRes, 111.0f ) },
            { "SmallBlockCorner_LCD_Flat_2", new PanelSize(_wRes, 111.0f ) },
            { "SmallTextPanelSlopeBase4", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeBase3", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeBase2", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeBase1", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeTip4", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeTip3", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeTip2", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeTip1", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeV", new PanelSize(_wRes, _hRes) },
            { "SmallTextPanelSlopeH", new PanelSize(_wRes, _hRes) }}
          }
        };

                _fontD = new Dictionary<char, int>
        {
          {' ', 8}, {'!', 8}, {'"', 10}, {'#', 19}, {'$', 20}, {'%', 24}, {'&', 20}, {'(', 9}, {')', 9}, {'*', 11}, {'+', 18}, {',', 9},
          {'-', 10}, {'.', 9}, {'/', 14}, {'0', 19}, {'1', 9}, {'2', 19}, {'3', 17}, {'4', 19}, {'5', 19}, {'6', 19}, {'7', 16}, {'8', 19},
          {'9', 19}, {':', 9}, {';', 9}, {'<', 18}, {'=', 18}, {'>', 18}, {'?', 16}, {'@', 25}, {'A', 21}, {'B', 21}, {'C', 19}, {'D', 21},
          {'E', 18}, {'F', 17}, {'G', 20}, {'H', 20}, {'I', 8}, {'J', 16}, {'K', 17}, {'L', 15}, {'M', 26}, {'N', 21}, {'O', 21}, {'P', 20},
          {'Q', 21}, {'R', 21}, {'S', 21}, {'T', 17}, {'U', 20}, {'V', 20}, {'W', 31}, {'X', 19}, {'Y', 20}, {'Z', 19}, {'[', 9}, {']', 9},
          {'^', 18}, {'_', 15}, {'`', 8}, {'a', 17}, {'b', 17}, {'c', 16}, {'d', 17}, {'e', 17}, {'f', 9}, {'g', 17}, {'h', 17}, {'i', 8},
          {'j', 8}, {'k', 17}, {'l', 8}, {'m', 27}, {'n', 17}, {'o', 17}, {'p', 17}, {'q', 17}, {'r', 10}, {'s', 17}, {'t', 9}, {'u', 17},
          {'v', 15}, {'w', 27}, {'x', 15}, {'y', 17}, {'z', 16}, {'{', 9}, {'|', 6}, {'}', 9}, {'~', 18}, {'\\', 12}, {'\'', 6}
        };
            }

            public string Run(List<string> commands, List<string> input, MyCubeSize gridSize, string lcdType, string fontID, float fontSize)
            {
                _centList.Clear();
                _rightList.Clear();
                _justList.Clear();
                _wrapList.Clear();
                _strings.Clear();

                _font = (fontID == "Monospace") ? FontType.Monospace : FontType.Default;

                if (!_panelD[gridSize].TryGetValue(lcdType, out _lcdRes))
                    _lcdRes = new PanelSize(_wRes, _hRes);

                _fontRatio = 1.0f / fontSize;
                _lcdWidth = (int)Math.Round(_lcdRes.Width * _fontRatio, MidpointRounding.AwayFromZero);
                _lcdRatio = _lcdRes.Width == _wResWD ? 2 : 1;

                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].ToLower().Contains("center"))
                    {
                        Center(input, out _centList);
                        if (commands.Any(x => x.ToLower().Contains("scroll")))
                        {
                            //Scroll(_centList, out _scrList); return string.Join("\n", _scrList);
                        }
                        else
                            return string.Join("\n", _centList);
                    }
                    else if (commands[i].ToLower().Contains("right"))
                    {
                        Right(input, out _rightList);
                        if (commands.Any(x => x.ToLower().Contains("scroll")))
                        {
                            //Scroll(_rightList, out _scrList); return string.Join("\n", _scrList);
                        }
                        else
                            return string.Join("\n", _rightList);
                    }
                    else if (commands[i].ToLower().Contains("justify"))
                    {
                        Wrap(input, out _wrapList, (int)(7 * _lcdRatio * _fontRatio));
                        Justify(_wrapList, out _justList);
                        if (commands.Any(x => x.ToLower().Contains("scroll")))
                        {
                            //Scroll(_justList, out _scrList); return string.Join("\n", _scrList);
                        }
                        else
                            return string.Join("\n", _justList);
                    }
                    else if (commands[i].ToLower().Contains("wrap"))
                    {
                        Wrap(input, out _wrapList);
                        if (commands.Any(x => x.ToLower().Contains("scroll")))
                        {
                            //Scroll(_wrapList, out _scrList); return string.Join("\n", _scrList);
                        }
                        else
                            return String.Join("\n", _wrapList);
                    }
                }
                return string.Join("\n", input);
            }

            public float AlignWord(StringBuilder addTo, string word, float maxChars, char fillChar = ' ')
            {
                var fillCharWidth = _fontD[fillChar];
                var maxWidth = fillCharWidth * maxChars;
                int width = 0;

                foreach (var ch in word)
                    width += _fontD[ch] + 1;

                var leftOver = maxWidth - width;
                var numToAdd = (int)(leftOver / fillCharWidth);
                var error = maxWidth - width - (numToAdd * fillCharWidth);

                addTo.Append(word)
                  .Append(fillChar, numToAdd);

                return error / fillCharWidth;
            }

            /// <summary>
            /// Centers text on an LCD screen.
            /// </summary>
            /// <param name="lcdType">The Subtype Name of the LCD to be used.</param>
            /// <param name="input">The string(s) to display.</param>
            /// <param name="output"></param>
            /// <returns></returns>
            private List<string> Center(List<string> input, out List<string> output)
            {
                output = new List<string>();

                _strings.Clear();
                _tempList.Clear();
                foreach (var s in input)
                {
                    _temp = s.Trim().Split(' ');
                    for (int i = 0; i < _temp.Count(); i++)
                        if (!string.IsNullOrWhiteSpace(_temp[i]))
                            _strings.Append($"{_temp[i].Trim()} ");

                    _tempList.Add(_strings.ToString().Trim());
                    _strings.Clear();
                }

                for (int i = 0; i < _tempList.Count; i++)
                {
                    _width = 0;
                    _padding = "";

                    foreach (char c in _tempList[i].Trim())
                        _width += (_font == FontType.Default ? _fontD.GetValueOrDefault(c) + 1 : 25);

                    _width -= 1;
                    _padWidth = Math.Max(2, Math.Round(((_lcdWidth - _width) / 18), MidpointRounding.AwayFromZero));
                    _padding = _padding.PadLeft(Math.Max(0, (int)_padWidth - 2));

                    output.Add($" {_padding}{_tempList[i].Trim()}");
                }
                return output;
            }

            /// <summary>
            /// Right justifies text on an LCD screen.
            /// </summary>
            /// <param name="lcdType">The Subtype Name of the LCD to be used.</param>
            /// <param name="input">The string(s) to display.</param>
            /// <param name="output"></param>
            /// <returns></returns>
            private List<string> Right(List<string> input, out List<string> output)
            {
                output = new List<string>();

                _strings.Clear();
                _tempList.Clear();
                foreach (var s in input)
                {
                    _temp = s.Trim().Split(' ');
                    for (int i = 0; i < _temp.Count(); i++)
                        if (!string.IsNullOrWhiteSpace(_temp[i]))
                            _strings.Append($"{_temp[i].Trim()} ");

                    _tempList.Add(_strings.ToString().Trim());
                    _strings.Clear();
                }

                for (int i = 0; i < _tempList.Count; i++)
                {
                    _width = 0;
                    _padding = "";

                    foreach (char c in _tempList[i].Trim())
                        _width += (_font == FontType.Default ? _fontD.GetValueOrDefault(c) + 1 : 25);

                    _width -= 1;
                    _padWidth = Math.Max(1, Math.Round(((_lcdWidth - _width) / 9), MidpointRounding.AwayFromZero));
                    _padding = _padding.PadLeft((int)_padWidth - 1);

                    output.Add($"{_padding}{_tempList[i].Trim()}");
                }
                return output;
            }

            /// <summary>
            /// Justifies text on an LCD screen.
            /// </summary>
            /// <param name="lcdType">The Subtype Name of the LCD to be used.</param>
            /// <param name="input">The string(s) to display.</param>
            /// <param name="output"></param>
            /// <returns></returns>
            private List<string> Justify(List<string> input, out List<string> output)
            {
                output = new List<string>();
                for (int i = 0; i < input.Count; i++)
                {
                    _width = 0;
                    _strings.Clear();
                    _strings.Append(" ");
                    _padding = "";

                    _words = input[i].Trim().Split(' ');
                    _num = _words.Count() - 1;

                    foreach (char c in input[i])
                        _width += (_font == FontType.Default ? _fontD.GetValueOrDefault(c) + 1 : 25);

                    _width -= _num + 1;
                    _padWidth = Math.Max(1, Math.Round(((_lcdWidth - _width) / _num / 9), 0));
                    int countWidth = 0;
                    int countAdded = 0;

                    if (_num != 0)
                        _padding = _padding.PadRight((int)_padWidth);

                    for (int j = 0; j < _num; j++)
                    {
                        _strings.Append($"{_words[j]}{_padding}");

                        countWidth++;
                        if (j + 1 < _num && _padWidth == 1)
                        {
                            countAdded++;
                            _strings.Append(" ");
                            countWidth = 0;
                        }
                        else if (countWidth >= 2)
                        {
                            if (j + 1 < _num || _padWidth == 1)
                            {
                                countAdded++;
                                _strings.Append(" ");
                                countWidth = 0;
                            }
                        }
                    }

                    // Find the actual width of all characters in the string
                    var aw = (_width + _num + 1) - (_num * 8);
                    // Subtract actual width from LCD width to get the remainder
                    var rem = (_lcdWidth - 16) - (aw + ((_padWidth * _num + countAdded) * 9));

                    // Add or remove spacers to complete the justification
                    if (rem >= 21)
                        _strings.Append("   ");
                    if (rem >= 13)
                        _strings.Append("  ");
                    if (rem >= 5)
                        _strings.Append(" ");
                    if (rem < -13)
                        _strings.Length -= 2;
                    if (rem < -8)
                        _strings.Length--;

                    _strings.Append($"{_words.Last()}");
                    output.Add(_strings.ToString());
                }
                return output;
            }

            /// <summary>
            /// Wraps the text on an LCD screen.
            /// </summary>
            /// <param name="lcdType">The Subtype Name of the LCD to be used.</param>
            /// <param name="input">The string(s) to display.</param>
            /// <param name="output"></param>
            /// <param name="wordCount"></param>
            /// <returns></returns>
            private List<string> Wrap(List<string> input, out List<string> output, int wordCount = 0)
            {
                output = new List<string>();
                int count = 0;
                _width = _cWidth = 0;

                _strings.Clear();
                foreach (var s in input)
                {
                    _temp = s.Trim().Split(' ');
                    for (int i = 0; i < _temp.Count(); i++)
                        if (!string.IsNullOrWhiteSpace(_temp[i]))
                            _strings.Append($"{_temp[i].Trim()} ");
                }

                foreach (char c in _strings.ToString().Trim())
                    _width += (_font == FontType.Default ? _fontD.GetValueOrDefault(c) + 1 : 25);

                if (_lcdWidth > _width)
                {
                    output.Add(string.Join("\n", input).Trim());
                    return output;
                }

                _words = _strings.ToString().Trim().Split(' ');

                _strings.Clear();
                _strings.Append(" ");
                _padding = " ";
                _width = 0;

                for (int i = 0; i < _words.Count(); i++)
                {
                    if (wordCount != 0)
                    {
                        count++;
                        if (count == wordCount)
                        {
                            output.Add(_strings.ToString().Trim());
                            _width = _cWidth = 0;
                            count = 0;
                            _strings.Clear();
                            _strings.Append(" ");
                        }
                    }

                    _chars = _words[i].Trim().ToCharArray();
                    _num = _chars.Count();

                    for (int j = 0; j < _num; j++)
                        _cWidth += (_font == FontType.Default ? _fontD.GetValueOrDefault(_chars[j]) + ((9 + _num) / _num) : 25);

                    _width += _cWidth;
                    if (_width > _lcdWidth - 16)
                    {
                        output.Add(_strings.ToString());
                        _strings.Clear();
                        _strings.Append($" {_words[i]}{_padding}");
                        _width = _cWidth;
                        count = 0;
                        _cWidth = 0;
                    }
                    else
                    {
                        _strings.Append($"{_words[i]}{_padding}");
                        _cWidth = 0;
                    }
                }
                output.Add(_strings.ToString());
                return output;
            }

            private struct PanelSize
            {
                public readonly float Width;
                public readonly float Height;

                public PanelSize(float width, float height)
                {
                    this.Width = width;
                    this.Height = height;
                }
            }
        }
