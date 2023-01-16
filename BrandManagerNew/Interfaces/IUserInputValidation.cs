using System.Collections.Generic;

namespace BrandManagerNew.Interfaces
{
    public interface IUserInputValidation
    {
        void CheckIfBrandNameHasInvalidCharacters(string brandName);

        void CheckIfBrandNameIsEmpty(string brandName);

        void CheckIfBrandNameAlreadyExists(string brandName, List<string> brandNames);

        void CheckIfIDExists(int id, List<int> ids);
    }
}
