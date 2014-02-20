using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.DB;

namespace WhiteXHazSkillz
{
	public class PlayerManager
	{
		private static SkillPlayer[] players = new SkillPlayer[255];
		private static IDbConnection database;

		static PlayerManager()
		{
			if (TShock.Config.StorageType.ToLower() == "sqlite")
			{
				string sql = Path.Combine(TShock.SavePath, "tshock.sqlite");
				database = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
			}
			else if (TShock.Config.StorageType.ToLower() == "mysql")
			{
				try
				{
					var hostport = TShock.Config.MySqlHost.Split(':');
					database = new MySqlConnection();
					database.ConnectionString =
						String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
									  hostport[0],
									  hostport.Length > 1 ? hostport[1] : "3306",
									  TShock.Config.MySqlDbName,
									  TShock.Config.MySqlUsername,
									  TShock.Config.MySqlPassword
							);
				}
				catch (MySqlException ex)
				{
					Log.Error(ex.ToString());
					throw new Exception("MySql not setup correctly");
				}
			}
			else
			{
				throw new Exception("Invalid storage type");
			}

			var table = new SqlTable("PlayerSkills",
									 new SqlColumn("UserAccountName", MySqlDbType.VarChar, 50) { Primary = true },
									 new SqlColumn("SkillInformation", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(database,
											  database.GetSqlType() == SqlType.Sqlite
												? (IQueryBuilder)new SqliteQueryCreator()
												: new MysqlQueryCreator());
			creator.EnsureExists(table);
		}

		public static List<SkillPlayer> GetPlayers()
		{
			lock (players)
			{
				var list = new List<SkillPlayer>(players.ToList());
				return list;
			}
		}

		public static SkillPlayer GetPlayer(int index)
		{
			lock (players)
			{
				if (players[index] == null)
					throw new InvalidOperationException(String.Format("Player at index {0} is null.", index));

				return players[index];
			}
		}

		public static void ActivatePlayer(SkillPlayer player, int index)
		{
			lock (players)
			{
				players[index] = player;
			}
		}

		public static void RemovePlayer(int index)
		{
			SkillPlayer player = null;
			lock (players)
			{
				if (players[index] != null)
				{
					player = new SkillPlayer(players[index]);
				}

				players[index] = null;
			}

			if(player != null)
				SavePlayer(player);
		}

		public static void SavePlayer(SkillPlayer player)
		{
			try
			{
				database.Query("REPLACE INTO PlayerSkills (UserAccountName, SkillInformation) VALUES (@0, @1)", player.AccountName,
					JsonConvert.SerializeObject(player.Skills()));
			}
			catch (Exception e)
			{
				TShockAPI.Log.ConsoleError("Failed to save player: {0}", e.Message);
			}
		}

		public static SkillPlayer LoadPlayer(TSPlayer player)
		{
			var list = LoadSkills(player.UserAccountName);
			SkillPlayer ply = new SkillPlayer(player, list);
			return ply;
		}

		public static List<SkillInformation> LoadSkills(string name)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT SkillInformation FROM PlayerSkills WHERE UserAccountName=@0", name)
					)
				{
					if (reader.Read())
					{
						List<SkillInformation> info =
							JsonConvert.DeserializeObject<List<SkillInformation>>(reader.Get<string>("SkillInformation"));
						return info;
					}
				}
			}
			catch (Exception e)
			{
				TShockAPI.Log.ConsoleError("Failed to load player: {0}", e.Message);
			}

			return new List<SkillInformation>();
		}
	}
}
