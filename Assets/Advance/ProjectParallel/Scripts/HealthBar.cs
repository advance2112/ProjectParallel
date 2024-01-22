using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Advance.ProjectParallel
{
    public class HealthBar : MonoBehaviour
    {
        public GameObject foreground;
        private float percentage = 1.0f;

        void Start()
        {
            foreground.transform.localScale = new Vector3(1, 1, 1);
        }

        public void AddDamage(float percentAdded)
        {
            SetDamage(percentage + percentAdded);
        }

        public void SetDamage(float percent)
        {
            float newPercentage = Mathf.Clamp(percent, 0f, 1f);
            foreground.transform.localScale = new Vector3(newPercentage, 1, 1);
            percentage = newPercentage;
        }
    }
}