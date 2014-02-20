using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhiteXHazSkillz.Events;

namespace WhiteXHazSkillz
{
	public interface EventRegister
	{
		void DeregisterSkillHandler(Plugin plugin, EventType type);
		void RegisterSkillHandler<T>(Plugin plugin, Action<T> handler, EventType type);
	}
}
