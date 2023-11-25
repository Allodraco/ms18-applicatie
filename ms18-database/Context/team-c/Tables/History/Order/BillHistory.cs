﻿
namespace Maasgroep.Database.Order
{
    public record BillHistory : GenericRecordHistory
    {
        public long BillId { get; set; }
        public long? MemberId { get; set; }
        public bool IsGuest { get; set; }
        public string? Note { get; set; }
        public string? Name { get; set; }
    }
}
