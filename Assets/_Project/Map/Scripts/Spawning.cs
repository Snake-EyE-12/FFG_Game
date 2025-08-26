using System.Collections.Generic;
using UnityEngine;

public static class Spawning
{
    public static List<Transform> spawnPoints = new List<Transform>();

    private static Queue<Transform> spawnQueue = new Queue<Transform>();
    private static System.Random rng = new System.Random();

    public static Transform GetSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
            return null;

        if (spawnQueue.Count == 0)
            RefillQueue();

        return spawnQueue.Dequeue();
    }

    private static void RefillQueue()
    {
        List<Transform> shuffled = new List<Transform>(spawnPoints);

        // Fisher-Yates shuffle awesome thank you greg
        int n = shuffled.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Transform temp = shuffled[k];
            shuffled[k] = shuffled[n];
            shuffled[n] = temp;
        }

        foreach (var sp in shuffled) spawnQueue.Enqueue(sp);
    }
}
