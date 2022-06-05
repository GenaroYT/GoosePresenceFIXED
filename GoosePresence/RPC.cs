using System;
using System.Reflection;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using GooseShared;

namespace GoosePresence
{
	// Token: 0x02000003 RID: 3
	internal class RPC
	{
		// Token: 0x06000005 RID: 5 RVA: 0x0000215C File Offset: 0x0000035C
		public RPC(string ClientId)
		{
			this.Client = new DiscordRpcClient(ClientId);
			this.Now = Timestamps.Now;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000225C File Offset: 0x0000045C
		public void Init()
		{
			this.tasks = API.TaskDatabase.getAllLoadedTaskIDs.Invoke();
			this.Client.Initialize();
			this.Client.Logger = new ConsoleLogger
			{
				Level = LogLevel.Warning
			};
			this.Client.OnConnectionEstablished += this.Client_OnConnectionEstablished;
			this.Client.OnConnectionFailed += this.Client_OnConnectionFailed;
			this.Client.OnReady += this.Client_OnReady;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020B7 File Offset: 0x000002B7
		private void Client_OnConnectionFailed(object sender, ConnectionFailedMessage args)
		{
			Console.WriteLine("[{1}] Pipe Connection Failed. Could not connect to pipe #{0}", args.FailedPipe, args.TimeCreated);
			this.isRunning = false;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020E0 File Offset: 0x000002E0
		private void Client_OnReady(object sender, ReadyMessage args)
		{
			Console.WriteLine("[{0}][{1}] Recieved Ready from user {2}", args.Version, args.TimeCreated, args.User);
			this.isRunning = true;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000210F File Offset: 0x0000030F
		private void Client_OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
		{
			Console.WriteLine("[{1}] Connection established! Connected to pipe #{0}", args.ConnectedPipe, args.TimeCreated);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002131 File Offset: 0x00000331
		private string getRand(string[] s)
		{
			return s[this.rand.Next(s.Length)];
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002143 File Offset: 0x00000343
		public void Disconnect()
		{
			this.Client.Dispose();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022E8 File Offset: 0x000004E8
		public void Update(GooseEntity g)
		{
			if (!this.isRunning)
			{
				return;
			}
			this.Goose = g;
			float walkSpeed = this.Goose.parameters.WalkSpeed;
			float runSpeed = this.Goose.parameters.RunSpeed;
			float chargeSpeed = this.Goose.parameters.ChargeSpeed;
			if (this.prevSpeed != this.Goose.currentSpeed || this.prevTask != this.tasks[this.Goose.currentTask])
			{
				this.speedTier = ((this.Goose.currentSpeed == walkSpeed) ? (this.getRand(this.Walk) + " (Walking)") : ((this.Goose.currentSpeed == runSpeed) ? (this.getRand(this.Run) + " (Running)") : ((this.Goose.currentSpeed == chargeSpeed) ? (this.getRand(this.Charge) + " (Charging)") : string.Format("{0} (Speed: {1})", this.getRand(this.Custom), this.Goose.currentSpeed))));
				foreach (FieldInfo fieldInfo in typeof(RPC.Taskz).GetFields())
				{
					if (fieldInfo.Name.Replace("ing", "").Replace("bb", "b").Replace("mes", "me").Replace("pads", "pad") == this.tasks[this.Goose.currentTask] || fieldInfo.Name == this.tasks[this.Goose.currentTask])
					{
						this.currentTask = this.getRand((string[])fieldInfo.GetValue(RPC.t)) + " (" + fieldInfo.Name + ")";
						break;
					}
					this.currentTask = this.getRand(this.Custom) + " (" + this.tasks[this.Goose.currentTask] + ")";
				}
			}
			this.prevSpeed = this.Goose.currentSpeed;
			this.prevTask = this.tasks[this.Goose.currentTask];
			this.Client.SetPresence(new RichPresence
			{
				Details = this.currentTask,
				State = this.speedTier,
				Timestamps = this.Now,
				Assets = new Assets
				{
					LargeImageKey = "big",
					LargeImageText = "Honk!",
					SmallImageKey = null
				}
			});
		}

		// Token: 0x04000002 RID: 2
		private Timestamps Now;

		// Token: 0x04000003 RID: 3
		private DiscordRpcClient Client;

		// Token: 0x04000004 RID: 4
		private GooseEntity Goose;

		// Token: 0x04000005 RID: 5
		private string[] tasks;

		// Token: 0x04000006 RID: 6
		private string prevTask;

		// Token: 0x04000007 RID: 7
		private float prevSpeed;

		// Token: 0x04000008 RID: 8
		private string speedTier;

		// Token: 0x04000009 RID: 9
		private string currentTask;

		// Token: 0x0400000A RID: 10
		private bool isRunning;

		// Token: 0x0400000B RID: 11
		public static RPC.Taskz t = new RPC.Taskz();

		// Token: 0x0400000C RID: 12
		private Random rand = new Random();

		// Token: 0x0400000D RID: 13
		public string[] Run = new string[]
		{
			"Doing the running man challenge.",
			"Running away from good choices.",
			"Trying to reduce weight.",
			"Wait, why am I running?"
		};

		// Token: 0x0400000E RID: 14
		public string[] Walk = new string[]
		{
			"Running at a slow pace.",
			"Doing the reverse moonwalk.",
			"Tap-tap-Tap-Tap!",
			"Going out on a walk.",
			"Why is my feet so loud?",
			"Exploring the monitor.",
			"Walking Loudly."
		};

		// Token: 0x0400000F RID: 15
		public string[] Charge = new string[]
		{
			"Walking at an incredibly fast pace.",
			"Surpassing the Speed Limit.",
			"Found some Nuggies.",
			"Probably Angry."
		};

		// Token: 0x04000010 RID: 16
		private string[] Custom = new string[]
		{
			"I wonder what this is?",
			"I didn't know the goose could do this.",
			"Doing Something new.",
			"Well, this is new."
		};

		// Token: 0x02000004 RID: 4
		public class Taskz
		{
			// Token: 0x04000011 RID: 17
			public string[] Wandering = new string[]
			{
				"Contemplating life choices.",
				"Thinking about memes",
				"Thinking of causing more chaos.",
				"Finding more ways to cause trouble.",
				"Searching for something.."
			};

			// Token: 0x04000012 RID: 18
			public string[] NabbingMouse = new string[]
			{
				"Biting the Mouse.",
				"MONCH.",
				"Figuring out if the mouse is a mice.",
				"Chasing the mouse."
			};

			// Token: 0x04000013 RID: 19
			public string[] CollectingMemes = new string[]
			{
				"Dragging Memes.",
				"Sending Memes.",
				"Giving some honky memes.",
				"Mm yes. The honk is made out of honk."
			};

			// Token: 0x04000014 RID: 20
			public string[] CollectingNotepads = new string[]
			{
				"Dragging a Not-epad.",
				"Being an amazing poet.",
				"Giving life advice.",
				"Typing with two feet.",
				"Getting a typewriter.",
				"Typing with eyes closed."
			};

			// Token: 0x04000015 RID: 21
			public string[] TrackingMud = new string[]
			{
				"Making the screen dirty.",
				"Might wanna clean that.",
				"This is my monitor now.",
				"Fertilizing the screen."
			};

			// Token: 0x04000016 RID: 22
			public string[] CustomMouseNab = new string[]
			{
				"ONE PUNCHH!!",
				"Chasing the mouse at an incredibly fast speed.",
				"Why do I hear boss music?"
			};

			// Token: 0x04000017 RID: 23
			public string[] Sleeping = new string[]
			{
				"Zzzzzz...",
				"Dreaming about getting infinte bells.",
				"Causing chaos in a dream.",
				"Ooh, Comfy.",
				"Dreaming.",
				"Chillin."
			};

			// Token: 0x04000018 RID: 24
			public string[] RunToBed = new string[]
			{
				"Going to bed.",
				"Found a comfy bed.",
				"About to take a nap.",
				"About to sleep.",
				"Going towards the comfy zone."
			};

			// Token: 0x04000019 RID: 25
			public string[] ChargeToStick = new string[]
			{
				"Ooh! A stick!",
				"Running towards a stick.",
				"GIMMIE GIMMIE GIMMIE-",
				"Stick Motion Detected!"
			};

			// Token: 0x0400001A RID: 26
			public string[] ReturnStick = new string[]
			{
				"Returning stick!",
				"Stick has been obtained!"
			};

			// Token: 0x0400001B RID: 27
			public string[] ChaseLaser = new string[]
			{
				"Laser Detected!",
				"Endlessly chasing a red dot."
			};
		}
	}
}
