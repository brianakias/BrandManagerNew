
using System.Collections.Generic;
using System.Linq;

namespace BrandManagerNew
{
    public class UserInputValidation
    {
        public void CheckIfBrandNameHasInvalidCharacters(string brandName)
        {
            bool hasInvalidCharacters = !brandName.All(c => char.IsLetter(c) || c == '-' || char.IsWhiteSpace(c));

            if (hasInvalidCharacters)
            {
                throw new InvalidBrandNameException("The brand name should be consisted of letters, whitespaces and dashes ( - ) only");
            }
        }

        public void CheckIfBrandNameAlreadyExists(string brandName, List<string> brandNames)
        {
            brandNames.ForEach(name =>
            {
                if (name.ToLower() == brandName.ToLower())
                {
                    throw new NameAlreadyExistsException($"A brand with name '{brandName}' already exists.");
                }
            });
        }

        public void CheckIfIDExists(int id, List<int> ids)
        {
            if (!ids.Contains(id))
            {
                string ids_string = string.Join(", ", ids);
                throw new NonExistentIDException($"Could not find id '{id}' in the database. Consider passing an id from the list: {ids_string}");
            }
        }
    }
}
