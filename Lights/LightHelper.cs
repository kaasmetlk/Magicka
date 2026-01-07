// Decompiled with JetBrains decompiler
// Type: PolygonHead.Lights.LightHelper
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace PolygonHead.Lights;

internal class LightHelper
{
  public const int SPHERE_VERTEX_COUNT = 42;
  public const int SPHERE_VERTEX_STRIDE = 12;
  public const int SPHERE_PRIMITIVE_COUNT = 80 /*0x50*/;
  public const int BOUNDING_SPHERE_DIVISIONS = 4;
  public const int BOUNDING_SPHERE_PRIMITIVES = 48 /*0x30*/;
  public const int BOUNDING_SPHERE_VERTEX_COUNT = 49;
  public const int BOUNDING_SPHERE_VERTEX_STRIDE = 12;
  private static VertexBuffer sVertexBuffer;
  private static VertexDeclaration sVertexDeclaration;
  private static IndexBuffer sIndexBuffer;
  public static readonly VertexElement[] VERTEX_ELEMENTS = new VertexElement[2]
  {
    new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
    new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, (byte) 0)
  };

  public static void CreateSphere(
    GraphicsDevice iDevice,
    out VertexBuffer oVertexBuffer,
    out VertexDeclaration oVertexDeclaration,
    out IndexBuffer oIndexBuffer)
  {
    if (LightHelper.sVertexBuffer != null && !LightHelper.sVertexBuffer.IsDisposed && LightHelper.sVertexBuffer.GraphicsDevice != iDevice)
    {
      LightHelper.sVertexBuffer.Dispose();
      LightHelper.sVertexDeclaration.Dispose();
      LightHelper.sIndexBuffer.Dispose();
    }
    if (LightHelper.sVertexBuffer == null || LightHelper.sVertexBuffer.IsDisposed)
    {
      Vector3[] data1 = new Vector3[42];
      int num1 = 0;
      Vector3[] vector3Array1 = data1;
      int index1 = num1;
      int index2 = index1 + 1;
      vector3Array1[index1] = Vector3.Up;
      double num2 = -1.0 * Math.PI / 10.0;
      double num3 = Math.PI / 6.0;
      for (int index3 = 0; index3 < 5; ++index3)
      {
        double num4 = (double) index3 * (2.0 * Math.PI / 5.0) + num2;
        data1[index2].Y = (float) Math.Cos(num3);
        double num5 = Math.Sin(num3);
        data1[index2].X = (float) (Math.Cos(num4) * num5);
        data1[index2].Z = (float) (Math.Sin(num4) * num5);
        ++index2;
      }
      double num6 = -1.0 * Math.PI / 10.0;
      double num7 = Math.PI / 3.0;
      for (int index4 = 0; index4 < 3; ++index4)
      {
        for (int index5 = 0; index5 < 10; ++index5)
        {
          double num8 = (double) index5 * (Math.PI / 5.0) + num6;
          data1[index2].Y = (float) Math.Cos(num7);
          double num9 = Math.Sin(num7);
          data1[index2].X = (float) (Math.Cos(num8) * num9);
          data1[index2].Z = (float) (Math.Sin(num8) * num9);
          ++index2;
        }
        num6 += Math.PI / 10.0;
        num7 += Math.PI / 6.0;
      }
      double num10 = num6 - Math.PI / 10.0;
      for (int index6 = 0; index6 < 5; ++index6)
      {
        double num11 = (double) index6 * (2.0 * Math.PI / 5.0) + num10;
        data1[index2].Y = (float) Math.Cos(num7);
        double num12 = Math.Sin(num7);
        data1[index2].X = (float) (Math.Cos(num11) * num12);
        data1[index2].Z = (float) (Math.Sin(num11) * num12);
        ++index2;
      }
      Vector3[] vector3Array2 = data1;
      int index7 = index2;
      int num13 = index7 + 1;
      vector3Array2[index7] = Vector3.Down;
      ushort[] data2 = new ushort[240 /*0xF0*/];
      int num14 = 0;
      for (int index8 = 0; index8 < 5; ++index8)
      {
        ushort[] numArray1 = data2;
        int index9 = num14;
        int num15 = index9 + 1;
        numArray1[index9] = (ushort) 0;
        ushort[] numArray2 = data2;
        int index10 = num15;
        int num16 = index10 + 1;
        int num17 = (int) (ushort) (index8 + 1);
        numArray2[index10] = (ushort) num17;
        ushort[] numArray3 = data2;
        int index11 = num16;
        num14 = index11 + 1;
        int num18 = (int) (ushort) ((index8 + 1) % 5 + 1);
        numArray3[index11] = (ushort) num18;
      }
      for (int index12 = 0; index12 < 5; ++index12)
      {
        ushort[] numArray4 = data2;
        int index13 = num14;
        int num19 = index13 + 1;
        int num20 = (int) (ushort) (index12 + 1);
        numArray4[index13] = (ushort) num20;
        ushort[] numArray5 = data2;
        int index14 = num19;
        int num21 = index14 + 1;
        int num22 = (int) (ushort) (index12 * 2 + 6);
        numArray5[index14] = (ushort) num22;
        ushort[] numArray6 = data2;
        int index15 = num21;
        int num23 = index15 + 1;
        int num24 = (int) (ushort) (index12 * 2 + 7);
        numArray6[index15] = (ushort) num24;
        ushort[] numArray7 = data2;
        int index16 = num23;
        int num25 = index16 + 1;
        int num26 = (int) (ushort) (index12 + 1);
        numArray7[index16] = (ushort) num26;
        ushort[] numArray8 = data2;
        int index17 = num25;
        int num27 = index17 + 1;
        int num28 = (int) (ushort) (index12 * 2 + 7);
        numArray8[index17] = (ushort) num28;
        ushort[] numArray9 = data2;
        int index18 = num27;
        int num29 = index18 + 1;
        int num30 = (int) (ushort) ((index12 + 1) % 5 + 1);
        numArray9[index18] = (ushort) num30;
        ushort[] numArray10 = data2;
        int index19 = num29;
        int num31 = index19 + 1;
        int num32 = (int) (ushort) ((index12 + 1) % 5 + 1);
        numArray10[index19] = (ushort) num32;
        ushort[] numArray11 = data2;
        int index20 = num31;
        int num33 = index20 + 1;
        int num34 = (int) (ushort) (index12 * 2 + 7);
        numArray11[index20] = (ushort) num34;
        ushort[] numArray12 = data2;
        int index21 = num33;
        num14 = index21 + 1;
        int num35 = (int) (ushort) ((index12 * 2 + 2) % 10 + 6);
        numArray12[index21] = (ushort) num35;
      }
      for (int index22 = 0; index22 < 2; ++index22)
      {
        for (int index23 = 0; index23 < 5; ++index23)
        {
          ushort[] numArray13 = data2;
          int index24 = num14;
          int num36 = index24 + 1;
          int num37 = (int) (ushort) (index23 * 2 + 6 + index22 * 10);
          numArray13[index24] = (ushort) num37;
          ushort[] numArray14 = data2;
          int index25 = num36;
          int num38 = index25 + 1;
          int num39 = (int) (ushort) (index23 * 2 + 16 /*0x10*/ + index22 * 10);
          numArray14[index25] = (ushort) num39;
          ushort[] numArray15 = data2;
          int index26 = num38;
          int num40 = index26 + 1;
          int num41 = (int) (ushort) (index23 * 2 + 7 + index22 * 10);
          numArray15[index26] = (ushort) num41;
          ushort[] numArray16 = data2;
          int index27 = num40;
          int num42 = index27 + 1;
          int num43 = (int) (ushort) (index23 * 2 + 7 + index22 * 10);
          numArray16[index27] = (ushort) num43;
          ushort[] numArray17 = data2;
          int index28 = num42;
          int num44 = index28 + 1;
          int num45 = (int) (ushort) (index23 * 2 + 16 /*0x10*/ + index22 * 10);
          numArray17[index28] = (ushort) num45;
          ushort[] numArray18 = data2;
          int index29 = num44;
          int num46 = index29 + 1;
          int num47 = (int) (ushort) (index23 * 2 + 17 + index22 * 10);
          numArray18[index29] = (ushort) num47;
          ushort[] numArray19 = data2;
          int index30 = num46;
          int num48 = index30 + 1;
          int num49 = (int) (ushort) (index23 * 2 + 7 + index22 * 10);
          numArray19[index30] = (ushort) num49;
          ushort[] numArray20 = data2;
          int index31 = num48;
          int num50 = index31 + 1;
          int num51 = (int) (ushort) (index23 * 2 + 17 + index22 * 10);
          numArray20[index31] = (ushort) num51;
          ushort[] numArray21 = data2;
          int index32 = num50;
          int num52 = index32 + 1;
          int num53 = (int) (ushort) ((index23 * 2 + 2) % 10 + 6 + index22 * 10);
          numArray21[index32] = (ushort) num53;
          ushort[] numArray22 = data2;
          int index33 = num52;
          int num54 = index33 + 1;
          int num55 = (int) (ushort) ((index23 * 2 + 2) % 10 + 6 + index22 * 10);
          numArray22[index33] = (ushort) num55;
          ushort[] numArray23 = data2;
          int index34 = num54;
          int num56 = index34 + 1;
          int num57 = (int) (ushort) (index23 * 2 + 17 + index22 * 10);
          numArray23[index34] = (ushort) num57;
          ushort[] numArray24 = data2;
          int index35 = num56;
          num14 = index35 + 1;
          int num58 = (int) (ushort) ((index23 * 2 + 2) % 10 + 16 /*0x10*/ + index22 * 10);
          numArray24[index35] = (ushort) num58;
        }
      }
      for (int index36 = 0; index36 < 5; ++index36)
      {
        ushort[] numArray25 = data2;
        int index37 = num14;
        int num59 = index37 + 1;
        int num60 = (int) (ushort) (index36 * 2 + 26);
        numArray25[index37] = (ushort) num60;
        ushort[] numArray26 = data2;
        int index38 = num59;
        int num61 = index38 + 1;
        int num62 = (int) (ushort) (index36 + 36);
        numArray26[index38] = (ushort) num62;
        ushort[] numArray27 = data2;
        int index39 = num61;
        int num63 = index39 + 1;
        int num64 = (int) (ushort) (index36 * 2 + 27);
        numArray27[index39] = (ushort) num64;
        ushort[] numArray28 = data2;
        int index40 = num63;
        int num65 = index40 + 1;
        int num66 = (int) (ushort) (index36 * 2 + 27);
        numArray28[index40] = (ushort) num66;
        ushort[] numArray29 = data2;
        int index41 = num65;
        int num67 = index41 + 1;
        int num68 = (int) (ushort) (index36 + 36);
        numArray29[index41] = (ushort) num68;
        ushort[] numArray30 = data2;
        int index42 = num67;
        int num69 = index42 + 1;
        int num70 = (int) (ushort) ((index36 + 1) % 5 + 36);
        numArray30[index42] = (ushort) num70;
        ushort[] numArray31 = data2;
        int index43 = num69;
        int num71 = index43 + 1;
        int num72 = (int) (ushort) (index36 * 2 + 27);
        numArray31[index43] = (ushort) num72;
        ushort[] numArray32 = data2;
        int index44 = num71;
        int num73 = index44 + 1;
        int num74 = (int) (ushort) ((index36 + 1) % 5 + 36);
        numArray32[index44] = (ushort) num74;
        ushort[] numArray33 = data2;
        int index45 = num73;
        num14 = index45 + 1;
        int num75 = (int) (ushort) ((index36 * 2 + 2) % 10 + 26);
        numArray33[index45] = (ushort) num75;
      }
      for (int index46 = 0; index46 < 5; ++index46)
      {
        ushort[] numArray34 = data2;
        int index47 = num14;
        int num76 = index47 + 1;
        int num77 = (int) (ushort) (index46 + 36);
        numArray34[index47] = (ushort) num77;
        ushort[] numArray35 = data2;
        int index48 = num76;
        int num78 = index48 + 1;
        numArray35[index48] = (ushort) 41;
        ushort[] numArray36 = data2;
        int index49 = num78;
        num14 = index49 + 1;
        int num79 = (int) (ushort) ((index46 + 1) % 5 + 36);
        numArray36[index49] = (ushort) num79;
      }
      lock (iDevice)
      {
        LightHelper.sVertexBuffer = new VertexBuffer(iDevice, data1.Length * 12, BufferUsage.WriteOnly);
        LightHelper.sVertexBuffer.SetData<Vector3>(data1);
        LightHelper.sIndexBuffer = new IndexBuffer(iDevice, 480, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
        LightHelper.sIndexBuffer.SetData<ushort>(data2);
        LightHelper.sVertexDeclaration = new VertexDeclaration(iDevice, LightHelper.VERTEX_ELEMENTS);
      }
    }
    oVertexBuffer = LightHelper.sVertexBuffer;
    oVertexDeclaration = LightHelper.sVertexDeclaration;
    oIndexBuffer = LightHelper.sIndexBuffer;
  }

  public static void CreateSphereBounding(out Vector3[] oVertices, out ushort[] oIndices)
  {
    oVertices = new Vector3[49];
    Quaternion result1 = Quaternion.Identity;
    Quaternion result2;
    Quaternion.CreateFromYawPitchRoll(0.3926991f, 0.0f, 0.0f, out result2);
    Quaternion result3;
    Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, 1.57079637f, out result3);
    Quaternion result4;
    Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, -1.57079637f, out result4);
    Vector3 forward = Vector3.Forward;
    int index1 = 0;
    for (int index2 = 0; index2 < 12; ++index2)
    {
      int num = 0;
      while (num < 4)
      {
        Vector3.Transform(ref forward, ref result1, out oVertices[index1]);
        Quaternion.Concatenate(ref result2, ref result1, out result1);
        ++num;
        ++index1;
      }
      if (index2 % 4 < 2)
        Quaternion.Concatenate(ref result3, ref result1, out result1);
      else
        Quaternion.Concatenate(ref result4, ref result1, out result1);
    }
    oVertices[oVertices.Length - 1] = oVertices[0];
    oIndices = new ushort[49];
    for (ushort index3 = 0; (int) index3 < oIndices.Length; ++index3)
      oIndices[(int) index3] = index3;
  }
}
