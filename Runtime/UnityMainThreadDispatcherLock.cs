using System.Collections;

public sealed class UnityMainThreadDispatcherLock : BaseUnityMainThreadDispatcher<UnityMainThreadDispatcherLock> {
	protected override void Update() {
		lock ( _executionQueue ) {
			ActionInvoke();
		}
	}
	
	public override void Enqueue(IEnumerator action) {
		lock ( _executionQueue ) {
			EnqueuePrivate(action);
		}
	}
}
