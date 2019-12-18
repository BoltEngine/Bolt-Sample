#if ENABLE_PLAYFABSERVER_API

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Bolt.Samples.PlayFab
{
	public class PlayfabLogger : BoltLog.IWriter
	{
		volatile bool running = true;
		readonly bool isServer;
		readonly Thread thread;
		readonly AutoResetEvent threadEvent;
		readonly Queue<string> threadQueue;
		private readonly string path;

		public PlayfabLogger(string path)
		{
			isServer = true;
			threadEvent = new AutoResetEvent(false);
			threadQueue = new Queue<string>(1024);
			this.path = path;

			// Run the File log only on standalone
			if (Application.isEditor == false)
			{
				thread = new Thread(WriteLoop);
				thread.IsBackground = true;
				thread.Start();
			}
		}

		void Queue(string message)
		{
			lock(threadQueue)
			{
				threadQueue.Enqueue(message);
				threadEvent.Set();
			}
		}

		void BoltLog.IWriter.Info(string message)
		{
			Queue(message);
		}

		void BoltLog.IWriter.Debug(string message)
		{
			Queue(message);
		}

		void BoltLog.IWriter.Warn(string message)
		{
			Queue(message);
		}

		void BoltLog.IWriter.Error(string message)
		{
			Queue(message);
		}

		public void Dispose()
		{
			running = false;
		}

		void WriteLoop()
		{
			try
			{
				var n = DateTime.Now;

				string logFile;

				if (string.IsNullOrEmpty(path) == false)
				{
					logFile = Path.Combine(path, "Bolt_Log_{7}_{0}Y-{1}M-{2}D_{3}H{4}M{5}S_{6}MS.txt");
				}
				else
				{
					logFile = "Bolt_Log_{7}_{0}Y-{1}M-{2}D_{3}H{4}M{5}S_{6}MS.txt";
				}

				logFile = string.Format(logFile, n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second, n.Millisecond, isServer ? "SERVER" : "CLIENT");

				using(var writer = System.IO.File.OpenWrite(logFile))
				{
					using(var stream = new System.IO.StreamWriter(writer))
					{
						while (running)
						{
							if (threadEvent.WaitOne(100))
							{
								lock(threadQueue)
								{
									while (threadQueue.Count > 0)
									{
										stream.WriteLine(threadQueue.Dequeue());
									}
								}
							}
							stream.Flush();
						}
					}
				}

				threadEvent.Close();
			}
			catch
			{ }
		}

		public override bool Equals(object obj)
		{
			return obj.GetType().FullName == this.GetType().FullName;
		}

		public override int GetHashCode()
		{
			return this.GetType().FullName.GetHashCode();
		}
	}
}

#endif