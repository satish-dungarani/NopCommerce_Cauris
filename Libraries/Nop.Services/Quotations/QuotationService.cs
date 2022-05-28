using Nop.Core.Domain.Quotations;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Quotations
{
    public class QuotationService : IQuotationService
    {
        #region Fields
        private readonly IRepository<Quotation> _quotationRepository;
        private readonly IEventPublisher _eventPublisher;
        #endregion

        #region Ctor
        public QuotationService(IRepository<Quotation> quotationRepository, IEventPublisher eventPublisher)
        {
            _quotationRepository = quotationRepository;
            _eventPublisher = eventPublisher;
        }
        #endregion

        #region Methodes
        public void Delete(Quotation quotation)
        {
            if (quotation == null)
                throw new ArgumentNullException(nameof(quotation));

            _quotationRepository.Delete(quotation);

            //event notification
            _eventPublisher.EntityDeleted(quotation);
        }

        public Quotation Get(int id)
        {
            if (id == 0)
                return null;

            return _quotationRepository.ToCachedGetById(id);
        }

        public IEnumerable<Quotation> GetAllByCustomerId(int customerId)
        {
            var query = _quotationRepository.Table;

            if (customerId > 0)
            {
                query = query.Where(x => x.CustomerId == customerId);
            }

            return query.ToList();
        }

        public IEnumerable<Quotation> GetAllByStatus(int status)
        {
            return _quotationRepository.Table.Where(x => x.Status == status).ToList();
        }

        public IEnumerable<Quotation> GetAllByVendorId(int vendorId)
        {
            var query = _quotationRepository.Table;

            if (vendorId > 0)
            {
                query = query.Where(x => x.VendorId == vendorId);
            }

            return query.ToList();
        }

        public Quotation GetByCustomerIdProductId(int customerId, int productId)
        {
            var query = _quotationRepository.Table;

            if (customerId > 0 && productId >0)
            {
                query = query.Where(x => x.CustomerId == customerId && x.ProductId == productId);
            }

            return query.FirstOrDefault();
        }

        public Quotation Save(Quotation quotation)
        {
            if (quotation == null)
                throw new ArgumentNullException(nameof(quotation));

            if (quotation.Id == 0)
            {
                _quotationRepository.Insert(quotation);

                //event notification
                _eventPublisher.EntityInserted(quotation); 
            }
            else
            {
                _quotationRepository.Update(quotation);

                //event notification
                _eventPublisher.EntityUpdated(quotation);
            }

            return quotation;
        } 
        #endregion
    }
}
