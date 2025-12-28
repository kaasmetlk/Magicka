using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000300 RID: 768
	public struct AuraDeflect
	{
		// Token: 0x060017A2 RID: 6050 RVA: 0x0009BC68 File Offset: 0x00099E68
		public AuraDeflect(float iStrength)
		{
			this.Strength = iStrength;
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x0009BC71 File Offset: 0x00099E71
		public AuraDeflect(ContentReader iInput)
		{
			this.Strength = iInput.ReadSingle();
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x0009BC80 File Offset: 0x00099E80
		public float Execute(Entity iOwner, float iDeltaTime, AuraTarget iAuraTarget, int iEffect, float iRadius)
		{
			Vector3 position = iOwner.Position;
			Vector3 up = Vector3.Up;
			List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(position, this.Strength, false);
			foreach (Entity entity in entities)
			{
				MissileEntity missileEntity = entity as MissileEntity;
				if (missileEntity != null && !missileEntity.Dead)
				{
					Vector3 position2 = missileEntity.Position;
					Vector3 velocity;
					Vector3.Subtract(ref position2, ref position, out velocity);
					float num = velocity.Length();
					velocity.Y = 0f;
					velocity.Normalize();
					Vector3 velocity2 = missileEntity.Body.Velocity;
					float angle = (float)(AuraDeflect.RANDOM.NextDouble() - 0.5) * 0.7853982f;
					Quaternion quaternion;
					Quaternion.CreateFromAxisAngle(ref up, angle, out quaternion);
					Vector3.Transform(ref velocity, ref quaternion, out velocity);
					Vector3.Multiply(ref velocity, velocity2.Length() * iDeltaTime * 100f * (1f - num / this.Strength), out velocity);
					Vector3.Add(ref velocity, ref velocity2, out velocity);
					missileEntity.Body.Velocity = velocity;
				}
			}
			iOwner.PlayState.EntityManager.ReturnEntityList(entities);
			return 1f;
		}

		// Token: 0x04001964 RID: 6500
		private static readonly Random RANDOM = new Random();

		// Token: 0x04001965 RID: 6501
		public float Strength;
	}
}
