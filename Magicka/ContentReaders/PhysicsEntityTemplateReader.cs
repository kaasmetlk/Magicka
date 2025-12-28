using System;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework.Content;

namespace Magicka.ContentReaders
{
	// Token: 0x02000617 RID: 1559
	public class PhysicsEntityTemplateReader : ContentTypeReader<PhysicsEntityTemplate>
	{
		// Token: 0x06002EC2 RID: 11970 RVA: 0x0017B324 File Offset: 0x00179524
		protected override PhysicsEntityTemplate Read(ContentReader iInput, PhysicsEntityTemplate iExistingInstance)
		{
			return PhysicsEntityTemplate.Read(iInput);
		}
	}
}
