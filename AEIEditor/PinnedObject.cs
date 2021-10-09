using System;
using System.Runtime.InteropServices;

namespace AEIEditor
{
	/// <summary>
	/// Pins an object and gets an unsafe pointer for it.
	/// </summary>
	public sealed class PinnedObject : IDisposable
	{
        private GCHandle _pinnedObject;

		public PinnedObject(object obj)
		{
			_pinnedObject = GCHandle.Alloc(obj, GCHandleType.Pinned);
		}

		public unsafe void* GetPointer()
		{
			return _pinnedObject.AddrOfPinnedObject().ToPointer();
		}

		public unsafe void* GetPointer(int dataOffset)
		{
			return (_pinnedObject.AddrOfPinnedObject() + dataOffset).ToPointer();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

        private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			_pinnedObject.Free();
		}
	}
}
