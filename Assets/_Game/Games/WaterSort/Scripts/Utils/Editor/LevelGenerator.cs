#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using _Game.Games.WaterSort.Scripts.Config;

public class LevelGenerator
{
    [MenuItem("Assets/WaterSort/Generate Level Colors")]
    public static void GenerateColors()
    {
        LevelConfigSO level = Selection.activeObject as LevelConfigSO;
        if (level == null) { Debug.LogError("Chọn file LevelConfigSO trước!"); return; }

        Undo.RecordObject(level, "Generate Level");

        int totalBottles = 0;
        foreach (int count in level.bottlesPerRow) totalBottles += count;

        if (totalBottles == 0 || level.colorCount >= totalBottles)
        {
            Debug.LogError("Cấu hình Layout hoặc Color Count không hợp lệ!");
            return;
        }

        // 1. Tạo trạng thái THẮNG (Solved State)
        List<List<int>> tempBottles = new List<List<int>>();
        for (int i = 0; i < level.colorCount; i++)
        {
            List<int> fullBottle = new List<int>();
            for (int j = 0; j < level.bottleCapacity; j++) fullBottle.Add(i);
            tempBottles.Add(fullBottle);
        }
        int emptyCount = totalBottles - level.colorCount;
        for (int i = 0; i < emptyCount; i++) tempBottles.Add(new List<int>());

        // 2. Trộn Ngược (Reverse Shuffle)
        int steps = 0;
        int safety = 0;
        while (steps < level.shuffleSteps && safety < 50000)
        {
            safety++;
            int from = Random.Range(0, totalBottles);
            int to = Random.Range(0, totalBottles);
            if (from == to) continue;

            List<int> src = tempBottles[from];
            List<int> dst = tempBottles[to];

            if (src.Count > 0 && dst.Count < level.bottleCapacity)
            {
                int color = src[src.Count - 1];
                src.RemoveAt(src.Count - 1);
                dst.Add(color);
                steps++;
            }
        }

        // 3. Lưu kết quả
        level.bottles = new List<BottleSetupData>();
        for (int i = 0; i < tempBottles.Count; i++)
        {
            BottleSetupData data = new BottleSetupData();
            data.colors = new List<int>(tempBottles[i]);
            level.bottles.Add(data);
        }

        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();
        Debug.Log($"<color=green>Đã sinh Level: {steps} bước trộn!</color>");
    }
}
#endif