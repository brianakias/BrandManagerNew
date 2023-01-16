
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

        public List<Brand> ReadRecord(int id)
        {
            List<int> ids = _brandRepo.ReadIDs();
            _validation.CheckIfIDExists(id, ids);
            List<Brand> record = _brandRepo.ReadRecord(id);
            return record;
        }

        public int UpdateRecord(int id, string brandName, bool flag)
        {
            List<int> ids = _brandRepo.ReadIDs();
            _validation.CheckIfIDExists(id, ids);
            _validation.CheckIfBrandNameHasInvalidCharacters(brandName);
            List<string> brandNames = _brandRepo.ReadBrandNames();
            _validation.CheckIfBrandNameAlreadyExists(brandName, brandNames);
            _validation.CheckIfBrandNameIsEmpty(brandName);
            Brand brand = new Brand(id, brandName, flag);
            int rowsAffected = _brandRepo.UpdateRecord(brand);
            return rowsAffected;
        }

        public int DeleteRecord(int id)
        {
            List<int> ids = _brandRepo.ReadIDs();
            _validation.CheckIfIDExists(id, ids);
            int recordsAffected = _brandRepo.DeleteRecord(id);
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
