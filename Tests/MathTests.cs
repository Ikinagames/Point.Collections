using NUnit.Framework;
using UnityEngine;

namespace Point.Collections.Tests
{
    public sealed class MathTests
    {
        [Test]
        public void TodBTest()
        {
            float linear = .4f;

            float dB = Math.TodB(linear);

            Debug.Log($"linear:{linear} :: dB:{dB}");
            Debug.Log($"linear:0 :: dB:{Math.TodB(0)}");
            Debug.Log($"linear:1 :: dB:{Math.TodB(1)}");
        }
        [Test]
        public void FromdBTest()
        {
            float dB = -5;

            float linear = Math.FromdB(dB);

            Debug.Log($"linear:{linear} :: dB:{dB}");
        }
    }
}
