// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.TimedObjectiveRuleset
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

#nullable disable
namespace Magicka.Levels;

public class TimedObjectiveRuleset : IRuleset
{
  private const float NETWORK_UPDATE_FREQ = 1f;
  public static readonly int LOC_TIME_ELAPSED = "#challenge_time".GetHashCodeCustom();
  private bool mNetworkDirty;
  private float mNetworkTimer;
  private List<TimedObjectiveRuleset.Objective> mObjectives;
  private int mTimeStorageID;
  private float mTimeElapsed;
  private TimedObjectiveRuleset.RenderData[] mRenderData;
  private GUIBasicEffect mGUIEffect;
  private TextBoxEffect mTextBoxEffect;
  private GameScene mGameScene;
  private float mLastUpdatedElapsedTime;
  private TimedObjectiveRuleset.TimeBonus mTimeBonus;

  public TimedObjectiveRuleset(GameScene iScene, XmlNode iNode)
  {
    this.mGameScene = iScene;
    string str = "timestorage";
    string assetName = "UI/HUD/Dialog_Say";
    int iBorderSize = 32 /*0x20*/;
    for (int i = 0; i < iNode.Attributes.Count; ++i)
    {
      XmlAttribute attribute = iNode.Attributes[i];
      if (attribute.Name.Equals("texture", StringComparison.OrdinalIgnoreCase))
        assetName = attribute.Value;
      else if (attribute.Name.Equals("bordersize", StringComparison.OrdinalIgnoreCase))
        iBorderSize = int.Parse(attribute.Value);
      else if (attribute.Name.Equals("timer", StringComparison.OrdinalIgnoreCase))
        str = attribute.Value;
    }
    this.mTimeStorageID = str.ToLowerInvariant().GetHashCodeCustom();
    this.mObjectives = new List<TimedObjectiveRuleset.Objective>(4);
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode = iNode.ChildNodes[i1];
      if (childNode.Name.Equals("Objective", StringComparison.OrdinalIgnoreCase))
      {
        int num1 = 0;
        int num2 = 0;
        int num3 = 1;
        int num4 = 0;
        for (int i2 = 0; i2 < childNode.Attributes.Count; ++i2)
        {
          XmlAttribute attribute = childNode.Attributes[i2];
          if (attribute.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
          {
            if (!string.IsNullOrEmpty(attribute.Value))
              num2 = attribute.Value.ToLowerInvariant().GetHashCodeCustom();
          }
          else if (attribute.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
          {
            if (!string.IsNullOrEmpty(attribute.Value))
              num1 = attribute.Value.ToLowerInvariant().GetHashCodeCustom();
          }
          else if (attribute.Name.Equals("max", StringComparison.OrdinalIgnoreCase))
            num4 = int.Parse(attribute.Value);
          else if (attribute.Name.Equals("score", StringComparison.OrdinalIgnoreCase))
            num3 = int.Parse(attribute.Value);
        }
        if (num2 != 0 && num1 != 0)
          this.mObjectives.Add(new TimedObjectiveRuleset.Objective()
          {
            CounterID = num1,
            NameID = num2,
            Value = 0,
            Score = num3,
            MaxValue = num4
          });
      }
      else if (childNode.Name.Equals("timebonus", StringComparison.OrdinalIgnoreCase))
      {
        this.mTimeBonus = new TimedObjectiveRuleset.TimeBonus();
        this.mTimeBonus.MaxTime = 0.0f;
        this.mTimeBonus.MinTime = 0.0f;
        this.mTimeBonus.MaxTimeBonus = 1f;
        this.mTimeBonus.MinTimeBonus = 1f;
        this.mTimeBonus.CounterID = 0;
        for (int i3 = 0; i3 < childNode.Attributes.Count; ++i3)
        {
          XmlAttribute attribute = childNode.Attributes[i3];
          if (attribute.Name.Equals("minTime", StringComparison.OrdinalIgnoreCase))
            this.mTimeBonus.MinTime = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          else if (attribute.Name.Equals("minTimeBonus", StringComparison.OrdinalIgnoreCase))
            this.mTimeBonus.MinTimeBonus = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          else if (attribute.Name.Equals("maxTime", StringComparison.OrdinalIgnoreCase))
            this.mTimeBonus.MaxTime = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          else if (attribute.Name.Equals("maxTimeBonus", StringComparison.OrdinalIgnoreCase))
            this.mTimeBonus.MaxTimeBonus = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          else if (attribute.Name.Equals("counter", StringComparison.OrdinalIgnoreCase))
            this.mTimeBonus.CounterID = attribute.Value.ToLowerInvariant().GetHashCodeCustom();
        }
      }
    }
    Point screenSize = RenderManager.Instance.ScreenSize;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mGUIEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
      this.mTextBoxEffect = new TextBoxEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
    }
    this.mGUIEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mGUIEffect.Color = Vector4.One;
    this.mTextBoxEffect.BorderSize = (float) iBorderSize;
    this.mTextBoxEffect.ScreenSize = new Vector2((float) screenSize.X, (float) screenSize.Y);
    this.mTextBoxEffect.Texture = (Texture) Magicka.Game.Instance.Content.Load<Texture2D>(assetName);
    this.mTextBoxEffect.Scale = 1f;
    this.mTextBoxEffect.Color = Vector4.One;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra16);
    this.mRenderData = new TimedObjectiveRuleset.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new TimedObjectiveRuleset.RenderData(this.mGUIEffect, this.mTextBoxEffect, font, Magicka.Game.Instance.Content.Load<Texture2D>(assetName), iBorderSize);
  }

  internal int TimerID => this.mTimeStorageID;

  internal List<TimedObjectiveRuleset.Objective> Objectives => this.mObjectives;

  internal bool TimeSuccess => this.mGameScene.Level.GetCounterValue(this.mTimeBonus.CounterID) > 0;

  internal float GetBonusMultiplier
  {
    get
    {
      return TimedObjectiveRuleset.TimeBonus.GetBonus(ref this.mTimeBonus, this.mGameScene.Level.GetTimerValue(this.mTimeStorageID) / 60f) * (float) this.mGameScene.Level.GetCounterValue(this.mTimeBonus.CounterID);
    }
  }

  public int GetAnyArea() => TriggerArea.ANYID;

  public void Update(float iDeltaTime, DataChannel iDataChannel)
  {
    if (iDataChannel == DataChannel.None)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      if (this.mNetworkDirty && (double) this.mNetworkTimer <= 0.0)
      {
        this.mNetworkTimer = 1f;
        this.NetworkUpdate();
      }
      this.mNetworkTimer -= iDeltaTime;
    }
    if (!this.mGameScene.PlayState.IsPaused && !this.mGameScene.PlayState.IsInCutscene && !this.mGameScene.PlayState.IsGameEnded)
      this.mTimeElapsed = this.mGameScene.Level.GetTimerValue(this.mTimeStorageID);
    for (int index = 0; index < this.mObjectives.Count; ++index)
    {
      TimedObjectiveRuleset.Objective mObjective = this.mObjectives[index];
      if (this.mGameScene.Level.GetCounterValue(mObjective.CounterID) != mObjective.Value)
      {
        this.mNetworkDirty = true;
        int counterValue = this.mGameScene.Level.GetCounterValue(mObjective.CounterID);
        mObjective.Value = counterValue;
        this.mObjectives[index] = mObjective;
        break;
      }
    }
    if ((double) this.mTimeElapsed - (double) this.mLastUpdatedElapsedTime >= 1.0)
    {
      this.mLastUpdatedElapsedTime = this.mTimeElapsed;
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) this.mTimeElapsed);
      string str = LanguageManager.Instance.GetString(TimedObjectiveRuleset.LOC_TIME_ELAPSED);
      string iText = (double) this.mTimeElapsed < 60.0 || (double) this.mTimeElapsed >= 3600.0 ? ((double) this.mTimeElapsed < 3600.0 ? str.Replace("#1;", $"0:00:{timeSpan.Seconds:00}") : str.Replace("#1;", $"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}")) : str.Replace("#1;", $"0:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
      for (int index = 0; index < 3; ++index)
        this.mRenderData[index].SetTime(iText);
    }
    if (this.mGameScene.PlayState.IsInCutscene)
      return;
    this.mGameScene.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) this.mRenderData[(int) iDataChannel]);
  }

  public void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
  {
    if (iDataChannel == DataChannel.None)
      return;
    if (!this.mGameScene.PlayState.IsPaused && !this.mGameScene.PlayState.IsInCutscene && !this.mGameScene.PlayState.IsGameEnded)
      this.mTimeElapsed = this.mGameScene.Level.GetTimerValue(this.mTimeStorageID);
    if ((double) this.mTimeElapsed - (double) this.mLastUpdatedElapsedTime >= 1.0)
    {
      this.mLastUpdatedElapsedTime = this.mTimeElapsed;
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) this.mTimeElapsed);
      string str = LanguageManager.Instance.GetString(TimedObjectiveRuleset.LOC_TIME_ELAPSED);
      string iText = (double) this.mTimeElapsed < 60.0 || (double) this.mTimeElapsed >= 3600.0 ? ((double) this.mTimeElapsed < 3600.0 ? str.Replace("#1;", $"0:00:{timeSpan.Seconds:00}") : str.Replace("#1;", $"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}")) : str.Replace("#1;", $"0:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
      for (int index = 0; index < 3; ++index)
        this.mRenderData[index].SetTime(iText);
    }
    if (this.mGameScene.PlayState.IsInCutscene)
      return;
    this.mGameScene.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) this.mRenderData[(int) iDataChannel]);
  }

  public void Initialize()
  {
    string iText = LanguageManager.Instance.GetString(TimedObjectiveRuleset.LOC_TIME_ELAPSED).Replace("#1;", "0:00:00");
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index].SetTime(iText);
    this.mNetworkDirty = true;
    this.mGameScene.Level.AddTimer(this.mTimeStorageID, true, 0.0f);
    this.mTimeElapsed = this.mGameScene.Level.GetTimerValue(this.mTimeStorageID);
    this.mLastUpdatedElapsedTime = 0.0f;
    for (int index = 0; index < this.mObjectives.Count; ++index)
    {
      TimedObjectiveRuleset.Objective mObjective = this.mObjectives[index];
      int counterValue = this.mGameScene.Level.GetCounterValue(mObjective.CounterID);
      mObjective.Value = counterValue;
      this.mObjectives[index] = mObjective;
    }
  }

  public void DeInitialize()
  {
  }

  private unsafe void NetworkUpdate()
  {
    RulesetMessage iMessage = new RulesetMessage();
    iMessage.Type = Rulesets.TimedObjective;
    iMessage.NrOfByteItems = (byte) Math.Min(this.mObjectives.Count, 16 /*0x10*/);
    for (int index = 0; index < this.mObjectives.Count; ++index)
      iMessage.Byte[index] = (byte) this.mGameScene.Level.GetCounterValue(this.mObjectives[index].CounterID);
    iMessage.Float01 = this.mGameScene.Level.GetTimerValue(this.mTimeBonus.CounterID);
    NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref iMessage);
    this.mNetworkDirty = false;
  }

  unsafe void IRuleset.NetworkUpdate(ref RulesetMessage iMsg)
  {
    fixed (byte* numPtr = iMsg.Byte)
    {
      for (int index = 0; index < (int) iMsg.NrOfByteItems; ++index)
      {
        TimedObjectiveRuleset.Objective mObjective = this.mObjectives[index];
        if (mObjective.Value != (int) numPtr[index])
        {
          this.mGameScene.Level.SetCounterValue(mObjective.CounterID, (int) numPtr[index]);
          mObjective.Value = (int) numPtr[index];
          this.mObjectives[index] = mObjective;
        }
      }
      this.mGameScene.Level.SetTimer(this.mTimeBonus.CounterID, iMsg.Float01);
    }
  }

  public Rulesets RulesetType => Rulesets.TimedObjective;

  public bool IsVersusRuleset => false;

  private class RenderData : IRenderableGUIObject
  {
    private static IndexBuffer sIndexBuffer;
    private static VertexBuffer sVertexBuffer;
    private static VertexDeclaration sVertexDeclaration;
    private TextBoxEffect mTextBoxEffect;
    private GUIBasicEffect mEffect;
    private Rectangle mTimeRect;
    private string mTimeTextString;
    private Text mTimeText;
    private Texture2D mTimeTexture;
    private int mBorderSize;
    private BitmapFont mFont;

    public RenderData(
      GUIBasicEffect iEffect,
      TextBoxEffect iBoxEffect,
      BitmapFont iFont,
      Texture2D iTimeTexture,
      int iBorderSize)
    {
      this.mEffect = iEffect;
      this.mTextBoxEffect = iBoxEffect;
      this.mTimeTexture = iTimeTexture;
      this.mBorderSize = iBorderSize;
      this.mTimeTextString = (string) null;
      this.mTimeText = new Text(200, iFont, TextAlign.Left, false);
      this.mTimeText.DefaultColor = Color.Cyan.ToVector4();
      this.mTimeText.SetText("");
      this.mTimeRect = new Rectangle(16 /*0x10*/, 16 /*0x10*/, 0, 0);
      this.mFont = iFont;
      if (TimedObjectiveRuleset.RenderData.sVertexBuffer != null)
        return;
      TimedObjectiveRuleset.RenderData.sIndexBuffer = new IndexBuffer(Magicka.Game.Instance.GraphicsDevice, 108, BufferUsage.None, IndexElementSize.SixteenBits);
      TimedObjectiveRuleset.RenderData.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 16 /*0x10*/ * VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
      TimedObjectiveRuleset.RenderData.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionNormalTexture.VertexElements);
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        TimedObjectiveRuleset.RenderData.sIndexBuffer.SetData<ushort>(TextBox.INDICES);
        TimedObjectiveRuleset.RenderData.sVertexBuffer.SetData<VertexPositionNormalTexture>(TextBox.VERTICES);
      }
    }

    public void ResetRect() => this.mTimeRect = new Rectangle(16 /*0x10*/, 16 /*0x10*/, 0, 0);

    public void SetTime(string iText) => this.mTimeTextString = iText;

    public void Draw(float iDeltaTime)
    {
      if (this.mTimeTextString != null)
      {
        this.mTimeText.SetText(this.mTimeTextString);
        Vector2 vector2 = this.mFont.MeasureText(this.mTimeTextString, true);
        this.mTimeRect.Width = (int) vector2.X;
        this.mTimeRect.Height = (int) vector2.Y;
        this.mTimeRect.X = this.mTimeRect.Width / 2 + this.mBorderSize;
        this.mTimeRect.Y = this.mTimeRect.Height / 2 + this.mBorderSize;
        this.mTimeTextString = (string) null;
      }
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.mTextBoxEffect.GraphicsDevice.Indices = TimedObjectiveRuleset.RenderData.sIndexBuffer;
      this.mTextBoxEffect.GraphicsDevice.Vertices[0].SetSource(TimedObjectiveRuleset.RenderData.sVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
      this.mTextBoxEffect.GraphicsDevice.VertexDeclaration = TimedObjectiveRuleset.RenderData.sVertexDeclaration;
      this.mTextBoxEffect.Position = new Vector2((float) this.mTimeRect.X, (float) this.mTimeRect.Y);
      this.mTextBoxEffect.Size = new Vector2((float) this.mTimeRect.Width, (float) this.mTimeRect.Height);
      this.mTextBoxEffect.Texture = (Texture) this.mTimeTexture;
      this.mTextBoxEffect.Begin();
      this.mTextBoxEffect.CurrentTechnique.Passes[0].Begin();
      this.mTextBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
      this.mTextBoxEffect.CurrentTechnique.Passes[0].End();
      this.mTextBoxEffect.End();
      this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      this.mTimeText.Draw(this.mEffect, (float) (this.mTimeRect.X - this.mTimeRect.Width / 2), (float) (this.mTimeRect.Y - this.mTimeRect.Height / 2));
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }

    public int ZIndex => 205;
  }

  internal struct Objective
  {
    public int CounterID;
    public int NameID;
    public int Value;
    public int MaxValue;
    public int Score;
  }

  private struct TimeBonus
  {
    public int CounterID;
    public float MinTime;
    public float MaxTime;
    public float MinTimeBonus;
    public float MaxTimeBonus;

    public static float GetBonus(ref TimedObjectiveRuleset.TimeBonus iBonus, float iTime)
    {
      if ((double) iTime > (double) iBonus.MaxTime)
        iTime = iBonus.MaxTime;
      else if ((double) iBonus.MinTime > (double) iTime)
        iTime = iBonus.MinTime;
      float amount = (float) (1.0 - ((double) iTime - (double) iBonus.MinTime) / ((double) iBonus.MaxTime - (double) iBonus.MinTime));
      return MathHelper.Lerp(iBonus.MaxTimeBonus, iBonus.MinTimeBonus, amount);
    }
  }
}
