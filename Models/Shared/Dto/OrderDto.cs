﻿namespace Shared.Dto
{
    public class OrderDto
    {
        public string PhoneNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string? Comment { get; set; }

        public AddressDto Address { get; set; }

        public int[] PizzaIds { get; set; }
    }
}