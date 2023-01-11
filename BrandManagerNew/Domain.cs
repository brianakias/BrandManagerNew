
using Moq;
using System.Collections.Generic;

namespace BrandManagerNew
{
    public class Domain : IDomain
    {
        private UserInputValidation Validation { get; set; }
        public BrandRepository BrandRepo { get; set; }
        public Domain(UserInputValidation validation, BrandRepository brandRepo)
        {
            Validation = validation;
            BrandRepo = brandRepo;

        }


        public Brand PrepareObjectForInsertion(string brandName, bool flag)
        {
            Validation.CheckIfBrandNameHasInvalidCharacters(brandName);
            List<string> brandNames = BrandRepo.ReadBrandNames();
            Validation.CheckIfBrandNameAlreadyExists(brandName, brandNames);
            return new Brand(brandName, flag);
        }

        public Brand PrepareObjectForUpdating(int id, string brandName, bool flag)
        {
            List<int> ids = BrandRepo.ReadIDs();
            Validation.CheckIfIDExists(id, ids);
            Validation.CheckIfBrandNameHasInvalidCharacters(brandName);
            List<string> brandNames = BrandRepo.ReadBrandNames();
            Validation.CheckIfBrandNameAlreadyExists(brandName, brandNames);
            Brand brand = new Brand(brandName, flag);
            brand.Id = id;
            return brand;
        }

        public int PrepareObjectForDeletion(int id)
        {
            List<int> ids = BrandRepo.ReadIDs();
            Validation.CheckIfIDExists(id, ids);
            return id;
        }

        public void ConfirmOneRecordWasAffected(int recordsAffected)
        {
            if (recordsAffected != 1)
            {
                throw new UnexpectedRecordsAffectedException($"Wrong number of records affected. Expected: 1 record, actual: {recordsAffected} records instead");
            }
        }
    }
}
