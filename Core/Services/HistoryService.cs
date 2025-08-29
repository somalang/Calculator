//계산 내역 저장 및 관리

using System;
using System.Collections.ObjectModel;
using Calculator.Core.Models;

namespace Calculator.Core.Services
{
    public class HistoryService : IHistoryProvider
    {
        //계산 기록 보관
        public ObservableCollection<HistoryItem> Items { get; }

        public HistoryService()
        {
            Items = new ObservableCollection<HistoryItem>();
        }

        //새로운 계산 기록 생성
        public void Add(string expression, CalcValue result)
        {
            Items.Insert(0, new HistoryItem
            {
                Expression = expression,
                Result = result,
                Timestamp = DateTime.Now
            });
        }

        //특정 항목 기록 제거
        public void Remove(HistoryItem item)
        {
            if (Items.Contains(item))
                Items.Remove(item);
        }

        //기록 전체 삭제
        public void Clear()
        {
            Items.Clear();
        }
    }
}
