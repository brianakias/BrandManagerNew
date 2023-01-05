
namespace BrandManagerNew
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        public Brand(string brandName, bool flag)
        {
            Name = brandName;
            IsEnabled = flag;
        }

    }
}
