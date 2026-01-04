// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.AchievementUI
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.IO;

#nullable disable
namespace Magicka.Achievements;

public class AchievementUI
{
  private static AchievementUI sSingelton;
  private static volatile object sSingeltonLock = new object();
  private Rectangle mPopupRect;
  private Texture2D mPopupTexture;
  private Texture2D mWidgetTexture;

  public static AchievementUI Instance
  {
    get
    {
      if (AchievementUI.sSingelton == null)
      {
        lock (AchievementUI.sSingeltonLock)
        {
          if (AchievementUI.sSingelton == null)
            AchievementUI.sSingelton = new AchievementUI();
        }
      }
      return AchievementUI.sSingelton;
    }
  }

  private AchievementUI()
  {
    string str = "achievementsUI/";
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    this.mPopupTexture = Texture2D.FromFile(graphicsDevice, str + "popup.png");
    TextReader textReader1 = (TextReader) File.OpenText(str + "popup.txt");
    int num1 = int.Parse(textReader1.ReadLine());
    if (num1 < 0 || num1 > 1)
    {
      textReader1.Close();
      throw new Exception("Invalid value in popup.txt");
    }
    textReader1.ReadLine();
    this.mPopupRect.X = int.Parse(textReader1.ReadLine());
    this.mPopupRect.Y = int.Parse(textReader1.ReadLine());
    this.mPopupRect.Width = int.Parse(textReader1.ReadLine());
    this.mPopupRect.Height = int.Parse(textReader1.ReadLine());
    textReader1.Close();
    this.mWidgetTexture = Texture2D.FromFile(graphicsDevice, str + "widget.png");
    TextReader textReader2 = (TextReader) File.OpenText(str + "widget.txt");
    int num2 = int.Parse(textReader2.ReadLine());
    for (int index = 0; index < num2; ++index)
    {
      textReader2.ReadLine().ToLowerInvariant();
      Rectangle rectangle = new Rectangle()
      {
        X = int.Parse(textReader2.ReadLine()),
        Y = int.Parse(textReader2.ReadLine()),
        Width = int.Parse(textReader2.ReadLine()),
        Height = int.Parse(textReader2.ReadLine())
      };
    }
  }

  private class WidgetRenderData : IRenderableGUIObject
  {
    public void Draw(float iDeltaTime) => throw new NotImplementedException();

    public int ZIndex => throw new NotImplementedException();
  }

  private class PopupRenderData : IRenderableGUIObject
  {
    public void Draw(float iDeltaTime) => throw new NotImplementedException();

    public int ZIndex => throw new NotImplementedException();
  }
}
