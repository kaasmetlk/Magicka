using System;
using System.Reflection;
using Magicka.AI;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.MoveAbilities
{
	// Token: 0x020000E3 RID: 227
	public abstract class MoveAbility
	{
		// Token: 0x060006FF RID: 1791 RVA: 0x000294C0 File Offset: 0x000276C0
		protected MoveAbility(ContentReader iInput)
		{
			this.mAnimationKeys = new Animations[iInput.ReadInt32()];
			for (int i = 0; i < this.mAnimationKeys.Length; i++)
			{
				this.mAnimationKeys[i] = (Animations)Enum.Parse(typeof(Animations), iInput.ReadString(), true);
			}
		}

		// Token: 0x06000700 RID: 1792
		public abstract void Execute(Agent iAgent);

		// Token: 0x06000701 RID: 1793
		public abstract void Update(Agent iAgent, float iDeltaTime);

		// Token: 0x06000702 RID: 1794 RVA: 0x0002951C File Offset: 0x0002771C
		protected static Type GetType(string name)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].BaseType == typeof(MoveAbility) && types[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return types[i];
				}
			}
			return null;
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x0002956C File Offset: 0x0002776C
		public static MoveAbility Read(ContentReader iInput)
		{
			Type type = MoveAbility.GetType(iInput.ReadString());
			return (MoveAbility)type.GetConstructor(new Type[]
			{
				typeof(ContentReader)
			}).Invoke(new object[]
			{
				iInput
			});
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x000295B8 File Offset: 0x000277B8
		public virtual float GetFuzzyWeight(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06000705 RID: 1797
		public abstract float GetMaxRange();

		// Token: 0x06000706 RID: 1798
		public abstract float GetMinRange();

		// Token: 0x06000707 RID: 1799
		public abstract float GetAngle();

		// Token: 0x06000708 RID: 1800
		public abstract float GetForceMultiplier();

		// Token: 0x040005B6 RID: 1462
		protected Animations[] mAnimationKeys;
	}
}
