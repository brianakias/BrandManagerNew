
using System.Collections.Generic;

namespace BrandManagerNew
{
    public class Domain : IDomain
    {
        private UserInputValidation validation { get; set; }
        public BrandRepository brandRepo { get; set; }

        public Domain()
        {
            validation = new UserInputValidation();
            brandRepo = new BrandRepository();

        }

        public Brand PrepareObjectForInsertion(string brandName, bool flag)
        {
            validation.CheckIfBrandNameHasInvalidCharacters(brandName);
            List<string> brandNames = brandRepo.ReadBrandNames();
            validation.CheckIfBrandNameAlreadyExists(brandName, brandNames);
            return new Brand(brandName, flag);
        }

        public Brand PrepareObjectForUpdating(int id, string brandName, bool flag)
        {
            List<int> ids = brandRepo.ReadIDs();
            validation.CheckIfIDExists(id, ids);
            validation.CheckIfBrandNameHasInvalidCharacters(brandName);
            List<string> brandNames = brandRepo.ReadBrandNames();
            validation.CheckIfBrandNameAlreadyExists(brandName, brandNames);
            Brand brand = new Brand(brandName, flag);
            brand.Id = id;
            return brand;
        }

        public int PrepareObjectForDeletion(int id)
        {
            List<int> ids = brandRepo.ReadIDs();
            validation.CheckIfIDExists(id, ids);
            return id;
        }
    }
}
