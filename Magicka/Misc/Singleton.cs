using System;

namespace Magicka.Misc
{
	// Token: 0x0200002C RID: 44
	public class Singleton<T> where T : class, new()
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000162 RID: 354 RVA: 0x0000A73C File Offset: 0x0000893C
		public static T Instance
		{
			get
			{
				if (Singleton<T>.sInstance == null)
				{
					lock (Singleton<T>.sLock)
					{
						if (Singleton<T>.sInstance == null)
						{
							Singleton<T>.sAllowInstancing = true;
							Singleton<T>.sInstance = Activator.CreateInstance<T>();
							Singleton<T>.sAllowInstancing = false;
						}
					}
				}
				return Singleton<T>.sInstance;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000163 RID: 355 RVA: 0x0000A7A4 File Offset: 0x000089A4
		public static bool HasInstance
		{
			get
			{
				return Singleton<T>.sInstance != null;
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000A7B6 File Offset: 0x000089B6
		public Singleton()
		{
			if (!Singleton<T>.sAllowInstancing)
			{
				throw new Exception(string.Format("Cannot create a new instance of {0} outside of Instance property.", typeof(T).Name));
			}
		}

		// Token: 0x04000121 RID: 289
		private const string EXCEPTION_TEXT = "Cannot create a new instance of {0} outside of Instance property.";

		// Token: 0x04000122 RID: 290
		private static bool sAllowInstancing = false;

		// Token: 0x04000123 RID: 291
		private static T sInstance;

		// Token: 0x04000124 RID: 292
		private static volatile object sLock = new object();
	}
}
