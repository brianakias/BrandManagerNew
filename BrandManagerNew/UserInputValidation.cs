
using System.Collections.Generic;
using System.Linq;

namespace BrandManagerNew
{
    public class UserInputValidation
    {
        public void CheckIfBrandNameIsValid(string brandName)
        {
            CheckIfBrandNameHasInvalidCharacters(brandName);
            CheckIfBrandNameAlreadyExists(brandName);
        }

        private void CheckIfBrandNameHasInvalidCharacters(string brandName)
        {
            if (!brandName.All(c => char.IsLetter(c) || c == '-' || char.IsWhiteSpace(c)))
            {
                throw new InvalidBrandNameException("The brand name should be consisted of letters, whitespaces and dashes ( - ) only");
            }
        }

        private void CheckIfBrandNameAlreadyExists(string brandName)
        {
            BrandRepository brandRepo = new BrandRepository();
            List<string> brandNames = brandRepo.ReadBrandNames();
            brandNames.ForEach(name =>
            {
                if (name == brandName)
                {
                    throw new NameAlreadyExistsException($"A brand with name '{brandName}' already exists.");
                }
            });
        }

        public void CheckIfIdIsValid(int id)
        {
            CheckIfIDHasInvalidCharacters(id);
            CheckIfIDExists(id);
        }

        private void CheckIfIDHasInvalidCharacters(int id)
        {
            string id_string = id.ToString();
            for (int i = 0; i < id_string.Length; i++)
            {
                char c = id_string[i];

                if (!char.IsDigit(c))
                {
                    throw new InvalidIDFormatException($"Invalid id '{id}' was passed. ID must be consisted of decimal numbers only");
                }
            }
        }

        private void CheckIfIDExists(int id)
        {
            BrandRepository brandRepo = new BrandRepository();
            List<int> ids = brandRepo.ReadIDs();
            if (!ids.Contains(id))
            {
                string ids_string = string.Join(", ", ids);
                throw new NonExistentIDException($"Could not find id '{id}' in the database. Consider passing an id from the list: {ids_string}");
            }
        }
    }
}
