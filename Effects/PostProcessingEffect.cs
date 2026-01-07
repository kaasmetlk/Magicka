// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.PostProcessingEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public abstract class PostProcessingEffect : Effect
{
  protected VertexBuffer mVertexBuffer;
  protected VertexDeclaration mVertexDeclaration;
  protected EffectParameter mSourceTexture0Parameter;
  protected EffectParameter mSourceTexture1Parameter;
  protected EffectParameter mSourceTexture2Parameter;
  protected EffectParameter mSourceTexture3Parameter;
  protected EffectParameter mDestinationDimensionsParameter;

  protected PostProcessingEffect(
    GraphicsDevice iGraphicsDevice,
    byte[] iEffectCode,
    EffectPool iPool)
    : base(iGraphicsDevice, iEffectCode, CompilerOptions.NotCloneable, iPool)
  {
    this.CacheParameters();
    this.CreateVertices();
  }

  protected PostProcessingEffect(GraphicsDevice iGraphicsDevice, Effect iCloneSource)
    : base(iGraphicsDevice, iCloneSource)
  {
    this.CacheParameters();
    this.CreateVertices();
  }

  public virtual void CacheParameters()
  {
    this.mSourceTexture0Parameter = this.Parameters["SourceTexture0"];
    this.mSourceTexture1Parameter = this.Parameters["SourceTexture1"];
    this.mSourceTexture2Parameter = this.Parameters["SourceTexture2"];
    this.mSourceTexture3Parameter = this.Parameters["SourceTexture3"];
    this.mDestinationDimensionsParameter = this.Parameters["DestinationDimensions"];
  }

  public virtual void CreateVertices()
  {
    this.mVertexBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionTexture.SizeInBytes * 4, BufferUsage.WriteOnly);
    this.mVertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionTexture.VertexElements);
    VertexPositionTexture[] data = new VertexPositionTexture[4];
    data[0].Position = new Vector3(1.5f, 1.5f, 1f);
    data[0].TextureCoordinate = new Vector2(1.25f, -0.25f);
    data[1].Position = new Vector3(1.5f, -1.5f, 1f);
    data[1].TextureCoordinate = new Vector2(1.25f, 1.25f);
    data[2].Position = new Vector3(-1.5f, 1.5f, 1f);
    data[2].TextureCoordinate = new Vector2(-0.25f, -0.25f);
    data[3].Position = new Vector3(-1.5f, -1.5f, 1f);
    data[3].TextureCoordinate = new Vector2(-0.25f, 1.25f);
    this.mVertexBuffer.SetData<VertexPositionTexture>(data);
  }

  public Texture2D SourceTexture0
  {
    get => this.mSourceTexture0Parameter.GetValueTexture2D();
    set => this.mSourceTexture0Parameter.SetValue((Texture) value);
  }

  public Texture2D SourceTexture1
  {
    get => this.mSourceTexture1Parameter.GetValueTexture2D();
    set => this.mSourceTexture1Parameter.SetValue((Texture) value);
  }

  public Texture2D SourceTexture2
  {
    get => this.mSourceTexture2Parameter.GetValueTexture2D();
    set => this.mSourceTexture2Parameter.SetValue((Texture) value);
  }

  public Texture2D SourceTexture3
  {
    get => this.mSourceTexture3Parameter.GetValueTexture2D();
    set => this.mSourceTexture3Parameter.SetValue((Texture) value);
  }

  public Vector2 DestinationDimensions
  {
    get => this.mDestinationDimensionsParameter.GetValueVector2();
    set => this.mDestinationDimensionsParameter.SetValue(value);
  }

  public VertexBuffer VertexBuffer => this.mVertexBuffer;

  public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;
}
