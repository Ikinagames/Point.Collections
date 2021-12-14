using NUnit.Framework;
using System.Collections.Generic;

namespace Point.Collections.Tests
{
    public sealed class FNV1aTests
    {
        [Test]
        public void FNV1a32StringTest()
        {
            string temp = "TEST";

            $"{temp} :: {FNV1a32.Calculate(in temp)}".ToLog();
        }
    }
}
