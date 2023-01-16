
namespace BrandManagerNew
{
    public interface IDomain
    {
        Brand PrepareObjectForInsertion(string brandName, bool flag);
        Brand PrepareObjectForUpdating(int id, string brandName, bool flag);
        int PrepareObjectForReadingOrDeletion(int id);
        void ConfirmOneRecordWasAffected(int recordsAffected);
    }
}
