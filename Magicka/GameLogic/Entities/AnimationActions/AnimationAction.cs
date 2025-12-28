using System;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using XNAnimation;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x0200012F RID: 303
	public abstract class AnimationAction
	{
		// Token: 0x0600087C RID: 2172 RVA: 0x00036F3F File Offset: 0x0003513F
		protected AnimationAction(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
		{
			this.mStartTime = iInput.ReadSingle();
			this.mEndTime = iInput.ReadSingle();
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x00036F60 File Offset: 0x00035160
		protected static Type GetType(string name)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].BaseType == typeof(AnimationAction) && types[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return types[i];
				}
			}
			return null;
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x00036FB0 File Offset: 0x000351B0
		public static AnimationAction Read(Animations iAnimation, ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
		{
			string text = iInput.ReadString();
			Type type = AnimationAction.GetType(text);
			if (type == null)
			{
				throw new Exception("Invalid AnimationAction \"" + text + "\"!");
			}
			ConstructorInfo constructor = type.GetConstructor(new Type[]
			{
				typeof(ContentReader),
				typeof(SkinnedModelBoneCollection)
			});
			if (constructor == null)
			{
				throw new Exception("AnimationAction \"" + text + "\" + does not define a valid constructor!");
			}
			AnimationAction animationAction = (AnimationAction)constructor.Invoke(new object[]
			{
				iInput,
				iSkeleton
			});
			animationAction.mAnimation = iAnimation;
			return animationAction;
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x00037054 File Offset: 0x00035254
		public void Execute(Character iOwner, AnimationController iController, ref bool iHasExecuted, ref bool iIsDead)
		{
			float num = iController.Time / iController.AnimationClip.Duration;
			if (num >= this.mStartTime && (!iHasExecuted || num <= this.mEndTime))
			{
				this.InternalExecute(iOwner, !iHasExecuted);
				iHasExecuted = true;
				iIsDead = false;
				return;
			}
			if (num >= this.mEndTime & iHasExecuted & !iIsDead)
			{
				this.Kill(iOwner);
				iIsDead = true;
			}
		}

		// Token: 0x06000880 RID: 2176
		protected abstract void InternalExecute(Character iOwner, bool iFirstExecution);

		// Token: 0x06000881 RID: 2177 RVA: 0x000370C1 File Offset: 0x000352C1
		public virtual void Kill(Character iOwner)
		{
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000882 RID: 2178
		public abstract bool UsesBones { get; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000883 RID: 2179 RVA: 0x000370C3 File Offset: 0x000352C3
		public Animations Animation
		{
			get
			{
				return this.mAnimation;
			}
		}

		// Token: 0x04000805 RID: 2053
		protected float mStartTime;

		// Token: 0x04000806 RID: 2054
		protected float mEndTime;

		// Token: 0x04000807 RID: 2055
		protected Animations mAnimation;
	}
}
