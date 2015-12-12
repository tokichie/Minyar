using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FP.DAO;

namespace FP.DAL.DAO {
	public class ItemSet {
		private List<Item> items;
		//list of items in item set

        
		private int supportCount;
		// support count of this item set

		public int SupportCount {
			get { return supportCount; }
			set { supportCount = value; }
		}

	    public int ItemCount {
	        get { return items.Count; }
	    }

	    public string Items {
	        get { return string.Join(", ", items); }
	    }

	    //constructor
		public ItemSet() {
			items = new List<Item>();
			supportCount = -1;
		}
		//add item into item set
		public void AddItem(Item item) {
			items.Add(item);
			supportCount = -1;
		}
		//remove item
		public Item GetItem(int position) {
			if (position < items.Count)
				return items[position];
			else
				return null;
		}
		//add item into item set
		public bool IsEmpty() {
			return items.Count == 0;
		}
		//add item into item set
		public int GetLength() {
			return items.Count;
		}

		public ItemSet Clone() {
			ItemSet itemSet = new ItemSet();
			itemSet.SupportCount = SupportCount;
			foreach (Item anItem in items) {
				itemSet.AddItem(anItem.Clone());
			}
			return itemSet;
		}

		public string GetInfoString() {
			string info = "";
            
			foreach (Item anItem in items) {
				info += (" " + anItem.Symbol.ToString());
			}

			return info;
		}

		public void Print() {
			Console.WriteLine(SupportCount);
			foreach (Item item in items) {
				Console.Write(item.Symbol.ToString() + " ");
			}
			Console.WriteLine();
		}

		public Item GetLastItem() {
			return items.Last();
		}

		public override string ToString() {
			var sb = new StringBuilder();
            var metaData = new StringBuilder();
		    metaData.AppendLine();
			if (items.Count > 0) {
				sb.Append("<");
                items.Sort((i1, i2) => i1.JsonItems.Count.CompareTo(i2.JsonItems.Count));
			    var url = "";
			    var path = "";
			    foreach (var jsonItem in items[0].JsonItems) {
			        url = jsonItem.GithubUrl;
			        path = jsonItem.ChangedPath;
                    if (url.StartsWith("https://github.com/chrisjenx/Calligraphy")) continue;
                    if (url.StartsWith("https://github.com/dropwizard/metrics")) continue;
                    var f = true;
                    var ff = false;
                    foreach (var item in items) {
                        var j = item.JsonItems.Where(i => i.GithubUrl == url && i.ChangedPath == path);
                        f &= item.JsonItems.Any(i => i.GithubUrl == url && i.ChangedPath == path);
                        if (!f) {
                            ff = true;
                            break;
                        }
                        Console.WriteLine("{0} {1} {2}: {3}", url, path, item, j.First());
                    }
			        if (ff) continue;
			        if (f) break;
			    }
			    var c = items[0].JsonItems.Count;
                //var i = new Random().Next(c - 1);
			    metaData.Append(url).Append(" ").Append(path).Append(" ");
				foreach (var item in items) {
					sb.Append(item.Symbol).Append(", ");
                    metaData.Append(item.JsonItems.First(x => x.GithubUrl == url && x.ChangedPath == path)).Append(" ");
				    //            var dic = new Dictionary<string, List<JsonItem>>();
				    //foreach (var jsonItem in item.JsonItems) {
				    //    if (! dic.ContainsKey(jsonItem.GithubUrl)) {
				    //        dic[jsonItem.GithubUrl] = new List<JsonItem>();
				    //    }
        //                dic[jsonItem.GithubUrl].Add(jsonItem);
        //            }
        //            foreach (var dicItem in dic) {
        //                metaData.Append("  ").Append(dicItem.Key).Append(" ");
        //                dicItem.Value.Sort((x, y) => y.OriginalPath.CompareTo(x.OriginalPath));
        //                foreach (var jsonItem in dicItem.Value) {
        //                    metaData.Append(jsonItem).Append(" ");
        //                }
        //                metaData.AppendLine();
        //            }
                }
			    sb.Remove(sb.Length - 2, 2).Append(">");
			}
            return string.Format("[SupportCount={0} {1}]{2}", SupportCount, sb, metaData);
		}
	}
}
