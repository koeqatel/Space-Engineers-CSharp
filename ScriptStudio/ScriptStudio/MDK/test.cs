IMyTextSurfaceProvider thisBlock;

public Program()
{
   thisBlock = GridTerminalSystem.GetBlockWithName("[Battery Logic]") as IMyTextSurfaceProvider;
}

public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means. 
    // 
    // This method is optional and can be removed if not
    // needed.
}

public void Main(string argument, UpdateType updateSource)
{
   thisBlock.GetSurface[0].FontColor = Color.Green;
}
