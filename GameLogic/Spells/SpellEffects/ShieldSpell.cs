// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellEffects.ShieldSpell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.Gamers;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells.SpellEffects;

internal class ShieldSpell : SpellEffect
{
  private static List<ShieldSpell> sCache;
  private bool mHasCast;

  public static void InitializeCache(int iSize)
  {
    ShieldSpell.sCache = new List<ShieldSpell>(iSize);
    for (int index = 0; index < iSize; ++index)
      ShieldSpell.sCache.Add(new ShieldSpell());
  }

  public static SpellEffect GetFromCache()
  {
    ShieldSpell fromCache = ShieldSpell.sCache[ShieldSpell.sCache.Count - 1];
    ShieldSpell.sCache.Remove(fromCache);
    return (SpellEffect) fromCache;
  }

  public static void ReturnToCache(ShieldSpell iEffect) => ShieldSpell.sCache.Add(iEffect);

  public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastArea(iSpell, iOwner, iFromStaff);
    this.mSpell = iSpell;
    Elements elements = this.mSpell.Element & ~Elements.Shield;
    this.mMinTTL = 0.5f;
    this.mHasCast = true;
    if (elements == Elements.None)
    {
      NetworkState state = NetworkManager.Instance.State;
      if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
        return;
      float iRadius = 5.5f;
      int iHitpoints = 1000;
      Vector3 result1 = Spell.SHIELDCOLOR;
      Vector3.Multiply(ref result1, 2f * (float) Math.Sqrt((double) iSpell[Elements.Shield]), out result1);
      Vector3 result2 = Spell.ARCANECOLOR;
      Vector3.Multiply(ref result2, 2f * (float) Math.Sqrt((double) iSpell[Elements.Arcane]), out result2);
      Vector3 result3 = Spell.LIFECOLOR;
      Vector3.Multiply(ref result3, 2f * (float) Math.Sqrt((double) iSpell[Elements.Life]), out result3);
      Vector3.Add(ref result1, ref result2, out result1);
      Vector3.Add(ref result1, ref result3, out result1);
      if (state == NetworkState.Client)
      {
        SpawnShieldRequestMessage iMessage;
        iMessage.OwnerHandle = iOwner.Handle;
        iMessage.Position = iOwner.Position;
        iMessage.Radius = iRadius;
        iMessage.Direction = iOwner.Direction;
        iMessage.ShieldType = ShieldType.SPHERE;
        iMessage.HitPoints = (float) iHitpoints;
        NetworkManager.Instance.Interface.SendMessage<SpawnShieldRequestMessage>(ref iMessage, 0);
      }
      else
      {
        Shield fromCache = Shield.GetFromCache(iOwner.PlayState);
        if (state == NetworkState.Server)
        {
          SpawnShieldMessage iMessage;
          iMessage.Handle = fromCache.Handle;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.Position = iOwner.Position;
          iMessage.Radius = iRadius;
          iMessage.Direction = iOwner.Direction;
          iMessage.ShieldType = ShieldType.SPHERE;
          iMessage.HitPoints = (float) iHitpoints;
          NetworkManager.Instance.Interface.SendMessage<SpawnShieldMessage>(ref iMessage);
        }
        fromCache.Initialize(iOwner, iOwner.Position, iRadius, iOwner.Direction, ShieldType.SPHERE, (float) iHitpoints, result1);
        iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache);
      }
    }
    else if ((elements & Elements.PhysicalElements) == Elements.None && (elements & Elements.Arcane) == Elements.Arcane | (elements & Elements.Life) == Elements.Life)
    {
      NetworkState state = NetworkManager.Instance.State;
      if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
        return;
      float scaleFactor = 4f;
      float num1 = scaleFactor * 6.28318548f;
      float num2 = num1 * 0.5f;
      int num3 = (int) num2;
      float iScale = (num2 + (float) num3) / (float) num3;
      float num4 = 6.28318548f / (float) num3;
      Vector3 oPos = iOwner.Position;
      Vector3 result4 = iOwner.Direction;
      Vector3.Multiply(ref result4, scaleFactor, out result4);
      Quaternion result5;
      Quaternion.CreateFromYawPitchRoll(num4 * 0.5f, 0.0f, 0.0f, out result5);
      Vector3 result6;
      Vector3.Transform(ref result4, ref result5, out result6);
      Vector3 result7;
      Vector3.Normalize(ref result6, out result7);
      Vector3.Add(ref oPos, ref result6, out result6);
      Quaternion.CreateFromYawPitchRoll(num4 * 1f, 0.0f, 0.0f, out result5);
      Quaternion result8;
      Quaternion.CreateFromYawPitchRoll(num4 * -0.5f, 0.0f, 0.0f, out result8);
      Vector3 result9;
      Vector3.Transform(ref result4, ref result8, out result9);
      Vector3 result10;
      Vector3.Normalize(ref result9, out result10);
      Vector3.Add(ref oPos, ref result9, out result9);
      Quaternion.CreateFromYawPitchRoll(num4 * -1f, 0.0f, 0.0f, out result8);
      Vector3 result11;
      Vector3.Subtract(ref result6, ref result9, out result11);
      Vector3 result12;
      Vector3.Subtract(ref result9, ref result6, out result12);
      float iDistanceBetweenMines = result11.Length();
      Segment iSeg = new Segment();
      iSeg.Delta.Y = -4f;
      this.mSpell = iSpell;
      DamageCollection5 oDamages;
      this.mSpell.CalculateDamage(SpellType.Shield, CastType.Area, out oDamages);
      iSeg.Origin = result6;
      float iRange = num1 * 0.5f - iDistanceBetweenMines;
      float oFrac;
      Vector3 oNrm;
      AnimatedLevelPart oAnimatedLevelPart;
      if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
      {
        if (state == NetworkState.Client)
        {
          SpawnMineRequestMessage iMessage;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
          iMessage.Position = oPos;
          iMessage.Direction = result7;
          iMessage.Scale = iScale;
          iMessage.Spell = iSpell;
          iMessage.Damage = oDamages;
          iMessage.Range = iRange;
          iMessage.NextDir = result11;
          iMessage.NextRotation = result5;
          iMessage.Distance = iDistanceBetweenMines;
          NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref iMessage, 0);
        }
        else
        {
          SpellMine instance = SpellMine.GetInstance();
          if (state == NetworkState.Server)
          {
            SpawnMineMessage iMessage;
            iMessage.Handle = instance.Handle;
            iMessage.OwnerHandle = iOwner.Handle;
            iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
            iMessage.Position = oPos;
            iMessage.Direction = result7;
            iMessage.Scale = iScale;
            iMessage.Spell = iSpell;
            iMessage.Damage = oDamages;
            NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref iMessage);
          }
          instance.Initialize(iOwner, oPos, result7, iScale, iRange, result11, result5, iDistanceBetweenMines, ref iSpell, ref oDamages, oAnimatedLevelPart);
          iOwner.PlayState.EntityManager.AddEntity((Entity) instance);
        }
      }
      iSeg.Origin = result9;
      if (!iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
        return;
      if (state == NetworkState.Client)
      {
        SpawnMineRequestMessage iMessage;
        iMessage.OwnerHandle = iOwner.Handle;
        iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
        iMessage.Position = oPos;
        iMessage.Direction = result10;
        iMessage.Scale = iScale;
        iMessage.Spell = iSpell;
        iMessage.Damage = oDamages;
        iMessage.Range = iRange;
        iMessage.NextDir = result12;
        iMessage.NextRotation = result8;
        iMessage.Distance = iDistanceBetweenMines;
        NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref iMessage, 0);
      }
      else
      {
        SpellMine instance = SpellMine.GetInstance();
        if (state == NetworkState.Server)
        {
          SpawnMineMessage iMessage;
          iMessage.Handle = instance.Handle;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
          iMessage.Position = oPos;
          iMessage.Direction = result10;
          iMessage.Scale = iScale;
          iMessage.Spell = iSpell;
          iMessage.Damage = oDamages;
          NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref iMessage);
        }
        instance.Initialize(iOwner, oPos, result10, iScale, iRange, result12, result8, iDistanceBetweenMines, ref iSpell, ref oDamages, oAnimatedLevelPart);
        iOwner.PlayState.EntityManager.AddEntity((Entity) instance);
      }
    }
    else
    {
      NetworkState state = NetworkManager.Instance.State;
      if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
      {
        float scaleFactor = iSpell[Elements.Shield] + 3f;
        float num5 = scaleFactor * 6.28318548f;
        float num6 = num5 * (float) (1.0 / ((double) Barrier.GetRadius((iSpell.Element & Elements.PhysicalElements) != Elements.None) * 2.0));
        int num7 = (int) num6;
        float iScale = (float) (1.0 + ((double) num6 - (double) num7) / (double) num7);
        float num8 = 6.28318548f / (float) num7;
        Vector3 oPos = iOwner.Position;
        Vector3 result13 = iOwner.Direction;
        Vector3.Multiply(ref result13, scaleFactor, out result13);
        Quaternion result14;
        Quaternion.CreateFromYawPitchRoll(num8 * 0.5f, 0.0f, 0.0f, out result14);
        Vector3 result15;
        Vector3.Transform(ref result13, ref result14, out result15);
        Vector3 result16;
        Vector3.Normalize(ref result15, out result16);
        Vector3.Add(ref oPos, ref result15, out result15);
        Quaternion.CreateFromYawPitchRoll(num8 * 1f, 0.0f, 0.0f, out result14);
        Quaternion result17;
        Quaternion.CreateFromYawPitchRoll(num8 * -0.5f, 0.0f, 0.0f, out result17);
        Vector3 result18;
        Vector3.Transform(ref result13, ref result17, out result18);
        Vector3 result19;
        Vector3.Normalize(ref result18, out result19);
        Vector3.Add(ref oPos, ref result18, out result18);
        Quaternion.CreateFromYawPitchRoll(num8 * -1f, 0.0f, 0.0f, out result17);
        Vector3 result20;
        Vector3.Subtract(ref result15, ref result18, out result20);
        Vector3 result21;
        Vector3.Subtract(ref result18, ref result15, out result21);
        float iDistanceBetweenBarriers = result20.Length();
        Segment iSeg = new Segment();
        iSeg.Delta.Y = -4f;
        this.mSpell = iSpell;
        DamageCollection5 oDamages;
        this.mSpell.CalculateDamage(SpellType.Shield, CastType.Area, out oDamages);
        iSeg.Origin = result15;
        float num9 = num5 - iDistanceBetweenBarriers;
        Barrier.HitListWithBarriers iHitList = (Barrier.HitListWithBarriers) null;
        if (state != NetworkState.Client)
          iHitList = Barrier.HitListWithBarriers.GetFromCache();
        bool flag = false;
        float iRange = num9 * 0.5f;
        float oFrac;
        Vector3 oNrm;
        AnimatedLevelPart oAnimatedLevelPart;
        if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
        {
          if (state == NetworkState.Client)
          {
            SpawnBarrierRequestMessage iMessage;
            iMessage.OwnerHandle = iOwner.Handle;
            iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
            iMessage.Position = oPos;
            iMessage.Direction = result16;
            iMessage.Scale = iScale;
            iMessage.Spell = iSpell;
            iMessage.Damage = oDamages;
            iMessage.Range = iRange;
            iMessage.NextDir = result20;
            iMessage.NextRotation = result14;
            iMessage.Distance = iDistanceBetweenBarriers;
            NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref iMessage, 0);
          }
          else
          {
            Barrier fromCache = Barrier.GetFromCache(iOwner.PlayState);
            if (state == NetworkState.Server)
            {
              SpawnBarrierMessage iMessage;
              iMessage.Handle = fromCache.Handle;
              iMessage.OwnerHandle = iOwner.Handle;
              iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
              iMessage.Position = oPos;
              iMessage.Direction = result16;
              iMessage.Scale = iScale;
              iMessage.Spell = iSpell;
              iMessage.Damage = oDamages;
              iMessage.HitlistHandle = iHitList.Handle;
              NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref iMessage);
            }
            fromCache.Initialize(iOwner, oPos, result16, iScale, iRange, result20, result14, iDistanceBetweenBarriers, ref iSpell, ref oDamages, iHitList, oAnimatedLevelPart);
            iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache);
            flag = true;
          }
        }
        iSeg.Origin = result18;
        if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
        {
          if (state == NetworkState.Client)
          {
            SpawnBarrierRequestMessage iMessage;
            iMessage.OwnerHandle = iOwner.Handle;
            iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
            iMessage.Position = oPos;
            iMessage.Direction = result19;
            iMessage.Scale = iScale;
            iMessage.Spell = iSpell;
            iMessage.Damage = oDamages;
            iMessage.Range = iRange;
            iMessage.NextDir = result21;
            iMessage.NextRotation = result17;
            iMessage.Distance = iDistanceBetweenBarriers;
            NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref iMessage, 0);
          }
          else
          {
            Barrier fromCache = Barrier.GetFromCache(iOwner.PlayState);
            if (state == NetworkState.Server)
            {
              SpawnBarrierMessage iMessage;
              iMessage.Handle = fromCache.Handle;
              iMessage.OwnerHandle = iOwner.Handle;
              iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
              iMessage.Position = oPos;
              iMessage.Direction = result19;
              iMessage.Scale = iScale;
              iMessage.Spell = iSpell;
              iMessage.Damage = oDamages;
              iMessage.HitlistHandle = iHitList.Handle;
              NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref iMessage);
            }
            fromCache.Initialize(iOwner, oPos, result19, iScale, iRange, result21, result17, iDistanceBetweenBarriers, ref iSpell, ref oDamages, iHitList, oAnimatedLevelPart);
            iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache);
            flag = true;
          }
        }
        if (!flag && iHitList != null)
          iHitList.Destroy();
      }
      float iMagnitude = (float) (0.25 + MagickaMath.Random.NextDouble() * 0.5);
      iOwner.PlayState.Camera.CameraShake(iMagnitude, 0.5f * iSpell[Elements.Shield]);
    }
  }

  public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    Elements elements = iSpell.Element & ~Elements.Shield;
    this.mSpell = iSpell;
    base.CastForce(iSpell, iOwner, iFromStaff);
    this.mMinTTL = 0.5f;
    this.mHasCast = true;
    if (elements == Elements.None)
    {
      NetworkState state = NetworkManager.Instance.State;
      if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
        return;
      float iRadius = 5.5f;
      int iHitpoints = 800;
      Vector3 shieldcolor = Spell.SHIELDCOLOR;
      Vector3 result1 = iOwner.Position;
      Vector3 result2 = iOwner.Direction;
      Vector3.Multiply(ref result2, iRadius - 2f, out result2);
      Vector3.Subtract(ref result1, ref result2, out result1);
      if (NetworkManager.Instance.State == NetworkState.Client)
      {
        SpawnShieldRequestMessage iMessage;
        iMessage.OwnerHandle = iOwner.Handle;
        iMessage.Position = result1;
        iMessage.Radius = iRadius;
        iMessage.Direction = iOwner.Direction;
        iMessage.ShieldType = ShieldType.DISC;
        iMessage.HitPoints = (float) iHitpoints;
        NetworkManager.Instance.Interface.SendMessage<SpawnShieldRequestMessage>(ref iMessage, 0);
      }
      else
      {
        Shield fromCache = Shield.GetFromCache(iOwner.PlayState);
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          SpawnShieldMessage iMessage;
          iMessage.Handle = fromCache.Handle;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.Position = result1;
          iMessage.Radius = iRadius;
          iMessage.Direction = iOwner.Direction;
          iMessage.ShieldType = ShieldType.DISC;
          iMessage.HitPoints = (float) iHitpoints;
          NetworkManager.Instance.Interface.SendMessage<SpawnShieldMessage>(ref iMessage);
        }
        fromCache.Initialize(iOwner, result1, iRadius, iOwner.Direction, ShieldType.DISC, (float) iHitpoints, shieldcolor);
        iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache);
      }
    }
    else if ((elements & Elements.PhysicalElements) == Elements.None && (elements & Elements.Arcane) == Elements.Arcane | (elements & Elements.Life) == Elements.Life)
    {
      NetworkState state = NetworkManager.Instance.State;
      if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
        return;
      float scaleFactor = 4f;
      float num1 = scaleFactor * 6.28318548f;
      float num2 = num1 * 0.5f;
      int num3 = (int) num2;
      float iScale = (float) (1.0 + ((double) num2 - (double) num3) / (double) num3);
      float num4 = 6.28318548f / (float) num3;
      Vector3 position = iOwner.Position;
      Vector3 result3 = iOwner.Direction;
      Vector3.Multiply(ref result3, scaleFactor - 3f, out result3);
      Vector3.Subtract(ref position, ref result3, out position);
      Vector3 result4 = iOwner.Direction;
      Vector3.Multiply(ref result4, scaleFactor, out result4);
      Quaternion result5;
      Quaternion.CreateFromYawPitchRoll(num4 * 0.5f, 0.0f, 0.0f, out result5);
      Vector3 result6;
      Vector3.Transform(ref result4, ref result5, out result6);
      Vector3 result7;
      Vector3.Normalize(ref result6, out result7);
      Vector3.Add(ref position, ref result6, out result6);
      Quaternion.CreateFromYawPitchRoll(num4 * 1f, 0.0f, 0.0f, out result5);
      Quaternion result8;
      Quaternion.CreateFromYawPitchRoll(num4 * -0.5f, 0.0f, 0.0f, out result8);
      Vector3 result9;
      Vector3.Transform(ref result4, ref result8, out result9);
      Vector3 result10;
      Vector3.Normalize(ref result9, out result10);
      Vector3.Add(ref position, ref result9, out result9);
      Quaternion.CreateFromYawPitchRoll(num4 * -1f, 0.0f, 0.0f, out result8);
      Vector3 result11;
      Vector3.Subtract(ref result6, ref result9, out result11);
      Vector3 result12;
      Vector3.Subtract(ref result9, ref result6, out result12);
      float iDistanceBetweenMines = result11.Length();
      Segment iSeg = new Segment();
      iSeg.Delta.Y = -4f;
      this.mSpell = iSpell;
      DamageCollection5 oDamages;
      this.mSpell.CalculateDamage(SpellType.Shield, CastType.Force, out oDamages);
      iSeg.Origin = result6;
      float iRange = num1 * 0.125f;
      float oFrac;
      Vector3 oNrm;
      AnimatedLevelPart oAnimatedLevelPart;
      if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out position, out oNrm, out oAnimatedLevelPart, iSeg))
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
        {
          SpawnMineRequestMessage iMessage;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
          iMessage.Position = position;
          iMessage.Direction = result7;
          iMessage.Scale = iScale;
          iMessage.Spell = iSpell;
          iMessage.Damage = oDamages;
          iMessage.Range = iRange;
          iMessage.NextDir = result11;
          iMessage.NextRotation = result5;
          iMessage.Distance = iDistanceBetweenMines;
          NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref iMessage, 0);
        }
        else
        {
          SpellMine instance = SpellMine.GetInstance();
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            SpawnMineMessage iMessage;
            iMessage.Handle = instance.Handle;
            iMessage.OwnerHandle = iOwner.Handle;
            iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
            iMessage.Position = position;
            iMessage.Direction = result7;
            iMessage.Scale = iScale;
            iMessage.Spell = iSpell;
            iMessage.Damage = oDamages;
            NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref iMessage);
          }
          instance.Initialize(iOwner, position, result7, iScale, iRange, result11, result5, iDistanceBetweenMines, ref iSpell, ref oDamages, oAnimatedLevelPart);
          iOwner.PlayState.EntityManager.AddEntity((Entity) instance);
        }
      }
      iSeg.Origin = result9;
      if (!iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out position, out oNrm, iSeg))
        return;
      if (NetworkManager.Instance.State == NetworkState.Client)
      {
        SpawnMineRequestMessage iMessage;
        iMessage.OwnerHandle = iOwner.Handle;
        iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
        iMessage.Position = position;
        iMessage.Direction = result10;
        iMessage.Scale = iScale;
        iMessage.Spell = iSpell;
        iMessage.Damage = oDamages;
        iMessage.Range = iRange;
        iMessage.NextDir = result12;
        iMessage.NextRotation = result8;
        iMessage.Distance = iDistanceBetweenMines;
        NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref iMessage, 0);
      }
      else
      {
        SpellMine instance = SpellMine.GetInstance();
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          SpawnMineMessage iMessage;
          iMessage.Handle = instance.Handle;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
          iMessage.Position = position;
          iMessage.Direction = result10;
          iMessage.Scale = iScale;
          iMessage.Spell = iSpell;
          iMessage.Damage = oDamages;
          NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref iMessage);
        }
        instance.Initialize(iOwner, position, result10, iScale, iRange, result12, result8, iDistanceBetweenMines, ref iSpell, ref oDamages, oAnimatedLevelPart);
        iOwner.PlayState.EntityManager.AddEntity((Entity) instance);
      }
    }
    else
    {
      NetworkState state = NetworkManager.Instance.State;
      if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
        return;
      float scaleFactor = 4f;
      float num5 = scaleFactor * 6.28318548f;
      float num6 = num5 * (float) (1.0 / ((double) Barrier.GetRadius((iSpell.Element & Elements.PhysicalElements) != Elements.None) * 2.0));
      int num7 = (int) num6;
      float iScale = (float) (1.0 + ((double) num6 - (double) num7) / (double) num7);
      float num8 = 6.28318548f / (float) num7;
      Vector3 position = iOwner.Position;
      if ((iSpell.Element & Elements.StatusEffects) == Elements.None && (iSpell.Element & Elements.Steam) == Elements.None && (iSpell.Element & Elements.Lightning) == Elements.None)
      {
        Vector3 result = iOwner.Direction;
        Vector3.Multiply(ref result, scaleFactor - 2f, out result);
        Vector3.Subtract(ref position, ref result, out position);
      }
      Vector3 result13 = iOwner.Direction;
      Vector3.Multiply(ref result13, scaleFactor, out result13);
      Quaternion result14;
      Quaternion.CreateFromYawPitchRoll(num8 * 0.5f, 0.0f, 0.0f, out result14);
      Vector3 result15;
      Vector3.Transform(ref result13, ref result14, out result15);
      Vector3 result16;
      Vector3.Normalize(ref result15, out result16);
      Vector3.Add(ref position, ref result15, out result15);
      Quaternion.CreateFromYawPitchRoll(num8 * 1f, 0.0f, 0.0f, out result14);
      Quaternion result17;
      Quaternion.CreateFromYawPitchRoll(num8 * -0.5f, 0.0f, 0.0f, out result17);
      Vector3 result18;
      Vector3.Transform(ref result13, ref result17, out result18);
      Vector3 result19;
      Vector3.Normalize(ref result18, out result19);
      Vector3.Add(ref position, ref result18, out result18);
      Quaternion.CreateFromYawPitchRoll(num8 * -1f, 0.0f, 0.0f, out result17);
      Vector3 result20;
      Vector3.Subtract(ref result15, ref result18, out result20);
      Vector3 result21;
      Vector3.Subtract(ref result18, ref result15, out result21);
      float iDistanceBetweenBarriers = result20.Length();
      Segment iSeg = new Segment();
      iSeg.Delta.Y = -4f;
      this.mSpell = iSpell;
      DamageCollection5 oDamages;
      this.mSpell.CalculateDamage(SpellType.Shield, CastType.Force, out oDamages);
      iSeg.Origin = result15;
      ++iSeg.Origin.Y;
      Barrier.HitListWithBarriers fromCache1 = Barrier.HitListWithBarriers.GetFromCache();
      bool flag = false;
      oDamages.MultiplyMagnitude(0.25f);
      float iRange = num5 * 0.125f;
      float oFrac;
      Vector3 oNrm;
      AnimatedLevelPart oAnimatedLevelPart;
      if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out position, out oNrm, out oAnimatedLevelPart, iSeg))
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
        {
          SpawnBarrierRequestMessage iMessage;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
          iMessage.Position = position;
          iMessage.Direction = result16;
          iMessage.Scale = iScale;
          iMessage.Spell = iSpell;
          iMessage.Damage = oDamages;
          iMessage.Range = iRange;
          iMessage.NextDir = result20;
          iMessage.NextRotation = result14;
          iMessage.Distance = iDistanceBetweenBarriers;
          NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref iMessage, 0);
        }
        else
        {
          Barrier fromCache2 = Barrier.GetFromCache(iOwner.PlayState);
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            SpawnBarrierMessage iMessage;
            iMessage.Handle = fromCache2.Handle;
            iMessage.OwnerHandle = iOwner.Handle;
            iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
            iMessage.Position = position;
            iMessage.Direction = result16;
            iMessage.Scale = iScale;
            iMessage.Spell = iSpell;
            iMessage.Damage = oDamages;
            iMessage.HitlistHandle = fromCache1.Handle;
            NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref iMessage);
          }
          flag = true;
          fromCache2.Initialize(iOwner, position, result16, iScale, iRange, result20, result14, iDistanceBetweenBarriers, ref iSpell, ref oDamages, fromCache1, oAnimatedLevelPart);
          iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache2);
        }
      }
      iSeg.Origin = result18;
      if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out position, out oNrm, out oAnimatedLevelPart, iSeg))
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
        {
          SpawnBarrierRequestMessage iMessage;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
          iMessage.Position = position;
          iMessage.Direction = result19;
          iMessage.Scale = iScale;
          iMessage.Spell = iSpell;
          iMessage.Damage = oDamages;
          iMessage.Range = iRange;
          iMessage.NextDir = result21;
          iMessage.NextRotation = result17;
          iMessage.Distance = iDistanceBetweenBarriers;
          NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref iMessage, 0);
        }
        else
        {
          Barrier fromCache3 = Barrier.GetFromCache(iOwner.PlayState);
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            SpawnBarrierMessage iMessage;
            iMessage.Handle = fromCache3.Handle;
            iMessage.OwnerHandle = iOwner.Handle;
            iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
            iMessage.Position = position;
            iMessage.Direction = result19;
            iMessage.Scale = iScale;
            iMessage.Spell = iSpell;
            iMessage.Damage = oDamages;
            iMessage.HitlistHandle = fromCache1.Handle;
            NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref iMessage);
          }
          flag = true;
          fromCache3.Initialize(iOwner, position, result19, iScale, num5 * 0.125f, result21, result17, iDistanceBetweenBarriers, ref iSpell, ref oDamages, fromCache1, oAnimatedLevelPart);
          iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache3);
        }
      }
      if (!flag)
        fromCache1.Destroy();
      float iMagnitude = (float) (0.25 + MagickaMath.Random.NextDouble() * 0.5);
      iOwner.PlayState.Camera.CameraShake(iMagnitude, 0.5f * iSpell[Elements.Shield]);
    }
  }

  public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    this.mSpell = iSpell;
    this.mHasCast = false;
    base.CastWeapon(iSpell, iOwner, iFromStaff);
  }

  internal override void AnimationEnd(ISpellCaster iOwner)
  {
    base.AnimationEnd(iOwner);
    Vector3 result1 = iOwner.Position;
    Vector3 result2 = iOwner.Direction;
    float radius = iOwner.Radius;
    Vector3.Multiply(ref result2, radius, out result2);
    Vector3.Add(ref result2, ref result1, out result1);
    Vector3 result3 = iOwner.Direction;
    if (this.mCastType != CastType.Weapon)
      return;
    Elements elements = this.mSpell.Element & ~Elements.Shield;
    this.mHasCast = true;
    if (elements == Elements.None)
    {
      NetworkState state = NetworkManager.Instance.State;
      if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
        return;
      float iRadius = 5.5f;
      int iHitpoints = 800;
      Vector3 shieldcolor = Spell.SHIELDCOLOR;
      Vector3.Multiply(ref result3, iRadius + 2f, out result3);
      Vector3.Add(ref result1, ref result3, out result1);
      if (NetworkManager.Instance.State == NetworkState.Client)
      {
        SpawnShieldRequestMessage iMessage;
        iMessage.OwnerHandle = iOwner.Handle;
        iMessage.Position = result1;
        iMessage.Radius = iRadius;
        iMessage.Direction = iOwner.Direction;
        iMessage.ShieldType = ShieldType.WALL;
        iMessage.HitPoints = (float) iHitpoints;
        NetworkManager.Instance.Interface.SendMessage<SpawnShieldRequestMessage>(ref iMessage, 0);
      }
      else
      {
        Shield fromCache = Shield.GetFromCache(iOwner.PlayState);
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          SpawnShieldMessage iMessage;
          iMessage.Handle = fromCache.Handle;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.Position = result1;
          iMessage.Radius = iRadius;
          iMessage.Direction = iOwner.Direction;
          iMessage.ShieldType = ShieldType.WALL;
          iMessage.HitPoints = (float) iHitpoints;
          NetworkManager.Instance.Interface.SendMessage<SpawnShieldMessage>(ref iMessage);
        }
        fromCache.Initialize(iOwner, result1, iRadius, iOwner.Direction, ShieldType.WALL, (float) iHitpoints, shieldcolor);
        iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache);
      }
    }
    else if ((elements & Elements.PhysicalElements) == Elements.None && (elements & Elements.Arcane) == Elements.Arcane | (elements & Elements.Life) == Elements.Life)
    {
      NetworkState state = NetworkManager.Instance.State;
      if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
        return;
      int num1 = 2 + (int) Math.Max(this.mSpell.TotalMagnitude() * 0.5f, 1f);
      float num2 = 2.5f;
      float iScale = 1f;
      float iRange = num2 * (float) num1;
      float iDistanceBetweenMines = num2;
      Vector3 vector3 = result1 + result3 * num2;
      DamageCollection5 oDamages;
      this.mSpell.CalculateDamage(SpellType.Shield, CastType.Weapon, out oDamages);
      Segment iSeg = new Segment();
      iSeg.Delta.Y = -4f;
      iSeg.Origin = vector3;
      ++iSeg.Origin.Y;
      Vector3 oPos;
      AnimatedLevelPart oAnimatedLevelPart;
      if (!iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, out oAnimatedLevelPart, iSeg))
        return;
      if (NetworkManager.Instance.State == NetworkState.Client)
      {
        SpawnMineRequestMessage iMessage;
        iMessage.OwnerHandle = iOwner.Handle;
        iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
        iMessage.Position = oPos;
        iMessage.Direction = new Vector3(result3.Z, 0.0f, -result3.X);
        iMessage.Scale = iScale;
        iMessage.Spell = this.mSpell;
        iMessage.Damage = oDamages;
        iMessage.Range = iRange;
        iMessage.NextDir = result3 * iDistanceBetweenMines;
        iMessage.NextRotation = Quaternion.Identity;
        iMessage.Distance = iDistanceBetweenMines;
        NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref iMessage, 0);
      }
      else
      {
        SpellMine instance = SpellMine.GetInstance();
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          SpawnMineMessage iMessage;
          iMessage.Handle = instance.Handle;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
          iMessage.Position = oPos;
          iMessage.Direction = new Vector3(result3.Z, 0.0f, -result3.X);
          iMessage.Scale = iScale;
          iMessage.Spell = this.mSpell;
          iMessage.Damage = oDamages;
          NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref iMessage);
        }
        instance.Initialize(iOwner, oPos, new Vector3(result3.Z, 0.0f, -result3.X), iScale, iRange, result3 * iDistanceBetweenMines, Quaternion.Identity, iDistanceBetweenMines, ref this.mSpell, ref oDamages, oAnimatedLevelPart);
        iOwner.PlayState.EntityManager.AddEntity((Entity) instance);
      }
    }
    else
    {
      NetworkState state = NetworkManager.Instance.State;
      if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
        return;
      int num3 = 2 + (int) Math.Max(this.mSpell.TotalMagnitude() * 0.5f, 1f);
      float num4 = 2.5f;
      float iScale = 1f;
      float iRange = num4 * (float) num3;
      float iDistanceBetweenBarriers = num4;
      Vector3 vector3 = result1 + result3 * num4;
      DamageCollection5 oDamages;
      this.mSpell.CalculateDamage(SpellType.Shield, CastType.Weapon, out oDamages);
      Segment iSeg = new Segment();
      iSeg.Delta.Y = -4f;
      iSeg.Origin = vector3;
      iSeg.Origin.Y += 2f;
      Vector3 oPos;
      AnimatedLevelPart oAnimatedLevelPart;
      if (!iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, out oAnimatedLevelPart, iSeg))
        return;
      if (NetworkManager.Instance.State == NetworkState.Client)
      {
        SpawnBarrierRequestMessage iMessage;
        iMessage.OwnerHandle = iOwner.Handle;
        iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
        iMessage.Position = oPos;
        iMessage.Direction = new Vector3(result3.Z, 0.0f, -result3.X);
        iMessage.Scale = iScale;
        iMessage.Spell = this.mSpell;
        iMessage.Damage = oDamages;
        iMessage.Range = iRange;
        iMessage.NextDir = result3 * iDistanceBetweenBarriers;
        iMessage.NextRotation = Quaternion.Identity;
        iMessage.Distance = iDistanceBetweenBarriers;
        NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref iMessage, 0);
      }
      else
      {
        Barrier fromCache1 = Barrier.GetFromCache(iOwner.PlayState);
        Barrier.HitListWithBarriers fromCache2 = Barrier.HitListWithBarriers.GetFromCache();
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          SpawnBarrierMessage iMessage;
          iMessage.Handle = fromCache1.Handle;
          iMessage.OwnerHandle = iOwner.Handle;
          iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
          iMessage.Position = oPos;
          iMessage.Direction = new Vector3(result3.Z, 0.0f, -result3.X);
          iMessage.Scale = iScale;
          iMessage.Spell = this.mSpell;
          iMessage.Damage = oDamages;
          iMessage.HitlistHandle = fromCache2.Handle;
          NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref iMessage);
        }
        fromCache1.Initialize(iOwner, oPos, new Vector3(result3.Z, 0.0f, -result3.X), iScale, iRange, result3 * iDistanceBetweenBarriers, Quaternion.Identity, iDistanceBetweenBarriers, ref this.mSpell, ref oDamages, fromCache2, oAnimatedLevelPart);
        iOwner.PlayState.EntityManager.AddEntity((Entity) fromCache1);
      }
    }
  }

  public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    this.mSpell = iSpell;
    Elements elements = iSpell.Element & ~Elements.Shield;
    base.CastSelf(iSpell, iOwner, iFromStaff);
    this.mMinTTL = 0.5f;
    this.mHasCast = true;
    if (elements == Elements.None)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Action = ActionType.SelfShield,
          Handle = iOwner.Handle,
          Param0I = (int) iSpell.Element,
          Param1F = iSpell[Elements.Earth],
          Param2F = iSpell[Elements.Ice]
        });
      iOwner.AddSelfShield(iSpell);
    }
    else
    {
      bool flag = false;
      if ((elements & Elements.PhysicalElements) != Elements.None)
      {
        if (NetworkManager.Instance.State != NetworkState.Client)
        {
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
            {
              Action = ActionType.SelfShield,
              Handle = iOwner.Handle,
              Param0I = (int) iSpell.Element,
              Param1F = iSpell[Elements.Earth],
              Param2F = iSpell[Elements.Ice]
            });
          iOwner.AddSelfShield(iSpell);
        }
        flag = true;
      }
      if ((elements & (Elements.InstantNonPhysical | Elements.Water | Elements.Cold | Elements.Fire)) == Elements.None || !(iOwner is Character))
        return;
      float val2 = 1f;
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements iElement = Spell.ElementFromIndex(iIndex);
        if ((iElement & elements) == iElement)
          val2 = Math.Max(this.mSpell[iElement], val2);
      }
      float iTTL = (float) (15.0 + 5.0 * (double) val2);
      float iRadius = (float) (0.0 + 1.5 * (double) val2);
      Resistance iResistance = new Resistance();
      iResistance.Multiplier = 0.0f;
      if (!flag)
        (iOwner as Character).RemoveSelfShield();
      (iOwner as Character).ClearAura();
      if ((elements & Elements.Lightning) != Elements.None)
      {
        iResistance.ResistanceAgainst = Elements.Lightning;
        BuffStorage iBuff = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.LIGHTNINGCOLOR);
        int[] iTargetType = new int[1];
        AuraStorage iAura = new AuraStorage(new AuraBuff(iBuff), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.LIGHTNINGCOLOR, iTargetType, Factions.NONE);
        (iOwner as Character).AddAura(ref iAura, true);
      }
      if ((elements & Elements.Fire) != Elements.None)
      {
        iResistance.ResistanceAgainst = Elements.Fire;
        BuffStorage iBuff = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.FIRECOLOR);
        int[] iTargetType = new int[1];
        AuraStorage iAura = new AuraStorage(new AuraBuff(iBuff), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.FIRECOLOR, iTargetType, Factions.NONE);
        (iOwner as Character).AddAura(ref iAura, true);
      }
      if ((elements & Elements.Cold) != Elements.None)
      {
        iResistance.ResistanceAgainst = Elements.Cold;
        BuffStorage iBuff = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.COLDCOLOR);
        int[] iTargetType = new int[1];
        AuraStorage iAura = new AuraStorage(new AuraBuff(iBuff), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.COLDCOLOR, iTargetType, Factions.NONE);
        (iOwner as Character).AddAura(ref iAura, true);
      }
      if ((elements & Elements.Steam) != Elements.None)
      {
        iResistance.ResistanceAgainst = Elements.Steam;
        BuffStorage iBuff = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.STEAMCOLOR);
        int[] iTargetType = new int[1];
        AuraStorage iAura = new AuraStorage(new AuraBuff(iBuff), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.STEAMCOLOR, iTargetType, Factions.NONE);
        (iOwner as Character).AddAura(ref iAura, true);
      }
      if ((elements & Elements.Water) != Elements.None)
      {
        iResistance.ResistanceAgainst = Elements.Water;
        BuffStorage iBuff = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.WATERCOLOR);
        int[] iTargetType = new int[1];
        AuraStorage iAura = new AuraStorage(new AuraBuff(iBuff), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.WATERCOLOR, iTargetType, Factions.NONE);
        (iOwner as Character).AddAura(ref iAura, true);
      }
      if ((elements & Elements.Life) != Elements.None)
      {
        iResistance.ResistanceAgainst = Elements.Life;
        BuffStorage iBuff = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.LIFECOLOR);
        int[] iTargetType = new int[1];
        AuraStorage iAura = new AuraStorage(new AuraBuff(iBuff), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.LIFECOLOR, iTargetType, Factions.NONE);
        (iOwner as Character).AddAura(ref iAura, true);
      }
      if ((elements & Elements.Arcane) == Elements.None)
        return;
      iResistance.ResistanceAgainst = Elements.Arcane;
      BuffStorage iBuff1 = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.ARCANECOLOR);
      int[] iTargetType1 = new int[1];
      AuraStorage iAura1 = new AuraStorage(new AuraBuff(iBuff1), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.ARCANECOLOR, iTargetType1, Factions.NONE);
      (iOwner as Character).AddAura(ref iAura1, true);
    }
  }

  public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
  {
    oTurnSpeed = 0.1f;
    this.mMinTTL -= iDeltaTime;
    if (!this.Active || !iOwner.AnimationController.HasFinished && (double) this.mMinTTL >= 0.0)
      return false;
    this.DeInitialize(iOwner);
    return false;
  }

  public override void DeInitialize(ISpellCaster iOwner)
  {
    if (!this.Active)
      return;
    if (!this.mHasCast)
      this.AnimationEnd(iOwner);
    ShieldSpell.ReturnToCache(this);
    this.Active = false;
  }
}
