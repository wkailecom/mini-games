

namespace LLFramework
{
    public interface IModule
    {
        /// <summary>
        /// 创建模块
        /// </summary> 
        void OnCreate(object createParam);

        /// <summary>
        /// 轮询模块 
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// 销毁模块
        /// </summary>
        void OnDestroy();

    }

    public class BaseManager<T> : Singleton<T> where T : BaseManager<T>, new()
    {

    }

}
