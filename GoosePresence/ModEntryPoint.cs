using System;
using System.Diagnostics;
using GooseShared;

namespace GoosePresence
{
	// Token: 0x02000002 RID: 2
	public class ModEntryPoint : IMod
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		void IMod.Init()
		{
			InjectionPoints.PreTickEvent += new InjectionPoints.PreTickEventHandler(this.PreTick);
			this.GoosePresence.Init();
			Process.GetCurrentProcess().Exited += this.OnProcessExit;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002084 File Offset: 0x00000284
		public void PreTick(GooseEntity g)
		{
			this.GoosePresence.Update(g);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002092 File Offset: 0x00000292
		public void OnProcessExit(object sender, EventArgs e)
		{
			this.GoosePresence.Disconnect();
		}

		// Token: 0x04000001 RID: 1
        // This is the fixed part. Don't change this to anything else if you're seeing this...
		private RPC GoosePresence = new RPC("982071417677950988");
	}
}
