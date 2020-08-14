using System;

namespace Nexus.Core
{
	public struct Address
	{
		public string Path { get; }
		private string _parent;
		private readonly int _hashCode;

		public Address(string path)
		{
			Path = path;
			_hashCode = Path != null ? Path.GetHashCode() : 0;

			_parent = null;
		}

		public override int GetHashCode()
		{
			return _hashCode == 0 ? base.GetHashCode() : _hashCode;
		}

		public override string ToString()
		{
			return Path;
		}

		public static bool operator ==(Address obj1, Address obj2)
		{
			return obj1.Path == obj2.Path;
		}

		public static bool operator !=(Address obj1, Address obj2)
		{
			return !(obj1 == obj2);
		}

		public bool IsStartFrom(Address address)
		{
			return Path.StartsWith(address.Path);
		}

		public string GetNext(Address address)
		{
			var result = Path.Replace(address.Path, "");
			return result.Split('/')[1];
		}

		public string GetLast()
		{
			return System.IO.Path.GetFileName(Path);
		}

		public bool IsEmpty()
		{
			return String.IsNullOrEmpty(Path);
		}

		public string GetFirst()
		{
			return Path.Split('/')[0];
		}

		public string Parent => _parent ?? (_parent = System.IO.Path.GetDirectoryName(Path));

		public static implicit operator Address(string path)
		{
			return new Address(path);
		}

		public static implicit operator string(Address address)
		{
			return address.Path;
		}
	}
}