using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"DOTween.dll",
		"FancyScrollView.dll",
		"LLFramework.dll",
		"Newtonsoft.Json.dll",
		"PackageJam3d.dll",
		"PackageScrew.dll",
		"System.Core.dll",
		"System.dll",
		"Unity.Addressables.dll",
		"Unity.ResourceManager.dll",
		"Unity.Usercentrics.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.Purchasing.dll",
		"UnityEngine.UI.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// DG.Tweening.Core.DOGetter<UnityEngine.Vector2>
	// DG.Tweening.Core.DOGetter<float>
	// DG.Tweening.Core.DOGetter<int>
	// DG.Tweening.Core.DOSetter<UnityEngine.Vector2>
	// DG.Tweening.Core.DOSetter<float>
	// DG.Tweening.Core.DOSetter<int>
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// DelegateList<float>
	// FancyScrollView.FancyCell<object,object>
	// FancyScrollView.FancyScrollRect<object,object>
	// FancyScrollView.FancyScrollRect<object>
	// FancyScrollView.FancyScrollRectCell<object,object>
	// FancyScrollView.FancyScrollRectCell<object>
	// FancyScrollView.FancyScrollView<object,object>
	// Jam3d.Singleton<object>
	// LLFramework.MonoSingleton<object>
	// LLFramework.Singleton<object>
	// MonoPool<object>
	// ScrewJam.MonoSingleton<object>
	// System.Action<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Action<System.DateTime,byte>
	// System.Action<System.ValueTuple<int,int>>
	// System.Action<TileGamePage.FloorRootItem>
	// System.Action<TileSlotData>
	// System.Action<UnityEngine.EventSystems.RaycastResult>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Action<UnityEngine.UIVertex>
	// System.Action<byte,object>
	// System.Action<byte>
	// System.Action<float>
	// System.Action<int,byte>
	// System.Action<int>
	// System.Action<object,object,object,object,object,object>
	// System.Action<object,object,object,object>
	// System.Action<object,object>
	// System.Action<object>
	// System.ByReference<ushort>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<int,int>>
	// System.Collections.Generic.ArraySortHelper<TileGamePage.FloorRootItem>
	// System.Collections.Generic.ArraySortHelper<TileSlotData>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.UIVertex>
	// System.Collections.Generic.ArraySortHelper<float>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.Comparer<System.Nullable<TileSlotData>>
	// System.Collections.Generic.Comparer<System.ValueTuple<int,int>>
	// System.Collections.Generic.Comparer<TileGamePage.FloorRootItem>
	// System.Collections.Generic.Comparer<TileSlotData>
	// System.Collections.Generic.Comparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.Comparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Comparer<UnityEngine.UIVertex>
	// System.Collections.Generic.Comparer<float>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Comparer<ushort>
	// System.Collections.Generic.Dictionary.Enumerator<int,System.ValueTuple<long,int>>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,double>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,System.ValueTuple<long,int>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,double>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,System.ValueTuple<long,int>>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,double>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,System.ValueTuple<long,int>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,double>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,System.ValueTuple<long,int>>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,double>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<int,System.ValueTuple<long,int>>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,double>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<System.Nullable<TileSlotData>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<long,int>>
	// System.Collections.Generic.EqualityComparer<double>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<int>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<long,int>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,double>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<int,int>>
	// System.Collections.Generic.ICollection<TileGamePage.FloorRootItem>
	// System.Collections.Generic.ICollection<TileSlotData>
	// System.Collections.Generic.ICollection<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ICollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ICollection<UnityEngine.UIVertex>
	// System.Collections.Generic.ICollection<float>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.IComparer<System.ValueTuple<int,int>>
	// System.Collections.Generic.IComparer<TileGamePage.FloorRootItem>
	// System.Collections.Generic.IComparer<TileSlotData>
	// System.Collections.Generic.IComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IComparer<UnityEngine.UIVertex>
	// System.Collections.Generic.IComparer<float>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IComparer<ushort>
	// System.Collections.Generic.IDictionary<object,int>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<long,int>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,double>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<int,int>>
	// System.Collections.Generic.IEnumerable<TileGamePage.FloorRootItem>
	// System.Collections.Generic.IEnumerable<TileSlotData>
	// System.Collections.Generic.IEnumerable<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IEnumerable<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IEnumerable<UnityEngine.UIVertex>
	// System.Collections.Generic.IEnumerable<float>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<long,int>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,double>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<int,int>>
	// System.Collections.Generic.IEnumerator<TileGamePage.FloorRootItem>
	// System.Collections.Generic.IEnumerator<TileSlotData>
	// System.Collections.Generic.IEnumerator<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IEnumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IEnumerator<UnityEngine.UIVertex>
	// System.Collections.Generic.IEnumerator<float>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IList<System.ValueTuple<int,int>>
	// System.Collections.Generic.IList<TileGamePage.FloorRootItem>
	// System.Collections.Generic.IList<TileSlotData>
	// System.Collections.Generic.IList<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IList<UnityEngine.UIVertex>
	// System.Collections.Generic.IList<float>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyCollection<object>
	// System.Collections.Generic.IReadOnlyList<object>
	// System.Collections.Generic.KeyValuePair<int,System.ValueTuple<long,int>>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,double>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<ushort,int>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<int,int>>
	// System.Collections.Generic.List.Enumerator<TileGamePage.FloorRootItem>
	// System.Collections.Generic.List.Enumerator<TileSlotData>
	// System.Collections.Generic.List.Enumerator<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.List.Enumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.List.Enumerator<UnityEngine.UIVertex>
	// System.Collections.Generic.List.Enumerator<float>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.List<System.ValueTuple<int,int>>
	// System.Collections.Generic.List<TileGamePage.FloorRootItem>
	// System.Collections.Generic.List<TileSlotData>
	// System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.List<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.List<UnityEngine.UIVertex>
	// System.Collections.Generic.List<float>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.ObjectComparer<System.Nullable<TileSlotData>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<int,int>>
	// System.Collections.Generic.ObjectComparer<TileGamePage.FloorRootItem>
	// System.Collections.Generic.ObjectComparer<TileSlotData>
	// System.Collections.Generic.ObjectComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ObjectComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ObjectComparer<UnityEngine.UIVertex>
	// System.Collections.Generic.ObjectComparer<float>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectComparer<ushort>
	// System.Collections.Generic.ObjectEqualityComparer<System.Nullable<TileSlotData>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<long,int>>
	// System.Collections.Generic.ObjectEqualityComparer<double>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<System.ValueTuple<object,object>>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<ushort,int>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<ushort,int>
	// System.Collections.Generic.SortedDictionary.Enumerator<ushort,int>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<ushort,int>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<ushort,int>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<ushort,int>
	// System.Collections.Generic.SortedDictionary.KeyCollection<ushort,int>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<ushort,int>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<ushort,int>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<ushort,int>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<ushort,int>
	// System.Collections.Generic.SortedDictionary.ValueCollection<ushort,int>
	// System.Collections.Generic.SortedDictionary<ushort,int>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.Stack.Enumerator<TileSlotData>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<TileSlotData>
	// System.Collections.Generic.Stack<object>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<ushort,int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<int,int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<TileGamePage.FloorRootItem>
	// System.Collections.ObjectModel.ReadOnlyCollection<TileSlotData>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.UIVertex>
	// System.Collections.ObjectModel.ReadOnlyCollection<float>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Comparison<System.ValueTuple<int,int>>
	// System.Comparison<TileGamePage.FloorRootItem>
	// System.Comparison<TileSlotData>
	// System.Comparison<UnityEngine.EventSystems.RaycastResult>
	// System.Comparison<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Comparison<UnityEngine.UIVertex>
	// System.Comparison<float>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.EventHandler<object>
	// System.Func<System.Collections.Generic.KeyValuePair<int,int>,byte>
	// System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Func<System.ValueTuple<float,float>>
	// System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// System.Func<byte>
	// System.Func<int,byte>
	// System.Func<int,int,object>
	// System.Func<int,object,object>
	// System.Func<int,object>
	// System.Func<int>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.Linq.Buffer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.Enumerable.<RepeatIterator>d__117<int>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.Enumerable.Iterator<int>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<int>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,int>
	// System.Linq.Enumerable.WhereSelectListIterator<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.Enumerable.WhereSelectListIterator<object,int>
	// System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Nullable<TileSlotData>
	// System.Nullable<float>
	// System.Predicate<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Predicate<System.ValueTuple<int,int>>
	// System.Predicate<TileGamePage.FloorRootItem>
	// System.Predicate<TileSlotData>
	// System.Predicate<UnityEngine.EventSystems.RaycastResult>
	// System.Predicate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Predicate<UnityEngine.UIVertex>
	// System.Predicate<float>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.ReadOnlySpan<ushort>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<int>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Span<ushort>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<int>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<int>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskCompletionSource<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<int>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<int>
	// System.Threading.Tasks.TaskFactory<object>
	// System.ValueTuple<float,float>
	// System.ValueTuple<int,System.Nullable<TileSlotData>>
	// System.ValueTuple<int,int>
	// System.ValueTuple<long,int>
	// System.ValueTuple<object,object>
	// Unity.Usercentrics.Singleton<object>
	// UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass79_0<object>
	// UnityEngine.EventSystems.ExecuteEvents.EventFunction<object>
	// UnityEngine.Events.InvokableCall<UnityEngine.Vector2>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.UnityAction<UnityEngine.Vector2>
	// UnityEngine.Events.UnityAction<byte,object>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<UnityEngine.Vector2>
	// UnityEngine.Events.UnityEvent<float>
	// UnityEngine.Pool.CollectionPool.<>c<object,object>
	// UnityEngine.Pool.CollectionPool<object,object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>
	// UnityEngine.ResourceManagement.ChainOperationTypelessDepedency<object>
	// UnityEngine.ResourceManagement.ResourceManager.CompletedOperation<object>
	// UnityEngine.ResourceManagement.Util.GlobalLinkedListNodeCache<object>
	// UnityEngine.ResourceManagement.Util.LinkedListNodeCache<object>
	// }}

	public void RefMethods()
	{
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> AssetManager.GetOrCreateHandle<object>(string)
		// object AssetManager.LoadAsset<object>(string)
		// System.Void AssetManager.ReleaseAsset<object>(string)
		// object DG.Tweening.TweenExtensions.Play<object>(object)
		// object DG.Tweening.TweenSettingsExtensions.OnComplete<object>(object,DG.Tweening.TweenCallback)
		// object DG.Tweening.TweenSettingsExtensions.SetDelay<object>(object,float)
		// object DG.Tweening.TweenSettingsExtensions.SetEase<object>(object,DG.Tweening.Ease)
		// object DG.Tweening.TweenSettingsExtensions.SetLoops<object>(object,int,DG.Tweening.LoopType)
		// int DataTool.AddValue<object>(System.Collections.Generic.IDictionary<object,int>,object,int)
		// string DataTool.DataToString<object>(object,Newtonsoft.Json.Formatting)
		// object DataTool.Deserialize<object>(string)
		// int DataTool.GetValue<object,int>(System.Collections.Generic.IDictionary<object,int>,object)
		// System.Void DataTool.Serialize<object>(string,object,Newtonsoft.Json.Formatting)
		// System.Void DataTool.SetValue<object,int>(System.Collections.Generic.IDictionary<object,int>,object,int)
		// object DataTool.StringToData<object>(string)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// System.Void System.Array.Sort<int>(int[],System.Comparison<int>)
		// bool System.Enum.TryParse<int>(string,bool,int&)
		// bool System.Enum.TryParse<int>(string,int&)
		// bool System.Linq.Enumerable.All<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// object System.Linq.Enumerable.Last<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<int,int>> System.Linq.Enumerable.OrderByDescending<System.Collections.Generic.KeyValuePair<int,int>,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Repeat<int>(int,int)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.RepeatIterator<int>(int,int)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<System.Collections.Generic.KeyValuePair<int,int>,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.List<int> System.Linq.Enumerable.ToList<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<int,int>>.Select<int>(System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<object>.Select<int>(System.Func<object,int>)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,InitializeUnityServices.<Start>d__1>(System.Runtime.CompilerServices.TaskAwaiter&,InitializeUnityServices.<Start>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<InitializeUnityServices.<Start>d__1>(InitializeUnityServices.<Start>d__1&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>(object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetAsync<object>(object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.TrackHandle<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>(bool)
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object[] UnityEngine.Component.GetComponentsInParent<object>()
		// object[] UnityEngine.Component.GetComponentsInParent<object>(bool)
		// bool UnityEngine.EventSystems.ExecuteEvents.Execute<object>(UnityEngine.GameObject,UnityEngine.EventSystems.BaseEventData,UnityEngine.EventSystems.ExecuteEvents.EventFunction<object>)
		// System.Void UnityEngine.EventSystems.ExecuteEvents.GetEventList<object>(UnityEngine.GameObject,System.Collections.Generic.IList<UnityEngine.EventSystems.IEventSystemHandler>)
		// bool UnityEngine.EventSystems.ExecuteEvents.ShouldSendToComponent<object>(UnityEngine.Component)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// System.Void UnityEngine.GameObject.GetComponents<object>(System.Collections.Generic.List<object>)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// object[] UnityEngine.GameObject.GetComponentsInParent<object>(bool)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Purchasing.IExtensionProvider.GetExtension<object>()
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Convert<object>()
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateChainOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationInternal<object>(object,bool,System.Exception,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationWithException<object>(object,System.Exception)
		// object UnityEngine.ResourceManagement.ResourceManager.CreateOperation<object>(System.Type,int,UnityEngine.ResourceManagement.Util.IOperationCacheKey,System.Action<UnityEngine.ResourceManagement.AsyncOperations.IAsyncOperation>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.ProvideResource<object>(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.StartOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle)
	}
}