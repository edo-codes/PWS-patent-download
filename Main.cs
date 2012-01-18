using System;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace pws
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var ns = XNamespace.Get ("http://www.epo.org/exchange");
			var opsns = XNamespace.Get ("http://ops.epo.org");
			
			//Search patents
			var docs = new System.Collections.Generic.List<string> ();
			{
				string query = "milking%20AND%20robot";
				int lastchecked = 0;
				int total = 0;
				int resultsperpage = 100;
				do {
					var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create (
						"http://ops.epo.org/2.6.2/rest-services/published-data/search/abstract/?q=" + query + "&Range=" + (lastchecked + 1) + "-" + (lastchecked + resultsperpage));
					var res = req.GetResponse ();
					var rdr = new System.IO.StreamReader (res.GetResponseStream ());
					string xmldoc = rdr.ReadToEnd ();
					XDocument results = XDocument.Parse (xmldoc);
					total = int.Parse (results.Root.Descendants (opsns + "biblio-search").Single ().Attribute ("total-result-count").Value);
					lastchecked = lastchecked + resultsperpage;
					foreach (XElement docid in
					         results.Root.Descendants (ns+"document-id")
					         .Where (x=>x.Attribute ("document-id-type").Value == "epodoc")) {
						docs.Add (docid.Element (ns + "doc-number").Value);
					}
					Console.WriteLine ("total=" + total + " and lastchecked=" + lastchecked);
				} while(lastchecked < total);
			}
			
			//retrieve and make pdf
			foreach (string doc in docs) {
				try {
					Console.WriteLine ("Making " + doc);
					//retrieve images xml
					XDocument imgdoc;
					{
						string str = "http://ops.epo.org/2.6.2/rest-services/published-data/publication/epodoc/"
							+ doc + "/images";
						var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create (str);
						req.Accept = "application/ops+xml";
						var res = req.GetResponse ().GetResponseStream ();
						var rdr = new System.IO.StreamReader (res, true);
						imgdoc = XDocument.Parse (rdr.ReadToEnd ());
					}
					Console.WriteLine ("imgdoc: " + imgdoc.ToString().Length);
				
					//retrieve every page
					var pageslist = new System.Collections.Generic.List<System.IO.Stream> ();
					{
						XElement docel = imgdoc.Root.Descendants (opsns + "document-instance")
							.Single (x => x.Attribute ("desc").Value == "FullDocument");
						string pdflink = "http://ops.epo.org/2.6.2/rest-services/" + docel.Attribute ("link").Value;
						int pdfpages = int.Parse (docel.Attribute ("number-of-pages").Value);
					
						for (int i = 1; i <= pdfpages; i++) {
							var str = pdflink + "&Range=" + i;
							Console.WriteLine (str);
							var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create (str);
							req.Accept = "application/pdf";
							var res = req.GetResponse ();
							pageslist.Add (res.GetResponseStream ());
						}
					}
				} catch (Exception ex) {
					Console.WriteLine (doc + " failed: ");
					if (ex is System.Net.WebException) {
						Console.WriteLine (ex.ToString ());
					}
				}
			}//foreach
			
			
			
			
		}
	}
}