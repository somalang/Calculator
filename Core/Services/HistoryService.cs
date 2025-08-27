using System;
using System.Collections.ObjectModel;
using Calculator.Core.Models;

namespace Calculator.Core.Services
{
    public class HistoryService : IHistoryProvider
    {
        public ObservableCollection<HistoryItem> Items { get; }

        public HistoryService()
        {
            Items = new ObservableCollection<HistoryItem>();
        }

        public void Add(string expression, CalcValue result)
        {
            Items.Insert(0, new HistoryItem
            {
                Expression = expression,
                Result = result,
                Timestamp = DateTime.Now
            });
        }

        public void Remove(HistoryItem item)
        {
            if (Items.Contains(item))
                Items.Remove(item);
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
