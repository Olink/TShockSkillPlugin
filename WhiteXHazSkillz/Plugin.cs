using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteXHazSkillz
{
	public abstract class Plugin : IDisposable
	{
		public virtual string Name { get { return ""; }}

		abstract public void Initialize();


		public Plugin(EventRegister register)
		{
			
		}

		~Plugin()
		{
			this.Dispose(false);
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public override bool Equals(object obj)
		{
			return GetType() == obj.GetType() && Equals((Plugin)obj);
		}

		public bool Equals(Plugin p)
		{
			return Name == p.Name;
		}
	}
}
