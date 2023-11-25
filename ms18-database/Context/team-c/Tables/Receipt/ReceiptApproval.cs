﻿using Maasgroep.Database.Context;

namespace Maasgroep.Database.Receipts
{
    public record ReceiptApproval : GenericRecord
	{
		public long ReceiptId { get; set; }
		public string? Note { get; set; }

        // EF receipt properties
        public Receipt Receipt { get; set; }
    }
}
