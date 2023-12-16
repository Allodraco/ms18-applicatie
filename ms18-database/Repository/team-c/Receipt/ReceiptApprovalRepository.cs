using Maasgroep.Database.Interfaces;
using Maasgroep.SharedKernel.ViewModels.Receipts;
using Maasgroep.SharedKernel.DataModels.Receipts;

namespace Maasgroep.Database.Receipts
{

    public class ReceiptApprovalRepository : WritableRepository<ReceiptApproval, ReceiptApprovalModel, ReceiptApprovalData>, IReceiptApprovalRepository
    {
		public ReceiptApprovalRepository(MaasgroepContext db) : base(db) {}

        /** Create ReceiptApprovalModel from ReceiptApproval record */
        public override ReceiptApprovalModel GetModel(ReceiptApproval approval)
        {
            var member = Db.Member.FirstOrDefault(c => c.Id == approval.MemberCreatedId);

            return new ReceiptApprovalModel() {
                Id = approval.Id,
				Approved = approval.Approved,
				Note = approval.Note,
				ReceiptId = approval.ReceiptId,
                MemberCreated = member == null ? null : new() {
					Id = member.Id,
					Name = member.Name,
				},
			};
        }

        /** Create or update ReceiptApproval record from data model */
        public override ReceiptApproval? GetRecord(ReceiptApprovalData data, ReceiptApproval? existingApproval = null)
        {
            if (existingApproval != null)
                return null; // Approval records are not editable

            var approval = new ReceiptApproval() {
                Approved = data.Approved,
				Note = data.Note,
				ReceiptId = data.ReceiptId,
            };
			return approval;
        }
    }
}