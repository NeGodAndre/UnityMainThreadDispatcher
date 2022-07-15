using System.Collections;
using System.Threading;

public sealed class UnityMainThreadDispatcherSemaphore : BaseUnityMainThreadDispatcher<UnityMainThreadDispatcherSemaphore> {
	private static SemaphoreSlim _semaphoreSlimLock = new SemaphoreSlim(1, 1);

	protected override void Update() {
		_semaphoreSlimLock.Wait();
		try {
			ActionInvoke();
		} finally {
			_semaphoreSlimLock.Release();
		}
	}

	public override void Enqueue(IEnumerator action) {
		_semaphoreSlimLock.Wait();
		try {
			EnqueuePrivate(action);
		} finally {
			_semaphoreSlimLock.Release();
		}
	}
}