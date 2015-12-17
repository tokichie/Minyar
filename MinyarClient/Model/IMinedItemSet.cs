using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinyarClient.Model {
    public interface IMinedItemSet<T> : IEnumerable<T> {
        int SupportCount { get; }
        int ItemCount { get; }
        int Items { get; }
    }
}
