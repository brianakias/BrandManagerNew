
using System.Collections.Generic;

namespace BrandManagerNew
{
    public interface IDomain
    {
        int CreateRecord(string brandName, bool flag);

        List<Brand> ReadRecord(string id);
        int UpdateRecord(string id, string brandName, bool flag);

        int DeleteRecord(string id);

        void ConfirmOneRecordWasAffected(int recordsAffected);
    }
}
