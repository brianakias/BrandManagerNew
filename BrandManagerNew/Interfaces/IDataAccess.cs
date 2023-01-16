
using System.Collections.Generic;


namespace BrandManagerNew
{
    public interface IDataAccess
    {
        int CreateRecord(Brand brand);

        List<Brand> ReadRecord(int id);

        List<string> ReadBrandNames();

        List<int> ReadIDs();

        int UpdateRecord(Brand brand);

        int DeleteRecord(int id);

        void CreateTableIfNotExists(string tableName);
    }
}
