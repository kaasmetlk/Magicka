using System;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells.SpellEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000404 RID: 1028
	internal class Potion : SpecialAbility
	{
		// Token: 0x06001FAC RID: 8108 RVA: 0x000DE3D4 File Offset: 0x000DC5D4
		public Potion(Animations iAnimation) : base(Animations.special0, Potion.DISPLAY_NAME)
		{
			this.mHealing = new Damage(AttackProperties.Damage, Elements.Life, -this.HITPOINTS, 1f);
			if (Potion.sModel == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					Potion.sModel = Game.Instance.Content.Load<Model>("Models/Missiles/flask_healing_potion");
				}
			}
		}

		// Token: 0x06001FAD RID: 8109 RVA: 0x000DE460 File Offset: 0x000DC660
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Potion have to be cast by a character!");
		}

		// Token: 0x06001FAE RID: 8110 RVA: 0x000DE46C File Offset: 0x000DC66C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			iOwner.Damage(this.mHealing, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
			return true;
		}

		// Token: 0x06001FAF RID: 8111 RVA: 0x000DE4A4 File Offset: 0x000DC6A4
		internal static void SpawnFlask(ref MissileEntity iMissile, ISpellCaster iOwner, ref Vector3 iPosition, ref Vector3 iVelocity)
		{
			ConditionCollection conditionCollection;
			lock (ProjectileSpell.sCachedConditions)
			{
				conditionCollection = ProjectileSpell.sCachedConditions.Dequeue();
			}
			conditionCollection.Clear();
			conditionCollection[0].Condition.EventConditionType = EventConditionType.Damaged;
			conditionCollection[0].Condition.Elements = Elements.None;
			conditionCollection[0].Condition.Hitpoints = 20f;
			conditionCollection[0].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[1].Condition.EventConditionType = EventConditionType.Timer;
			conditionCollection[1].Condition.Time = Potion.FLASK_TTL;
			conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[2].Condition.EventConditionType = EventConditionType.Default;
			conditionCollection[2].Condition.Repeat = true;
			iMissile.Initialize(iOwner as Entity, Potion.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, Potion.sModel, conditionCollection, false);
			iMissile.Body.AngularVelocity = new Vector3(0f, 0f, 2f * iMissile.Body.Mass);
			iMissile.Danger = 0f;
			iOwner.PlayState.EntityManager.AddEntity(iMissile);
			lock (ProjectileSpell.sCachedConditions)
			{
				ProjectileSpell.sCachedConditions.Enqueue(conditionCollection);
			}
		}

		// Token: 0x040021FA RID: 8698
		private static readonly int DISPLAY_NAME = "#specab_drink".GetHashCodeCustom();

		// Token: 0x040021FB RID: 8699
		private readonly Damage mHealing;

		// Token: 0x040021FC RID: 8700
		private readonly float HITPOINTS = 501f;

		// Token: 0x040021FD RID: 8701
		private static readonly float FLASK_TTL = 15f;

		// Token: 0x040021FE RID: 8702
		private static Model sModel;
	}
}
