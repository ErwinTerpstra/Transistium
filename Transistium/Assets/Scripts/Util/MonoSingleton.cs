using UnityEngine;

namespace Transistium.Util
{
	public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T instance;

		protected virtual void Awake()
		{
			if (instance != null)
			{
				Debug.LogWarningFormat(this, "Warning! Multiple instances of MonoSingleton<{0}>", typeof(T));
				Destroy(this);
				return;
			}

			instance = this as T;
		}

		public static T Instance
		{
			get { return instance; }
		}
	}

}