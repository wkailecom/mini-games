using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrewJam
{
    public class ScrewView : MonoBehaviour
    {
        public Color Color { get; private set; }

        void Start()
        {

        }

        public void SetColor(Color color)
        {
            this.Color = color;
            var renderers = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].color = color;
            }
        }

        public void Update()
        {
            transform.GetChild(0).rotation = Quaternion.identity;
        }
    }
}
