using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// Author: Pim de Witte (pimdewitte.com) and contributors, https://github.com/PimDeWitte/UnityMainThreadDispatcher
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public abstract class BaseUnityMainThreadDispatcher<T> : MonoBehaviour 
		where T : BaseUnityMainThreadDispatcher<T> {
	private static BaseUnityMainThreadDispatcher<T> _instance = null;

	protected static Queue<Action> _executionQueue = new Queue<Action>();

	public static BaseUnityMainThreadDispatcher<T> Instance() {
		if ( !_instance ) {
			Create();
		}
		return _instance;
	}

	private static void Create() {
		var go = new GameObject();
		go.name = "[UnityMainThreadDispatcher]";
		_instance = go.AddComponent<T>();
		DontDestroyOnLoad(go);
	}

	private void Awake() {
		if ( _instance && (_instance != this) ) {
			Destroy(gameObject);
			return;
		}
		_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void OnDestroy() {
		if ( _instance == this ) {
			_instance = null;
		}
	}

	protected abstract void Update();

	/// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
	public abstract void Enqueue(IEnumerator action);

	protected void ActionInvoke() {
		try {
			while ( _executionQueue.Count > 0 ) {
				_executionQueue.Dequeue()?.Invoke();
			}
		} catch ( Exception e ) {
			Debug.LogError("UnityMainThreadDispatcherLock: " + e);
		}
	}

	protected void EnqueuePrivate(IEnumerator action) {
		try {
			_executionQueue.Enqueue(() => { StartCoroutine(action); });
		} catch ( Exception e ) {
			Debug.LogError("UnityMainThreadDispatcherLock: " + e);
		}
	}

	/// <summary>
	/// Locks the queue and adds the Action to the queue
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	public void Enqueue(Action action) {
		Enqueue(ActionWrapper(action));
	}

	/// <summary>
	/// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	/// <returns>A Task that can be awaited until the action completes</returns>
	public Task EnqueueAsync(Action action) {
		var tcs = new TaskCompletionSource<bool>();

		void WrappedAction() {
			try {
				action();
				tcs.TrySetResult(true);
			} catch ( Exception ex ) {
				tcs.TrySetException(ex);
			}
		}

		Enqueue(ActionWrapper(WrappedAction));
		return tcs.Task;
	}

	private IEnumerator ActionWrapper(Action action) {
		action();
		yield return null;
	}
}