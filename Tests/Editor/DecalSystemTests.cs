using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using kTools.Pooling;

namespace kTools.Decals.Editor.Tests
{
    public sealed class DecalSystemTests
    {
#region Fields
        DecalData m_DecalData;
        DecalData m_DecalDataPooled;
#endregion

#region Properties
        DecalData decalData
        {
            get
            {
                if(m_DecalData == null)
                    m_DecalData = Resources.Load("TestDecal") as DecalData;
                return m_DecalData;
            }
        }

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
        public void CanCreateDecalPool()
        {
            // Execution
            var hasPool = DecalSystem.HasDecalPool(decalDataPooled);

            // Result
            Assert.IsTrue(hasPool);
            LogAssert.NoUnexpectedReceived();
        }
        
        [Test]
        public void CanDestroyDecalPool()
        {
            // Execution
            DecalSystem.DestroyDecalPool(decalDataPooled);
            var hasPool = DecalSystem.HasDecalPool(decalDataPooled);

            // Result
            Assert.IsFalse(hasPool);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanGetDecal()
        {
            // Execution
            var decal = DecalSystem.GetDecal(decalData);
            var hasPool = PoolingSystem.HasPool<DecalData>(decalData);

            // Result
            Assert.IsFalse(hasPool);
            Assert.IsNotNull(decal);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanGetDecalPooled()
        {
            // Execution
            var hasPool = DecalSystem.HasDecalPool(decalDataPooled);
            var decal = DecalSystem.GetDecal(decalDataPooled);

            // Result
            Assert.IsTrue(hasPool);
            Assert.IsNotNull(decal);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanRemoveDecal()
        {
            // Execution
            var decal = DecalSystem.GetDecal(decalData);
            var obj = decal.gameObject;
            DecalSystem.RemoveDecal(decal);

            // Result
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanRemoveDecalPooled()
        {
            // Execution
            var decal = DecalSystem.GetDecal(decalDataPooled);
            DecalSystem.RemoveDecal(decal);
            var isRemoved = !decal.gameObject.activeSelf;

            // Result
            Assert.IsTrue(isRemoved);
            Assert.IsNotNull(decal);
            LogAssert.NoUnexpectedReceived();
        }
#endregion
    }
}
