
using System.Collections.Generic;


namespace BrandManagerNew
{
    public interface IDataAccess
    {
        void CreateRecord(Brand brand);
        List<Brand> ReadRecords();
        List<string> ReadBrandNames();
        List<int> ReadIDs();
        void UpdateRecord(Brand brand);
        void DeleteRecord(int id);
        void CreateTableIfNotExists(string tableName);

    }
}
