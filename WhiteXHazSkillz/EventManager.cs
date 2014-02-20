using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WhiteXHazSkillz.Events;

namespace WhiteXHazSkillz
{
	class EventManager : EventDispatcher, EventRegister
	{
		public Dictionary<EventType, EventHandlerList> handlerList = new Dictionary<EventType, EventHandlerList>();

		public void InvokeHandler<T>(T args, EventType type)
		{
			if (!handlerList.ContainsKey(type))
				return;

			EventHandlerList handlers = handlerList[type];
			handlers.Invoke(args);
		}

		public void DeregisterSkillHandler(Plugin plugin, EventType type)
		{
			if (!handlerList.ContainsKey(type))
				return;

			EventHandlerList handlers = handlerList[type];

			handlers.Deregister(plugin);
		}

		public void RegisterSkillHandler<T>(Plugin plugin, Action<T> handler, EventType type)
		{
			EventHandlerList handlers;

			if (!handlerList.ContainsKey(type))
				handlerList.Add(type, new SkillEventHandlerList<T>());

			handlers = handlerList[type];

			handlers.Register(plugin, handler);
		}
	}
}
