﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Maasgroep.Database
{
	public record Member
	{
		public long Id { get; set; }
		[Column(TypeName = "varchar(256)")]
		public string Name { get; set; }


        //Generic
        public long? UserCreatedId { get; set; }
        public long? UserModifiedId { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public DateTime? DateTimeModified { get; set; }


        //Ef receipt properties
        public ICollection<CostCentre> CostCentresCreated { get; set; }
        public ICollection<CostCentre> CostCentresModified { get; set; }
        public ICollection<Store> StoresCreated { get; set; }
        public ICollection<Store> StoresModified { get; set; }
        public ICollection<ReceiptStatus> ReceiptStatusesCreated { get; set; }
        public ICollection<ReceiptStatus> ReceiptStatusesModified { get; set; }
        public ICollection<Receipt> ReceiptsCreated { get; set; }
        public ICollection<Receipt> ReceiptsModified { get; set; }
        public ICollection<ReceiptApproval> ReceiptApprovalsCreated { get; set; }
        public ICollection<ReceiptApproval> ReceiptApprovalsModified { get; set; }
        public ICollection<Permission> PermissionsCreated { get; set; }
        public ICollection<Permission> PermissionsModified { get; set; }
        public ICollection<MemberPermission> MemberPermissionsCreated { get; set; }
        public ICollection<MemberPermission> MemberPermissionsModified { get; set; }

        //Ef admin properties
        public ICollection<MemberPermission> MemberPermissions { get; set; }
        //public ICollection<Permission> Permissions { get; set; }

        //Ef generic properties
        public Member? UserCreated { get; set; }
        public Member? UserModified { get; set; }
        public ICollection<Member>? MembersCreated { get; set; }
        public ICollection<Member>? MembersModified { get; set; } 

    }
}
