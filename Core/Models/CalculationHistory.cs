using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Calculator.Core.Models
{
    public class CalculationHistory
    {
        private readonly List<HistoryItem> items;
        public ReadOnlyCollection<HistoryItem> Items => items.AsReadOnly();

        public CalculationHistory()
        {
            items = new List<HistoryItem>();
        }

        public void AddItem(string expression, CalcValue result)
        {
            items.Add(new HistoryItem
            {
                Expression = expression,
                Result = result,
                Timestamp = DateTime.Now
            });
        }

        public void Clear()
        {
            items.Clear();
        }

        public HistoryItem GetLastItem()
        {
            return items.Count > 0 ? items[^1] : null;
        }
    }

    public class HistoryItem
    {
        public string Expression { get; set; }
        public CalcValue Result { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"{Expression} = {Result}";
        }
    }
}