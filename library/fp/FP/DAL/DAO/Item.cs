﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FP.DAO;

namespace FP.DAL.DAO {
	public class Item {
		public int SupportCount { get; set; }

		public string Symbol { get; private set; }

	    public List<JsonItem> JsonItems;

		//constructors
		public Item() : this(null, -1) {
		}

		public Item(string _symbol) : this(_symbol, -1) {
		}

		public Item(string _symbol, int _supportCount) {
			Symbol = _symbol;
			SupportCount = _supportCount;
		    JsonItems = new List<JsonItem>();
		}

		public Item Clone() {
		    var item = new Item(Symbol, SupportCount);
		    item.JsonItems = JsonItems;
		    return item;
		}

	    public override string ToString() {
	        return Symbol;
	    }
	}
}
