using System;
using System.Collections.Generic;
using Magicka.CoreFramework;
using Magicka.WebTools.Paradox.Telemetry.TypeValidators;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x020004EA RID: 1258
	public static class EventParameter
	{
		// Token: 0x0600254C RID: 9548 RVA: 0x0010F720 File Offset: 0x0010D920
		static EventParameter()
		{
			EventParameter.RegisterValidator(EventParameter.Type.Boolean, new BaseTypeValidator<bool>());
			EventParameter.RegisterValidator(EventParameter.Type.Int, new BaseTypeValidator<int>());
			EventParameter.RegisterValidator(EventParameter.Type.Int64, new BaseTypeValidator<long>());
			EventParameter.RegisterValidator(EventParameter.Type.UInt, new BaseTypeValidator<uint>());
			EventParameter.RegisterValidator(EventParameter.Type.UInt64, new BaseTypeValidator<ulong>());
			EventParameter.RegisterValidator(EventParameter.Type.Float, new BaseTypeValidator<float>());
			EventParameter.RegisterValidator(EventParameter.Type.String, new BaseTypeValidator<string>());
			EventParameter.RegisterValidator(EventParameter.Type.PlayerSegment, new PlayerSegmentTypeValidator());
			EventParameter.RegisterValidator(EventParameter.Type.TutorialActionEnum, new BaseTypeValidator<EventEnums.TutorialAction>());
			EventParameter.RegisterValidator(EventParameter.Type.ControllerTypeEnum, new BaseTypeValidator<EventEnums.ControllerType>());
			EventParameter.RegisterValidator(EventParameter.Type.NetworkStateEnum, new NetworkStateTypeValidator());
			EventParameter.RegisterValidator(EventParameter.Type.MagickTypeEnum, new MagickTypeTypeValidator());
			EventParameter.RegisterValidator(EventParameter.Type.PlayerDeath, new BaseTypeValidator<EventEnums.DeathCategory>());
		}

		// Token: 0x0600254D RID: 9549 RVA: 0x0010F7CA File Offset: 0x0010D9CA
		private static void RegisterValidator(EventParameter.Type iType, ITypeValidator iValidator)
		{
			if (iValidator == null)
			{
				throw new NullReferenceException("Cannot register a null type validator.");
			}
			EventParameter.sTypeValidators.Add(iType, iValidator);
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x0010F7E8 File Offset: 0x0010D9E8
		public static bool MatchType(object iObject, EventParameter.Type iExpectedType)
		{
			if (EventParameter.sTypeValidators.ContainsKey(iExpectedType))
			{
				return EventParameter.sTypeValidators[iExpectedType].MatchType(iObject);
			}
			throw new Exception(string.Format("Missing validator for type {0}.", iExpectedType.ToString()));
		}

		// Token: 0x0600254F RID: 9551 RVA: 0x0010F834 File Offset: 0x0010DA34
		public static string ToString(object iObject, EventParameter.Type iExpectedType)
		{
			string text = string.Empty;
			if (EventParameter.MatchType(iObject, iExpectedType))
			{
				text = EventParameter.sTypeValidators[iExpectedType].GetFormattedString(iObject);
				if (text.Length > 128)
				{
					Logger.LogWarning(Logger.Source.EventParameter, "The data sent for this param is too big and will be truncated. Truncated string will be : " + text.Substring(0, 128) + "##Truncated##");
					Logger.LogWarning(Logger.Source.EventParameter, "Truncated string will be : \"" + text.Substring(0, 128) + "\"##Truncated##");
				}
			}
			return text;
		}

		// Token: 0x06002550 RID: 9552 RVA: 0x0010F8B2 File Offset: 0x0010DAB2
		public static System.Type ToSystemType(EventParameter.Type iType)
		{
			if (EventParameter.sTypeValidators.ContainsKey(iType))
			{
				return EventParameter.sTypeValidators[iType].GetSystemType();
			}
			throw new Exception(string.Format("Missing validator for type {0}.", iType.ToString()));
		}

		// Token: 0x040028AB RID: 10411
		private const string EXCEPTION_NULL_VALIDATOR = "Cannot register a null type validator.";

		// Token: 0x040028AC RID: 10412
		private const string EXCEPTION_MISSING_VALIDATOR = "Missing validator for type {0}.";

		// Token: 0x040028AD RID: 10413
		private const int MAX_ALLOWED_CHARACTERS = 128;

		// Token: 0x040028AE RID: 10414
		private static readonly Dictionary<EventParameter.Type, ITypeValidator> sTypeValidators = new Dictionary<EventParameter.Type, ITypeValidator>();

		// Token: 0x020004EB RID: 1259
		public enum Type
		{
			// Token: 0x040028B0 RID: 10416
			Boolean,
			// Token: 0x040028B1 RID: 10417
			Int,
			// Token: 0x040028B2 RID: 10418
			Int64,
			// Token: 0x040028B3 RID: 10419
			UInt,
			// Token: 0x040028B4 RID: 10420
			UInt64,
			// Token: 0x040028B5 RID: 10421
			Float,
			// Token: 0x040028B6 RID: 10422
			String,
			// Token: 0x040028B7 RID: 10423
			PlayerSegment,
			// Token: 0x040028B8 RID: 10424
			TutorialActionEnum,
			// Token: 0x040028B9 RID: 10425
			ControllerTypeEnum,
			// Token: 0x040028BA RID: 10426
			NetworkStateEnum,
			// Token: 0x040028BB RID: 10427
			MagickTypeEnum,
			// Token: 0x040028BC RID: 10428
			PlayerDeath
		}
	}
}
