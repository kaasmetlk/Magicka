// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.IconRenderer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

public class IconRenderer
{
  private static Texture2D mHUDTexture;
  public static readonly int sMaxMagickIconCount = 5;
  private static readonly Vector2[] sIconTargets = new Vector2[5]
  {
    new Vector2(-50f, 44f),
    new Vector2(-25f, 44f),
    new Vector2(0.0f, 44f),
    new Vector2(25f, 44f),
    new Vector2(50f, 44f)
  };
  private MagickType mDisplayedMagick;
  private bool mMagickDirty;
  private float mMagickTextAlpha;
  private Elements[] mTomeMagickElements;
  private IconRenderer.RenderData[] mRenderData;
  private VertexBuffer mVertexBuffer;
  private VertexDeclaration mVertexDeclaration;
  private GUIHardwareInstancingEffect mInstancingEffect;
  private GUIBasicEffect mBasicEffect;
  private int mMaxIconCount;
  private int mCombineIconCount;
  private int mIconCount;
  private IconRenderer.Icon[] mCombineIcons;
  private IconRenderer.Icon[] mIcons;
  private Vector2 mIconSizeUV;
  private int mMaxIcons;
  private PlayState mPlayState;
  private WeakReference mPlayer;

  public IconRenderer(Player iPlayer, PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mPlayer = new WeakReference((object) iPlayer);
    this.mMaxIcons = 25;
    this.mMaxIconCount = 5;
    lock (Magicka.Game.Instance.GraphicsDevice)
      IconRenderer.mHUDTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Hud/Hud");
    IconRenderer.VertexPositionTextureIndex[] data = new IconRenderer.VertexPositionTextureIndex[6 + 6 * this.mMaxIcons + 6 * IconRenderer.sMaxMagickIconCount];
    IconRenderer.VertexPositionTextureIndex positionTextureIndex;
    positionTextureIndex.Index = 0.0f;
    Vector2 vector2_1 = new Vector2(8f, 16f);
    Vector2 vector2_2 = new Vector2(vector2_1.X / (float) IconRenderer.mHUDTexture.Width, vector2_1.Y / (float) IconRenderer.mHUDTexture.Height);
    Vector2 vector2_3 = new Vector2(248f / (float) IconRenderer.mHUDTexture.Width, 8f / (float) IconRenderer.mHUDTexture.Height);
    positionTextureIndex.Position = new Vector2(vector2_1.X * -0.5f, vector2_1.Y * -0.5f);
    positionTextureIndex.TexCoord = new Vector2(vector2_3.X, vector2_3.Y);
    data[0] = positionTextureIndex;
    positionTextureIndex.Position = new Vector2(vector2_1.X * 0.5f, vector2_1.Y * -0.5f);
    positionTextureIndex.TexCoord = new Vector2(vector2_2.X + vector2_3.X, vector2_3.Y);
    data[1] = positionTextureIndex;
    positionTextureIndex.Position = new Vector2(vector2_1.X * -0.5f, vector2_1.Y * 0.5f);
    positionTextureIndex.TexCoord = new Vector2(vector2_3.X, vector2_2.Y + vector2_3.Y);
    data[2] = positionTextureIndex;
    positionTextureIndex.Position = new Vector2(vector2_1.X * 0.5f, vector2_1.Y * -0.5f);
    positionTextureIndex.TexCoord = new Vector2(vector2_2.X + vector2_3.X, vector2_3.Y);
    data[3] = positionTextureIndex;
    positionTextureIndex.Position = new Vector2(vector2_1.X * 0.5f, vector2_1.Y * 0.5f);
    positionTextureIndex.TexCoord = new Vector2(vector2_2.X + vector2_3.X, vector2_2.Y + vector2_3.Y);
    data[4] = positionTextureIndex;
    positionTextureIndex.Position = new Vector2(vector2_1.X * -0.5f, vector2_1.Y * 0.5f);
    positionTextureIndex.TexCoord = new Vector2(vector2_3.X, vector2_2.Y + vector2_3.Y);
    data[5] = positionTextureIndex;
    vector2_3 = new Vector2(0.0f, 156f / (float) IconRenderer.mHUDTexture.Height);
    this.mIconSizeUV = new Vector2(50f / (float) IconRenderer.mHUDTexture.Width, 49f / (float) IconRenderer.mHUDTexture.Height);
    for (int index = 0; index < this.mMaxIcons + IconRenderer.sMaxMagickIconCount; ++index)
    {
      positionTextureIndex.Index = (float) index;
      positionTextureIndex.Position = new Vector2(-12.5f, -12.5f);
      positionTextureIndex.TexCoord = new Vector2(0.0f, vector2_3.Y);
      data[6 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector2(12.5f, -12.5f);
      positionTextureIndex.TexCoord = new Vector2(this.mIconSizeUV.X, vector2_3.Y);
      data[7 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector2(-12.5f, 12.5f);
      positionTextureIndex.TexCoord = new Vector2(0.0f, vector2_3.Y + this.mIconSizeUV.Y);
      data[8 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector2(12.5f, -12.5f);
      positionTextureIndex.TexCoord = new Vector2(this.mIconSizeUV.X, vector2_3.Y);
      data[9 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector2(12.5f, 12.5f);
      positionTextureIndex.TexCoord = new Vector2(this.mIconSizeUV.X, vector2_3.Y + this.mIconSizeUV.Y);
      data[10 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector2(-12.5f, 12.5f);
      positionTextureIndex.TexCoord = new Vector2(0.0f, vector2_3.Y + this.mIconSizeUV.Y);
      data[11 + index * 6] = positionTextureIndex;
    }
    this.mBasicEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    this.mBasicEffect.Texture = (Texture) IconRenderer.mHUDTexture;
    this.mBasicEffect.Color = new Vector4(1f);
    this.mBasicEffect.TextureEnabled = true;
    this.mInstancingEffect = new GUIHardwareInstancingEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
    this.mInstancingEffect.Texture = (Texture) IconRenderer.mHUDTexture;
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mInstancingEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mIcons = new IconRenderer.Icon[this.mMaxIcons + IconRenderer.sMaxMagickIconCount];
    this.mCombineIcons = new IconRenderer.Icon[this.mMaxIcons];
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, (6 + 6 * this.mMaxIcons + 6 * IconRenderer.sMaxMagickIconCount) * 20, BufferUsage.WriteOnly);
      this.mVertexBuffer.Name = "IconRendererVB";
      this.mVertexBuffer.SetData<IconRenderer.VertexPositionTextureIndex>(data);
      this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, IconRenderer.VertexPositionTextureIndex.VertexElements);
    }
    this.mInstancingEffect.SetTechnique(GUIHardwareInstancingEffect.Technique.Sprites);
    this.mRenderData = new IconRenderer.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      IconRenderer.RenderData renderData = new IconRenderer.RenderData(this.mMaxIcons + IconRenderer.sMaxMagickIconCount);
      this.mRenderData[index] = renderData;
      renderData.mVertexDeclaration = this.mVertexDeclaration;
      renderData.mVertexBuffer = this.mVertexBuffer;
      renderData.mInstancingEffect = this.mInstancingEffect;
      renderData.mBasicEffect = this.mBasicEffect;
    }
  }

  public void Initialize(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.TomeMagick = MagickType.None;
  }

  public void SetCapacity(int iCapacity) => this.mMaxIconCount = iCapacity;

  public void AddIcon(Elements iElement, bool iFromSpellWheel)
  {
    if (this.mIconCount > 5 || GlobalSettings.Instance.SpellWheel == SettingOptions.Off)
      return;
    IconRenderer.Icon[] mIcons = this.mIcons;
    IconRenderer.Icon icon = new IconRenderer.Icon();
    icon.Color.X = 1f;
    icon.Color.Y = 1f;
    icon.Color.Z = 1f;
    icon.Color.W = 0.0f;
    int mIconCount1 = this.mIconCount;
    int mIconCount2 = this.mIconCount;
    int index1 = Defines.ElementIndex(iElement);
    for (int index2 = 0; index2 < mIconCount1; ++index2)
    {
      if (mIcons[index2].Removed || mIcons[index2].Index < 0)
        --mIconCount2;
    }
    if (mIconCount2 >= this.mMaxIconCount)
    {
      icon.Index = -1;
    }
    else
    {
      icon.Index = 4;
      for (int index3 = 0; index3 < mIcons.Length; ++index3)
        mIcons[index3].Index = Math.Min(Math.Max(mIcons[index3].Index - 1, 5 - this.mMaxIconCount), 4);
    }
    icon.State = IconRenderer.Icon.AnimationState.Grow | IconRenderer.Icon.AnimationState.Glow;
    icon.Element = iElement;
    icon.TextureOffset.X = (float) (index1 % 5) * this.mIconSizeUV.X;
    icon.TextureOffset.Y = (float) (index1 / 5) * this.mIconSizeUV.Y;
    if (iFromSpellWheel)
    {
      icon.Position.X = (float) Math.Cos((double) Defines.ElementUIRadian[index1]) * 64f;
      icon.Position.Y -= (float) Math.Sin((double) Defines.ElementUIRadian[index1]) * 64f;
    }
    else
      icon.Position = this.mIconCount > 4 ? new Vector2() : IconRenderer.sIconTargets[this.mIconCount];
    mIcons[mIconCount1] = icon;
    ++this.mIconCount;
  }

  public void TransformIconForCombine(
    Elements iResultingElement,
    int iSourceIndex,
    int iTargetIndex)
  {
    if (GlobalSettings.Instance.SpellWheel == SettingOptions.Off)
      return;
    IconRenderer.Icon[] mIcons = this.mIcons;
    for (int index = 0; index <= iSourceIndex; ++index)
    {
      if (index < mIcons.Length && (mIcons[index].Removed || index < iSourceIndex && mIcons[index].Index < 0))
        ++iSourceIndex;
    }
    for (int index = 0; index <= iTargetIndex; ++index)
    {
      if (index < mIcons.Length && (mIcons[index].Removed || index < iSourceIndex && mIcons[index].Index < 0))
        ++iTargetIndex;
    }
    if (iSourceIndex >= this.mIconCount)
      iSourceIndex = this.mIconCount - 1;
    IconRenderer.Icon icon = mIcons[iSourceIndex];
    IconRenderer.Icon.RemoveItem(mIcons, iSourceIndex);
    --this.mIconCount;
    if (icon.Index >= 0)
    {
      for (int index = 0; index < iSourceIndex; ++index)
        mIcons[index].Index = Math.Min(Math.Max(mIcons[index].Index + 1, 5 - this.mMaxIconCount), 4);
    }
    icon.Index = iTargetIndex >= 0 ? iTargetIndex : throw new Exception();
    icon.Element = iResultingElement;
    this.mCombineIcons[this.mCombineIconCount] = icon;
    ++this.mCombineIconCount;
  }

  public void TransformIconForRemoval(int iSourceIndex, int iTargetIndex)
  {
    if (GlobalSettings.Instance.SpellWheel == SettingOptions.Off)
      return;
    IconRenderer.Icon[] mIcons = this.mIcons;
    for (int index = 0; index <= iSourceIndex; ++index)
    {
      if (index < mIcons.Length && (mIcons[index].Removed || index < iSourceIndex && mIcons[index].Index < 0))
        ++iSourceIndex;
    }
    for (int index = 0; index <= iTargetIndex; ++index)
    {
      if (index < mIcons.Length && (mIcons[index].Removed || index < iSourceIndex && mIcons[index].Index < 0))
        ++iTargetIndex;
    }
    if (iSourceIndex >= mIcons.Length)
      return;
    IconRenderer.Icon icon = mIcons[iSourceIndex];
    IconRenderer.Icon.RemoveItem(mIcons, iSourceIndex);
    --this.mIconCount;
    if (icon.Index >= 0)
    {
      for (int index = 0; index < iSourceIndex; ++index)
        mIcons[index].Index = Math.Min(Math.Max(mIcons[index].Index + 1, 5 - this.mMaxIconCount), 4);
    }
    icon.Index = iTargetIndex;
    mIcons[iTargetIndex].Removed = true;
    icon.Element = Elements.None;
    this.mCombineIcons[this.mCombineIconCount] = icon;
    ++this.mCombineIconCount;
  }

  public void Clear()
  {
    IconRenderer.Icon[] mIcons = this.mIcons;
    int mIconCount = this.mIconCount;
    for (int index = 0; index < mIconCount; ++index)
    {
      mIcons[index].State = IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade;
      mIcons[index].Removed = true;
    }
    IconRenderer.Icon[] mCombineIcons = this.mCombineIcons;
    int combineIconCount = this.mCombineIconCount;
    for (int index = 0; index < combineIconCount; ++index)
    {
      mCombineIcons[index].State = IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade;
      mCombineIcons[index].Removed = true;
    }
  }

  public void ClearElements(Elements iElements)
  {
    for (int index = 0; index < this.mIcons.Length; ++index)
    {
      if ((this.mIcons[index].Element & iElements) != Elements.None)
      {
        this.mIcons[index].State = IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade;
        this.mIcons[index].Removed = true;
      }
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Vector2 vector2_1 = new Vector2(1f);
    Vector2 vector2_2 = new Vector2(1.5f);
    Vector2 vector2_3 = new Vector2();
    Vector4 vector4_1 = new Vector4(6f, 6f, 6f, 1f);
    Vector4 vector4_2 = new Vector4(1f);
    Vector4 vector4_3 = new Vector4(1f, 1f, 1f, 0.0f);
    float amount = MathHelper.Clamp(1f - (float) Math.Pow(1.0 / 400.0, (double) iDeltaTime), 0.0f, 1f);
    int mIconCount = this.mIconCount;
    int combineIconCount = this.mCombineIconCount;
    IconRenderer.Icon[] mIcons = this.mIcons;
    IconRenderer.Icon[] mCombineIcons = this.mCombineIcons;
    float result;
    for (int iItem = 0; iItem < combineIconCount; ++iItem)
    {
      IconRenderer.Icon icon = mCombineIcons[iItem];
      if ((icon.State & IconRenderer.Icon.AnimationState.Glow) == IconRenderer.Icon.AnimationState.Glow)
        Vector4.Lerp(ref icon.Color, ref vector4_1, amount, out mCombineIcons[iItem].Color);
      else if ((icon.State & IconRenderer.Icon.AnimationState.Fade) == IconRenderer.Icon.AnimationState.Fade)
      {
        Vector4.Lerp(ref icon.Color, ref vector4_3, amount, out mCombineIcons[iItem].Color);
        Vector4.DistanceSquared(ref mCombineIcons[iItem].Color, ref vector4_3, out result);
        if ((double) result < 0.10000000149011612)
        {
          IconRenderer.Icon.RemoveItem(mCombineIcons, iItem);
          --iItem;
          --combineIconCount;
          this.mCombineIconCount = combineIconCount;
          continue;
        }
      }
      else
        Vector4.Lerp(ref icon.Color, ref vector4_2, amount, out mCombineIcons[iItem].Color);
      if ((icon.State & (IconRenderer.Icon.AnimationState.Grow | IconRenderer.Icon.AnimationState.Glow)) == IconRenderer.Icon.AnimationState.Glow)
      {
        if (icon.Element == Elements.None)
        {
          mIcons[icon.Index].State = IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade;
        }
        else
        {
          mIcons[icon.Index].Element = icon.Element;
          mIcons[icon.Index].TextureOffset.X = (float) (Defines.ElementIndex(icon.Element) % 5) * this.mIconSizeUV.X;
          mIcons[icon.Index].TextureOffset.Y = (float) (Defines.ElementIndex(icon.Element) / 5) * this.mIconSizeUV.Y;
          Vector4.DistanceSquared(ref mCombineIcons[iItem].Color, ref vector4_1, out result);
          if ((double) result < 0.10000000149011612)
            mCombineIcons[iItem].State = IconRenderer.Icon.AnimationState.Fade;
        }
      }
      if ((icon.State & IconRenderer.Icon.AnimationState.Grow) == IconRenderer.Icon.AnimationState.Grow)
      {
        Vector2.Lerp(ref icon.Scale, ref vector2_2, amount, out mCombineIcons[iItem].Scale);
        Vector2.DistanceSquared(ref mCombineIcons[iItem].Scale, ref vector2_2, out result);
        if ((double) result < 0.10000000149011612)
          mCombineIcons[iItem].State &= ~(IconRenderer.Icon.AnimationState.Grow | IconRenderer.Icon.AnimationState.Glow);
      }
      else if ((icon.State & IconRenderer.Icon.AnimationState.Shrink) == IconRenderer.Icon.AnimationState.Shrink)
      {
        Vector2.Lerp(ref icon.Scale, ref vector2_3, amount, out mCombineIcons[iItem].Scale);
        Vector2.DistanceSquared(ref mCombineIcons[iItem].Scale, ref vector2_3, out result);
        if ((double) result < 0.10000000149011612)
        {
          IconRenderer.Icon.RemoveItem(mCombineIcons, iItem);
          --iItem;
          --combineIconCount;
          this.mCombineIconCount = combineIconCount;
        }
      }
      else if (icon.Index < 0)
      {
        mIcons[iItem].State = IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade;
      }
      else
      {
        Vector2 position = mIcons[icon.Index].Position;
        Vector2.Lerp(ref icon.Position, ref position, amount, out mCombineIcons[iItem].Position);
        Vector2.Lerp(ref icon.Scale, ref vector2_1, amount, out mCombineIcons[iItem].Scale);
        if ((mCombineIcons[iItem].State & IconRenderer.Icon.AnimationState.Fade) != IconRenderer.Icon.AnimationState.Fade)
        {
          Vector2.DistanceSquared(ref mCombineIcons[iItem].Position, ref position, out result);
          if ((double) result < 0.75)
            mCombineIcons[iItem].State = icon.Element != Elements.None ? IconRenderer.Icon.AnimationState.Glow : IconRenderer.Icon.AnimationState.Glow | IconRenderer.Icon.AnimationState.Shrink;
        }
      }
    }
    for (int iItem1 = 0; iItem1 < mIconCount; ++iItem1)
    {
      IconRenderer.Icon icon = mIcons[iItem1];
      if ((icon.State & IconRenderer.Icon.AnimationState.Glow) == IconRenderer.Icon.AnimationState.Glow)
        Vector4.Lerp(ref icon.Color, ref vector4_1, amount, out mIcons[iItem1].Color);
      else if ((icon.State & IconRenderer.Icon.AnimationState.Fade) == IconRenderer.Icon.AnimationState.Fade)
        Vector4.Lerp(ref icon.Color, ref vector4_3, amount, out mIcons[iItem1].Color);
      else
        Vector4.Lerp(ref icon.Color, ref vector4_2, amount, out mIcons[iItem1].Color);
      if ((icon.State & IconRenderer.Icon.AnimationState.Grow) == IconRenderer.Icon.AnimationState.Grow)
      {
        Vector2.Lerp(ref icon.Scale, ref vector2_2, amount, out mIcons[iItem1].Scale);
        Vector2.DistanceSquared(ref mIcons[iItem1].Scale, ref vector2_2, out result);
        if ((double) result < 0.10000000149011612)
          mIcons[iItem1].State &= ~(IconRenderer.Icon.AnimationState.Grow | IconRenderer.Icon.AnimationState.Glow);
      }
      else if ((icon.State & IconRenderer.Icon.AnimationState.Shrink) == IconRenderer.Icon.AnimationState.Shrink)
      {
        Vector2.Lerp(ref icon.Scale, ref vector2_3, amount, out mIcons[iItem1].Scale);
        Vector2.DistanceSquared(ref mIcons[iItem1].Scale, ref vector2_3, out result);
        if ((double) result < 0.10000000149011612)
        {
          IconRenderer.Icon.RemoveItem(mIcons, iItem1);
          if (icon.Index >= 0)
          {
            for (int index = 0; index < iItem1; ++index)
              mIcons[index].Index = Math.Min(Math.Max(mIcons[index].Index + 1, 5 - this.mMaxIconCount), 4);
          }
          for (int iItem2 = 0; iItem2 < combineIconCount; ++iItem2)
          {
            int index = mCombineIcons[iItem2].Index;
            if (index == iItem1)
            {
              IconRenderer.Icon.RemoveItem(mCombineIcons, iItem2);
              --iItem2;
              --combineIconCount;
              this.mCombineIconCount = combineIconCount;
            }
            else if (index > iItem1)
              mCombineIcons[iItem2].Index = Math.Min(Math.Max(index + 1, 5 - this.mMaxIconCount), 4);
          }
          --iItem1;
          --mIconCount;
          this.mIconCount = mIconCount;
        }
      }
      else if (icon.Index < 0)
      {
        mIcons[iItem1].State = IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade;
      }
      else
      {
        int index1 = iItem1;
        for (int index2 = 0; index2 < index1; ++index2)
        {
          if (mIcons[index2].Removed)
            --index1;
        }
        if (index1 < IconRenderer.sIconTargets.Length)
          Vector2.Lerp(ref icon.Position, ref IconRenderer.sIconTargets[index1], amount, out mIcons[iItem1].Position);
        Vector2.Lerp(ref icon.Scale, ref vector2_1, amount, out mIcons[iItem1].Scale);
      }
    }
    IconRenderer.RenderData iObject = this.mRenderData[(int) iDataChannel];
    int iTargetStartIndex1 = 0;
    if (this.mIconCount < 0)
      this.mIconCount = 0;
    IconRenderer.Icon.CopyToArrays(this.mIcons, 0, iTargetStartIndex1, this.mIconCount, iObject.mIconPositions, iObject.mScales, iObject.mTextureOffsets, iObject.mColors, iObject.mSaturations);
    int iTargetStartIndex2 = iTargetStartIndex1 + this.mIconCount;
    IconRenderer.Icon.CopyToArrays(this.mCombineIcons, 0, iTargetStartIndex2, this.mCombineIconCount, iObject.mIconPositions, iObject.mScales, iObject.mTextureOffsets, iObject.mColors, iObject.mSaturations);
    int num1 = iTargetStartIndex2 + this.mCombineIconCount;
    Avatar avatar = this.Player.Avatar;
    if (avatar != null)
    {
      iObject.mWorldPosition = avatar.Position;
      iObject.mWorldPosition.Y -= avatar.Capsule.Length * 0.5f + avatar.Capsule.Radius;
    }
    iObject.mCombiningToRender = this.mCombineIconCount;
    iObject.mElementsToRender = num1;
    if (this.mMagickDirty)
    {
      if ((double) this.mMagickTextAlpha > 0.0)
      {
        this.mMagickTextAlpha = Math.Max(0.0f, this.mMagickTextAlpha - iDeltaTime * 4f);
      }
      else
      {
        this.mMagickDirty = false;
        this.mTomeMagickElements = SpellManager.Instance.GetMagickCombo(this.mDisplayedMagick);
        if (this.mTomeMagickElements != null)
        {
          for (int index = 0; index < 3; ++index)
            this.mRenderData[index].SetTomeMagick(this.mDisplayedMagick, this.mTomeMagickElements.Length);
        }
        else
        {
          for (int index = 0; index < 3; ++index)
            this.mRenderData[index].SetTomeMagick(this.mDisplayedMagick, 0);
        }
      }
    }
    else
      this.mMagickTextAlpha = Math.Min(1f, this.mMagickTextAlpha + iDeltaTime * 4f);
    iObject.mMagickAlpha = this.mMagickTextAlpha;
    if (this.mTomeMagickElements != null)
    {
      int mMaxIcons = this.mMaxIcons;
      int index = 0;
      while (mMaxIcons < this.mMaxIcons + this.mTomeMagickElements.Length)
      {
        int num2 = Defines.ElementIndex(this.mTomeMagickElements[index]);
        this.mIcons[mMaxIcons].Element = this.mTomeMagickElements[index];
        this.mIcons[mMaxIcons].Saturation = 0.0f;
        this.mIcons[mMaxIcons].Color = new Vector4(1f, 1f, 1f, 0.5f * this.mMagickTextAlpha);
        this.mIcons[mMaxIcons].Scale = new Vector2(1f, 1f);
        this.mIcons[mMaxIcons].TextureOffset.X = (float) (num2 % 5) * this.mIconSizeUV.X;
        this.mIcons[mMaxIcons].TextureOffset.Y = (float) (num2 / 5) * this.mIconSizeUV.Y;
        this.mIcons[mMaxIcons].Position.X = IconRenderer.sIconTargets[index].X;
        this.mIcons[mMaxIcons].Position.Y = IconRenderer.sIconTargets[index].Y;
        ++mMaxIcons;
        ++index;
      }
      IconRenderer.Icon.CopyToArrays(this.mIcons, this.mMaxIcons, this.mMaxIcons, 5, iObject.mIconPositions, iObject.mScales, iObject.mTextureOffsets, iObject.mColors, iObject.mSaturations);
    }
    if (!KeyboardHUD.Instance.UIEnabled)
      return;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  protected Player Player => this.mPlayer.Target as Player;

  public MagickType TomeMagick
  {
    get => this.mDisplayedMagick;
    set
    {
      if (this.Player.Gamer is NetworkGamer)
        return;
      MagickType iMagick = value;
      while (iMagick != MagickType.None && !SpellManager.Instance.IsMagickAllowed(this.Player, this.mPlayState.GameType, iMagick))
      {
        int num = (int) (iMagick + 1);
        iMagick = num >= 35 ? MagickType.None : (MagickType) num;
        if (iMagick == value)
          iMagick = this.mDisplayedMagick;
      }
      if (iMagick != this.mDisplayedMagick)
      {
        this.mMagickDirty = true;
        TutorialManager.Instance.SetTip(TutorialManager.Tips.MagicksScroll, TutorialManager.Position.Top);
      }
      this.mDisplayedMagick = iMagick;
    }
  }

  private class RenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    private const float mBracketXPos = 62.5f;
    private static readonly BitmapFont sFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
    public GUIBasicEffect mBasicEffect;
    public GUIHardwareInstancingEffect mInstancingEffect;
    public VertexDeclaration mVertexDeclaration;
    public VertexBuffer mVertexBuffer;
    private Vector2 mPosition;
    public Vector3 mWorldPosition;
    public Vector2[] mIconPositions;
    public Vector3[] mScales;
    public Vector2[] mTextureOffsets;
    public Vector4[] mColors;
    public float[] mSaturations;
    public int mCombiningToRender;
    public int mElementsToRender;
    private int mMagickElementsToRender;
    private Text mMagickText;
    public float mMagickAlpha;
    private int mDirtyMagickElementsToRender;

    public RenderData(int iSize)
    {
      this.mIconPositions = new Vector2[iSize];
      this.mScales = new Vector3[iSize];
      this.mTextureOffsets = new Vector2[iSize];
      this.mColors = new Vector4[iSize];
      this.mSaturations = new float[iSize];
      this.mMagickAlpha = 0.0f;
      this.mMagickText = new Text(64 /*0x40*/, IconRenderer.RenderData.sFont, TextAlign.Center, false);
      this.mMagickText.SetText("");
    }

    public void SetTomeMagick(MagickType iType, int iElements)
    {
      this.mDirtyMagickElementsToRender = iElements;
      if (iType == MagickType.None)
        this.mMagickText.SetText("");
      else
        this.mMagickText.SetText(LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int) iType]));
    }

    public void Draw(float iDeltaTime)
    {
      if (this.mMagickElementsToRender != this.mDirtyMagickElementsToRender)
        this.mMagickElementsToRender = this.mDirtyMagickElementsToRender;
      if (this.mElementsToRender == 0 && this.mMagickElementsToRender == 0)
        return;
      Point screenSize = RenderManager.Instance.ScreenSize;
      Matrix identity = Matrix.Identity;
      this.mBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 20);
      this.mBasicEffect.Texture = (Texture) IconRenderer.mHUDTexture;
      this.mBasicEffect.SetTechnique(GUIBasicEffect.Technique.Texture2D);
      this.mBasicEffect.TextureOffset = new Vector2();
      this.mBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      this.mBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
      identity.M41 = (float) Math.Floor((double) this.mPosition.X - 62.5);
      identity.M42 = this.mPosition.Y + 44f;
      this.mBasicEffect.Transform = identity;
      this.mBasicEffect.Begin();
      this.mBasicEffect.CurrentTechnique.Passes[0].Begin();
      this.mBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
      identity.M41 = (float) Math.Floor((double) this.mPosition.X + 62.5);
      identity.M11 *= -1f;
      this.mBasicEffect.Transform = identity;
      this.mBasicEffect.CommitChanges();
      this.mBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
      this.mBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
      this.mBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mMagickAlpha);
      this.mBasicEffect.CommitChanges();
      this.mMagickText.Draw(this.mBasicEffect, this.mPosition.X, (float) ((double) this.mPosition.Y + 44.0 + 10.0));
      this.mBasicEffect.CurrentTechnique.Passes[0].End();
      this.mBasicEffect.End();
      this.mInstancingEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mInstancingEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mInstancingEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 20);
      this.mInstancingEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
      this.mInstancingEffect.Scales = this.mScales;
      this.mInstancingEffect.TextureOffsets = this.mTextureOffsets;
      this.mInstancingEffect.Colors = this.mColors;
      this.mInstancingEffect.Saturations = this.mSaturations;
      if (this.mElementsToRender > 0 && this.mMagickElementsToRender > 0)
      {
        for (int index = 0; index < this.mElementsToRender; ++index)
        {
          this.mIconPositions[index].X += this.mPosition.X;
          this.mIconPositions[index].Y += this.mPosition.Y;
        }
        for (int index = 0; index < Math.Min(this.mElementsToRender - this.mCombiningToRender, 5); ++index)
        {
          this.mIconPositions[25 + index].X += 2000f;
          this.mIconPositions[25 + index].Y += 2000f;
        }
        for (int index = this.mElementsToRender - this.mCombiningToRender; index < this.mMagickElementsToRender; ++index)
        {
          this.mIconPositions[25 + index].X += this.mPosition.X;
          this.mIconPositions[25 + index].Y += this.mPosition.Y;
        }
        this.mInstancingEffect.Positions = this.mIconPositions;
        int count = this.mInstancingEffect.CurrentTechnique.Passes.Count;
        this.mInstancingEffect.Begin();
        for (int index = 0; index < count; ++index)
        {
          EffectPass pass = this.mInstancingEffect.CurrentTechnique.Passes[index];
          pass.Begin();
          this.mInstancingEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 6, 2 * this.mElementsToRender);
          this.mInstancingEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 156, 2 * this.mMagickElementsToRender);
          pass.End();
        }
        this.mInstancingEffect.End();
      }
      else if (this.mMagickElementsToRender > 0)
      {
        for (int elementsToRender = this.mElementsToRender; elementsToRender < this.mMagickElementsToRender; ++elementsToRender)
        {
          this.mIconPositions[25 + elementsToRender].X += this.mPosition.X;
          this.mIconPositions[25 + elementsToRender].Y += this.mPosition.Y;
        }
        this.mInstancingEffect.Positions = this.mIconPositions;
        int count = this.mInstancingEffect.CurrentTechnique.Passes.Count;
        this.mInstancingEffect.Begin();
        for (int index = 0; index < count; ++index)
        {
          EffectPass pass = this.mInstancingEffect.CurrentTechnique.Passes[index];
          pass.Begin();
          this.mInstancingEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 156, 2 * this.mMagickElementsToRender);
          pass.End();
        }
        this.mInstancingEffect.End();
      }
      else
      {
        if (this.mElementsToRender <= 0)
          return;
        for (int index = 0; index < this.mElementsToRender; ++index)
        {
          this.mIconPositions[index].X += this.mPosition.X;
          this.mIconPositions[index].Y += this.mPosition.Y;
        }
        this.mInstancingEffect.Positions = this.mIconPositions;
        int count = this.mInstancingEffect.CurrentTechnique.Passes.Count;
        this.mInstancingEffect.Begin();
        for (int index = 0; index < count; ++index)
        {
          EffectPass pass = this.mInstancingEffect.CurrentTechnique.Passes[index];
          pass.Begin();
          this.mInstancingEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 6, 2 * this.mElementsToRender);
          pass.End();
        }
        this.mInstancingEffect.End();
      }
    }

    public int ZIndex => 51;

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      this.mPosition = MagickaMath.WorldToScreenPosition(ref this.mWorldPosition, ref iViewProjectionMatrix);
    }
  }

  private struct VertexPositionTextureIndex
  {
    public const int SIZEINBYTES = 20;
    public Vector2 Position;
    public Vector2 TexCoord;
    public float Index;
    public static readonly VertexElement[] VertexElements = new VertexElement[3]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 0, (short) 16 /*0x10*/, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 1)
    };
  }

  private struct Icon
  {
    public Vector2 Position;
    public Vector2 Scale;
    public Vector2 TextureOffset;
    public Vector4 Color;
    public Elements Element;
    public float Saturation;
    public int Index;
    public IconRenderer.Icon.AnimationState State;
    public bool Removed;

    public static void CopyToArrays(
      IconRenderer.Icon[] iSource,
      int iSourceStartIndex,
      int iTargetStartIndex,
      int iCount,
      Vector2[] iTargetPositions,
      Vector3[] iTargetScales,
      Vector2[] iTargetTextureOffsets,
      Vector4[] iTargetColors,
      float[] iSaturations)
    {
      for (int index1 = 0; index1 < iCount; ++index1)
      {
        IconRenderer.Icon icon = iSource[index1 + iSourceStartIndex];
        int index2 = index1 + iTargetStartIndex;
        iTargetPositions[index2] = icon.Position;
        iTargetScales[index2] = new Vector3(icon.Scale, 1f);
        iTargetTextureOffsets[index2] = icon.TextureOffset;
        iTargetColors[index2] = icon.Color;
        iSaturations[index2] = icon.Saturation;
      }
    }

    public static void RemoveItem(IconRenderer.Icon[] iArray, int iItem)
    {
      for (int index = iItem + 1; index < iArray.Length; ++index)
        iArray[index - 1] = iArray[index];
    }

    [Flags]
    public enum AnimationState : byte
    {
      None = 0,
      Grow = 1,
      Glow = 2,
      Shrink = 4,
      Fade = 8,
    }
  }
}
