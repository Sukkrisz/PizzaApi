using System;
namespace Data.Db.Models.Order
{
    public struct AddressModel
    {
        public string City { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }

        public AddressModel(string city, string line1, string? line2)
        {
            City = city;
            Line1 = line1;
            Line2 = line2;
        }
    }
}
