namespace ACT.Windows.Tools.ConsoleMenu;

public class CloseTrigger
{
  private bool close;

  public void SetOn() => this.close = true;

  public void SetOff() => this.close = false;

  public bool IsOn() => this.close;
}
