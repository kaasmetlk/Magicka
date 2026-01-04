// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.LightEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PolygonHead.Lights;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct LightEvent
{
  public float Radius;
  public Vector3 DiffuseColor;
  public Vector3 AmbientColor;
  public float SpecularAmount;
  public LightVariationType VariationType;
  public float VariationAmount;
  public float VariationSpeed;

  public LightEvent(ContentReader iInput)
  {
    this.Radius = iInput.ReadSingle();
    this.DiffuseColor = iInput.ReadVector3();
    this.AmbientColor = iInput.ReadVector3();
    this.SpecularAmount = iInput.ReadSingle();
    this.VariationType = (LightVariationType) iInput.ReadByte();
    this.VariationAmount = iInput.ReadSingle();
    this.VariationSpeed = iInput.ReadSingle();
  }

  public LightEvent(
    float iRadius,
    Vector3 iDiffuseColor,
    Vector3 iAmbientColor,
    float iSpecularAmount)
  {
    this.Radius = iRadius;
    this.DiffuseColor = iDiffuseColor;
    this.AmbientColor = iAmbientColor;
    this.SpecularAmount = iSpecularAmount;
    this.VariationType = LightVariationType.None;
    this.VariationAmount = 0.0f;
    this.VariationSpeed = 0.0f;
  }

  public LightEvent(
    float iRadius,
    Vector3 iDiffuseColor,
    Vector3 iAmbientColor,
    float iSpecularAmount,
    LightVariationType iVariationType,
    float iVariationAmount,
    float iVariationSpeed)
  {
    this.Radius = iRadius;
    this.DiffuseColor = iDiffuseColor;
    this.AmbientColor = iAmbientColor;
    this.SpecularAmount = iSpecularAmount;
    this.VariationType = iVariationType;
    this.VariationAmount = iVariationAmount;
    this.VariationSpeed = iVariationSpeed;
  }

  public void Execute(Entity iItem, Entity iTarget)
  {
    if (!(iItem is MissileEntity))
      return;
    DynamicLight cachedLight = DynamicLight.GetCachedLight();
    cachedLight.Intensity = 1f;
    cachedLight.Position = iItem.Position;
    cachedLight.Radius = this.Radius;
    cachedLight.DiffuseColor = this.DiffuseColor;
    cachedLight.AmbientColor = this.AmbientColor;
    cachedLight.SpecularAmount = this.SpecularAmount;
    cachedLight.VariationType = this.VariationType;
    cachedLight.VariationAmount = this.VariationAmount;
    cachedLight.VariationSpeed = this.VariationSpeed;
    (iItem as MissileEntity).AddLight(cachedLight);
    cachedLight.Enable(iItem.PlayState.Scene);
  }
}
