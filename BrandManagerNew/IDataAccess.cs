
using System.Collections.Generic;


namespace BrandManagerNew
{
    public interface IDataAccess
    {
        int CreateRecord(Brand brand);
        List<Brand> ReadRecords();
        List<string> ReadBrandNames();
        List<int> ReadIDs();
        int UpdateRecord(Brand brand);
        int DeleteRecord(int id);
        void CreateTableIfNotExists(string tableName);

    }
}
