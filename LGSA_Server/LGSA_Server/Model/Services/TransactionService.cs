﻿using LGSA.Model.UnitOfWork;
using LGSA_Server.Model;
using LGSA_Server.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LGSA.Model.Services
{

    public interface ITransactionService : IDataService<transactions>
    {
        Task<bool> AcceptSellTransaction(sell_Offer sellOffer, buy_Offer buyOffer, product buyerProduct, product sellerProduct);
        Task<bool> AcceptBuyTransaction(sell_Offer sellOffer, buy_Offer buyOffer, product buyerProduct, product sellerProduct);

    }
    public class TransactionService : ITransactionService
    {
        private IUnitOfWorkFactory _factory;
        public TransactionService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }
        public async Task<bool> AcceptSellTransaction(sell_Offer sellOffer, buy_Offer buyOffer, product buyerProduct, product sellerProduct)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    buyerProduct.stock += sellOffer.amount;
                    sellerProduct.stock -= sellOffer.amount;
                    if(buyerProduct.ID == 0)
                    {
                        buyerProduct = unitOfWork.ProductRepository.Add(buyerProduct);
                    }
                    else
                    {
                        unitOfWork.ProductRepository.Update(buyerProduct);
                    }
                    buyOffer.product_id = buyerProduct.ID;
                    unitOfWork.ProductRepository.Update(sellerProduct);
                    unitOfWork.SellOfferRepository.Update(sellOffer);
                    var addedOffer = unitOfWork.BuyOfferRepository.Add(buyOffer);
                    var transaction = new transactions()
                    {
                        buyer_id = addedOffer.buyer_id,
                        seller_id = sellOffer.seller_id,
                        buy_offer_id = addedOffer.ID,
                        sell_offer_id = sellOffer.ID,
                        status_id = (int)TransactionState.Finished,
                        transaction_Date = DateTime.Now,
                        Update_Who = addedOffer.buyer_id,
                        Update_Date = DateTime.Now
                    };
                    unitOfWork.TransactionRepository.Add(transaction);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
        }
        public async Task<bool> AcceptBuyTransaction(sell_Offer sellOffer, buy_Offer buyOffer, product buyerProduct, product sellerProduct)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    buyerProduct.stock += buyOffer.amount;
                    sellerProduct.stock -= buyOffer.amount;
                    unitOfWork.ProductRepository.Update(buyerProduct);
                    unitOfWork.ProductRepository.Update(sellerProduct);
                    unitOfWork.BuyOfferRepository.Update(buyOffer);
                    var addedOffer = unitOfWork.SellOfferRepository.Add(sellOffer);
                    var transaction = new transactions()
                    {
                        buyer_id = buyOffer.buyer_id,
                        seller_id = addedOffer.seller_id,
                        buy_offer_id = buyOffer.ID,
                        sell_offer_id = addedOffer.ID,
                        status_id = (int)TransactionState.Finished,
                        transaction_Date = DateTime.Now,
                        Update_Who = addedOffer.seller_id,
                        Update_Date = DateTime.Now
                    };
                    unitOfWork.TransactionRepository.Add(transaction);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
        }
        public async Task<bool> Add(transactions entity)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.TransactionRepository.Add(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
        }

        public async Task<bool> Delete(transactions entity)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.TransactionRepository.Delete(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
        }

        public async Task<transactions> GetById(int id)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entity = await unitOfWork.TransactionRepository.GetById(id);
                    return entity;
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }

        public async Task<IEnumerable<transactions>> GetData(Expression<Func<transactions, bool>> filter)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entities = await unitOfWork.TransactionRepository.GetData(filter);

                    return entities;
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }

        public async Task<bool> Update(transactions entity)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.TransactionRepository.Update(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
        }
    }
}
