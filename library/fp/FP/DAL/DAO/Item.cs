using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FP.DAL.DAO {
	public class Item {
		public int SupportCount { get; set; }

		public string Symbol { get; private set; }

        public List<string> AstItems { get; private set; }

		//constructors
		public Item() : this(null, -1, new List<string>()) {
		}

		public Item(string _symbol) : this(_symbol, -1, new List<string>()) {
		}

		public Item(string _symbol, int _supportCount, List<string> astItems ) {
			Symbol = _symbol;
			SupportCount = _supportCount;
		    AstItems = astItems;
		}

		public Item Clone() {
			Item item = new Item(Symbol, SupportCount, AstItems.Select(x => (string)x.Clone()).ToList());
			return item;
		}
	}
}
