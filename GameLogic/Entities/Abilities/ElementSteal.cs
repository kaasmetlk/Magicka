// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.ElementSteal
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class ElementSteal : Ability
{
  private float mRange;
  private float mAngle;

  public ElementSteal(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mRange = iInput.ReadSingle();
    this.mAngle = iInput.ReadSingle();
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    throw new NotImplementedException("Elementsteal must define a desirability expression!");
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    if (iAgent.Owner.NextAttackAnimation != this.mAnimationKeys[0])
    {
      if (iAgent.Owner.CurrentAnimation != this.mAnimationKeys[0])
      {
        iAgent.Owner.Attack(this.mAnimationKeys[0], false);
      }
      else
      {
        if (iAgent.Owner.CurrentAnimation == this.mAnimationKeys[0])
        {
          if ((double) this.mAngle == 0.0)
          {
            List<Entity> entities = iAgent.Owner.PlayState.EntityManager.GetEntities(iAgent.Owner.Position, this.mRange, true);
            for (int index = 0; index < entities.Count; ++index)
            {
              if (entities[index] is Character character && character.SpellQueue.Count > 0 && (character.Faction & iAgent.Owner.Faction) == Factions.NONE)
              {
                character.SpellQueue.Clear();
                if (character is Avatar)
                  (character as Avatar).Player.IconRenderer.Clear();
              }
            }
            iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
          }
          else if (iAgent.CurrentTarget is Character)
          {
            (iAgent.CurrentTarget as Character).SpellQueue.Clear();
            if (iAgent.CurrentTarget is Avatar)
              (iAgent.CurrentTarget as Avatar).Player.IconRenderer.Clear();
          }
        }
        return true;
      }
    }
    return false;
  }

  public override float GetMaxRange(Agent iAgent) => this.mRange;

  public override float GetMinRange(Agent iAgent) => 0.0f;

  public override float GetArc(Agent iAgent)
  {
    return (double) this.mAngle != 0.0 ? this.mAngle : 6.28318548f;
  }

  public override int[] GetWeapons() => (int[]) null;

  public override Vector3 GetDesiredDirection(Agent iAgent)
  {
    return iAgent.Owner.Body.Orientation.Forward;
  }
}
