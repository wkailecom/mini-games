using System.Collections;

namespace LLFramework
{
    public class TaskManager : MonoSingleton<TaskManager>
    {
        public void StartTask(IEnumerator pCoroutine)
        {
            StartCoroutine(pCoroutine);
        }

        public void StopTask(IEnumerator pCoroutine)
        {
            StopCoroutine(pCoroutine);
        }
    }
}