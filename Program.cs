/*
 * Remove special characters from pos-printer
 * https://github.com/stefankugler/print-qrk-json
 * Stefan Kugler 2017
 */

using System;

namespace removeesc
{
	class Program
	{
		public static void Main(string[] args)
		{
			if(args.Length > 0) {
				//1st argument: full path to json-file
				string filename = @args[0];
				
				try {
					//Get path from filename
					string path = System.IO.Path.GetDirectoryName(@filename);
					
					//Debug: Append current filename to debug.txt
					//System.IO.File.AppendAllText(path + "\\debug.txt", filename + Environment.NewLine);
					
					//Read json-file and remove leading and trailing special chars
					string content = System.IO.File.ReadAllText(@filename);
					string json = content.Substring(content.IndexOf("{"), content.LastIndexOf("}") - content.IndexOf("{") + 1);
					
					//create timestamp for filename and save clean json data
					string outname = DateTime.Now.ToString("yyyyMMdd-HHmmss");
					System.IO.File.WriteAllText(@path + "\\" + outname + ".json", json);
					
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
					Console.Write("Press any key to continue . . . ");
					Console.ReadKey(true);
				}
			}
			
		}
	}
}
