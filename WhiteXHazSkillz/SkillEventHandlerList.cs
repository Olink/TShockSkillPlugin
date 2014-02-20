using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WhiteXHazSkillz
{
	class SkillEventHandlerList<T> : EventHandlerList
	{
		readonly Dictionary<Plugin, Action<T>> handlers = new Dictionary<Plugin, Action<T>>();

		public void Register(Plugin p, object t)
		{
			lock (handlers)
			{
				if (handlers.ContainsKey(p))
					handlers.Remove(p);

				handlers.Add(p, (Action<T>) t);
			}
		}

		public void Deregister(Plugin p)
		{
			lock (handlers)
			{
				if (handlers.ContainsKey(p))
					handlers.Remove(p);
			}
		}

		public void Invoke(object args)
		{
			lock (handlers)
			{
				foreach (var handler in handlers.Values)
					handler.Invoke((T) args);
			}
		}
	}
}
