
using BrandManagerNew.Exceptions;
using BrandManagerNew.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BrandManagerNew
{
    public class UserInputValidation : IUserInputValidation
    {
        public void CheckIfBrandNameHasInvalidCharacters(string brandName)
        {
            bool hasInvalidCharacters = !brandName.All(c => char.IsLetter(c) || c == '-' || char.IsWhiteSpace(c));

            if (hasInvalidCharacters)
            {
                throw new InvalidBrandNameException("The brand name should be consisted of letters, whitespaces and dashes ( - ) only");
            }
        }

        public void CheckIfBrandNameIsEmpty(string brandName)
        {
            if (string.IsNullOrEmpty(brandName))
            {
                throw new EmptyBrandNameException("The brand name should not be empty");
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

        public int CheckIfIDIsInCorrectFormat(string id)
        {
            if (!int.TryParse(id, out int convertedID))
            {
                throw new InvalidIDFormatException("ID must be a number");
            }
            else if (convertedID <= 0)
            {
                throw new InvalidIDFormatException("ID must be greater than 0");
            }
            return convertedID;
        }
    }
}
