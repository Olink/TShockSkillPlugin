using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhiteXHazSkillz.Events;

namespace WhiteXHazSkillz
{
	interface EventDispatcher
	{
		void InvokeHandler<T>(T args, EventType type);
	}
}
