using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using WhiteXHazSkillz;
using WhiteXHazSkillz.Events;

namespace TestPlugin
{
	public class TestSkill : WhiteXHazSkillz.Plugin
	{
		public EventRegister register;

		public override string Name { get { return "Test Skill"; }}

		public TestSkill(EventRegister register) : base(register)
		{
			this.register = register;
		}

		public override void Initialize()
		{
			register.RegisterSkillHandler<PlayerDamageEvent>(this, OnPlayerTakesDamage, EventType.PlayerTakesDamage);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
				register.DeregisterSkillHandler(this, EventType.PlayerTakesDamage);
		}

		Random rand = new Random();
		private void OnPlayerTakesDamage(PlayerDamageEvent args)
		{
			if(!args.PVP)
				FireRocket(args.HurtPlayer, args.Damage);
		}

		private void FireRocket(SkillPlayer origin, int damage)
		{
			int projectileIndex = Projectile.NewProjectile(origin.Player.TPlayer.position.X, origin.Player.TPlayer.position.Y - 300, 0, 10, 207, damage, -10, origin.Player.Index, 0, 0);
			NetMessage.SendData(27, -1, -1, "", projectileIndex);
		}
	}
}
