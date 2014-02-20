using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteXHazSkillz
{
	public class NpcDamageEvent
	{
		public int Damage { get; set; }
		public bool Crit { get; set; }
		public int NpcIndex { get; set; }
		public int PlayerIndex { get; set; }
	}
}
