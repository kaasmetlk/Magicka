// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Credits
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class Credits
{
  private static Credits sSingelton;
  private static volatile object sSingeltonLock = new object();
  private float SCROLL_SPEED = 16f;
  private float DIV_SCROLL = 1f / 16f;
  private int mNrOfTexts;
  private bool mInitialized;
  private bool mStarted;
  private bool mFadeOut;
  private bool mFadingOut;
  private int mStartIndex;
  private int mEndIndex;
  private Vector3 mSkymapColor;
  private Vector3 mBlack = new Vector3(0.0f, 0.0f, 0.0f);
  private float mSkymapFadeInTime;
  private float mFadeInAlpha;
  private float mTotalLength;
  private SaveData mSaveSlot;
  private float mPosition;
  private List<Text> mTexts;
  private List<Credits.ElementData> mTextDatas;
  private Credits.RenderData[] mRenderData;
  private int mPlayTime;

  public static Credits Instance
  {
    get
    {
      if (Credits.sSingelton == null)
      {
        lock (Credits.sSingeltonLock)
        {
          if (Credits.sSingelton == null)
            Credits.sSingelton = new Credits();
        }
      }
      return Credits.sSingelton;
    }
  }

  private Credits()
  {
    this.mInitialized = false;
    this.mRenderData = new Credits.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new Credits.RenderData();
  }

  public void Initialize(string fileName)
  {
    if (this.mInitialized)
      return;
    List<Credits.CreditData> creditDataList = new List<Credits.CreditData>(32 /*0x20*/);
    XmlDocument xmlDocument = new XmlDocument();
    if (fileName == null || fileName == "")
      xmlDocument.Load("Content/Data/Credits.xml");
    else
      xmlDocument.Load($"Content/Data/{fileName}");
    XmlNode xmlNode = (XmlNode) null;
    for (int i1 = 0; i1 < xmlDocument.ChildNodes.Count; ++i1)
    {
      XmlNode childNode = xmlDocument.ChildNodes[i1];
      if (childNode.Name.Equals("credits", StringComparison.OrdinalIgnoreCase))
      {
        for (int i2 = 0; i2 < childNode.Attributes.Count; ++i2)
        {
          if (childNode.Attributes[i2].Name.Equals("speed", StringComparison.OrdinalIgnoreCase))
          {
            this.SCROLL_SPEED = float.Parse(childNode.Attributes[i2].Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            this.DIV_SCROLL = 1f / this.SCROLL_SPEED;
          }
        }
        xmlNode = childNode;
        break;
      }
    }
    for (int i3 = 0; i3 < xmlNode.ChildNodes.Count; ++i3)
    {
      XmlNode childNode = xmlNode.ChildNodes[i3];
      if (childNode.Name.Equals("credit", StringComparison.OrdinalIgnoreCase))
      {
        string lowerInvariant = childNode.InnerText.ToLowerInvariant();
        Credits.CreditData creditData;
        creditData.ID = lowerInvariant.GetHashCodeCustom();
        creditData.COLOR = Vector3.One;
        creditData.FONT = MagickaFont.Maiandra18;
        creditData.PADDING = 0.0f;
        for (int i4 = 0; i4 < childNode.Attributes.Count; ++i4)
        {
          XmlAttribute attribute = childNode.Attributes[i4];
          if (attribute.Name.Equals("color", StringComparison.OrdinalIgnoreCase))
          {
            string[] strArray = attribute.Value.Replace(" ", "").Split(',');
            creditData.COLOR.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            creditData.COLOR.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            creditData.COLOR.Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          }
          else if (attribute.Name.Equals("font", StringComparison.OrdinalIgnoreCase))
            creditData.FONT = (MagickaFont) Enum.Parse(typeof (MagickaFont), attribute.Value, true);
          else if (attribute.Name.Equals("padding", StringComparison.OrdinalIgnoreCase))
            creditData.PADDING = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        }
        creditDataList.Add(creditData);
      }
    }
    this.mTotalLength = 0.0f;
    this.mNrOfTexts = creditDataList.Count;
    this.mTexts = new List<Text>(this.mNrOfTexts);
    this.mTextDatas = new List<Credits.ElementData>(this.mNrOfTexts);
    int iTargetLineWidth = 400;
    int num = 0;
    for (int index1 = 0; index1 < this.mNrOfTexts; ++index1)
    {
      BitmapFont font = FontManager.Instance.GetFont(creditDataList[index1].FONT);
      string iText1 = LanguageManager.Instance.GetString(creditDataList[index1].ID);
      string[] strArray = font.Wrap(iText1, iTargetLineWidth, true).Split('\n');
      num += strArray.Length - 1;
      for (int index2 = 0; index2 < strArray.Length; ++index2)
      {
        string iText2 = strArray[index2];
        Text text = new Text(iText2.Length + 1, font, TextAlign.Center, false);
        text.DefaultColor = new Vector4(creditDataList[index1].COLOR, 1f);
        text.SetText(iText2);
        this.mTexts.Add(text);
        this.mTextDatas.Add(new Credits.ElementData()
        {
          Alpha = 0.0f,
          Height = font.MeasureText(iText2, true).Y,
          Padding = index2 == 0 ? creditDataList[index1].PADDING : 0.0f
        });
        this.mTotalLength += this.mTextDatas[index1].Height + this.mTextDatas[index1].Padding;
      }
    }
    this.mNrOfTexts += num;
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].Texts = this.mTexts;
      this.mRenderData[index].TextDatas = this.mTextDatas;
    }
    this.mInitialized = true;
    this.mFadeInAlpha = 0.0f;
  }

  public bool IsActive => this.mStarted;

  public void Kill()
  {
    this.mStarted = false;
    this.mFadeInAlpha = 0.0f;
    this.mSkymapFadeInTime = 0.0f;
    this.mFadeOut = false;
    this.mInitialized = false;
    ControlManager.Instance.UnlimitInput();
    if (this.mSaveSlot != null)
    {
      SubMenuEndGame.Instance.Set(true, this.mSaveSlot);
      SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
      if (currentSaveData != null && currentSaveData.CurrentPlayTime + this.mPlayTime <= 14400)
        AchievementsManager.Instance.AwardAchievement(PlayState.RecentPlayState, "missionimprobable");
    }
    this.mSaveSlot = (SaveData) null;
  }

  public void Start(SaveData iSaveSlot)
  {
    if (!this.mInitialized)
      this.Initialize("");
    this.mSaveSlot = iSaveSlot;
    this.mFadeInAlpha = 0.0f;
    this.mPosition = (float) RenderManager.Instance.ScreenSize.Y * 0.5f;
    this.mStartIndex = 0;
    float num = 0.0f;
    for (int index = 0; index < this.mNrOfTexts; ++index)
    {
      num += this.mTextDatas[index].Height + this.mTextDatas[index].Padding;
      if ((double) num > (double) this.mPosition)
      {
        this.mEndIndex = index;
        break;
      }
    }
    this.mFadeOut = false;
    this.mFadingOut = false;
    this.mStarted = true;
    this.mSkymapColor = RenderManager.Instance.SkyMapColor;
    this.mFadeInAlpha = 0.0f;
    this.mSkymapFadeInTime = 0.0f;
  }

  private void ScreenCaptureCallback()
  {
    RenderManager.Instance.EndTransition(Transitions.CrossFade, Color.White, 1f);
    SubMenuEndGame.Instance.CreditsEnd = true;
    Tome.Instance.PushMenuInstant((SubMenu) SubMenuEndGame.Instance, 1);
    Tome.Instance.ChangeState((Tome.TomeState) Tome.OpenState.Instance);
    GameStateManager.Instance.PopState();
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime, PlayState iPlayState)
  {
    if (!this.mInitialized | !this.mStarted)
      return;
    if (this.mFadeOut)
    {
      if (!this.mFadingOut)
      {
        this.mFadingOut = true;
        this.mPlayTime = (int) iPlayState.PlayTime;
        iPlayState.Endgame(EndGameCondition.EndOffGame, false, false, 0.0f);
        RenderManager.Instance.TransitionEnd += new TransitionEnd(this.OnTransitionEnd);
        RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 1f);
      }
      Credits.RenderData iObject = this.mRenderData[(int) iDataChannel];
      iPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    }
    else if ((double) this.mSkymapFadeInTime < 1.0)
    {
      this.mSkymapFadeInTime = Math.Min(this.mSkymapFadeInTime + iDeltaTime * 0.5f, 1f);
      Vector3 result = iPlayState.Level.CurrentScene.SkyMapColor;
      Vector3.Lerp(ref this.mSkymapColor, ref this.mBlack, this.mSkymapFadeInTime, out result);
      RenderManager.Instance.SkyMapColor = result;
    }
    else
    {
      this.mFadeInAlpha = Math.Min(this.mFadeInAlpha + iDeltaTime * 2f, 1f);
      Credits.RenderData iObject = this.mRenderData[(int) iDataChannel];
      iObject.FadeInAlpha = this.mFadeInAlpha;
      if ((double) this.mFadeInAlpha >= 1.0)
      {
        float num1 = (float) RenderManager.Instance.ScreenSize.Y * 0.5f;
        this.mPosition -= iDeltaTime * this.DIV_SCROLL * num1;
        if ((double) this.mPosition <= -(double) this.mTotalLength)
          this.mFadeOut = true;
        float num2 = this.mPosition;
        this.mStartIndex = 0;
        this.mEndIndex = this.mNrOfTexts;
        for (int index = 0; index < this.mNrOfTexts; ++index)
        {
          Credits.ElementData mTextData = this.mTextDatas[index];
          float height = mTextData.Height;
          float num3 = num2 + this.mTextDatas[index].Padding;
          mTextData.Position = num3;
          float num4 = Math.Min(num3 / height, 1f);
          float num5 = Math.Max(Math.Min((num1 - (num3 + height)) / height, 1f), 0.0f);
          mTextData.Alpha = num4 * num5;
          if ((double) num4 <= 0.0 && index < this.mStartIndex)
            this.mStartIndex = index;
          else if ((double) num5 <= 0.0 && index < this.mEndIndex)
            this.mEndIndex = index;
          this.mTextDatas[index] = mTextData;
          num2 = num3 + height;
        }
        iObject.StartIndex = this.mStartIndex;
        iObject.EndIndex = this.mEndIndex;
      }
      iPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    }
  }

  private void OnTransitionEnd(TransitionEffect iDeadTransition)
  {
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.OnTransitionEnd);
    this.mStarted = false;
    this.mFadeInAlpha = 0.0f;
    this.mSkymapFadeInTime = 0.0f;
    ControlManager.Instance.UnlimitInput();
    Texture2D screenShot = RenderManager.Instance.GetScreenShot(new Action(this.ScreenCaptureCallback));
    SubMenuEndGame.Instance.ScreenShot = screenShot;
    RenderManager.Instance.GetTransitionEffect(Transitions.CrossFade).SourceTexture1 = screenShot;
    SubMenuEndGame.Instance.Set(true, this.mSaveSlot);
    if (this.mSaveSlot == null || SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType != GameType.Campaign || SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.CurrentPlayTime + this.mPlayTime > 14400)
      return;
    AchievementsManager.Instance.AwardAchievement(PlayState.RecentPlayState, "missionimprobable");
  }

  protected class RenderData : IRenderableGUIObject
  {
    public const int ARRAY_SIZE = 32 /*0x20*/;
    private static GUIBasicEffect sEffect;
    private static VertexBuffer sVertexBuffer;
    private static VertexDeclaration sVertexDeclaration;
    public int StartIndex;
    public int EndIndex;
    public float FadeInAlpha;
    public List<Text> Texts;
    public List<Credits.ElementData> TextDatas;

    static RenderData()
    {
      VertexPositionColor[] data = new VertexPositionColor[6]
      {
        new VertexPositionColor(new Vector3(0.0f, 0.0f, 0.0f), new Color((byte) 0, (byte) 0, (byte) 0, byte.MaxValue)),
        new VertexPositionColor(new Vector3(1f, 0.0f, 0.0f), new Color((byte) 0, (byte) 0, (byte) 0, byte.MaxValue)),
        new VertexPositionColor(new Vector3(0.0f, 1f, 0.0f), new Color((byte) 0, (byte) 0, (byte) 0, byte.MaxValue)),
        new VertexPositionColor(new Vector3(1f, 1f, 0.0f), new Color((byte) 0, (byte) 0, (byte) 0, byte.MaxValue)),
        new VertexPositionColor(new Vector3(0.0f, 1.1f, 0.0f), new Color((byte) 0, (byte) 0, (byte) 0, (byte) 0)),
        new VertexPositionColor(new Vector3(1f, 1.1f, 0.0f), new Color((byte) 0, (byte) 0, (byte) 0, (byte) 0))
      };
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        Credits.RenderData.sEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
        Credits.RenderData.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, VertexPositionColor.SizeInBytes * data.Length, BufferUsage.WriteOnly);
        Credits.RenderData.sVertexBuffer.SetData<VertexPositionColor>(data);
        Credits.RenderData.sVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionColor.VertexElements);
      }
    }

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      Credits.RenderData.sEffect.SetScreenSize(screenSize.X, screenSize.Y);
      Credits.RenderData.sEffect.GraphicsDevice.Vertices[0].SetSource(Credits.RenderData.sVertexBuffer, 0, VertexPositionColor.SizeInBytes);
      Credits.RenderData.sEffect.GraphicsDevice.VertexDeclaration = Credits.RenderData.sVertexDeclaration;
      Matrix matrix = new Matrix((float) screenSize.X, 0.0f, 0.0f, 0.0f, 0.0f, (float) screenSize.Y * 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
      Credits.RenderData.sEffect.Transform = matrix;
      Credits.RenderData.sEffect.Color = new Vector4(1f, 1f, 1f, this.FadeInAlpha);
      Credits.RenderData.sEffect.VertexColorEnabled = true;
      Credits.RenderData.sEffect.TextureEnabled = false;
      Credits.RenderData.sEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      Credits.RenderData.sEffect.Begin();
      Credits.RenderData.sEffect.CurrentTechnique.Passes[0].Begin();
      Credits.RenderData.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);
      for (int startIndex = this.StartIndex; startIndex < this.EndIndex; ++startIndex)
      {
        Credits.RenderData.sEffect.Color = new Vector4(1f, 1f, 1f, this.TextDatas[startIndex].Alpha);
        this.Texts[startIndex].Draw(Credits.RenderData.sEffect, (float) screenSize.X * 0.5f, this.TextDatas[startIndex].Position);
      }
      Credits.RenderData.sEffect.CurrentTechnique.Passes[0].End();
      Credits.RenderData.sEffect.End();
      Credits.RenderData.sEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    }

    public int ZIndex => 200;
  }

  private struct CreditData
  {
    public MagickaFont FONT;
    public Vector3 COLOR;
    public float PADDING;
    public int ID;
  }

  protected struct ElementData
  {
    public float Alpha;
    public float Position;
    public float Padding;
    public float Height;
    public bool Index;
  }
}
