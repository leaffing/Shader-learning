using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample{

    public class SampleClass
    {

        public int SampleIntReturn(int input)
        {
            return input * 2;
        }
    }

    public class SampleClassMonoBehavior : MonoBehaviour
    {

        private bool m_IsUpdateNeeded = false;
        private int count = 0;
        // Use this for initialization
        void Start()
        {
            transform.position.Set(0, 0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_IsUpdateNeeded)
            {
                m_IsUpdateNeeded = false;
                transform.position += new Vector3(1f, 1f, 1f);
            }

        }

        public void TriggerUpdate()
        {
            m_IsUpdateNeeded = true;
        }

        public int NumOfCall()
        {
            return ++count;
        }
    }
}