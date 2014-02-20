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
		private List<SkillInformation> acquiredSkills = new List<SkillInformation>();
		
		public string AccountName { get; set; }
		public TSPlayer Player { get; set; }

		public SkillPlayer(TSPlayer ply, List<SkillInformation> skills)
		{
			Player = ply;
			AccountName = ply.UserAccountName;
			acquiredSkills = skills;
		}

		public SkillPlayer(SkillPlayer old)
		{
			lock (old.acquiredSkills)
			{
				Player = old.Player;
				AccountName = old.Player.UserAccountName;
				acquiredSkills = old.acquiredSkills;
			}
		}

		public SkillInformation GetSkillInformation(string name)
		{
			lock (acquiredSkills)
			{
				return acquiredSkills.FirstOrDefault(s => s.Name == name);
			}
		}

		public void SetSkillInformation(SkillInformation info)
		{
			lock (acquiredSkills)
			{
				SkillInformation old = acquiredSkills.FirstOrDefault(s => s.Name == info.Name);

				if (old != null)
				{
					acquiredSkills.Remove(old);
				}

				acquiredSkills.Add(info);
			}
		}

		public List<SkillInformation> Skills()
		{
			lock (acquiredSkills)
			{
				return new List<SkillInformation>(acquiredSkills);
			}
		}
	}
}
