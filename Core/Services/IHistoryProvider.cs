// 히스토리 인터페이스

using System.Collections.ObjectModel;
using Calculator.Core.Models;

namespace Calculator.Core.Services
{
    public interface IHistoryProvider
    {
        ObservableCollection<HistoryItem> Items { get; }

        void Add(string expression, CalcValue result);
        void Remove(HistoryItem item);
        void Clear();
    }
}
 