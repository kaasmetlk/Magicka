// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Versus.Pyrite
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Xml;

#nullable disable
namespace Magicka.Levels.Versus;

internal class Pyrite : VersusRuleset
{
  private Pyrite.Settings mSettings;

  public Pyrite(GameScene iScene, XmlNode iNode, Pyrite.Settings iSettings)
    : base(iScene, iNode)
  {
    this.mSettings = iSettings;
  }

  public override void Initialize() => base.Initialize();

  public override void DeInitialize()
  {
  }

  public override Rulesets RulesetType => Rulesets.Pyrite;

  public override bool CanRevive(Magicka.GameLogic.Player iReviver, Magicka.GameLogic.Player iRevivee)
  {
    return false;
  }

  internal override short[] GetScores() => (short[]) null;

  internal override bool Teams => false;

  internal override short[] GetTeamScores() => (short[]) null;

  internal new class Settings : VersusRuleset.Settings
  {
    public override bool TeamsEnabled => false;
  }
}
