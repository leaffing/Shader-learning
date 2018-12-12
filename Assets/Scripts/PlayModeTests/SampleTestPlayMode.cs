using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;


namespace Sample.Tests
{
    public class SampleClassMonoBehaviorPlayModeTests
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

        [UnityTest]
        public IEnumerator ShouldUpdatePosition()
        {
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
            m_Target.TriggerUpdate();
            Assert.AreEqual(new Vector3(0, 0, 0), m_Target.transform.position);
            //yield return new WaitForFixedUpdate();
            yield return null;
            Assert.AreEqual(new Vector3(1f, 1f, 1f), m_Target.transform.position);
        }

        [UnityTest]
        public IEnumerator IsApplicationRunning()
        {
            yield return null;
            Assert.IsTrue(Application.isPlaying);
        }
    }
}