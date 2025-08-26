using Calculator.Core.Models;
using System.Collections.ObjectModel;

namespace Calculator.UI.ViewModels
{
    public interface IHistoryProvider
    {
        ObservableCollection<HistoryItem> HistoryItems { get; }
        void RemoveHistoryItem(HistoryItem item);
        void ClearHistory();
    }
}
