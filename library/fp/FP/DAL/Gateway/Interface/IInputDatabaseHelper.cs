using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FP.DAL.DAO;
using FP.DAO;

namespace FP.DAL.Gateway.Interface
{
    public interface IInputDatabaseHelper
    {
        string DatabaseType //indicates database type (TEXTFILE,SQL etc.)
        {
            get;
        }
        string DatabaseName //indicates database type (TEXTFILE,SQL etc.)
        {
            get;
        }
        int TotalTransactionNumber
        {
            get;
        }

        void OpenDatabaseConnection();
        void CloseDatabaseConnection();
        List<string> GetNextTransaction();
        List<JsonItem> CalculateFrequencyAllItems(); //get frequency or support count of all 1-itemsets or items
        int GetFrequency(JsonItemSet itemSet); //get frequency of an item set
    }
}
