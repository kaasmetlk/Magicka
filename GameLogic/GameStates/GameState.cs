// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.GameState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.GameStates;

public abstract class GameState
{
  protected Scene mScene;

  public GameState(Camera iCamera)
  {
    this.mScene = new Scene(Game.Instance.GraphicsDevice, 512 /*0x0200*/, iCamera);
  }

  public abstract void OnEnter();

  public abstract void OnExit();

  public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mScene.ClearObjects(iDataChannel);
    ControlManager.Instance.HandleInput(iDataChannel, iDeltaTime);
  }

  public Scene Scene => this.mScene;
}
