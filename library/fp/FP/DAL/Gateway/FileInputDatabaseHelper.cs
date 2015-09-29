using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using FP.DAL.DAO;
using FP.DAL.Gateway.Interface;
using System.IO;
using FP.DAO;

namespace FP.DAL.Gateway
{
	public class FileInputDatabaseHelper : IInputDatabaseHelper
	{
		private string dbType; // database type
		private string dbName; // database type
		private StreamReader inputFilePointer; 
		public int TotalTransactionNumber { get; private set; }
		public string DatabaseType
		{
			get { return dbType; }           
		}
		public string DatabaseName
		{
			get { return dbName; }
		}
		public string Path { get; private set; }

		public void OpenDatabaseConnection()
		{
			try
			{
				inputFilePointer = new System.IO.StreamReader(Path);//open file for streaming
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
		public void CloseDatabaseConnection()
		{
			try
			{
				inputFilePointer.Close();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
		public List<string> GetNextTransaction()
		{
			List<string> transaction = new List<string>();
			string line=""; 
			try
			{
				if ((line = inputFilePointer.ReadLine()) != null)
				{
					transaction = new List<string>(line.Trim().Split(new []{"$&$"}, StringSplitOptions.RemoveEmptyEntries));
					transaction = transaction.Select(s => s.Trim()).ToList();
				}
				else
				{
					return transaction;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				return transaction;
			}
			return transaction;
		}

		//constructor 
		public FileInputDatabaseHelper(string path)
		{
			Path = path;
			TotalTransactionNumber = 0;
			ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["FileDB"]; // get connection settings for File Type Database
			string line;
			System.IO.StreamReader file;
			try
			{
				file = new System.IO.StreamReader(Path);//open file for streaming

				while ((line = file.ReadLine()) != null) TotalTransactionNumber++;

				file.Close(); // close file
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
		}

		//get support count of all items
		public List<JsonItem> CalculateFrequencyAllItems()
		{
			List<JsonItem> items = new List<JsonItem>();
			IDictionary<string, List<string>> astItems = new Dictionary<string, List<string>>();
			IDictionary<string, int> dictionary = new Dictionary<string, int>(); // temporary associative array for counting frequency of items
			string line;
			System.IO.StreamReader file ;
            //try
            //{
				file = new System.IO.StreamReader(Path);//open file for streaming
				while ((line = file.ReadLine()) != null) {
				    string[] tempItems = line.Split(new[] {"$&$"}, StringSplitOptions.RemoveEmptyEntries);
					foreach(string tempItem in tempItems)
					{
						string item = tempItem.Trim();
						if (item.Length == 0) continue;
                        //var nodeName = item.Substring(0, item.IndexOf("|"));
                        //var metaData = item.Substring(item.IndexOf("|"));
					    var nodeName = item;
					    var metaData = "hoge";
					    if (dictionary.ContainsKey(nodeName)) {
					        dictionary[nodeName]++; // increase frequency of item
					    } else {
					        dictionary[nodeName] = 1; //set initial frequency
                            astItems[nodeName] = new List<string>();
					    }
                        astItems[nodeName].Add(metaData);
					}
				}

				file.Close(); // close file
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Console.WriteLine(e.StackTrace);
            //}
			//insert all the item, frequency pair in items list
			foreach (KeyValuePair<string, int> pair in dictionary)
			{
				JsonItem anItem = new JsonItem(pair.Key, pair.Value);
				items.Add(anItem);
			}

			return items;
		}

		//get frequency of an item set
		public int GetFrequency(JsonItemSet itemSet)
		{
			int frequency = 0;
			IDictionary<string, int> dictionary = new Dictionary<string, int>(); // temporary associative array for counting frequency of items
			string line;
			System.IO.StreamReader file;
			try
			{
				file = new System.IO.StreamReader(Path);//open file for streaming
				while ((line = file.ReadLine()) != null)
				{
					string[] tempItems = line.Split(' ');
					dictionary.Clear();
					foreach (string tempItem in tempItems)
					{
						string item = tempItem.Trim();
						dictionary[item] = 1; //set dictionary for this item
					}

					bool itemSetExist = true; //indicates if this transaction contains itemset 
					for(int i=0; i<itemSet.GetLength(); ++i)
					{
						JsonItem item = itemSet.GetItem(i);
						if(!dictionary.ContainsKey(item.Symbol))
						{
							itemSetExist = false;
							break;
						}
					}
					if(itemSetExist)
					{
						frequency++;
					}
				}

				file.Close(); // close file
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}

			itemSet.SupportCount = frequency;
			return frequency;
		}

	}
}
