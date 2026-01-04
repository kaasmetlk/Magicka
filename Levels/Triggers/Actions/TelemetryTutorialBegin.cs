// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.TelemetryTutorialBegin
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.WebTools.Paradox.Telemetry;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class TelemetryTutorialBegin(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mTutorialName = string.Empty;
  private string mStepName = string.Empty;

  public string TutorialName
  {
    get => this.mTutorialName;
    set => this.mTutorialName = value;
  }

  public string StepName
  {
    get => this.mStepName;
    set => this.mStepName = value;
  }

  protected override void Execute() => TutorialUtils.Start(this.mTutorialName, this.mStepName);

  public override void QuickExecute() => this.Execute();
}
