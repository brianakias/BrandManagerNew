
using BrandManagerNew.Interfaces;
using System;
using System.Collections.Generic;

namespace BrandManagerNew
{
    public class Domain : IDomain
    {
        private readonly IDataAccess _brandRepo;
        private readonly IUserInputValidation _validation;

        public Domain(IUserInputValidation validation, IDataAccess brandRepo)
        {
            _validation = validation;
            _brandRepo = brandRepo;
        }

        public Brand PrepareObjectForInsertion(string brandName, bool flag)
        {
            if (brandName == null)
            {
                throw new ArgumentNullException();
            }

            _validation.CheckIfBrandNameIsEmpty(brandName);
            if (!string.IsNullOrEmpty(brandName))
            {
                _validation.CheckIfBrandNameHasInvalidCharacters(brandName);
                List<string> brandNames = _brandRepo.ReadBrandNames();
                _validation.CheckIfBrandNameAlreadyExists(brandName, brandNames);
            }
            return new Brand(brandName, flag);
        }

        public Brand PrepareObjectForUpdating(int id, string brandName, bool flag)
        {
            List<int> ids = _brandRepo.ReadIDs();
            _validation.CheckIfIDExists(id, ids);
            _validation.CheckIfBrandNameHasInvalidCharacters(brandName);
            List<string> brandNames = _brandRepo.ReadBrandNames();
            _validation.CheckIfBrandNameAlreadyExists(brandName, brandNames);
            _validation.CheckIfBrandNameIsEmpty(brandName);
            Brand brand = new Brand(id, brandName, flag);
            return brand;
        }

        public int PrepareObjectForReadingOrDeletion(int id)
        {
            List<int> ids = _brandRepo.ReadIDs();
            _validation.CheckIfIDExists(id, ids);
            return id;
        }

        public void ConfirmOneRecordWasAffected(int recordsAffected)
        {
            if (recordsAffected != 1)
            {
                throw new UnexpectedRecordsAffectedException($"Wrong number of records affected. Expected: 1 record, actual: {recordsAffected}");
            }
        }

    }
}
