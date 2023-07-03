

namespace Syncfusion.EJ2.FileManager.Base
{
	public interface SQLFileProviderBase : FileProviderBase
	{
		void SetSQLConnection(string tableName, string tableID);
		void SetRules(AccessDetails details);

	}

}
