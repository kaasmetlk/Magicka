using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PolygonHead.ParticleEffects;

namespace Magicka.Graphics
{
	// Token: 0x0200055F RID: 1375
	internal class TracerMan
	{
		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x060028FC RID: 10492 RVA: 0x00141FC8 File Offset: 0x001401C8
		public static TracerMan Instance
		{
			get
			{
				if (TracerMan.sSingelton == null)
				{
					lock (TracerMan.sSingeltonLock)
					{
						if (TracerMan.sSingelton == null)
						{
							TracerMan.sSingelton = new TracerMan();
						}
					}
				}
				return TracerMan.sSingelton;
			}
		}

		// Token: 0x060028FD RID: 10493 RVA: 0x0014201C File Offset: 0x0014021C
		private TracerMan()
		{
			this.mTracerTemplate.Drag = 1f;
			this.mTracerTemplate.Gravity = 0f;
			this.mTracerTemplate.Rotation = float.NaN;
			this.mTracerTemplate.RotationVelocity = float.NaN;
			this.mTracerTemplate.Drag = 1f;
			this.mTracerTemplate.Color = Vector4.One;
			this.mTracerTemplate.AlphaBlended = false;
			this.mTracerTemplate.Colorize = false;
		}

		// Token: 0x060028FE RID: 10494 RVA: 0x001420C1 File Offset: 0x001402C1
		public void Clear()
		{
			this.mDelays.Clear();
			this.mTracers.Clear();
		}

		// Token: 0x060028FF RID: 10495 RVA: 0x001420D9 File Offset: 0x001402D9
		public void AddTracer(ref Vector3 iSourcePos, ref Vector3 iTargetPos, float iVelocity, float iRadius, byte iSprite)
		{
			this.AddTracer(ref iSourcePos, ref iTargetPos, iVelocity, iRadius, iSprite, 0f);
		}

		// Token: 0x06002900 RID: 10496 RVA: 0x001420F0 File Offset: 0x001402F0
		public void AddTracer(ref Vector3 iSourcePos, ref Vector3 iTargetPos, float iVelocity, float iRadius, byte iSprite, float iDelay)
		{
			this.mTracerTemplate.Sprite = iSprite;
			Vector3 vector;
			Vector3.Subtract(ref iTargetPos, ref iSourcePos, out vector);
			float num = vector.Length();
			Vector3 vector2;
			Vector3.Divide(ref vector, num, out vector2);
			Vector3 vector3;
			Vector3.Multiply(ref vector2, iRadius, out vector3);
			this.mTracerTemplate.StartSize = iRadius;
			this.mTracerTemplate.EndSize = iRadius;
			this.mTracerTemplate.TTL = (num - iRadius * 2f) / iVelocity;
			Vector3.Add(ref iSourcePos, ref vector3, out this.mTracerTemplate.Position);
			Vector3.Multiply(ref vector2, iVelocity, out this.mTracerTemplate.Velocity);
			if (this.mTracerTemplate.TTL > 0f)
			{
				this.mTracers.Add(this.mTracerTemplate);
				this.mDelays.Add(iDelay);
			}
		}

		// Token: 0x06002901 RID: 10497 RVA: 0x001421B8 File Offset: 0x001403B8
		public void Update(float iDeltaTime)
		{
			for (int i = 0; i < this.mDelays.Count; i++)
			{
				float num = this.mDelays[i] - iDeltaTime;
				if (num <= 0f)
				{
					Particle particle = this.mTracers[i];
					ParticleSystem.Instance.SpawnParticle(ref particle);
					this.mDelays.RemoveAt(i);
					this.mTracers.RemoveAt(i);
					i--;
				}
				else
				{
					this.mDelays[i] = num;
				}
			}
		}

		// Token: 0x04002C62 RID: 11362
		private static TracerMan sSingelton;

		// Token: 0x04002C63 RID: 11363
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002C64 RID: 11364
		private List<float> mDelays = new List<float>(32);

		// Token: 0x04002C65 RID: 11365
		private List<Particle> mTracers = new List<Particle>(32);

		// Token: 0x04002C66 RID: 11366
		private Particle mTracerTemplate;
	}
}
