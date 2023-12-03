﻿
namespace Maasgroep.Database.Receipts
{
	public record ReceiptActiveRecord
	{
		// Generic
		public long MemberCreatedId { get; set; }
		public long? MemberModifiedId { get; set; }
		public long? MemberDeletedId { get; set; }
		public DateTime DateTimeCreated { get; set; }
		public DateTime? DateTimeModified { get; set; }
		public DateTime? DateTimeDeleted { get; set; }

		// EF generic properties
		public Member MemberCreated { get; set; }
		public Member? MemberModified { get; set; }
		public Member? MemberDeleted { get; set; }
	}
}
