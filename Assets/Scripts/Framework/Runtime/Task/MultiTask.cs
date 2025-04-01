using System.Collections;

namespace LLFramework
{
    public class MultiTask : PoolObjectBase
    {
        public static IEnumerator DoMultiTask(params IEnumerator[] pTasks)
        {
            var tMultiTask = ObjectPoolManager.Instance.GetPoolObject<MultiTask>();
            yield return tMultiTask.StartTasks(pTasks);
            ObjectPoolManager.Instance.ReturnPoolObject(ref tMultiTask);
        }

        int mDoingTaskCount;

        IEnumerator StartTasks(params IEnumerator[] pTasks)
        {
            mDoingTaskCount = 0;

            for (int i = 0; i < pTasks.Length; i++)
            {
                TaskManager.Instance.StartTask(DoTask(pTasks[i]));
            }

            while (mDoingTaskCount > 0)
            {
                yield return null;
            }
        }

        IEnumerator DoTask(IEnumerator pTask)
        {
            ++mDoingTaskCount;
            yield return pTask;
            --mDoingTaskCount;
        }
    }
}