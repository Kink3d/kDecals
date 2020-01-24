using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using kTools.Pooling;

namespace kTools.Decals.Editor.Tests
{
    public sealed class DecalTests
    {
#region Fields
        DecalData m_DecalDataPooled;
#endregion

#region Properties
        DecalData decalDataPooled
        {
            get
            {
                if(m_DecalDataPooled == null)
                    m_DecalDataPooled = Resources.Load("TestDecalPooled") as DecalData;
                return m_DecalDataPooled;
            }
        }
#endregion

#region Setup
        [SetUp]
        public void SetUp()
        {
            // Ensure Pools are set up
            if(!DecalSystem.HasDecalPool(decalDataPooled))
            {
                DecalSystem.CreateDecalPool(decalDataPooled);
            }
        }
#endregion

#region Tests
        [Test]
        public void CanRegisterDecal()
        {
            // Execution
            var decal = DecalSystem.GetDecal(decalDataPooled);
            var registered = DecalSystem.decals.Contains(decal);

            // Result
            Assert.IsTrue(registered);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanUnregisterDecal()
        {
            // Execution
            var decal = DecalSystem.GetDecal(decalDataPooled);
            DecalSystem.RemoveDecal(decal);
            var registered = DecalSystem.decals.Contains(decal);

            // Result
            Assert.IsFalse(registered);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanSetTransform()
        {
            // Execution
            var positon = Vector3.one;
            var direction = new Vector3(0, 0, 1);
            var scale = Vector2.one;
            var decal = DecalSystem.GetDecal(decalDataPooled, positon, direction, scale);
            var forward = Vector3.zero;

            // Result
            Assert.AreEqual(positon, decal.transform.position);
            Assert.AreEqual(forward, decal.transform.eulerAngles);
            Assert.AreEqual(scale, new Vector2(decal.transform.localScale.x, decal.transform.localScale.y));
            LogAssert.NoUnexpectedReceived();
        }
#endregion
    }
}
