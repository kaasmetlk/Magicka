using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels.Triggers.Actions;
using Magicka.Levels.Triggers.Conditions;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.Levels.Triggers
{
	// Token: 0x0200000F RID: 15
	public class Trigger
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00003CA4 File Offset: 0x00001EA4
		protected Trigger(GameScene iGameScene)
		{
			this.mGameScene = iGameScene;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003CBC File Offset: 0x00001EBC
		public virtual void Update(float iDeltaTime)
		{
			this.mTimeTillRepeat -= iDeltaTime;
			if (this.mAutoRun & NetworkManager.Instance.State != NetworkState.Client)
			{
				this.Execute(null, false);
			}
			for (int i = 0; i < this.mActions.Length; i++)
			{
				Action[] array = this.mActions[i];
				for (int j = 0; j < array.Length; j++)
				{
					array[j].Update(iDeltaTime);
				}
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003D2C File Offset: 0x00001F2C
		public virtual void Execute(Magicka.GameLogic.Entities.Character iSender, bool iIgnoreConditions)
		{
			if (iIgnoreConditions | !this.mHasTriggered | (this.mRepeat > -1E-45f && this.mTimeTillRepeat < 1E-45f))
			{
				bool flag = true;
				for (int i = 0; i < this.mConditions.Length; i++)
				{
					flag = true;
					for (int j = 0; j < this.mConditions[i].Length; j++)
					{
						if (!this.mConditions[i][j].IsMet(iSender))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (flag || iIgnoreConditions)
				{
					this.mHasTriggered = true;
					this.mTimeTillRepeat = this.mRepeat;
					int num = 0;
					if (this.mActions.Length > 1)
					{
						num = Trigger.sRandom.Next(this.mActions.Length);
					}
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
						triggerActionMessage.ActionType = TriggerActionType.TriggerExecute;
						triggerActionMessage.Id = this.mID;
						triggerActionMessage.Scene = this.mGameScene.ID;
						triggerActionMessage.Arg = num;
						if (iSender != null)
						{
							triggerActionMessage.Handle = iSender.Handle;
						}
						else
						{
							triggerActionMessage.Handle = ushort.MaxValue;
						}
						NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
					}
					for (int k = 0; k < this.mActions[num].Length; k++)
					{
						this.mActions[num][k].OnTrigger(iSender);
					}
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003E84 File Offset: 0x00002084
		public void ClearDelayedActions()
		{
			for (int i = 0; i < this.mActions.Length; i++)
			{
				Action[] array = this.mActions[i];
				for (int j = 0; j < array.Length; j++)
				{
					array[j].ClearDelayed();
				}
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003EC4 File Offset: 0x000020C4
		public static Trigger Read(GameScene iScene, XmlNode iNode)
		{
			Trigger trigger = new Trigger(iScene);
			trigger.mID = 0;
			trigger.mRepeat = -1f;
			for (int i = 0; i < iNode.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = iNode.Attributes[i];
				if (xmlAttribute.Name.Equals("ID", StringComparison.OrdinalIgnoreCase))
				{
					trigger.mIDString = xmlAttribute.Value.ToLowerInvariant();
					trigger.mID = trigger.mIDString.GetHashCodeCustom();
				}
				else
				{
					if (!xmlAttribute.Name.Equals("Autorun", StringComparison.OrdinalIgnoreCase))
					{
						if (xmlAttribute.Name.Equals("Repeat", StringComparison.OrdinalIgnoreCase))
						{
							try
							{
								trigger.mRepeat = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
								goto IL_E7;
							}
							catch (FormatException)
							{
								bool flag = bool.Parse(xmlAttribute.Value);
								trigger.mRepeat = (flag ? 0f : -1f);
								goto IL_E7;
							}
						}
						throw new NotImplementedException();
					}
					trigger.mAutoRun = bool.Parse(xmlAttribute.Value);
				}
				IL_E7:;
			}
			List<Condition[]> list = new List<Condition[]>();
			for (int j = 0; j < iNode.ChildNodes.Count; j++)
			{
				XmlNode xmlNode = iNode.ChildNodes[j];
				if (xmlNode.Name.Equals("If", StringComparison.OrdinalIgnoreCase))
				{
					list.Add(Trigger.ReadConditions(iScene, xmlNode));
				}
				else if (xmlNode.Name.Equals("Then", StringComparison.OrdinalIgnoreCase))
				{
					trigger.mActions = Trigger.ReadActions(iScene, trigger, xmlNode);
				}
			}
			trigger.mConditions = list.ToArray();
			return trigger;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004064 File Offset: 0x00002264
		protected static Condition[] ReadConditions(GameScene iScene, XmlNode iNode)
		{
			List<Condition> list = new List<Condition>();
			for (int i = 0; i < iNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = iNode.ChildNodes[i];
				if (!(xmlNode is XmlComment))
				{
					list.Add(Condition.Read(iScene, xmlNode));
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000040B8 File Offset: 0x000022B8
		public static Action[][] ReadActions(GameScene iScene, Trigger iTrigger, XmlNode iNode)
		{
			List<Action> list = new List<Action>();
			List<List<Action>> list2 = new List<List<Action>>();
			for (int i = 0; i < iNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = iNode.ChildNodes[i];
				if (!(xmlNode is XmlComment))
				{
					if (xmlNode.Name.Equals("Looped", StringComparison.OrdinalIgnoreCase))
					{
						if (NetworkManager.Instance.State == NetworkState.Client)
						{
							NetworkClient networkClient = NetworkManager.Instance.Interface as NetworkClient;
							while (!networkClient.SaveSlot.IsValid)
							{
								Thread.Sleep(1);
							}
							if (!networkClient.SaveSlot.Looped)
							{
								goto IL_1A7;
							}
						}
						else if (!SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.Looped)
						{
							goto IL_1A7;
						}
						for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
						{
							XmlNode xmlNode2 = xmlNode.ChildNodes[j];
							if (xmlNode2.Name.Equals("Random", StringComparison.OrdinalIgnoreCase))
							{
								List<Action> list3 = new List<Action>();
								for (int k = 0; k < xmlNode2.ChildNodes.Count; k++)
								{
									list3.Add(Action.Read(iScene, iTrigger, xmlNode2.ChildNodes[k]));
								}
								list2.Add(list3);
							}
							else
							{
								list.Add(Action.Read(iScene, iTrigger, xmlNode2));
							}
						}
					}
					else if (xmlNode.Name.Equals("Random", StringComparison.OrdinalIgnoreCase))
					{
						List<Action> list4 = new List<Action>();
						for (int l = 0; l < xmlNode.ChildNodes.Count; l++)
						{
							list4.Add(Action.Read(iScene, iTrigger, xmlNode.ChildNodes[l]));
						}
						list2.Add(list4);
					}
					else
					{
						list.Add(Action.Read(iScene, iTrigger, xmlNode));
					}
				}
				IL_1A7:;
			}
			if (list2.Count == 0)
			{
				list2.Add(new List<Action>());
			}
			for (int m = 0; m < list2.Count; m++)
			{
				list2[m].AddRange(list);
			}
			Action[][] array = new Action[list2.Count][];
			for (int n = 0; n < list2.Count; n++)
			{
				array[n] = list2[n].ToArray();
			}
			return array;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000042ED File Offset: 0x000024ED
		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000042F5 File Offset: 0x000024F5
		public bool HasTriggered
		{
			get
			{
				return this.mHasTriggered;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000042FD File Offset: 0x000024FD
		public void ResetTimers()
		{
			this.mTimeTillRepeat = 0f;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000430C File Offset: 0x0000250C
		public virtual void Initialize()
		{
			if (this.mConditions == null)
			{
				this.mConditions = new Condition[0][];
			}
			for (int i = 0; i < this.mConditions.Length; i++)
			{
				this.mConditions[i].Initialize();
			}
			for (int j = 0; j < this.mActions.Length; j++)
			{
				Action[] array = this.mActions[j];
				for (int k = 0; k < array.Length; k++)
				{
					array[k].Initialize();
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000437F File Offset: 0x0000257F
		public virtual Trigger.State GetState()
		{
			return new Trigger.State(this);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004388 File Offset: 0x00002588
		internal static void NetworkAction(ref TriggerActionMessage iMsg)
		{
			switch (iMsg.ActionType)
			{
			case TriggerActionType.TriggerExecute:
				Trigger.TriggerExecute(ref iMsg);
				return;
			case TriggerActionType.SpawnNPC:
				Trigger.SpawnNPC(ref iMsg);
				return;
			case TriggerActionType.SpawnElemental:
				Trigger.SpawnElemental(ref iMsg);
				return;
			case TriggerActionType.SpawnLuggage:
				Trigger.SpawnLuggage(ref iMsg);
				return;
			case TriggerActionType.SpawnItem:
				Trigger.SpawnItem(ref iMsg);
				return;
			case TriggerActionType.SpawnMagick:
				Trigger.SpawnMagick(ref iMsg);
				return;
			case TriggerActionType.ChangeScene:
				Trigger.ChangeScene(ref iMsg);
				return;
			case TriggerActionType.EnterScene:
				Trigger.EnterScene(ref iMsg);
				return;
			case TriggerActionType.ThunderBolt:
				Trigger.Thunderbolt(ref iMsg);
				return;
			case TriggerActionType.LightningBolt:
				Trigger.TrigLightningBolt(ref iMsg);
				return;
			case TriggerActionType.SpawnGrease:
				Trigger.SpawnGrease(ref iMsg);
				return;
			case TriggerActionType.Nullify:
				Trigger.Nullify(ref iMsg);
				return;
			case TriggerActionType.SpawnTornado:
				Trigger.SpawnTornado(ref iMsg);
				return;
			case TriggerActionType.NapalmStrike:
				Trigger.NapalmStrike(ref iMsg);
				return;
			case TriggerActionType.Charm:
				Trigger.Charm(ref iMsg);
				return;
			case TriggerActionType.Starfall:
				Trigger.Starfall(ref iMsg);
				return;
			case TriggerActionType.OtherworldlyDischarge:
				Trigger.OtherworldlyDischarge(ref iMsg);
				return;
			case TriggerActionType.OtherworldlyBoltDestroyed:
				Trigger.OtherworldlyBoltDestroyed(ref iMsg);
				return;
			case TriggerActionType.Confuse:
				Trigger.Confuse(ref iMsg);
				return;
			case TriggerActionType.StarGaze:
				Trigger.StarGaze(ref iMsg);
				return;
			}
			throw new NotImplementedException();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004490 File Offset: 0x00002690
		private static void TriggerExecute(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			GameScene scene = recentPlayState.Level.GetScene(iMsg.Scene);
			if (scene == null)
			{
				return;
			}
			Entity entity;
			Entity.TryGetFromHandle(iMsg.Handle, out entity);
			Trigger trigger = scene.Triggers[iMsg.Id];
			trigger.mHasTriggered = true;
			trigger.mTimeTillRepeat = trigger.mRepeat;
			for (int i = 0; i < trigger.mActions[iMsg.Arg].Length; i++)
			{
				trigger.mActions[iMsg.Arg][i].OnTrigger(entity as Magicka.GameLogic.Entities.Character);
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004528 File Offset: 0x00002728
		private static void SpawnElemental(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			ElementalEgg elementalEgg = Entity.GetFromHandle((int)iMsg.Handle) as ElementalEgg;
			elementalEgg.Initialize(ref iMsg.Position, ref iMsg.Direction, iMsg.Id);
			elementalEgg.Proximity = iMsg.Time;
			if (iMsg.Arg != 0)
			{
				Magicka.GameLogic.Entities.Character character = Entity.GetFromHandle(iMsg.Arg) as Magicka.GameLogic.Entities.Character;
				if (character != null)
				{
					elementalEgg.SetSummoned(character);
				}
			}
			recentPlayState.EntityManager.AddEntity(elementalEgg);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000045A0 File Offset: 0x000027A0
		private static void SpawnNPC(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			NonPlayerCharacter nonPlayerCharacter = Entity.GetFromHandle((int)iMsg.Handle) as NonPlayerCharacter;
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(iMsg.Template);
			nonPlayerCharacter.Initialize(cachedTemplate, iMsg.Arg, iMsg.Position, iMsg.Id);
			nonPlayerCharacter.Dialog = iMsg.Point0;
			if (iMsg.Bool0)
			{
				nonPlayerCharacter.ForceDraw();
			}
			nonPlayerCharacter.Color = iMsg.Color;
			Vector3 vector = default(Vector3);
			vector.Y = 1f;
			Vector3 right;
			Vector3.Cross(ref iMsg.Direction, ref vector, out right);
			Matrix orientation = default(Matrix);
			orientation.M44 = 1f;
			orientation.M22 = 1f;
			orientation.Forward = iMsg.Direction;
			orientation.Right = right;
			nonPlayerCharacter.CharacterBody.Orientation = orientation;
			nonPlayerCharacter.CharacterBody.DesiredDirection = iMsg.Direction;
			Animations animations = (Animations)iMsg.Point1;
			if (animations != Animations.None)
			{
				nonPlayerCharacter.ForceAnimation(animations);
			}
			animations = (Animations)iMsg.Point2;
			if (animations != Animations.None && animations != Animations.idle && animations != Animations.idle_agg)
			{
				nonPlayerCharacter.SpawnAnimation = animations;
				nonPlayerCharacter.ChangeState(RessurectionState.Instance);
			}
			animations = (Animations)iMsg.Point3;
			if (animations != Animations.None)
			{
				nonPlayerCharacter.SpecialIdleAnimation = animations;
				if (!(nonPlayerCharacter.CurrentState is RessurectionState))
				{
					nonPlayerCharacter.ForceAnimation(animations);
				}
			}
			if (iMsg.Scene != 0)
			{
				Magicka.GameLogic.Entities.Character character = Entity.GetFromHandle(iMsg.Scene) as Magicka.GameLogic.Entities.Character;
				if (character != null)
				{
					nonPlayerCharacter.Summoned(character);
				}
			}
			else if (recentPlayState.Level.CurrentScene.RuleSet != null && recentPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
			{
				nonPlayerCharacter.Faction = Factions.EVIL;
				(recentPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(nonPlayerCharacter, iMsg.Bool0);
			}
			recentPlayState.EntityManager.AddEntity(nonPlayerCharacter);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004778 File Offset: 0x00002978
		private static void SpawnLuggage(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			NonPlayerCharacter nonPlayerCharacter = Entity.GetFromHandle((int)iMsg.Handle) as NonPlayerCharacter;
			nonPlayerCharacter.Initialize(CharacterTemplate.GetCachedTemplate(iMsg.Template), iMsg.Position, iMsg.Id);
			Vector3 vector = default(Vector3);
			vector.Y = 1f;
			Vector3 right;
			Vector3.Cross(ref iMsg.Direction, ref vector, out right);
			Matrix orientation = default(Matrix);
			orientation.M44 = 1f;
			orientation.M22 = 1f;
			orientation.Forward = iMsg.Direction;
			orientation.Right = right;
			nonPlayerCharacter.CharacterBody.Orientation = orientation;
			nonPlayerCharacter.CharacterBody.DesiredDirection = iMsg.Direction;
			nonPlayerCharacter.SpawnAnimation = (Animations)iMsg.Point0;
			nonPlayerCharacter.ChangeState(RessurectionState.Instance);
			orientation.Translation = iMsg.Position;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect("luggage_spawn".GetHashCodeCustom(), ref orientation, out visualEffectReference);
			recentPlayState.EntityManager.AddEntity(nonPlayerCharacter);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000487C File Offset: 0x00002A7C
		private static void SpawnItem(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			Item item = Entity.GetFromHandle((int)iMsg.Handle) as Item;
			if (item == null)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Log(1, "Trigger", "Avoiding NULL-exception in SpawnItem() ! The requested item will not be spawnd. \n");
				}
				return;
			}
			try
			{
				Item.GetCachedWeapon(iMsg.Template, item);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Log(1, "Trigger", "Ignoring exception in Trigger::SpawnItem() !\nMessage: " + ex.Message + "\n");
				}
				return;
			}
			item.OnPickup = iMsg.Point0;
			item.Detach();
			Vector3 position = iMsg.Position;
			Matrix orientation;
			Matrix.CreateFromQuaternion(ref iMsg.Orientation, out orientation);
			item.Body.MoveTo(position, orientation);
			item.Body.Velocity = iMsg.Direction;
			if (iMsg.Time > 0f)
			{
				item.Despawn(iMsg.Time);
			}
			recentPlayState.EntityManager.AddEntity(item);
			if (iMsg.Bool0)
			{
				item.Body.EnableBody();
			}
			item.IgnoreTractorPull = iMsg.Bool1;
			AnimatedLevelPart animatedLevelPart;
			if (iMsg.Point1 != 0 && recentPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(iMsg.Point1, out animatedLevelPart))
			{
				animatedLevelPart.AddEntity(item);
				item.AnimatedLevelPartID = iMsg.Point1;
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000049D0 File Offset: 0x00002BD0
		private static void SpawnMagick(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			Vector3 position = iMsg.Position;
			Matrix iOrientation;
			Matrix.CreateFromQuaternion(ref iMsg.Orientation, out iOrientation);
			BookOfMagick bookOfMagick = Entity.GetFromHandle((int)iMsg.Handle) as BookOfMagick;
			Vector3 direction = iMsg.Direction;
			bookOfMagick.Initialize(position, iOrientation, (MagickType)iMsg.Template, iMsg.Bool0, direction, iMsg.Time, iMsg.Point0);
			recentPlayState.EntityManager.AddEntity(bookOfMagick);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004A40 File Offset: 0x00002C40
		private unsafe static void ChangeScene(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			SpawnPoint iSpawnPoint = default(SpawnPoint);
			iSpawnPoint.Scene = iMsg.Scene;
			*(&iSpawnPoint.Locations.FixedElementField) = iMsg.Point0;
			(&iSpawnPoint.Locations.FixedElementField)[1] = iMsg.Point1;
			(&iSpawnPoint.Locations.FixedElementField)[2] = iMsg.Point2;
			(&iSpawnPoint.Locations.FixedElementField)[3] = iMsg.Point3;
			iSpawnPoint.SpawnPlayers = iMsg.Bool0;
			recentPlayState.Level.GoToScene(iSpawnPoint, (Transitions)iMsg.Template, iMsg.Time, iMsg.Bool1, null);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004C10 File Offset: 0x00002E10
		private unsafe static void EnterScene(ref TriggerActionMessage iMsg)
		{
			PlayState ps = PlayState.RecentPlayState;
			SpawnPoint iSpawnPoint = default(SpawnPoint);
			iSpawnPoint.Scene = iMsg.Scene;
			*(&iSpawnPoint.Locations.FixedElementField) = iMsg.Point0;
			(&iSpawnPoint.Locations.FixedElementField)[1] = iMsg.Point1;
			(&iSpawnPoint.Locations.FixedElementField)[2] = iMsg.Point2;
			(&iSpawnPoint.Locations.FixedElementField)[3] = iMsg.Point3;
			iSpawnPoint.SpawnPlayers = true;
			int[] targets = new int[4];
			targets[0] = iMsg.Target0;
			targets[1] = iMsg.Target1;
			targets[2] = iMsg.Target2;
			targets[3] = iMsg.Target3;
			bool fixedDirection = iMsg.Bool2;
			bool ignoreTriggers = iMsg.Bool0;
			int moveTrigger = iMsg.Arg;
			ps.Level.GoToScene(iSpawnPoint, (Transitions)iMsg.Template, iMsg.Time, iMsg.Bool1, delegate()
			{
				GameScene currentScene = ps.Level.CurrentScene;
				AIEvent aievent = default(AIEvent);
				aievent.EventType = AIEventType.Move;
				aievent.MoveEvent.Delay = 0f;
				aievent.MoveEvent.Speed = 1f;
				aievent.MoveEvent.FixedDirection = fixedDirection;
				aievent.MoveEvent.Trigger = moveTrigger;
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					Player player = players[i];
					Matrix matrix;
					if (player.Playing && player.Avatar != null && currentScene.TryGetLocator(targets[i], out matrix))
					{
						aievent.MoveEvent.Waypoint = matrix.Translation;
						aievent.MoveEvent.Direction = matrix.Forward;
						player.Avatar.Events = new AIEvent[]
						{
							aievent
						};
						player.Avatar.IgnoreTriggers = ignoreTriggers;
					}
				}
			});
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004D3C File Offset: 0x00002F3C
		private static void Thunderbolt(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			Entity fromHandle = Entity.GetFromHandle((int)iMsg.Handle);
			Entity fromHandle2 = Entity.GetFromHandle(iMsg.Id);
			Vector3 vector = iMsg.Position;
			Vector3 vector2 = vector;
			vector2.Y += 40f;
			Magicka.Graphics.Flash.Instance.Execute(recentPlayState.Scene, 0.125f);
			LightningBolt lightning = LightningBolt.GetLightning();
			Vector3 vector3 = new Vector3(0f, -1f, 0f);
			Vector3 lightningcolor = Spell.LIGHTNINGCOLOR;
			Vector3 position = recentPlayState.Scene.Camera.Position;
			if (iMsg.Id != 0)
			{
				vector3 = Vector3.Right;
			}
			float iScale = 1f;
			lightning.InitializeEffect(ref vector2, ref vector3, ref vector, ref position, ref lightningcolor, false, iScale, 1f, recentPlayState);
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.EFFECT, ref vector, ref vector3, out visualEffectReference);
			if (!(fromHandle2 is Shield))
			{
				Segment iSeg = default(Segment);
				iSeg.Origin = vector;
				iSeg.Origin.Y = iSeg.Origin.Y + 1f;
				iSeg.Delta.Y = iSeg.Delta.Y - 10f;
				float num;
				Vector3 vector4;
				Vector3 vector5;
				AnimatedLevelPart iAnimation;
				if (recentPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector4, out vector5, out iAnimation, iSeg))
				{
					vector = vector4;
					DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched, iAnimation, 4f, ref vector, ref vector5, 60f);
				}
			}
			Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.sAudioEmitter.Position = vector;
			Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.sAudioEmitter.Up = Vector3.Up;
			Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.sAudioEmitter.Forward = Vector3.Right;
			AudioManager.Instance.PlayCue(Banks.Spells, Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.SOUND, Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Thunderbolt.sAudioEmitter);
			recentPlayState.Camera.CameraShake(vector, 1.5f, 0.333f);
			if (iMsg.Id != 0)
			{
				(fromHandle2 as IDamageable).Damage(Thunderstorm.sDamage, fromHandle, recentPlayState.PlayTime, vector);
				if (fromHandle2 is Avatar && (fromHandle2 as Avatar).HitPoints > 0f && !((fromHandle2 as Avatar).Player.Gamer is NetworkGamer))
				{
					AchievementsManager.Instance.AwardAchievement(recentPlayState, "oneinamillion");
				}
			}
			Thunderstorm.Instance.CheckPerfectStorm();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004F60 File Offset: 0x00003160
		private static void TrigLightningBolt(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			Entity fromHandle = Entity.GetFromHandle((int)iMsg.Handle);
			Entity.GetFromHandle(iMsg.Id);
			Vector3 position = iMsg.Position;
			Vector3 direction = iMsg.Direction;
			Spell spell = iMsg.Spell;
			Magicka.Graphics.Flash.Instance.Execute(recentPlayState.Scene, 0.075f);
			recentPlayState.Camera.CameraShake(position, 0.5f, 0.075f);
			LightningBolt lightning = LightningBolt.GetLightning();
			lightning.AirToSurface = true;
			HitList iHitList = new HitList(16);
			DamageCollection5 damageCollection;
			spell.CalculateDamage(SpellType.Lightning, CastType.Force, out damageCollection);
			lightning.Cast(fromHandle as ISpellCaster, position, position - direction, iHitList, Spell.LIGHTNINGCOLOR, 10f, ref damageCollection, new Spell?(spell), recentPlayState);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000501C File Offset: 0x0000321C
		private static void Nullify(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Nullify.Instance.NullifyArea(recentPlayState, iMsg.Position, iMsg.Bool0);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005048 File Offset: 0x00003248
		private static void SpawnGrease(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			Magicka.GameLogic.Entities.Character iOwner = Entity.GetFromHandle(iMsg.Id) as Magicka.GameLogic.Entities.Character;
			Grease.GreaseField specificInstance = Grease.GreaseField.GetSpecificInstance(iMsg.Handle);
			AnimatedLevelPart iAnimation = null;
			if (iMsg.Arg != 65535)
			{
				iAnimation = AnimatedLevelPart.GetFromHandle(iMsg.Arg);
			}
			specificInstance.Initialize(iOwner, iAnimation, ref iMsg.Position, ref iMsg.Direction);
			recentPlayState.EntityManager.AddEntity(specificInstance);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000050B4 File Offset: 0x000032B4
		private static void SpawnTornado(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			ISpellCaster iOwner = Entity.GetFromHandle(iMsg.Id) as ISpellCaster;
			TornadoEntity specificInstance = TornadoEntity.GetSpecificInstance(iMsg.Handle);
			Matrix iOrientation;
			Matrix.CreateFromQuaternion(ref iMsg.Orientation, out iOrientation);
			iOrientation.Translation = iMsg.Position;
			specificInstance.Initialize(recentPlayState, iOrientation, iOwner);
			recentPlayState.EntityManager.AddEntity(specificInstance);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00005114 File Offset: 0x00003314
		private static void NapalmStrike(ref TriggerActionMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			Vector3 up = Vector3.Up;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Napalm.NAPALM_EFFECT, ref iMsg.Position, ref up, out visualEffectReference);
			AudioManager.Instance.PlayCue(Banks.Additional, Napalm.NAPALM_SOUND);
			Magicka.GameLogic.Entities.Character iOwner = Entity.GetFromHandle((int)iMsg.Handle) as Magicka.GameLogic.Entities.Character;
			recentPlayState.Camera.CameraShake(iMsg.Position, 0.25f, 0.5f);
			if (iMsg.Id != -2147483648)
			{
				Grease.GreaseField specificInstance = Grease.GreaseField.GetSpecificInstance((ushort)iMsg.Id);
				AnimatedLevelPart iAnimation = null;
				if (iMsg.Arg != 65535)
				{
					iAnimation = AnimatedLevelPart.GetFromHandle(iMsg.Arg);
				}
				specificInstance.Initialize(iOwner, iAnimation, ref iMsg.Position, ref iMsg.Direction);
				specificInstance.Burn(3f);
				recentPlayState.EntityManager.AddEntity(specificInstance);
			}
			Napalm.NapalmBlast(recentPlayState, ref iMsg.Position, iOwner, iMsg.TimeStamp);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00005204 File Offset: 0x00003404
		private static void Charm(ref TriggerActionMessage iMsg)
		{
			Magicka.GameLogic.Entities.Character character = Entity.GetFromHandle((int)iMsg.Handle) as Magicka.GameLogic.Entities.Character;
			if (character != null)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Charm.SOUND, character.AudioEmitter);
				Entity fromHandle = Entity.GetFromHandle((int)((ushort)iMsg.Id));
				character.Charm(fromHandle, iMsg.Time, iMsg.Arg);
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00005260 File Offset: 0x00003460
		private static void Confuse(ref TriggerActionMessage iMsg)
		{
			NonPlayerCharacter nonPlayerCharacter = Entity.GetFromHandle((int)iMsg.Handle) as NonPlayerCharacter;
			if (nonPlayerCharacter != null)
			{
				if (iMsg.Bool0)
				{
					nonPlayerCharacter.Confuse(Factions.NONE);
					return;
				}
				nonPlayerCharacter.Confuse(nonPlayerCharacter.Template.Faction);
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000052A4 File Offset: 0x000034A4
		private static void Starfall(ref TriggerActionMessage iMsg)
		{
			Entity entity;
			Entity.TryGetFromHandle(iMsg.Handle, out entity);
			Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Starfall.Instance.Execute(entity as ISpellCaster, PlayState.RecentPlayState, iMsg.Position, false);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000052DC File Offset: 0x000034DC
		private static void OtherworldlyDischarge(ref TriggerActionMessage iMsg)
		{
			Entity entity;
			Entity entity2;
			if (Entity.TryGetFromHandle(iMsg.Handle, out entity) && Entity.TryGetFromHandle((ushort)iMsg.Arg, out entity2) && entity2 is IDamageable)
			{
				Magicka.GameLogic.Entities.Abilities.SpecialAbilities.OtherworldlyDischarge.Instance.Execute(entity2 as IDamageable, entity as ISpellCaster, entity2.PlayState);
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000532C File Offset: 0x0000352C
		private static void OtherworldlyBoltDestroyed(ref TriggerActionMessage iMsg)
		{
			Entity entity;
			Entity iTarget;
			if (Entity.TryGetFromHandle(iMsg.Handle, out entity) && Entity.TryGetFromHandle((ushort)iMsg.Arg, out iTarget) && entity is OtherworldlyBolt)
			{
				bool @bool = iMsg.Bool0;
				bool bool2 = iMsg.Bool1;
				bool bool3 = iMsg.Bool2;
				(entity as OtherworldlyBolt).Destroy(@bool, bool2, iTarget, bool3);
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005388 File Offset: 0x00003588
		private static void StarGaze(ref TriggerActionMessage iMsg)
		{
			Entity entity;
			Entity entity2;
			if (Entity.TryGetFromHandle(iMsg.Handle, out entity) && Entity.TryGetFromHandle((ushort)iMsg.Arg, out entity2) && entity2 is Magicka.GameLogic.Entities.Character && entity is ISpellCaster)
			{
				Magicka.GameLogic.Entities.Abilities.SpecialAbilities.StarGaze.Instance.Execute(entity as ISpellCaster, entity2 as Magicka.GameLogic.Entities.Character, entity2.PlayState);
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000053E0 File Offset: 0x000035E0
		internal static void NetworkAction(ref TriggerRequestMessage iMsg)
		{
			PlayState recentPlayState = PlayState.RecentPlayState;
			GameScene scene = recentPlayState.Level.GetScene(iMsg.Scene);
			Entity entity;
			Entity.TryGetFromHandle(iMsg.Handle, out entity);
			scene.ExecuteTrigger(iMsg.Id, entity as Magicka.GameLogic.Entities.Character, true);
		}

		// Token: 0x0400005A RID: 90
		protected static Random sRandom = new Random();

		// Token: 0x0400005B RID: 91
		protected string mIDString;

		// Token: 0x0400005C RID: 92
		protected int mID;

		// Token: 0x0400005D RID: 93
		protected float mRepeat;

		// Token: 0x0400005E RID: 94
		protected float mTimeTillRepeat;

		// Token: 0x0400005F RID: 95
		protected bool mHasTriggered;

		// Token: 0x04000060 RID: 96
		protected Condition[][] mConditions;

		// Token: 0x04000061 RID: 97
		protected Action[][] mActions;

		// Token: 0x04000062 RID: 98
		protected bool mAutoRun = true;

		// Token: 0x04000063 RID: 99
		protected GameScene mGameScene;

		// Token: 0x02000010 RID: 16
		public class State
		{
			// Token: 0x0600005E RID: 94 RVA: 0x00005434 File Offset: 0x00003634
			public State(Trigger iTrigger)
			{
				this.mTrigger = iTrigger;
				this.mActions = new Action.State[this.mTrigger.mActions.Length][];
				for (int i = 0; i < this.mActions.Length; i++)
				{
					this.mActions[i] = new Action.State[this.mTrigger.mActions[i].Length];
				}
				this.UpdateState();
			}

			// Token: 0x0600005F RID: 95 RVA: 0x0000549C File Offset: 0x0000369C
			public virtual void UpdateState()
			{
				this.mTimeTillRepeat = this.mTrigger.mTimeTillRepeat;
				this.mHasTriggered = this.mTrigger.mHasTriggered;
				for (int i = 0; i < this.mActions.Length; i++)
				{
					for (int j = 0; j < this.mActions[i].Length; j++)
					{
						this.mActions[i][j] = new Action.State(this.mTrigger.mActions[i][j]);
					}
				}
			}

			// Token: 0x06000060 RID: 96 RVA: 0x0000551C File Offset: 0x0000371C
			public virtual void ResetState()
			{
				this.mTrigger.mTimeTillRepeat = 0f;
				this.mTrigger.mHasTriggered = false;
				for (int i = 0; i < this.mActions.Length; i++)
				{
					for (int j = 0; j < this.mActions[i].Length; j++)
					{
						this.mActions[i][j].Reset(this.mTrigger.mActions[i][j]);
					}
				}
			}

			// Token: 0x06000061 RID: 97 RVA: 0x00005590 File Offset: 0x00003790
			public virtual void ApplyState()
			{
				this.mTrigger.mTimeTillRepeat = this.mTimeTillRepeat;
				this.mTrigger.mHasTriggered = this.mHasTriggered;
				for (int i = 0; i < this.mActions.Length; i++)
				{
					for (int j = 0; j < this.mActions[i].Length; j++)
					{
						this.mActions[i][j].AssignToAction(this.mTrigger.mActions[i][j]);
					}
				}
			}

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x06000062 RID: 98 RVA: 0x00005609 File Offset: 0x00003809
			public Trigger Trigger
			{
				get
				{
					return this.mTrigger;
				}
			}

			// Token: 0x06000063 RID: 99 RVA: 0x00005614 File Offset: 0x00003814
			internal void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.mTimeTillRepeat);
				iWriter.Write(this.mHasTriggered);
				for (int i = 0; i < this.mActions.Length; i++)
				{
					for (int j = 0; j < this.mActions[i].Length; j++)
					{
						this.mActions[i][j].Write(iWriter, this.mTrigger.mActions[i][j]);
					}
				}
			}

			// Token: 0x06000064 RID: 100 RVA: 0x00005684 File Offset: 0x00003884
			internal void Read(BinaryReader iReader)
			{
				this.mTimeTillRepeat = iReader.ReadSingle();
				this.mHasTriggered = iReader.ReadBoolean();
				for (int i = 0; i < this.mActions.Length; i++)
				{
					for (int j = 0; j < this.mActions[i].Length; j++)
					{
						this.mActions[i][j] = new Action.State(iReader, this.mTrigger.mActions[i][j]);
					}
				}
			}

			// Token: 0x04000064 RID: 100
			protected Trigger mTrigger;

			// Token: 0x04000065 RID: 101
			private float mTimeTillRepeat;

			// Token: 0x04000066 RID: 102
			private bool mHasTriggered;

			// Token: 0x04000067 RID: 103
			protected Action.State[][] mActions;
		}
	}
}
