// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Footstep
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class Footstep(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : AnimationAction(iInput, iSkeleton)
{
  private static readonly int[] sCueNames = new int[10]
  {
    "footstep_generic".GetHashCodeCustom(),
    "footstep_gravel".GetHashCodeCustom(),
    "footstep_grass".GetHashCodeCustom(),
    "footstep_wood".GetHashCodeCustom(),
    "footstep_snow".GetHashCodeCustom(),
    "footstep_stone".GetHashCodeCustom(),
    "footstep_mud".GetHashCodeCustom(),
    "footstep_generic".GetHashCodeCustom(),
    "footstep_water".GetHashCodeCustom(),
    0
  };
  private static readonly int[] sEffectNames = new int[10]
  {
    0,
    "footstep_gravel".GetHashCodeCustom(),
    "footstep_grass".GetHashCodeCustom(),
    0,
    "footstep_snow".GetHashCodeCustom(),
    0,
    "footstep_mud".GetHashCodeCustom(),
    0,
    "footstep_water".GetHashCodeCustom(),
    "footstep_lava".GetHashCodeCustom()
  };

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution || !iOwner.CharacterBody.IsTouchingGround || iOwner.IsLevitating)
      return;
    if (Footstep.sCueNames[(int) iOwner.CharacterBody.GroundMaterial] != 0)
    {
      Footstep.FootstepVariables iVariables;
      iVariables.Weight = iOwner.CharacterBody.Mass;
      iVariables.Depth = iOwner.WaterDepth;
      AudioManager.Instance.PlayCue<Footstep.FootstepVariables>(Banks.Footsteps, Footstep.sCueNames[(int) iOwner.CharacterBody.GroundMaterial], iVariables, iOwner.AudioEmitter);
    }
    if (Footstep.sEffectNames[(int) iOwner.CharacterBody.GroundMaterial] == 0)
      return;
    Vector3 position = iOwner.Position;
    position.Y += iOwner.HeightOffset + iOwner.WaterDepth;
    Vector3 direction = iOwner.Direction;
    EffectManager.Instance.StartEffect(Footstep.sEffectNames[(int) iOwner.CharacterBody.GroundMaterial], ref position, ref direction, out VisualEffectReference _);
  }

  public override bool UsesBones => false;

  internal struct FootstepVariables(float iWeight, float iDepth) : IAudioVariables
  {
    public static readonly string VARIABLE_WEIGHT = nameof (Weight);
    public static readonly string VARIABLE_DEPTH = "WaterDepth";
    public float Weight = iWeight;
    public float Depth = iDepth;

    public void AssignToCue(Cue iCue)
    {
      iCue.SetVariable(Footstep.FootstepVariables.VARIABLE_WEIGHT, this.Weight);
      iCue.SetVariable(Footstep.FootstepVariables.VARIABLE_DEPTH, this.Depth);
    }
  }
}
