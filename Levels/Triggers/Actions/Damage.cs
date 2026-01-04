// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Damage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class Damage : Action
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  protected string mID;
  protected int mIDHash;
  protected string mType;
  protected Factions mFactions;
  protected int mTypeHash;
  protected string mArea;
  protected int mAreaHash;
  protected string mSource;
  protected int mSourceID;
  protected bool mIgnoreShields = true;
  private Magicka.GameLogic.Damage[] mDamages;
  protected bool mIsSpecific;

  public Damage(Trigger iTrigger, GameScene iScene, XmlNode iNode)
    : base(iTrigger, iScene)
  {
    List<Magicka.GameLogic.Damage> damageList = new List<Magicka.GameLogic.Damage>();
    foreach (XmlNode childNode in iNode.ChildNodes)
    {
      if (!(childNode is XmlComment) && childNode.Name.Equals("damage", StringComparison.OrdinalIgnoreCase))
      {
        Magicka.GameLogic.Damage damage = new Magicka.GameLogic.Damage();
        foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode.Attributes)
        {
          if (attribute.Name.Equals("attackProperty", StringComparison.OrdinalIgnoreCase))
            damage.AttackProperty = (AttackProperties) Enum.Parse(typeof (AttackProperties), attribute.Value, true);
          else if (attribute.Name.Equals("element", StringComparison.OrdinalIgnoreCase))
            damage.Element = (Elements) Enum.Parse(typeof (Elements), attribute.Value, true);
          else if (attribute.Name.Equals("amount", StringComparison.OrdinalIgnoreCase))
            damage.Amount = float.Parse(attribute.Value);
          else if (attribute.Name.Equals("magnitude", StringComparison.OrdinalIgnoreCase))
            damage.Magnitude = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        }
        damageList.Add(damage);
      }
    }
    this.mDamages = damageList.ToArray();
  }

  protected override void Execute()
  {
    Vector3 iAttackPosition = new Vector3();
    if (this.mSourceID != 0)
    {
      Matrix oLocator;
      this.GameScene.GetLocator(this.mSourceID, out oLocator);
      iAttackPosition = oLocator.Translation;
    }
    if (this.mIsSpecific)
    {
      if (!(Entity.GetByID(this.mIDHash) is IDamageable byId))
        return;
      for (int index = 0; index < this.mDamages.Length; ++index)
      {
        int num = (int) byId.Damage(this.mDamages[index], (Entity) null, 0.0, iAttackPosition);
      }
    }
    else
    {
      TriggerArea triggerArea = this.GameScene.GetTriggerArea(this.mAreaHash);
      for (int iIndex = 0; iIndex < triggerArea.PresentCharacters.Count; ++iIndex)
      {
        Character presentCharacter = triggerArea.PresentCharacters[iIndex];
        if (presentCharacter != null && (this.mTypeHash == Damage.ANYID || presentCharacter.Type == this.mTypeHash || (presentCharacter.GetOriginalFaction & this.mFactions) != Factions.NONE))
        {
          if (!this.mIgnoreShields)
          {
            if (this.mSourceID == 0)
            {
              if (this.GameScene.PlayState.EntityManager.IsProtectedByShield((Entity) presentCharacter, out Shield _))
                continue;
            }
            else
            {
              Segment iSeg;
              iSeg.Origin = iAttackPosition;
              iSeg.Delta = presentCharacter.Position;
              Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
              List<Shield> shields = this.GameScene.PlayState.EntityManager.Shields;
              bool flag = false;
              foreach (Shield shield in shields)
              {
                if (shield.SegmentIntersect(out Vector3 _, iSeg, 0.25f))
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
                continue;
            }
          }
          for (int index = 0; index < this.mDamages.Length; ++index)
          {
            int num = (int) presentCharacter.Damage(this.mDamages[index], (Entity) null, 0.0, iAttackPosition);
          }
        }
      }
    }
  }

  public override void QuickExecute() => this.Execute();

  public Factions Factions
  {
    get => this.mFactions;
    set => this.mFactions = value;
  }

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      if (!string.IsNullOrEmpty(this.mID))
      {
        this.mIsSpecific = true;
        this.mIDHash = this.mID.GetHashCodeCustom();
      }
      else
      {
        this.mIsSpecific = false;
        this.mIDHash = 0;
      }
    }
  }

  public string Type
  {
    get => this.mType;
    set
    {
      this.mType = value;
      this.mTypeHash = this.mType.GetHashCodeCustom();
    }
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaHash = this.mArea.GetHashCodeCustom();
    }
  }

  public string Source
  {
    get => this.mSource;
    set
    {
      this.mSource = value;
      this.mSourceID = this.mSource.GetHashCodeCustom();
    }
  }

  public bool IgnoreShields
  {
    get => this.mIgnoreShields;
    set => this.mIgnoreShields = value;
  }
}
