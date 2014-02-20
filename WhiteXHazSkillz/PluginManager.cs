using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TerrariaApi.Server;
using TShockAPI;

namespace WhiteXHazSkillz
{
	class PluginManager
	{
		private readonly Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();
		private static readonly List<Plugin> plugins = new List<Plugin>();
		private EventRegister eventRegister;

		public PluginManager(EventRegister eventRegister)
		{
			this.eventRegister = eventRegister;
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

		public void LoadPlugins()
		{
			List<FileInfo> fileInfos = new DirectoryInfo(ServerApi.ServerPluginsDirectoryPath).GetFiles("*.dll").ToList();
			foreach (FileInfo fileInfo in fileInfos)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
				
				try
				{
					Assembly assembly;
					// The plugin assembly might have been resolved by another plugin assembly already, so no use to
					// load it again, but we do still have to verify it and create plugin instances.
					if (!loadedAssemblies.TryGetValue(fileNameWithoutExtension, out assembly))
					{
						assembly = Assembly.Load(File.ReadAllBytes(fileInfo.FullName));
						loadedAssemblies.Add(fileNameWithoutExtension, assembly);
					}

					foreach (Type type in assembly.GetExportedTypes())
					{
						if (!type.IsSubclassOf(typeof(Plugin)) || !type.IsPublic || type.IsAbstract)
							continue;
						
						Plugin pluginInstance;
						try
						{
							pluginInstance = (Plugin)Activator.CreateInstance(type, eventRegister);
						}
						catch (Exception ex)
						{
							// Broken plugins better stop the entire server init.
							throw new InvalidOperationException(
								string.Format("Could not create an instance of plugin class \"{0}\".", type.FullName), ex);
						}

						plugins.Add(pluginInstance);
					}
				}
				catch (Exception ex)
				{
					// Broken assemblies / plugins better stop the entire server init.
					throw new InvalidOperationException(
						string.Format("Failed to load assembly \"{0}\".", fileInfo.Name), ex);
				}
			}

			foreach (Plugin current in plugins)
			{
				try
				{
					current.Initialize();
				}
				catch (Exception ex)
				{
					// Broken plugins better stop the entire server init.
					throw new InvalidOperationException(string.Format(
						"Plugin \"{0}\" has thrown an exception during initialization.", current.Name), ex);
				}
			}
		}

		public void UnloadPlugins()
		{
			foreach (Plugin plugin in plugins)
			{
				try
				{
					plugin.Dispose();
				}
				catch (Exception ex)
				{
					Log.Error(string.Format("Plugin \"{0}\" has thrown an exception while being disposed:\n{1}", plugin.Name, ex));
				}
			}
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string fileName = args.Name.Split(',')[0];
			string path = Path.Combine(ServerApi.ServerPluginsDirectoryPath, fileName + ".dll");
			try
			{
				if (File.Exists(path))
				{
					Assembly assembly;
					if (!loadedAssemblies.TryGetValue(fileName, out assembly))
					{
						assembly = Assembly.Load(File.ReadAllBytes(path));
						loadedAssemblies.Add(fileName, assembly);
					}
					return assembly;
				}
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Error on resolving assembly \"{0}.dll\":\n{1}", fileName, ex));
			}
			return null;
		}
	}
}
