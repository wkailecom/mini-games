using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
   public ParticleSystem ps;
   public Animator animator;
   private static readonly int RunHash = Animator.StringToHash("RunInt");

   public void Start()
   {
      ps.Play();
      animator.SetInteger(RunHash, 1);
   }

   public void OnDestroy()
   {
      ps.Stop();
   }
}
