
using System.Collections.Generic;

namespace BrandManagerNew
{
    public interface IDomain
    {
        int CreateRecord(string brandName, bool flag);
        int UpdateRecord(int id, string brandName, bool flag);
        List<Brand> ReadRecord(int id);
        int DeleteRecord(int id);
        void ConfirmOneRecordWasAffected(int recordsAffected);
    }
}
