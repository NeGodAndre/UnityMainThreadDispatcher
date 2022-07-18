# UnityMainThreadDispatcher

A thread-safe way of dispatching IEnumerator functions to the main thread in unity. Useful for calling UI functions and other actions that Unity limits to the main thread from different threads. Initially written for Firebase Unity but now used across the board!

## Cradits

This fork is based on version from [PimDeWitte](https://github.com/PimDeWitte/UnityMainThreadDispatcher) and adds changes from [nvandamme](https://github.com/PimDeWitte/UnityMainThreadDispatcher/pull/26) and [ssootube](https://github.com/PimDeWitte/UnityMainThreadDispatcher/pull/24)

## Installation

You have a few different options to install into your Unity project:

1. [Unity Package Manger](#unity-package-manager)

   Probably the easiest way. Just add the git URL and let the package manager install it for you.
2. [Manual Installation](#manual-installation)

   Edit the project manifest file by hand.
3. [Install from a File](#install-from-a-file)

   Download a tarball and install in folder.

### Unity Package Manager

Open the Package Manager (UPM) in Unity ``Windows -> Package Manager``.

Select ``+`` in the top-left of the UPM panel and select ``Add package from Git URL...``

Enter ``https://github.com/NeGodAndre/UnityMainThreadDispatcher.git`` in the text box and click add.

More info: [Unity Manual: Installing from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

| Syntax:                | URL example                                                                                            |
|------------------------|--------------------------------------------------------------------------------------------------------|
| Latest default branch  | "https://github.com/NeGodAndre/UnityMainThreadDispatcher.git"                                          |
| Specific branch        | "https://github.com/NeGodAndre/UnityMainThreadDispatcher.git/#branch-name"                             |
| Specific version (tag) | "https://github.com/NeGodAndre/UnityMainThreadDispatcher.git#v.1.0.0"                                  |
| Commit hash            | "https://github.com/NeGodAndre/UnityMainThreadDispatcher.git#4f712b3070ed21ba239d472158885efa0d3e26b3" |

### Manual Installation

Open ``Packages/manifest.json`` with your favorite text editor. Add the following line to the dependencies block.

```json
{
  "dependencies": {
    "unity.main.thread.dispatcher": "https://github.com/NeGodAndre/UnityMainThreadDispatcher.git"
  },
}
```

**Notice:** Unity Package Manager records the current commit to a lock entry of the manifest.json. To update to the latest version, change the ``"hash"`` value manually or just remove the lock entry to resolve the package in ``Packages/packages-lock.json``.

```json
{
  "version": "https://github.com/NeGodAndre/UnityMainThreadDispatcher.git",
  "depth": 0,
  "source": "git",
  "dependencies": {},
  "hash": "4f712b3070ed21ba239d472158885efa0d3e26b3"
}
```

More info: [Unity Manual about](https://docs.unity3d.com/Manual/upm-localpath.html)

### Install from a File

1. Download and extract a [release](https://github.com/NeGodAndre/UnityMainThreadDispatcher/releases) or a [tag](https://github.com/NeGodAndre/UnityMainThreadDispatcher/tags) to your machine. 
2. Import it into the following directory in your Unity project:
    - For ``Unity 2018.1 or later`` : ``Packages``
    - For ``Unity 2017.x or later`` : ``Assets``

## Usage

Repository have two version scrits:

 - UnityMainThreadDispatcherSemaphore use [SemaphoreSlim](https://docs.microsoft.com/dotnet/api/system.threading.semaphoreslim) for wait.

 - UnityMainThreadDispatcherLock use [lock](https://docs.microsoft.com/dotnet/csharp/language-reference/statements/lock) for wait.

> Although scripts create of an object on the first call. Recommend to create an object with the selected script in advance.

Example for UnityMainThreadDispatcherSemaphore (similar to UnityMainThreadDispatcherLock)
```C#
	public IEnumerator ThisWillBeExecutedOnTheMainThread() {
		Debug.Log ("This is executed from the main thread");
		yield return null;
	}
	public void ExampleMainThreadCall() {
		UnityMainThreadDispatcherSemaphore.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread()); 
	}
```
OR
```C#
	UnityMainThreadDispatcherSemaphore.Instance().Enqueue(() => Debug.Log ("This is executed from the main thread"));
```

## Version
1.1.0 - Added alternative script using SemaphoreSlim. Added functional for autocreate object. Unity package is created.

1.0. - Tested and functional in one or more production applications, including those from major companies. 