using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WhiteXHazSkillz
{
	public class SkillPlayer
	{
		private List<String> acquiredSkills = new List<string>();

		public TSPlayer Player { get; set; }

		public SkillPlayer(TSPlayer ply, List<string> skills)
		{
			Player = ply;
			acquiredSkills = skills;
		}

		public bool HasSkill(string name)
		{
			return acquiredSkills.Contains(name);
		}
	}
}
