using System.Collections.Generic;
using System.Data;

namespace Shopping.Utilities
{
    public class RepositoryHelper
    {
        public RepositoryHelper()
        {

        }

        public static List<T> ConvertToList<T>(DataTable dataTable)
            where T:IEntity, new()
        {
            List<T> TList = new List<T>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                T item = new T();
                item.SetFeilds(dataRow);
                TList.Add(item);
            }

            return TList;
        }
    }
}