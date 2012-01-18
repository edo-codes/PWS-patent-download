using System;
using System.Xml.

namespace pws
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var Patents = new System.Collections.Generic.List<Patent>();
			foreach(string xmlstring in System.IO.Directory.GetFiles ("/home/edo/pws/res3"))
			{
				var sr = new System.IO.StreamReader(xmlstring);
				Patents.Add (new Patent(sr.ReadToEnd ()));
				sr.Close ();
			}
			foreach(Patent patent in Patents)
			{
				Console.WriteLine (patent.xml);
			}
		}
	}
	struct Patent
	{
		public Patent(string xmlstring)
		{
			var tr = new System.IO.TextReader();
			var xr = new System.Xml.XmlReader();
			this.xml = xr.
		}
		private System.Xml.XmlDocument xml;
		public string pubdocdb
		{
			get
			{
				
			}
		}
		public string fulltext
		{
			get
			{
				
			}
		}
	}
}
