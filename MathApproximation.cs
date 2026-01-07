// Decompiled with JetBrains decompiler
// Type: PolygonHead.MathApproximation
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

#nullable disable
namespace PolygonHead;

public static class MathApproximation
{
  public static void FastSinCos(float iAngle, out float oSin, out float oCos)
  {
    oSin = (double) iAngle < 0.0 ? (float) (1.2732394933700562 * (double) iAngle + 0.40528473258018494 * (double) iAngle * (double) iAngle) : (float) (1.2732394933700562 * (double) iAngle - 0.40528473258018494 * (double) iAngle * (double) iAngle);
    iAngle += 1.57079637f;
    if ((double) iAngle > 3.1415927410125732)
      iAngle -= 6.28318548f;
    oCos = (double) iAngle < 0.0 ? (float) (1.2732394933700562 * (double) iAngle + 0.40528473258018494 * (double) iAngle * (double) iAngle) : (float) (1.2732394933700562 * (double) iAngle - 0.40528473258018494 * (double) iAngle * (double) iAngle);
  }

  public static void FastSin(float iAngle, out float oSin)
  {
    oSin = (double) iAngle < 0.0 ? (float) (1.2732394933700562 * (double) iAngle + 0.40528473258018494 * (double) iAngle * (double) iAngle) : (float) (1.2732394933700562 * (double) iAngle - 0.40528473258018494 * (double) iAngle * (double) iAngle);
  }
}
