using UnityEngine;

namespace GameLogic
{
    public class FakeItem : VirusItem
    {
        public GameObject fake;

        public ParticleSystem effect;

        public bool canPath;
        protected override void OnStateChanged(VirusState value)
        {
            switch (value)
            {
                case VirusState.Born:
                    DoFake();
                    VirusState = VirusState.Fake;
                    sourceType = SourceType.Fake;
                    break;
                case VirusState.Reborn:
                {
                    DoFake();
                
                    var pos = JamManager.GetSingleton().TileIndexToPosition(index);
                    transform.localPosition = pos;
                    VirusState = VirusState.Fake;
                    break;
                }
                default:
                    base.OnStateChanged(value);
                    break;
            }
        }

        public override void DoPath()
        {
            Unlock();
            base.DoPath();
        }

        public void DoBorn()
        {
            canPath = true;
            fake.SetActive(false);
            downRenderer.gameObject.SetActive(true);
            skinnedMeshRenderer.gameObject.SetActive(false);
        }

        private void DoFake()
        {
            if(!Animator)
                return;
            canPath = false;
            downRenderer.gameObject.SetActive(false);
            skinnedMeshRenderer.gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            fake.SetActive(true);
            skinnedMeshRenderer.gameObject.SetActive(false);
            StandUp = false;
            Animator.Play("Idle_Short");
        }
        
        public override void Unlock()
        {
            if(VirusState != VirusState.Fake)
                return;
            fake.SetActive(false);
            downRenderer.gameObject.SetActive(false);
            skinnedMeshRenderer.gameObject.SetActive(true);
            effect.Play();
            VirusState = VirusState.Idle;
        }
    }
}