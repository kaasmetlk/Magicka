// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.SpellWheel
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

public class SpellWheel
{
  private static readonly Point[] DIRECTION_INDICES = new Point[4]
  {
    new Point(2, 3),
    new Point(5, 6),
    new Point(4, 1),
    new Point(0, 7)
  };
  private static readonly int[] ELEMENT_DIRECTIONS = new int[8]
  {
    3,
    2,
    0,
    0,
    2,
    1,
    1,
    3
  };
  private float[] mFadeTimers;
  private float[] mMoveTimers;
  private float mHelpTimer;
  private ControllerDirection mDirection;
  private Elements mEnabledElements;
  private SpellWheel.Icon[] mIcons;
  private float mMoveTime;
  private SpellWheel.RenderData[] mRenderData;
  private PlayState mPlayState;
  private WeakReference mPlayer;

  public SpellWheel(Player iPlayer, PlayState iPlayState)
  {
    this.mFadeTimers = new float[4];
    this.mMoveTimers = new float[4];
    this.mIcons = new SpellWheel.Icon[8];
    for (int index = 0; index < 8; ++index)
    {
      this.mIcons[index].Cooldown = 0.0f;
      this.mIcons[index].Enabled = true;
      this.mIcons[index].Intensity = 1f;
      this.mIcons[index].ResetTimer = 0.0f;
      this.mIcons[index].Saturation = 1f;
    }
    this.mEnabledElements = TutorialManager.Instance.EnabledElements;
    this.mPlayer = new WeakReference((object) iPlayer);
    Texture2D texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
    GUIBasicEffect guiBasicEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    guiBasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
    guiBasicEffect.Texture = (Texture) texture2D;
    guiBasicEffect.TextureEnabled = true;
    guiBasicEffect.Color = new Vector4(1f);
    float num1 = 1f / (float) texture2D.Height;
    float num2 = 1f / (float) texture2D.Width;
    int num3 = 48 /*0x30*/;
    int num4 = 6;
    VertexPositionTexture[] data = new VertexPositionTexture[num3 + num4];
    Vector2 vector2_1 = new Vector2(50f * num2, 49.5f * num1);
    Vector2 vector2_2 = new Vector2(200f * num2, 106f * num1);
    int num5 = 25;
    int num6 = 25;
    data[0].Position.X = (float) -num5;
    data[0].Position.Y = (float) num6;
    data[0].TextureCoordinate.X = vector2_2.X + 0.0f;
    data[0].TextureCoordinate.Y = vector2_2.Y + vector2_1.Y;
    data[1].Position.X = (float) -num5;
    data[1].Position.Y = (float) -num6;
    data[1].TextureCoordinate.X = vector2_2.X + 0.0f;
    data[1].TextureCoordinate.Y = vector2_2.Y + 0.0f;
    data[2].Position.X = (float) num5;
    data[2].Position.Y = (float) -num6;
    data[2].TextureCoordinate.X = vector2_2.X + vector2_1.X;
    data[2].TextureCoordinate.Y = vector2_2.Y + 0.0f;
    data[3].Position.X = (float) num5;
    data[3].Position.Y = (float) -num6;
    data[3].TextureCoordinate.X = vector2_2.X + vector2_1.X;
    data[3].TextureCoordinate.Y = vector2_2.Y + 0.0f;
    data[4].Position.X = (float) num5;
    data[4].Position.Y = (float) num6;
    data[4].TextureCoordinate.X = vector2_2.X + vector2_1.X;
    data[4].TextureCoordinate.Y = vector2_2.Y + vector2_1.Y;
    data[5].Position.X = (float) -num5;
    data[5].Position.Y = (float) num6;
    data[5].TextureCoordinate.X = vector2_2.X + 0.0f;
    data[5].TextureCoordinate.Y = vector2_2.Y + vector2_1.Y;
    vector2_2 = new Vector2(0.0f, 156f * num1);
    int num7 = 25;
    int num8 = 25;
    VertexPositionTexture vertexPositionTexture = new VertexPositionTexture();
    for (int index1 = 0; index1 < 8; ++index1)
    {
      int index2 = num4 + index1 * 6;
      float num9 = (float) (index1 % 5);
      int num10 = index1 / 5;
      vertexPositionTexture.Position.X = (float) -num7;
      vertexPositionTexture.Position.Y = (float) num8;
      vertexPositionTexture.TextureCoordinate.X = vector2_2.X + num9 * vector2_1.X;
      vertexPositionTexture.TextureCoordinate.Y = vector2_2.Y + (float) num10 * vector2_1.Y + vector2_1.Y;
      data[index2] = vertexPositionTexture;
      vertexPositionTexture.Position.X = (float) -num7;
      vertexPositionTexture.Position.Y = (float) -num8;
      vertexPositionTexture.TextureCoordinate.X = vector2_2.X + num9 * vector2_1.X;
      vertexPositionTexture.TextureCoordinate.Y = vector2_2.Y + (float) num10 * vector2_1.Y;
      data[index2 + 1] = vertexPositionTexture;
      vertexPositionTexture.Position.X = (float) num7;
      vertexPositionTexture.Position.Y = (float) -num8;
      vertexPositionTexture.TextureCoordinate.X = vector2_2.X + num9 * vector2_1.X + vector2_1.X;
      vertexPositionTexture.TextureCoordinate.Y = vector2_2.Y + (float) num10 * vector2_1.Y;
      data[index2 + 2] = vertexPositionTexture;
      vertexPositionTexture.Position.X = (float) num7;
      vertexPositionTexture.Position.Y = (float) -num8;
      vertexPositionTexture.TextureCoordinate.X = vector2_2.X + num9 * vector2_1.X + vector2_1.X;
      vertexPositionTexture.TextureCoordinate.Y = vector2_2.Y + (float) num10 * vector2_1.Y;
      data[index2 + 3] = vertexPositionTexture;
      vertexPositionTexture.Position.X = (float) num7;
      vertexPositionTexture.Position.Y = (float) num8;
      vertexPositionTexture.TextureCoordinate.X = vector2_2.X + num9 * vector2_1.X + vector2_1.X;
      vertexPositionTexture.TextureCoordinate.Y = vector2_2.Y + (float) num10 * vector2_1.Y + vector2_1.Y;
      data[index2 + 4] = vertexPositionTexture;
      vertexPositionTexture.Position.X = (float) -num7;
      vertexPositionTexture.Position.Y = (float) num8;
      vertexPositionTexture.TextureCoordinate.X = vector2_2.X + num9 * vector2_1.X;
      vertexPositionTexture.TextureCoordinate.Y = vector2_2.Y + (float) num10 * vector2_1.Y + vector2_1.Y;
      data[index2 + 5] = vertexPositionTexture;
    }
    VertexBuffer vertexBuffer;
    VertexDeclaration vertexDeclaration;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      vertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * data.Length, BufferUsage.None);
      vertexBuffer.SetData<VertexPositionTexture>(data);
      vertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    }
    this.mRenderData = new SpellWheel.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new SpellWheel.RenderData();
      this.mIcons.CopyTo((Array) this.mRenderData[index].Icons, 0);
      this.mRenderData[index].Effect = guiBasicEffect;
      this.mRenderData[index].VertexBuffer = vertexBuffer;
      this.mRenderData[index].VertexDeclaration = vertexDeclaration;
      this.mRenderData[index].IconOverlayStartVertex = 0;
      this.mRenderData[index].IconOverlayPrimitiveCount = 2;
      this.mRenderData[index].IconsStartVertex = 6;
      this.mRenderData[index].IconsPrimitiveCount = 2;
    }
    this.Initialize(iPlayState);
  }

  public void Initialize(PlayState iPlayState) => this.mPlayState = iPlayState;

  public void Enable(Elements iElement)
  {
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements elements = Spell.ElementFromIndex(iIndex);
      if ((elements & iElement) == elements)
      {
        this.mIcons[iIndex].Enabled = true;
        this.mIcons[iIndex].ResetTimer = 0.5f;
        if (iIndex <= 7)
        {
          if (this.Player.Controller is DirectInputController)
            (this.Player.Controller as DirectInputController).SetFadeTime(SpellWheel.ELEMENT_DIRECTIONS[iIndex], 1.5f);
          else
            (this.Player.Controller as XInputController).SetFadeTime(SpellWheel.ELEMENT_DIRECTIONS[iIndex], 1.5f);
        }
      }
    }
    this.mMoveTime = 0.0f;
  }

  public void Disable(Elements iElement)
  {
    for (int iIndex = 0; iIndex < this.mIcons.Length; ++iIndex)
    {
      Elements elements = Spell.ElementFromIndex(iIndex);
      if ((elements & iElement) == elements)
      {
        this.mIcons[iIndex].Enabled = false;
        this.mIcons[iIndex].Intensity = 1f;
      }
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Avatar avatar = this.Player.Avatar;
    if (avatar == null || avatar.Dead || GlobalSettings.Instance.SpellWheel != SettingOptions.On)
      return;
    SpellWheel.RenderData iObject = this.mRenderData[(int) iDataChannel];
    for (int index = 0; index < this.mIcons.Length; ++index)
    {
      if (this.mIcons[index].Enabled)
      {
        if ((double) this.mIcons[index].ResetTimer > 0.0)
        {
          this.mIcons[index].ResetTimer -= iDeltaTime;
          this.mIcons[index].Intensity = MathHelper.Min(this.mIcons[index].Intensity + iDeltaTime * 20f, 20f);
          if (this.Player.Controller is DirectInputController)
          {
            this.mMoveTime = Math.Min(this.mMoveTime + iDeltaTime, 1f);
            (this.Player.Controller as DirectInputController).SetMoveTime(SpellWheel.ELEMENT_DIRECTIONS[index], this.mMoveTime);
          }
        }
        else
          this.mIcons[index].Intensity = MathHelper.Min(this.mIcons[index].Intensity + iDeltaTime * 10f, 1f);
        this.mIcons[index].Saturation = MathHelper.Min(this.mIcons[index].Saturation + iDeltaTime * 4f, 1f);
      }
      else
        this.mIcons[index].Saturation = MathHelper.Max(this.mIcons[index].Saturation - iDeltaTime * 4f, 0.0f);
    }
    this.mIcons.CopyTo((Array) iObject.Icons, 0);
    iObject.WorldPosition = avatar.Position;
    iObject.Direction = this.mDirection;
    iObject.FadeTimers = this.mFadeTimers;
    iObject.MoveTimers = this.mMoveTimers;
    iObject.HelpTime = this.mHelpTimer;
    this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public void HelpTimer(float iHelpTimer) => this.mHelpTimer = iHelpTimer;

  internal void Direction(ControllerDirection iDirection) => this.mDirection = iDirection;

  public void MoveTimers(float[] iTimers) => this.mMoveTimers = iTimers;

  public void FadeTimers(float[] iTimers) => this.mFadeTimers = iTimers;

  internal static int GetDirectionIndex(ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Right:
        return 0;
      case ControllerDirection.Up:
        return 1;
      case ControllerDirection.Left:
        return 2;
      case ControllerDirection.Down:
        return 3;
      default:
        return -1;
    }
  }

  protected Player Player => this.mPlayer.Target as Player;

  protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    public GUIBasicEffect Effect;
    public VertexBuffer VertexBuffer;
    public VertexDeclaration VertexDeclaration;
    public int IconsStartVertex;
    public int IconsPrimitiveCount;
    public int IconOverlayStartVertex;
    public int IconOverlayPrimitiveCount;
    private Vector2 mPosition;
    public Vector3 WorldPosition;
    internal ControllerDirection Direction;
    public float[] MoveTimers;
    public float[] FadeTimers;
    public float HelpTime;
    public SpellWheel.Icon[] Icons;

    public RenderData()
    {
      this.MoveTimers = new float[4];
      this.FadeTimers = new float[4];
      this.Icons = new SpellWheel.Icon[8];
    }

    public void Draw(float iDeltaTime)
    {
      this.Effect.GraphicsDevice.Vertices[0].SetSource(this.VertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      this.Effect.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
      this.Effect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
      this.Effect.Begin();
      this.Effect.CurrentTechnique.Passes[0].Begin();
      if ((double) this.HelpTime > 1.4012984643248171E-45)
      {
        float iAlpha = MathHelper.Clamp(this.HelpTime / 0.2f, 0.0f, 1f);
        for (int iIndex = 0; iIndex < 4; ++iIndex)
          this.RenderDirectionIcons(iIndex, iAlpha);
      }
      else
      {
        for (int iIndex = 0; iIndex < 4; ++iIndex)
        {
          float iAlpha = MathHelper.Clamp(this.FadeTimers[iIndex] / 0.2f, 0.0f, 1f);
          if ((double) this.FadeTimers[iIndex] > 1.4012984643248171E-45)
            this.RenderDirectionIcons(iIndex, iAlpha);
        }
      }
      this.Effect.CurrentTechnique.Passes[0].End();
      this.Effect.End();
    }

    private void RenderDirectionIcons(int iIndex, float iAlpha)
    {
      Point directionIndex = SpellWheel.DIRECTION_INDICES[iIndex];
      Vector2 vector2_1 = new Vector2(this.mPosition.X + (float) Math.Cos((double) Defines.ElementUIRadian[directionIndex.X]) * 64f, this.mPosition.Y - (float) Math.Sin((double) Defines.ElementUIRadian[directionIndex.X]) * 64f);
      Vector2 vector2_2 = new Vector2(this.mPosition.X + (float) Math.Cos((double) Defines.ElementUIRadian[directionIndex.Y]) * 64f, this.mPosition.Y - (float) Math.Sin((double) Defines.ElementUIRadian[directionIndex.Y]) * 64f);
      Vector2 result1 = vector2_1;
      Vector2 result2 = vector2_2;
      if ((double) this.MoveTimers[iIndex] < 0.20000000298023224)
      {
        Vector2 vector2_3 = new Vector2(this.mPosition.X + (float) Math.Cos((double) iIndex * 1.5707963705062866) * 64f, this.mPosition.Y - (float) Math.Sin((double) iIndex * 1.5707963705062866) * 64f);
        Vector2.Lerp(ref vector2_3, ref vector2_1, this.MoveTimers[iIndex] / 0.2f, out result1);
        Vector2.Lerp(ref vector2_3, ref vector2_2, this.MoveTimers[iIndex] / 0.2f, out result2);
      }
      Matrix result3 = Matrix.Identity with
      {
        M41 = result2.X,
        M42 = result2.Y
      };
      this.Effect.Transform = result3;
      this.Effect.Saturation = this.Icons[directionIndex.Y].Saturation;
      this.Effect.Color = new Vector4(this.Icons[directionIndex.Y].Intensity, this.Icons[directionIndex.Y].Intensity, this.Icons[directionIndex.Y].Intensity, iAlpha);
      this.Effect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
      this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
      this.Effect.CommitChanges();
      this.Effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, this.IconsStartVertex + directionIndex.Y * 6, 2);
      result3.M41 = result1.X;
      result3.M42 = result1.Y;
      this.Effect.Transform = result3;
      this.Effect.Saturation = this.Icons[directionIndex.X].Saturation;
      this.Effect.Color = new Vector4(this.Icons[directionIndex.X].Intensity, this.Icons[directionIndex.X].Intensity, this.Icons[directionIndex.X].Intensity, iAlpha);
      this.Effect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
      this.Effect.GraphicsDevice.RenderState.SourceBlend = Blend.One;
      this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.Zero;
      this.Effect.CommitChanges();
      this.Effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, this.IconsStartVertex + directionIndex.X * 6, 2);
      Matrix.CreateRotationZ((float) iIndex * 1.57079637f, out result3);
      result3.M41 = result2.X;
      result3.M42 = result2.Y;
      this.Effect.Transform = result3;
      this.Effect.Saturation = this.Icons[directionIndex.Y].Saturation;
      this.Effect.Color = new Vector4(this.Icons[directionIndex.Y].Intensity, this.Icons[directionIndex.Y].Intensity, this.Icons[directionIndex.Y].Intensity, iAlpha);
      this.Effect.GraphicsDevice.RenderState.BlendFunction = BlendFunction.ReverseSubtract;
      this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
      this.Effect.CommitChanges();
      this.Effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, this.IconOverlayStartVertex, 2);
      this.Effect.Transform = Matrix.Identity with
      {
        M41 = result1.X,
        M42 = result1.Y
      };
      this.Effect.Saturation = this.Icons[directionIndex.X].Saturation;
      this.Effect.Color = new Vector4(this.Icons[directionIndex.X].Intensity, this.Icons[directionIndex.X].Intensity, this.Icons[directionIndex.X].Intensity, iAlpha);
      this.Effect.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
      this.Effect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
      this.Effect.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationAlpha;
      this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseDestinationAlpha;
      this.Effect.CommitChanges();
      this.Effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, this.IconsStartVertex + directionIndex.X * 6, 2);
      this.Effect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
      this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
      this.Effect.Color = new Vector4(1f, 1f, 1f, 1f);
    }

    public int ZIndex => 100;

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      this.mPosition = MagickaMath.WorldToScreenPosition(ref this.WorldPosition, ref iViewProjectionMatrix);
    }
  }

  public struct Icon
  {
    public bool Enabled;
    public float Saturation;
    public float Cooldown;
    public float ResetTimer;
    public float Intensity;
  }
}
