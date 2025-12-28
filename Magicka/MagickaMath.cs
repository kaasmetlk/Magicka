using System;
using System.Collections.Generic;
using System.Threading;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka
{
	// Token: 0x020000A8 RID: 168
	public static class MagickaMath
	{
		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x0001B684 File Offset: 0x00019884
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

		// Token: 0x060004B6 RID: 1206 RVA: 0x0001B6E8 File Offset: 0x000198E8
		public static float RandomBetween(float min, float max)
		{
			float num = (float)MagickaMath.Random.NextDouble();
			return min * (1f - num) + max * num;
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x0001B710 File Offset: 0x00019910
		public static float RandomBetween(Random rnd, float min, float max)
		{
			float num = (float)rnd.NextDouble();
			return min * (1f - num) + max * num;
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0001B732 File Offset: 0x00019932
		public static float SetBetween(float min, float max, float random)
		{
			return min * (1f - random) + max * random;
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0001B741 File Offset: 0x00019941
		public static Color MultiplyColor(Color a, Color b)
		{
			return new Color(a.ToVector4() * b.ToVector4());
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x0001B75C File Offset: 0x0001995C
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

		// Token: 0x060004BB RID: 1211 RVA: 0x0001B7E7 File Offset: 0x000199E7
		public static bool IsApproximately(float a, float b, float precision)
		{
			return Math.Abs(a - b) <= precision;
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0001B7F8 File Offset: 0x000199F8
		public static bool IsApproximately(ref Vector3 a, ref Vector3 b, float precision)
		{
			return MagickaMath.IsApproximately(a.X, b.X, precision) && MagickaMath.IsApproximately(a.Y, b.Y, precision) && MagickaMath.IsApproximately(a.Z, b.Z, precision);
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0001B844 File Offset: 0x00019A44
		public static float DistanceToLine(Vector2 iLineA, Vector2 iLineB, Vector2 iPoint)
		{
			Vector2 value = iLineB - iLineA;
			Vector2 value2 = iPoint - iLineA;
			float num = Vector2.Dot(value, value2);
			num *= num;
			float num2 = value2.LengthSquared();
			return (float)Math.Sqrt((double)(num2 - num));
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0001B880 File Offset: 0x00019A80
		public static bool ConstrainVector(ref Vector3 iVector, ref Vector3 iNormal, float iMinAngle, float iMaxAngle)
		{
			Vector2 vector = new Vector2(iVector.X, iVector.Z);
			Vector2 vector2 = new Vector2(iNormal.X, iNormal.Z);
			float num = MagickaMath.Angle(vector);
			float num2 = MagickaMath.Angle(vector2);
			float num3 = num - num2;
			if (num3 < iMinAngle)
			{
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(num3 - iMinAngle, 0f, 0f, out quaternion);
				Vector3.Transform(ref iVector, ref quaternion, out iVector);
				return true;
			}
			if (num3 > iMaxAngle)
			{
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(num3 - iMaxAngle, 0f, 0f, out quaternion);
				Vector3.Transform(ref iVector, ref quaternion, out iVector);
				return true;
			}
			return false;
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0001B911 File Offset: 0x00019B11
		public static Vector3 PixelsToScreen(Vector2 iPosition)
		{
			return MagickaMath.PixelsToScreen((int)iPosition.X, (int)iPosition.Y);
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0001B928 File Offset: 0x00019B28
		public static Vector3 PixelsToScreen(int iX, int iY)
		{
			return new Vector3
			{
				X = (float)iX / (float)Game.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth * 2f,
				Y = (float)iY / (float)Game.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight * 2f
			};
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0001B988 File Offset: 0x00019B88
		public static Vector2 ScreenToPixels(Vector3 iPosition)
		{
			return new Vector2
			{
				X = (1f + iPosition.X) * 0.5f * (float)Game.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth,
				Y = (float)Game.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight - (1f + iPosition.Y) * 0.5f * (float)Game.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight
			};
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0001BA14 File Offset: 0x00019C14
		public static Vector2 WorldToScreenPosition(ref Vector3 iWorldPosition, ref Matrix iViewProjectionMatrix)
		{
			Vector4 vector;
			Vector4.Transform(ref iWorldPosition, ref iViewProjectionMatrix, out vector);
			Point screenSize = RenderManager.Instance.ScreenSize;
			return new Vector2
			{
				X = (vector.X / vector.W * 0.5f + 0.5f) * (float)screenSize.X,
				Y = (vector.Y / vector.W * -0.5f + 0.5f) * (float)screenSize.Y
			};
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0001BA94 File Offset: 0x00019C94
		public static float InvSqrt(float x)
		{
			float num = 0.5f * x;
			int num2 = (int)x;
			num2 = 1597463007 - (num2 >> 1);
			x = (float)num2;
			x *= 1.5f - num * x * x;
			return x;
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001BACC File Offset: 0x00019CCC
		public static float InvSqrtTwoItr(float x)
		{
			float num = 0.5f * x;
			int num2 = (int)x;
			num2 = 1597463007 - (num2 >> 1);
			x = (float)num2;
			x *= 1.5f - num * x * x;
			x *= 1.5f - num * x * x;
			return x;
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001BB14 File Offset: 0x00019D14
		public static float Angle(Vector2 vector)
		{
			float num = (float)Math.Acos((double)MathHelper.Clamp(Vector2.Dot(vector, Vector2.UnitX), -1f, 1f));
			if (vector.Y < 0f)
			{
				num *= -1f;
			}
			return num;
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0001BB5C File Offset: 0x00019D5C
		public static float Angle(ref Vector2 vector, ref Vector2 secondVector)
		{
			float value;
			Vector2.Dot(ref vector, ref secondVector, out value);
			return (float)Math.Acos((double)MathHelper.Clamp(value, -1f, 1f));
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0001BB89 File Offset: 0x00019D89
		public static float Angle(Vector3 vector, Vector3 secondVector)
		{
			return MagickaMath.Angle(ref vector, ref secondVector);
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0001BB94 File Offset: 0x00019D94
		public static float Angle(ref Vector3 vector, ref Vector3 secondVector)
		{
			float value;
			Vector3.Dot(ref vector, ref secondVector, out value);
			return (float)Math.Acos((double)MathHelper.Clamp(value, -1f, 1f));
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0001BBC4 File Offset: 0x00019DC4
		public static void MakeOrientationMatrix(ref Vector3 iForward, out Matrix orientation)
		{
			Vector3 up = default(Vector3);
			up.Y = 1f;
			Vector3 right;
			Vector3.Cross(ref iForward, ref up, out right);
			right.Normalize();
			Vector3.Cross(ref right, ref iForward, out up);
			orientation = default(Matrix);
			orientation.Right = right;
			orientation.Up = up;
			orientation.Forward = iForward;
			orientation.M44 = 1f;
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0001BC2C File Offset: 0x00019E2C
		public static int CountTrailingZeroBits(uint iValue)
		{
			return MagickaMath.MULTIPLY_DEBRUIJN_BIT_POSITION[(int)((UIntPtr)((uint)(((ulong)iValue & -(ulong)iValue) * 125613361UL) >> 27))];
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0001BC46 File Offset: 0x00019E46
		public static void SegmentNegate(ref Segment iSeg, out Segment oSeg)
		{
			Vector3.Add(ref iSeg.Origin, ref iSeg.Delta, out oSeg.Origin);
			Vector3.Negate(ref iSeg.Delta, out oSeg.Delta);
		}

		// Token: 0x04000378 RID: 888
		public const float SQRT2 = 1.4142135f;

		// Token: 0x04000379 RID: 889
		private static readonly int[] MULTIPLY_DEBRUIJN_BIT_POSITION = new int[]
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
			31,
			27,
			13,
			23,
			21,
			19,
			16,
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

		// Token: 0x0400037A RID: 890
		private static volatile object locker = new object();

		// Token: 0x0400037B RID: 891
		private static Dictionary<int, Random> random = new Dictionary<int, Random>();
	}
}
