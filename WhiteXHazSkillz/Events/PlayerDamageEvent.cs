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
		public int HurtPlayerIndex { get; set; }
		public int DamagingEntityIndex { get; set; }
	}
}
