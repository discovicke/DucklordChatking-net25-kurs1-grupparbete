namespace ChatServer.Store;

public abstract class ConcurrentStoreBase
{
  // Lock that coordinates safe access to shared state.
  // Readers can proceed together while writers get exclusive access.
  protected readonly ReaderWriterLockSlim locker =
      new(LockRecursionPolicy.NoRecursion);

  // Runs a read-only operation while protecting the shared collections
  // from concurrent writes. Many reads can run at the same time.
  protected T WithRead<T>(Func<T> action)
  {
    locker.EnterReadLock();
    try
    {
      return action();
    }
    finally
    {
      locker.ExitReadLock();
    }
  }

  // Runs a write operation that modifies shared state.
  // Only one writer is allowed at a time and readers are paused.
  protected T WithWrite<T>(Func<T> action)
  {
    locker.EnterWriteLock();
    try
    {
      return action();
    }
    finally
    {
      locker.ExitWriteLock();
    }
  }

  // Overload for write operations that do not return a value.
  protected void WithWrite(Action action)
  {
    locker.EnterWriteLock();
    try
    {
      action();
    }
    finally
    {
      locker.ExitWriteLock();
    }
  }
}
