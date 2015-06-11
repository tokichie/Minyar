using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FP.DAL.DAO {
	public class Item {
		public int SupportCount { get; set; }

		public string Symbol { get; private set; }

		//constructors
		public Item() : this(null, -1) {
		}

		public Item(string _symbol) : this(_symbol, -1) {
		}

		public Item(string _symbol, int _supportCount) {
			Symbol = _symbol;
			SupportCount = _supportCount;
		}

		public Item Clone() {
			Item item = new Item(Symbol, SupportCount);
			return item;
		}
	}
}
