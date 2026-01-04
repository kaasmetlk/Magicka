// Decompiled with JetBrains decompiler
// Type: Magicka.MagickaMath
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Magicka;

public static class MagickaMath
{
  public const float SQRT2 = 1.41421354f;
  private static readonly int[] MULTIPLY_DEBRUIJN_BIT_POSITION = new int[32 /*0x20*/]
  {
    0,
    1,
    28,
    2,
    29,
    14,
    24,
    3,
    30,
    22,
    20,
    15,
    25,
    17,
    4,
    8,
    31 /*0x1F*/,
    27,
    13,
    23,
    21,
    19,
    16 /*0x10*/,
    7,
    26,
    12,
    18,
    6,
    11,
    5,
    10,
    9
  };
  private static volatile object locker = new object();
  private static Dictionary<int, Random> random = new Dictionary<int, Random>();

  public static Random Random
  {
    get
    {
      int managedThreadId = Thread.CurrentThread.ManagedThreadId;
      Random random;
      if (!MagickaMath.random.TryGetValue(managedThreadId, out random))
      {
        lock (MagickaMath.locker)
        {
          random = new Random();
          MagickaMath.random.Add(managedThreadId, random);
        }
      }
      return random;
    }
  }

  public static float RandomBetween(float min, float max)
  {
    float num = (float) MagickaMath.Random.NextDouble();
    return (float) ((double) min * (1.0 - (double) num) + (double) max * (double) num);
  }

  public static float RandomBetween(Random rnd, float min, float max)
  {
    float num = (float) rnd.NextDouble();
    return (float) ((double) min * (1.0 - (double) num) + (double) max * (double) num);
  }

  public static float SetBetween(float min, float max, float random)
  {
    return (float) ((double) min * (1.0 - (double) random) + (double) max * (double) random);
  }

  public static Color MultiplyColor(Color a, Color b) => new Color(a.ToVector4() * b.ToVector4());

  public static void UniformMatrixScale(ref Matrix iM, float iScale)
  {
    iM.M11 *= iScale;
    iM.M12 *= iScale;
    iM.M13 *= iScale;
    iM.M21 *= iScale;
    iM.M22 *= iScale;
    iM.M23 *= iScale;
    iM.M31 *= iScale;
    iM.M32 *= iScale;
    iM.M33 *= iScale;
  }

  public static bool IsApproximately(float a, float b, float precision)
  {
    return (double) Math.Abs(a - b) <= (double) precision;
  }

  public static bool IsApproximately(ref Vector3 a, ref Vector3 b, float precision)
  {
    return MagickaMath.IsApproximately(a.X, b.X, precision) && MagickaMath.IsApproximately(a.Y, b.Y, precision) && MagickaMath.IsApproximately(a.Z, b.Z, precision);
  }

  public static float DistanceToLine(Vector2 iLineA, Vector2 iLineB, Vector2 iPoint)
  {
    Vector2 vector2_1 = iLineB - iLineA;
    Vector2 vector2_2 = iPoint - iLineA;
    float num1 = Vector2.Dot(vector2_1, vector2_2);
    float num2 = num1 * num1;
    return (float) Math.Sqrt((double) vector2_2.LengthSquared() - (double) num2);
  }

  public static bool ConstrainVector(
    ref Vector3 iVector,
    ref Vector3 iNormal,
    float iMinAngle,
    float iMaxAngle)
  {
    Vector2 vector1 = new Vector2(iVector.X, iVector.Z);
    Vector2 vector2 = new Vector2(iNormal.X, iNormal.Z);
    float num = MagickaMath.Angle(vector1) - MagickaMath.Angle(vector2);
    if ((double) num < (double) iMinAngle)
    {
      Quaternion result;
      Quaternion.CreateFromYawPitchRoll(num - iMinAngle, 0.0f, 0.0f, out result);
      Vector3.Transform(ref iVector, ref result, out iVector);
      return true;
    }
    if ((double) num <= (double) iMaxAngle)
      return false;
    Quaternion result1;
    Quaternion.CreateFromYawPitchRoll(num - iMaxAngle, 0.0f, 0.0f, out result1);
    Vector3.Transform(ref iVector, ref result1, out iVector);
    return true;
  }

  public static Vector3 PixelsToScreen(Vector2 iPosition)
  {
    return MagickaMath.PixelsToScreen((int) iPosition.X, (int) iPosition.Y);
  }

  public static Vector3 PixelsToScreen(int iX, int iY)
  {
    return new Vector3()
    {
      X = (float) ((double) iX / (double) Game.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth * 2.0),
      Y = (float) ((double) iY / (double) Game.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight * 2.0)
    };
  }

  public static Vector2 ScreenToPixels(Vector3 iPosition)
  {
    return new Vector2()
    {
      X = (float) ((1.0 + (double) iPosition.X) * 0.5) * (float) Game.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth,
      Y = (float) Game.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight - (float) ((1.0 + (double) iPosition.Y) * 0.5) * (float) Game.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight
    };
  }

  public static Vector2 WorldToScreenPosition(
    ref Vector3 iWorldPosition,
    ref Matrix iViewProjectionMatrix)
  {
    Vector4 result;
    Vector4.Transform(ref iWorldPosition, ref iViewProjectionMatrix, out result);
    Point screenSize = RenderManager.Instance.ScreenSize;
    return new Vector2()
    {
      X = (float) ((double) result.X / (double) result.W * 0.5 + 0.5) * (float) screenSize.X,
      Y = (float) ((double) result.Y / (double) result.W * -0.5 + 0.5) * (float) screenSize.Y
    };
  }

  public static float InvSqrt(float x)
  {
    float num = 0.5f * x;
    x = (float) (1597463007 - ((int) x >> 1));
    x *= (float) (1.5 - (double) num * (double) x * (double) x);
    return x;
  }

  public static float InvSqrtTwoItr(float x)
  {
    float num = 0.5f * x;
    x = (float) (1597463007 - ((int) x >> 1));
    x *= (float) (1.5 - (double) num * (double) x * (double) x);
    x *= (float) (1.5 - (double) num * (double) x * (double) x);
    return x;
  }

  public static float Angle(Vector2 vector)
  {
    float num = (float) Math.Acos((double) MathHelper.Clamp(Vector2.Dot(vector, Vector2.UnitX), -1f, 1f));
    if ((double) vector.Y < 0.0)
      num *= -1f;
    return num;
  }

  public static float Angle(ref Vector2 vector, ref Vector2 secondVector)
  {
    float result;
    Vector2.Dot(ref vector, ref secondVector, out result);
    return (float) Math.Acos((double) MathHelper.Clamp(result, -1f, 1f));
  }

  public static float Angle(Vector3 vector, Vector3 secondVector)
  {
    return MagickaMath.Angle(ref vector, ref secondVector);
  }

  public static float Angle(ref Vector3 vector, ref Vector3 secondVector)
  {
    float result;
    Vector3.Dot(ref vector, ref secondVector, out result);
    return (float) Math.Acos((double) MathHelper.Clamp(result, -1f, 1f));
  }

  public static void MakeOrientationMatrix(ref Vector3 iForward, out Matrix orientation)
  {
    Vector3 result1 = new Vector3();
    result1.Y = 1f;
    Vector3 result2;
    Vector3.Cross(ref iForward, ref result1, out result2);
    result2.Normalize();
    Vector3.Cross(ref result2, ref iForward, out result1);
    orientation = new Matrix();
    orientation.Right = result2;
    orientation.Up = result1;
    orientation.Forward = iForward;
    orientation.M44 = 1f;
  }

  public static int CountTrailingZeroBits(uint iValue)
  {
    return MagickaMath.MULTIPLY_DEBRUIJN_BIT_POSITION[(IntPtr) ((uint) (((ulong) iValue & (ulong) -iValue) * 125613361UL) >> 27)];
  }

  public static void SegmentNegate(ref Segment iSeg, out Segment oSeg)
  {
    Vector3.Add(ref iSeg.Origin, ref iSeg.Delta, out oSeg.Origin);
    Vector3.Negate(ref iSeg.Delta, out oSeg.Delta);
  }
}
