using NUnit.Framework;
using Point.Collections.ResourceControl;
using System;
using UnityEngine;

namespace Point.Collections.Tests
{
    public class GeneralTests
    {
        [Test]
        public void ConstTypeGuidTest()
        {
            Guid guid = TypeHelper.TypeOf<AssetInfo>.Type.GUID;
            $"{guid} : {TypeHelper.TypeOf<AssetInfo>.Type.IsVisible} : {TypeHelper.TypeOf<AssetInfo>.Type.IsCOMObject}".ToLog();

            Type assetInfoType = Type.GetTypeFromCLSID(Guid.Parse("{b92cc9a9-b577-4759-b623-d794bd86d430}"));
            object assetInfoObj = Activator.CreateInstance(assetInfoType);
            Assert.AreEqual("AssetInfo", assetInfoObj.GetType().Name);
        }
    }
}
