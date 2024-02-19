namespace ModelLibrary.Shared.Dto
{
    public struct AddressDto
    {
        public Cities City { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }

        public AddressDto(Cities city, string line1, string? line2)
        {
            City = city;
            Line1 = line1;
            Line2 = line2;
        }
    }
}
