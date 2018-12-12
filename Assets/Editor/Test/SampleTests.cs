using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;


namespace Sample.Tests
{
    public class SampleClassTests
    {
        SampleClass m_Target;

        [SetUp]
        public void SetUp()
        {
            m_Target = new SampleClass();
        }

        [Test]
        public void SampleTestsSimplePasses()
        {
            Assert.AreEqual(2, m_Target.SampleIntReturn(1));
        }

    }

    public class SampleClassMonoBehaviorTests
    {
        SampleClassMonoBehavior m_Target;
        GameObject m_GO;

        [SetUp]
        public void SetUp()
        {
            m_GO = new GameObject("TestGameObject");
            m_Target = m_GO.AddComponent<SampleClassMonoBehavior>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_GO);
        }

        [Test]
        public void ShouldReturnCallCount()
        {
            Assert.AreEqual(1, m_Target.NumOfCall());
        }
        [Test]
        public void ShouldRecordCallCount()
        {
            m_Target.NumOfCall();
            Assert.AreEqual(2, m_Target.NumOfCall());
        }

        [UnityTest]
        public IEnumerator ShouldNotUpdatePosition()
        {
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
            m_Target.TriggerUpdate();
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
            yield return null;
            // editor 中等待的是 EditorApplication.update callback loop
            // 所以在这里不会更新位置
            // https://docs.unity3d.com/ScriptReference/EditorApplication-update.html
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);

        }
        [UnityTest]
        public IEnumerator ShouldNotUpdatePositionWithSet()
        {
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
            m_Target.transform.position.Set(1, 1, 1);
            yield return null;
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
        }

        [UnityTest]
        public IEnumerator ShouldUpdatePosition()
        {
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
            m_Target.transform.position = new Vector3(1f, 1f, 1f);
            yield return null;
            Assert.AreEqual(new Vector3(1f, 1f, 1f), m_Target.transform.position);

        }

        [Test]
        public void ShouldUpdatePositionInEditMode()
        {
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
            m_Target.transform.position = new Vector3(1f, 1f, 1f);
            Assert.AreEqual(new Vector3(1f, 1f, 1f), m_Target.transform.position);

        }
        [Test]
        public void ShouldNotUpdatePositionWithSetInEditMode()
        {
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
            m_Target.transform.position.Set(1, 1, 1);
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
        }
    }
}