using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteXHazSkillz
{
	interface EventHandlerList
	{
		void Register(Plugin p, object t);
		void Deregister(Plugin p);
		void Invoke(object args);
	}
}
