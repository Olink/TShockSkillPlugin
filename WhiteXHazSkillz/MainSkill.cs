using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using WhiteXHazSkillz.Events;

namespace WhiteXHazSkillz
{
	[ApiVersion(1, 15)]
	public class MainSkill : TerrariaPlugin
	{
		private EventManager manager;
		private PluginManager plugins;
		public SkillPlayer[] SkillPlayers = new SkillPlayer[255];

		public MainSkill(Main game) : base(game)
		{
			manager = new EventManager();
			plugins = new PluginManager(manager);
		}

		public override void Initialize()
		{
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += HandlePlayerLogin;
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
			plugins.LoadPlugins();
		}

		protected override void Dispose(bool disposing)
		{
			plugins.UnloadPlugins();

			if (disposing)
			{
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				TShockAPI.Hooks.PlayerHooks.PlayerPostLogin -= HandlePlayerLogin;
				ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
			}
			
			base.Dispose(disposing);
		}

		private void OnGetData(GetDataEventArgs e)
		{
			if (e.Handled)
				return;

			PacketTypes type = e.MsgID;
			var player = TShock.Players[e.Msg.whoAmI];
			if (player == null || !player.ConnectionAlive)
			{
				e.Handled = true;
				return;
			}

			if (player.RequiresPassword && type != PacketTypes.PasswordSend)
			{
				e.Handled = true;
				return;
			}

			if ((player.State < 10 || player.Dead) && (int)type > 12 && (int)type != 16 && (int)type != 42 && (int)type != 50 &&
				(int)type != 38 && (int)type != 21)
			{
				e.Handled = true;
				return;
			}

			using (var data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
			{
				if (type == PacketTypes.PlayerDamage)
					OnPlayerDamage(new GetDataHandlerArgs(player, data));
			}
		}

		private void OnPlayerDamage(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var direction = args.Data.ReadInt8();
			var dmg = args.Data.ReadInt16();
			var pvp = args.Data.ReadBoolean();
			var crit = args.Data.ReadBoolean();

			if (SkillPlayers[args.Player.Index] != null)
				manager.InvokeHandler(new PlayerDamageEvent() { Crit = crit, Damage = dmg, HurtPlayer = SkillPlayers[args.Player.Index], DamagingPlayer = SkillPlayers[id], PVP = pvp }, EventType.PlayerTakesDamage);
		}

		private void OnLeave(LeaveEventArgs args)
		{
			SkillPlayers[args.Who] = null;
		}

		private void HandlePlayerLogin(TShockAPI.Hooks.PlayerPostLoginEventArgs args)
		{
			SkillPlayers[args.Player.Index] = new SkillPlayer(args.Player, null);
		}
	}
}
