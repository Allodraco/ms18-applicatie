﻿using Ms18.Database.Models.TeamC.Admin;

namespace Ms18.Database.Models.TeamC.Stock
{
    public record Stockpile
    {
        public long ProductId { get; set; }
        public long Quantity { get; set; }

        // Generic
        public long MemberCreatedId { get; set; }
        public long? MemberModifiedId { get; set; }
        public long? MemberDeletedId { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime? DateTimeModified { get; set; }
        public DateTime? DateTimeDeleted { get; set; }

        // EF
        public Product Product { get; set; }

        // EF generic properties
        public Member MemberCreated { get; set; }
        public Member? MemberModified { get; set; }
        public Member? MemberDeleted { get; set; }
    }
}