using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteXHazSkillz
{
	public class PlayerDamageEvent
	{
		public int Damage { get; set; }
		public bool PVP { get; set; }
		public bool Crit { get; set; }
		public SkillPlayer HurtPlayer { get; set; }
		public SkillPlayer DamagingPlayer { get; set; }
	}
}
