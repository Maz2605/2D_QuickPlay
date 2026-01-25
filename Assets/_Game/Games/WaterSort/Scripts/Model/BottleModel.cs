using System.Collections.Generic;
using UnityEngine;

namespace _Game.Games.WaterSort.Scripts.Model
{
    [System.Serializable]
    public class BottleModel
    {
        [SerializeField] private int _capacity;
        [SerializeField] private List<int> _liquids;

        public int Capacity => _capacity;
        public int Count => _liquids.Count;
        public bool IsEmpty => _liquids.Count == 0;
        public bool IsFull => _liquids.Count >= _capacity;
        
        public int TopColor => IsEmpty ? -1 : _liquids[_liquids.Count - 1];

        public BottleModel(int capacity)
        {
            _capacity = capacity;
            _liquids = new List<int>();
        }

        public void Push(int colorId)
        {
            if (!IsFull) _liquids.Add(colorId);
        }

        public int Pop()
        {
            if (IsEmpty) return -1;
            int topIndex = _liquids.Count - 1;
            int color = _liquids[topIndex];
            _liquids.RemoveAt(topIndex);
            return color;
        }

        // --- MỚI: Dùng cho Undo (Force Push không kiểm tra logic) ---
        public void UndoPush(int colorId)
        {
            _liquids.Add(colorId);
        }   

        public bool CanPush(int colorId)
        {
            if (IsFull) return false;
            if (IsEmpty) return true;
            return TopColor == colorId;
        }

        public int[] GetLiquidsForView() => _liquids.ToArray();

        public int GetCountSameTopColor()
        {
            if (IsEmpty) return 0;
            int count = 0;
            int topColor = TopColor;
            for (int i = _liquids.Count - 1; i >= 0; i--)
            {
                if (_liquids[i] == topColor) count++;
                else break;
            }
            return count;
        }

        public bool IsUniformColor()
        {
            if (IsEmpty) return false;
            int firstColor = _liquids[0];
            for (int i = 1; i < _liquids.Count; i++)
                if (_liquids[i] != firstColor) return false;
            return true;
        }

        public bool IsSolved() => !IsEmpty && IsFull && IsUniformColor();
        
        // Hỗ trợ Reset/Reshuffle
        public List<int> ClearAndGetAllLiquids()
        {
            List<int> currentData = new List<int>(_liquids);
            _liquids.Clear();
            return currentData;
        }
        
        public void ForceSetLiquids(List<int> newLiquids) => _liquids = new List<int>(newLiquids);
    }
}