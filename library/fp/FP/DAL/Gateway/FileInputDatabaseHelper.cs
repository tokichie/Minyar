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
				inputFilePointer = new StreamReader(Path);//open file for streaming
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
		public List<Item> GetNextTransaction()
		{
			var transaction = new List<Item>();
			string line=""; 
			try
			{
				if ((line = inputFilePointer.ReadLine()) != null) {
				    var itemWrapper = ItemWrapper.Deserialize(line.Trim());
				    foreach (var item in itemWrapper.Items) {
				        transaction.Add(new Item(item.Symbol));
				    }
				    //transaction = new list<string>(line.trim().split(new []{"$&$"}, stringsplitoptions.removeemptyentries));
				    //transaction = transaction.Select(s => s.Trim()).ToList();
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
		public List<Item> CalculateFrequencyAllItems()
		{
			var items = new List<Item>();
			var dictionary = new Dictionary<string, int>();
            var jsonItemsDic = new Dictionary<string, List<JsonItem>>();
			string line;
			StreamReader file ;
            try {
                file = new StreamReader(Path);//open file for streaming
				while ((line = file.ReadLine()) != null) {
				    var itemWrapper = ItemWrapper.Deserialize(line.Trim());
                    var isAdded = new HashSet<string>();
					foreach(JsonItem item in itemWrapper.Items) {
					    item.GithubUrl = itemWrapper.GithubUrl;
					    if (!jsonItemsDic.ContainsKey(item.Symbol)) {
					        jsonItemsDic[item.Symbol] = new List<JsonItem>();
					    }
                        jsonItemsDic[item.Symbol].Add(item);
					    if (!isAdded.Contains(item.Symbol)) {
					        isAdded.Add(item.Symbol);
					        if (dictionary.ContainsKey(item.Symbol)) {
					            dictionary[item.Symbol]++; // increase frequency of item
					        } else {
					            dictionary[item.Symbol] = 1; //set initial frequency
					        }
					    }
					}
				}

				file.Close(); // close file
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

		    foreach (var dicItem in dictionary) {
		        var item = new Item(dicItem.Key, dicItem.Value);
		        item.JsonItems = jsonItemsDic[dicItem.Key];
                items.Add(item);
		    }

			return items;
		}

		//get frequency of an item set
		//public int GetFrequency(JsonItemSet itemSet)
		//{
		//	int frequency = 0;
		//	IDictionary<string, int> dictionary = new Dictionary<string, int>(); // temporary associative array for counting frequency of items
		//	string line;
		//	System.IO.StreamReader file;
		//	try
		//	{
		//		file = new System.IO.StreamReader(Path);//open file for streaming
		//		while ((line = file.ReadLine()) != null)
		//		{
		//			string[] tempItems = line.Split(' ');
		//			dictionary.Clear();
		//			foreach (string tempItem in tempItems)
		//			{
		//				string item = tempItem.Trim();
		//				dictionary[item] = 1; //set dictionary for this item
		//			}

		//			bool itemSetExist = true; //indicates if this transaction contains itemset 
		//			for(int i=0; i<itemSet.GetLength(); ++i)
		//			{
		//				JsonItem item = itemSet.GetItem(i);
		//				if(!dictionary.ContainsKey(item.Symbol))
		//				{
		//					itemSetExist = false;
		//					break;
		//				}
		//			}
		//			if(itemSetExist)
		//			{
		//				frequency++;
		//			}
		//		}

		//		file.Close(); // close file
		//	}
		//	catch (Exception e)
		//	{
		//		Console.WriteLine(e.Message);
		//		Console.WriteLine(e.StackTrace);
		//	}

		//	itemSet.SupportCount = frequency;
		//	return frequency;
		//}

	}
}
