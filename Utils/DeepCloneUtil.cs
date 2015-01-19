using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Utils
{
	public static class DeepCloneUtil
	{
		public static T DeepClone<T> (T obj)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (memoryStream, obj);
				memoryStream.Position = 0;

				return (T)formatter.Deserialize (memoryStream);
			}
		}
	}
}

