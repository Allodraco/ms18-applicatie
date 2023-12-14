using Maasgroep.Database.Interfaces;
using Maasgroep.SharedKernel.ViewModels.Receipts;

namespace Maasgroep.Database.Receipts
{

    public class ReceiptPhotoRepository : DeletableRepository<ReceiptPhoto, ReceiptPhotoModel>, IReceiptPhotoRepository
    {
		public ReceiptPhotoRepository(MaasgroepContext db) : base(db) {}

        /** Create ReceiptPhotoModel from ReceiptPhoto record */
        public override ReceiptPhotoModel GetModel(ReceiptPhoto photo)
        {
            return new ReceiptPhotoModel() {
				FileExtension = photo.FileExtension,
				FileName = photo.FileName,
                ReceiptId = photo.ReceiptId,
				Base64Image = photo.Base64Image,
			};
        }

        /** Create or update ReceiptPhoto record from model */
        public override ReceiptPhoto? GetRecord(ReceiptPhotoModel model, ReceiptPhoto? existingPhoto = null)
        {
            if (existingPhoto != null)
                return null; // Photo records are not editable

            var photo = new ReceiptPhoto() {
                Base64Image = model.Base64Image,
				FileExtension = model.FileExtension,
				FileName = model.FileName,
				ReceiptId = model.ReceiptId,
            };
			return photo;
        }

        /** List photos by receipt */
		public IEnumerable<ReceiptPhotoModel> ListByReceipt(long receiptId, int offset = default, int limit = default, bool includeDeleted = default)
			=> GetList(item => item.ReceiptId == receiptId, null, offset, limit, includeDeleted).Select(item => GetModel(item)!);
    }
}
