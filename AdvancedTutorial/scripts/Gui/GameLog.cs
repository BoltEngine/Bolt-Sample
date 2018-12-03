using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{

	public class GameLog : Bolt.GlobalEventListener
	{
		struct Line
		{
			public float time;
			public string message;
		}

		BoltRingBuffer<Line> buffer;

		[SerializeField]
		Transform log;

		[SerializeField]
		TypogenicText[] lines;

		void Awake ()
		{
			buffer = new BoltRingBuffer<Line> (3);
			buffer.autofree = true;
		}

		void Update ()
		{
			log.position = new Vector3 (
				-(Screen.width / 2) + 4,
				+(Screen.height / 2) - 4,
				0
			);

			// remove old lines
			while ((buffer.count > 0) && ((buffer [0].time + 5f) < Time.time)) {
				buffer.Dequeue ();
				UpdateLines ();
			}
		}

		public override void OnEvent (LogEvent evnt)
		{
			buffer.Enqueue (new Line { time = Time.time, message = evnt.Message });
			UpdateLines ();
		}

		void UpdateLines ()
		{
			for (int i = 0; i < lines.Length; ++i) {
				lines [i].Set ("");
			}

			for (int i = 0; i < buffer.count; ++i) {
				lines [i].Set (buffer [(buffer.count - 1) - i].message);
			}
		}
	}

}
