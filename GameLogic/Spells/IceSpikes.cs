// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.IceSpikes
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Spells;

internal class IceSpikes : IAbilityEffect
{
  private static List<IceSpikes> sCache;
  private static Random sRandom = new Random();
  private static SkinnedModel[] sModels;
  private static AnimationClip[] sAnimations;
  private IceSpikes.RenderData[] mRenderData;
  private bool[] mDraw;
  private Matrix[] mRoots;
  private AnimationController[] mControllers;
  private BoundingSphere mBoundingSphere;
  private bool mDead;
  private Scene mScene;

  public static IceSpikes GetInstance()
  {
    if (IceSpikes.sCache.Count <= 0)
      return new IceSpikes();
    IceSpikes instance = IceSpikes.sCache[IceSpikes.sCache.Count - 1];
    IceSpikes.sCache.RemoveAt(IceSpikes.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    IceSpikes.sCache = new List<IceSpikes>(iNr);
    for (int index = 0; index < iNr; ++index)
      IceSpikes.sCache.Add(new IceSpikes());
  }

  private IceSpikes()
  {
    if (IceSpikes.sModels == null)
    {
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      IceSpikes.sModels = new SkinnedModel[1];
      lock (graphicsDevice)
        IceSpikes.sModels[0] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/IceBarrier0_mesh");
      SkinnedModel skinnedModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/IceBarrier_animation");
      IceSpikes.sAnimations = new AnimationClip[skinnedModel.AnimationClips.Count];
      int num = 0;
      foreach (AnimationClip animationClip in skinnedModel.AnimationClips.Values)
        IceSpikes.sAnimations[num++] = animationClip;
    }
    this.mDraw = new bool[18];
    this.mRoots = new Matrix[18];
    this.mControllers = new AnimationController[18];
    for (int index = 0; index < this.mControllers.Length; ++index)
    {
      this.mControllers[index] = new AnimationController();
      this.mControllers[index].Skeleton = IceSpikes.sModels[0].SkeletonBones;
    }
    this.mBoundingSphere = IceSpikes.sModels[0].Model.Meshes[0].BoundingSphere;
    this.mRenderData = new IceSpikes.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      IceSpikes.RenderData renderData = new IceSpikes.RenderData(this.mControllers.Length);
      this.mRenderData[index] = renderData;
      renderData.DoDraw = this.mDraw;
    }
  }

  public bool Initialize(PlayState iPlayState, ref Vector3 iCenter, float iRadius)
  {
    this.mDead = false;
    this.mScene = iPlayState.Scene;
    float iScale = iRadius / 4.332f;
    bool flag = false;
    Segment iSeg = new Segment();
    iSeg.Delta.Y = -3f;
    int index1 = 0;
    float num = 2.61799383f;
    Vector3 vector3 = new Vector3();
    float oFrac;
    Vector3 oPos;
    Vector3 oNrm;
    for (int index2 = 0; index2 < 12; ++index2)
    {
      vector3.X = (float) (Math.Sin((double) num) * 2.0 * 1.6660000085830688) * iScale;
      vector3.Z = (float) (Math.Cos((double) num) * 2.0 * 1.6660000085830688) * iScale;
      Vector3.Add(ref vector3, ref iCenter, out iSeg.Origin);
      iSeg.Origin.Y += 0.5f;
      this.mDraw[index1] = iPlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out oPos, out oNrm, iSeg);
      if (this.mDraw[index1])
      {
        flag = true;
        Matrix.CreateRotationY((float) IceSpikes.sRandom.NextDouble() * 6.28318548f, out this.mRoots[index1]);
        MagickaMath.UniformMatrixScale(ref this.mRoots[index1], iScale);
        this.mRoots[index1].Translation = oPos;
        this.mControllers[index1].PlaybackMode = PlaybackMode.Forward;
        this.mControllers[index1].ClipSpeed = 2f;
        this.mControllers[index1].PlayClip(IceSpikes.sAnimations[IceSpikes.sRandom.Next(IceSpikes.sAnimations.Length)], false);
      }
      ++index1;
      num += 0.5235988f;
    }
    for (int index3 = 0; index3 < 6; ++index3)
    {
      vector3.X = (float) (Math.Sin((double) num) * 1.0 * 1.6660000085830688) * iScale;
      vector3.Z = (float) (Math.Cos((double) num) * 1.0 * 1.6660000085830688) * iScale;
      Vector3.Add(ref vector3, ref iCenter, out iSeg.Origin);
      iSeg.Origin.Y += 1.5f;
      this.mDraw[index1] = iPlayState.Level.CurrentScene.SegmentIntersect(out oFrac, out oPos, out oNrm, iSeg);
      if (this.mDraw[index1])
      {
        flag = true;
        Matrix.CreateRotationY((float) IceSpikes.sRandom.NextDouble() * 6.28318548f, out this.mRoots[index1]);
        MagickaMath.UniformMatrixScale(ref this.mRoots[index1], iScale);
        this.mRoots[index1].Translation = oPos;
        this.mControllers[index1].PlaybackMode = PlaybackMode.Forward;
        this.mControllers[index1].ClipSpeed = 2f;
        this.mControllers[index1].PlayClip(IceSpikes.sAnimations[IceSpikes.sRandom.Next(IceSpikes.sAnimations.Length)], false);
      }
      ++index1;
      num += 1.04719758f;
    }
    ModelMesh mesh = IceSpikes.sModels[0].Model.Meshes[0];
    ModelMeshPart meshPart = mesh.MeshParts[0];
    SkinnedModelDeferredBasicMaterial oMaterial;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(meshPart.Effect as SkinnedModelBasicEffect, out oMaterial);
    for (int index4 = 0; index4 < 3; ++index4)
    {
      IceSpikes.RenderData renderData = this.mRenderData[index4];
      renderData.mBoundingSphere.Center = iCenter;
      renderData.mBoundingSphere.Radius = 3.332f;
      renderData.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, 0, 3, 4);
      renderData.mMaterial = oMaterial;
    }
    if (!flag)
      return false;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead
  {
    get
    {
      if (!this.mDead)
        return false;
      for (int index = 0; index < this.mControllers.Length; ++index)
      {
        if (this.mDraw[index] & !this.mControllers[index].HasFinished)
          return false;
      }
      return true;
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    IceSpikes.RenderData iObject = this.mRenderData[(int) iDataChannel];
    for (int index = 0; index < this.mControllers.Length; ++index)
    {
      if (this.mDraw[index])
      {
        AnimationController mController = this.mControllers[index];
        if (mController.HasFinished && mController.PlaybackMode == PlaybackMode.Forward)
        {
          mController.PlaybackMode = PlaybackMode.Backward;
          mController.StartClip(mController.AnimationClip, false);
          this.mDead = true;
        }
        mController.Update(iDeltaTime, ref this.mRoots[index], true);
        this.mBoundingSphere.Center = this.mRoots[index].Translation;
        iObject.mBoundingSphere = this.mBoundingSphere;
        Array.Copy((Array) mController.SkinnedBoneTransforms, (Array) iObject.Bones[index], mController.Skeleton.Count);
      }
    }
    this.mScene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
  }

  public void OnRemove() => this.mScene = (Scene) null;

  private class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
  {
    public bool[] DoDraw;
    public Matrix[][] Bones;

    public RenderData(int iControllerCount)
    {
      this.Bones = new Matrix[iControllerCount][];
      for (int index = 0; index < iControllerCount; ++index)
        this.Bones[index] = new Matrix[80 /*0x50*/];
    }

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      for (int index = 0; index < this.DoDraw.Length; ++index)
      {
        if (this.DoDraw[index])
        {
          (iEffect as SkinnedModelDeferredEffect).Bones = this.Bones[index];
          base.Draw(iEffect, iViewFrustum);
        }
      }
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      for (int index = 0; index < this.DoDraw.Length; ++index)
      {
        if (this.DoDraw[index])
        {
          (iEffect as SkinnedModelDeferredEffect).Bones = this.Bones[index];
          base.DrawShadow(iEffect, iViewFrustum);
        }
      }
    }
  }
}
