using System;
using System.Globalization;
using System.Xml;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000568 RID: 1384
	internal class Message
	{
		// Token: 0x06002938 RID: 10552 RVA: 0x00143554 File Offset: 0x00141754
		public Message(XmlNode iInput)
		{
			this.mTurnToTarget = true;
			this.mForceOnScreen = true;
			for (int i = 0; i < iInput.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = iInput.Attributes[i];
				if (xmlAttribute.Name.Equals("character", StringComparison.OrdinalIgnoreCase))
				{
					this.mOwner = xmlAttribute.Value.ToLowerInvariant();
					this.mOwnerID = this.mOwner.GetHashCodeCustom();
				}
				else if (xmlAttribute.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
				{
					this.mType = (MessageType)Enum.Parse(typeof(MessageType), xmlAttribute.Value, true);
				}
				else if (xmlAttribute.Name.Equals("forceonscreen", StringComparison.OrdinalIgnoreCase))
				{
					bool.TryParse(xmlAttribute.Value, out this.mForceOnScreen);
				}
				else if (xmlAttribute.Name.Equals("facing", StringComparison.OrdinalIgnoreCase))
				{
					bool.TryParse(xmlAttribute.Value, out this.mTurnToTarget);
				}
				else
				{
					if (!xmlAttribute.Name.Equals("width", StringComparison.OrdinalIgnoreCase))
					{
						throw new NotImplementedException();
					}
					int num;
					if (int.TryParse(xmlAttribute.Value, out num))
					{
						this.mLineLength = num;
					}
				}
			}
			this.mMessageTTLs = new float[iInput.ChildNodes.Count];
			this.mMessageIDs = new int[iInput.ChildNodes.Count];
			this.mAnimations = new Animations[iInput.ChildNodes.Count];
			this.mSounds = new int[iInput.ChildNodes.Count];
			this.mBanks = new Banks[iInput.ChildNodes.Count];
			this.mMessageArgs = new int[iInput.ChildNodes.Count][];
			for (int j = 0; j < this.mAnimations.Length; j++)
			{
				this.mAnimations[j] = Animations.talk_casual0;
				this.mBanks[j] = Banks.Characters;
			}
			int num2 = 0;
			foreach (object obj in iInput.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				float num3 = -1f;
				foreach (object obj2 in xmlNode.Attributes)
				{
					XmlAttribute xmlAttribute2 = (XmlAttribute)obj2;
					if (xmlAttribute2.Name.Equals("ttl", StringComparison.OrdinalIgnoreCase))
					{
						num3 = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture.NumberFormat);
					}
					else if (xmlAttribute2.Name.Equals("anim", StringComparison.OrdinalIgnoreCase))
					{
						this.mAnimations[num2] = (Animations)Enum.Parse(typeof(Animations), xmlAttribute2.Value, true);
					}
					else if (xmlAttribute2.Name.Equals("sound", StringComparison.OrdinalIgnoreCase))
					{
						string[] array = xmlAttribute2.Value.ToLowerInvariant().Split(new char[]
						{
							'/'
						});
						if (array.Length == 1)
						{
							this.mSounds[num2] = array[0].GetHashCodeCustom();
						}
						else
						{
							if (array.Length != 2)
							{
								throw new Exception("Invalid syntax in \"Sound\" attribute!");
							}
							this.mBanks[num2] = (Banks)Enum.Parse(typeof(Banks), array[0], true);
							this.mSounds[num2] = array[1].GetHashCodeCustom();
						}
					}
				}
				this.mMessageTTLs[num2] = num3;
				string[] array2 = xmlNode.InnerText.ToLowerInvariant().Split(new char[]
				{
					','
				});
				if (array2.Length > 1)
				{
					this.mMessageArgs[num2] = new int[array2.Length - 1];
					for (int k = 1; k < array2.Length; k++)
					{
						this.mMessageArgs[num2][k - 1] = array2[k].GetHashCodeCustom();
					}
				}
				this.mMessageIDs[num2] = array2[0].GetHashCodeCustom();
				num2++;
			}
		}

		// Token: 0x06002939 RID: 10553 RVA: 0x001439AC File Offset: 0x00141BAC
		public void Initialize()
		{
			this.mMessages = new string[this.mMessageIDs.Length];
			BitmapFont font = FontManager.Instance.GetFont(this.mFont);
			LanguageManager instance = LanguageManager.Instance;
			for (int i = 0; i < this.mMessageIDs.Length; i++)
			{
				string text = instance.GetString(this.mMessageIDs[i]);
				for (int j = text.IndexOf('#'); j >= 0; j = text.IndexOf('#'))
				{
					int num = text.IndexOf(';', j + 1);
					if (num <= j)
					{
						break;
					}
					string text2 = text.Substring(j, num - j);
					int iID;
					try
					{
						int num2 = int.Parse(text2.Substring(1));
						iID = this.mMessageArgs[i][num2 - 1];
					}
					catch (FormatException)
					{
						iID = text2.ToLowerInvariant().GetHashCodeCustom();
					}
					text = string.Concat(new string[]
					{
						text.Substring(0, j),
						"[c=1,1,1]",
						instance.GetString(iID),
						"[/c]",
						text.Substring(num + 1)
					});
				}
				text = font.Wrap(text, this.mLineLength, true);
				this.mMessages[i] = text;
				this.mMinBoxSize = Vector2.Max(this.mMinBoxSize, font.MeasureText(text, true));
			}
		}

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x0600293A RID: 10554 RVA: 0x00143B10 File Offset: 0x00141D10
		public int Owner
		{
			get
			{
				return this.mOwnerID;
			}
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x0600293B RID: 10555 RVA: 0x00143B18 File Offset: 0x00141D18
		public MessageType Type
		{
			get
			{
				return this.mType;
			}
		}

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x0600293C RID: 10556 RVA: 0x00143B20 File Offset: 0x00141D20
		public string[] Text
		{
			get
			{
				return this.mMessages;
			}
		}

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x0600293D RID: 10557 RVA: 0x00143B28 File Offset: 0x00141D28
		public bool TurnToTarget
		{
			get
			{
				return this.mTurnToTarget;
			}
		}

		// Token: 0x0600293E RID: 10558 RVA: 0x00143B30 File Offset: 0x00141D30
		public void StopCue()
		{
			if (this.mCue != null)
			{
				this.mCue.Stop(AudioStopOptions.Immediate);
			}
		}

		// Token: 0x0600293F RID: 10559 RVA: 0x00143B48 File Offset: 0x00141D48
		public bool Advance(TextBox iBox, Scene iScene, Vector2 iScreenPosition)
		{
			if (this.mCurrent >= this.mMessageIDs.Length)
			{
				this.mCurrent = 0;
				iBox.Hide();
				return true;
			}
			if (this.mCue != null && !this.mCue.IsStopping)
			{
				this.mCue.Stop(AudioStopOptions.AsAuthored);
			}
			if (this.mSounds[this.mCurrent] != 0)
			{
				this.mCue = AudioManager.Instance.PlayCue(this.mBanks[this.mCurrent], this.mSounds[this.mCurrent]);
			}
			iBox.Initialize(iScene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, iScreenPosition, this.mForceOnScreen, this.mOwnerID, this.mMessageTTLs[this.mCurrent]);
			this.mCurrent++;
			return false;
		}

		// Token: 0x06002940 RID: 10560 RVA: 0x00143C14 File Offset: 0x00141E14
		public bool Advance(TextBox iBox, Scene iScene, Vector3 iWorldPosition, bool iFocusOwner)
		{
			Magicka.GameLogic.Entities.Character character = Entity.GetByID(this.mOwnerID) as Magicka.GameLogic.Entities.Character;
			MagickCamera magickCamera = iScene.Camera as MagickCamera;
			if (this.mCue != null && !this.mCue.IsStopping)
			{
				this.mCue.Stop(AudioStopOptions.AsAuthored);
			}
			if (this.mCurrent >= this.mMessageIDs.Length)
			{
				this.mCurrent = 0;
				iBox.Hide();
				if (character != null)
				{
					if (iFocusOwner)
					{
						magickCamera.Release(Message.CAMERATIME);
					}
					if (this.mAnimations[Math.Max(this.mMessageIDs.Length - 1, 0)] != Animations.None)
					{
						if (character.SpecialIdleAnimation != Animations.None)
						{
							character.GoToAnimation(character.SpecialIdleAnimation, 0.3f);
						}
						else
						{
							character.GoToAnimation(Animations.idle, 0.3f);
						}
					}
				}
				return true;
			}
			if (character != null)
			{
				if (this.mAnimations[this.mCurrent] != Animations.None)
				{
					character.GoToAnimation(this.mAnimations[this.mCurrent], 0.3f);
				}
				if (this.mSounds[this.mCurrent] != 0)
				{
					this.mCue = AudioManager.Instance.PlayCue(this.mBanks[this.mCurrent], this.mSounds[this.mCurrent], character.AudioEmitter);
				}
				if (iFocusOwner)
				{
					magickCamera.MoveTo(character.Position, Message.CAMERATIME);
					magickCamera.Magnification = 1.5f;
				}
				iBox.Initialize(iScene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, this.mForceOnScreen, character, this.mMessageTTLs[this.mCurrent]);
			}
			else
			{
				this.mCue = AudioManager.Instance.PlayCue(this.mBanks[this.mCurrent], this.mSounds[this.mCurrent]);
				iBox.Initialize(iScene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, iWorldPosition, this.mForceOnScreen, this.mOwnerID, this.mMessageTTLs[this.mCurrent]);
			}
			this.mCurrent++;
			return false;
		}

		// Token: 0x06002941 RID: 10561 RVA: 0x00143DFC File Offset: 0x00141FFC
		public bool Advance(TextBox iBox, Entity iDefaultOwner, bool iFocusOwner)
		{
			Entity entity;
			if (this.mOwnerID == Message.OWNERHASH)
			{
				entity = iDefaultOwner;
			}
			else
			{
				entity = Entity.GetByID(this.mOwnerID);
			}
			if (entity == null || entity.Dead)
			{
				this.mCurrent++;
				return this.mCurrent >= this.mMessageIDs.Length || this.Advance(iBox, iDefaultOwner, iFocusOwner);
			}
			if (this.mCue != null && !this.mCue.IsStopping)
			{
				this.mCue.Stop(AudioStopOptions.AsAuthored);
			}
			MagickCamera magickCamera = null;
			if (entity != null)
			{
				magickCamera = (entity.PlayState.Scene.Camera as MagickCamera);
			}
			if (this.mCurrent >= this.mMessageIDs.Length)
			{
				Magicka.GameLogic.Entities.Character character = entity as Magicka.GameLogic.Entities.Character;
				this.mCurrent = 0;
				iBox.Hide();
				if (iFocusOwner)
				{
					magickCamera.Release(Message.CAMERATIME);
				}
				if (character != null && !(character.CurrentState is DeadState) && this.mAnimations[Math.Max(this.mMessageIDs.Length - 1, 0)] != Animations.None)
				{
					if (character.SpecialIdleAnimation != Animations.None)
					{
						character.GoToAnimation(character.SpecialIdleAnimation, 0.3f);
					}
					else
					{
						character.GoToAnimation(Animations.idle, 0.3f);
					}
				}
				return true;
			}
			NonPlayerCharacter nonPlayerCharacter = entity as NonPlayerCharacter;
			if (this.mAnimations[this.mCurrent] != Animations.None)
			{
				if (nonPlayerCharacter != null)
				{
					if (nonPlayerCharacter.AI.Events == null || nonPlayerCharacter.AI.Events.Length == 0 || nonPlayerCharacter.AI.CurrentEvent >= nonPlayerCharacter.AI.Events.Length)
					{
						(entity as Magicka.GameLogic.Entities.Character).GoToAnimation(this.mAnimations[this.mCurrent], 0.3f);
					}
				}
				else if (entity is Avatar && (entity as Avatar).Events == null)
				{
					(entity as Magicka.GameLogic.Entities.Character).GoToAnimation(this.mAnimations[this.mCurrent], 0.3f);
				}
			}
			if (this.mSounds[this.mCurrent] != 0)
			{
				this.mCue = AudioManager.Instance.PlayCue(this.mBanks[this.mCurrent], this.mSounds[this.mCurrent], entity.AudioEmitter);
			}
			if (iFocusOwner)
			{
				magickCamera.MoveTo(entity.Position, Message.CAMERATIME);
				magickCamera.Magnification = 1.5f;
			}
			if (entity != null)
			{
				iBox.Initialize(entity.PlayState.Scene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, this.mForceOnScreen, entity, this.mMessageTTLs[this.mCurrent]);
			}
			else
			{
				iBox.Initialize(entity.PlayState.Scene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, entity.Position, this.mForceOnScreen, entity.UniqueID, this.mMessageTTLs[this.mCurrent]);
			}
			this.mCurrent++;
			return false;
		}

		// Token: 0x06002942 RID: 10562 RVA: 0x001440AD File Offset: 0x001422AD
		public void Reset()
		{
			this.mCurrent = 0;
		}

		// Token: 0x04002C9D RID: 11421
		private static readonly int OWNERHASH = "owner".GetHashCodeCustom();

		// Token: 0x04002C9E RID: 11422
		private static readonly float CAMERATIME = 2f;

		// Token: 0x04002C9F RID: 11423
		private string mOwner;

		// Token: 0x04002CA0 RID: 11424
		private int mOwnerID;

		// Token: 0x04002CA1 RID: 11425
		private MessageType mType;

		// Token: 0x04002CA2 RID: 11426
		private Animations[] mAnimations;

		// Token: 0x04002CA3 RID: 11427
		private Banks[] mBanks;

		// Token: 0x04002CA4 RID: 11428
		private int[] mSounds;

		// Token: 0x04002CA5 RID: 11429
		private Cue mCue;

		// Token: 0x04002CA6 RID: 11430
		private bool mTurnToTarget;

		// Token: 0x04002CA7 RID: 11431
		private float[] mMessageTTLs;

		// Token: 0x04002CA8 RID: 11432
		private int[] mMessageIDs;

		// Token: 0x04002CA9 RID: 11433
		private int[][] mMessageArgs;

		// Token: 0x04002CAA RID: 11434
		private string[] mMessages;

		// Token: 0x04002CAB RID: 11435
		private MagickaFont mFont = MagickaFont.Maiandra16;

		// Token: 0x04002CAC RID: 11436
		private int mCurrent;

		// Token: 0x04002CAD RID: 11437
		private bool mForceOnScreen;

		// Token: 0x04002CAE RID: 11438
		private Vector2 mMinBoxSize;

		// Token: 0x04002CAF RID: 11439
		private int mLineLength = 600;
	}
}
