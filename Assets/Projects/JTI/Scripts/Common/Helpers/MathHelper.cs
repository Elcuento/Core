using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JTI.Scripts.Common;
using UnityEngine;

namespace JTI.Examples
{
    public  static class MathHelper
    {
        public class PercentDrop<T>
        {
            public float Percent;
            public T Object;
        }
        public static T GetDrop<T>(List<PercentDrop<T>> drops)
        {
            var totalChance = drops.Sum(x => x.Percent);
            var slotsChances = new Dictionary<PercentDrop<T>, float>();

            for (var index = 0; index < drops.Count; index++)
            {
                var slot = drops[index];
                var percent = slot.Percent;
                var realChance = (percent / totalChance);

                slotsChances.Add(slot, realChance);
            }


            var sorted = slotsChances.OrderBy(x => x.Key.Percent).ToList();

            var chance = UnityEngine.Random.Range(0f, totalChance);


            var count = 0f;
            foreach (var item in sorted)
            {
                if (chance < count + item.Value)
                {
                    var sameChance = sorted.Where(x => x.Value == chance).ToList();
                    if (sameChance.Count > 0)
                    {
                        var random = UnityEngine.Random.Range(0, sorted.Count);

                        return sorted[random].Key.Object;
                    }

                    return item.Key.Object;
                }

                count += item.Value;
            }

            return sorted.RandomElement().Key.Object;

        }
    }
}