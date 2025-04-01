using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrewJam
{
    public class FlyItem : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void SetColor(Color color)
        {
            var r = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < r.Length; i++)
            {
                r[i].color = color;
            }
        }

        public void AnimUp()
        {
            animator.Play("Up");
        }

        public void AnimDown()
        {
            animator.Play("Down");
        }
    }
}

