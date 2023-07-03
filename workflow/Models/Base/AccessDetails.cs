using System;
using System.Collections.Generic;
using System.Linq;

namespace Syncfusion.EJ2.FileManager.Base
{
	public class AccessDetails
	{
		public string Role { get; set; }
		public IEnumerable<AccessRule> AccessRules { get; set; }
	}

	public class AccessRule
	{
		public string Path { get; set; }
		public Permission Copy { get; set; }
		public Permission Download { get; set; }
		/// <summary>
		/// Rename,Delete,Cut
		/// </summary>
		public Permission Write { get; set; } 
		public string Id { get; set; }
		public Permission Read { get; set; }
		public string Role { get; set; }
		/// <summary>
		/// Newfolder 
		/// </summary>
		public Permission WriteContents { get; set; }
		public Permission Upload { get; set; }
		public Permission Permission { get; set; }
		public bool IsFile { get; set; }
		public string Message { get; set; }
	}
	public enum Permission
	{
		Allow,
		Deny
	}
}