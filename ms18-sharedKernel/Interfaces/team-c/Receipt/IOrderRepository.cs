﻿using Maasgroep.SharedKernel.ViewModels.Orders;

namespace Maasgroep.SharedKernel.Interfaces.Orders
{
	public interface IOrderRepository
	{
		#region Product
		IEnumerable<ProductModel> GetProducts(int offset, int fetch);
		ProductModel GetProduct(long id);
		long Add(ProductModelCreateDb product);
		bool Modify(ProductModelUpdateDb product);
		bool Delete(ProductModelDeleteDb product);
		#endregion

		#region Stock
		IEnumerable<StockModel> GetStock(int offset, int fetch);
		StockModel GetStock(long id);
		long Add(StockModelCreateDb product);
		bool Modify(StockModelUpdateDb product);
		bool Delete(StockModelDeleteDb product);
		#endregion

	}
}