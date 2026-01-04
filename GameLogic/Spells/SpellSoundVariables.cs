// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellSoundVariables
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Microsoft.Xna.Framework.Audio;

#nullable disable
namespace Magicka.GameLogic.Spells;

public struct SpellSoundVariables : IAudioVariables
{
  public static readonly string VARIABLE_MAGNITUDE = "Magnitude";
  public float mMagnitude;

  public void AssignToCue(Cue iCue)
  {
    iCue.SetVariable(SpellSoundVariables.VARIABLE_MAGNITUDE, this.mMagnitude);
  }
}
