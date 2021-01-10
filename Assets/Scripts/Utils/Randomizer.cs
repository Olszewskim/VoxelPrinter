using System;
using System.Collections.Generic;
using URandom = UnityEngine.Random;

public static class Randomizer {

    public static float GetRandomNumber((float min, float max) range) {
        return GetRandomNumber(range.min, range.max);
    }

    public static float GetRandomNumber(float min, float max) {
        InitRandomSeed();
        return URandom.Range(min, max);
    }

    public static int GetRandomNumber(int min, int max) {
        InitRandomSeed();
        return URandom.Range(min, max);
    }

    public static void InitRandomSeed() {
        var seed = Guid.NewGuid().GetHashCode();
        URandom.InitState(seed);
    }

    public static T GetRandomElement<T>(this List<T> list) {
        if (list.Count == 0) {
            return default;
        }
        return list[GetRandomNumber(0, list.Count)];
    }

    public static T GetRandomElement<T>(this T[] array) {
        if (array.Length == 0) {
            return default;
        }
        return array[GetRandomNumber(0, array.Length)];
    }

    public static bool GetRandomDecision(float percentChance) {
        return GetRandomNumber(0f, 1f) <= percentChance;
    }

    internal static bool GetRandomDecision() {
        return GetRandomNumber(0, 2) == 0;
    }
}
