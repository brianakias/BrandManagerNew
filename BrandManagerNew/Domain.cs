
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
            _brandRepo.CreateTableIfNotExists("brands");
        }

        public int CreateRecord(string brandName, bool flag)
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
            Brand brand = new Brand(brandName, flag);
            int rowsAffected = _brandRepo.CreateRecord(brand);
            return rowsAffected;
        }

        public List<Brand> ReadRecord(string id)
        {
            int convertedID = _validation.CheckIfIDIsInCorrectFormat(id);
            List<int> ids = _brandRepo.ReadIDs();
            _validation.CheckIfIDExists(convertedID, ids);
            List<Brand> record = _brandRepo.ReadRecord(convertedID);
            return record;
        }

        public int UpdateRecord(string id, string brandName, bool flag)
        {
            int convertedID = _validation.CheckIfIDIsInCorrectFormat(id);
            List<int> ids = _brandRepo.ReadIDs();
            _validation.CheckIfIDExists(convertedID, ids);
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
            Brand brand = new Brand(convertedID, brandName, flag);
            int rowsAffected = _brandRepo.UpdateRecord(brand);
            return rowsAffected;
        }

        public int DeleteRecord(string id)
        {
            int convertedID = _validation.CheckIfIDIsInCorrectFormat(id);
            List<int> ids = _brandRepo.ReadIDs();
            _validation.CheckIfIDExists(convertedID, ids);
            int recordsAffected = _brandRepo.DeleteRecord(convertedID);
            return recordsAffected;
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
