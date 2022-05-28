using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Tax
{
    public class FeesService : IFeesService
    {
        #region Fields
        private readonly IRepository<Fees> _feesRepository;
        private readonly IEventPublisher _eventPublisher;
        #endregion

        #region Ctor
        public FeesService(IRepository<Fees> feesRepository, IEventPublisher eventPublisher)
        {
            _feesRepository = feesRepository;
            _feesRepository = feesRepository;
        }
        #endregion

        #region Methodes
        public void Delete(Fees fees)
        {
            if (fees == null)
                throw new ArgumentNullException(nameof(fees));

            _feesRepository.Delete(fees);

            //event notification
            _eventPublisher.EntityDeleted(fees);
        }

        public Fees Get(int id)
        {
            if (id == 0)
                return null;

            return _feesRepository.ToCachedGetById(id);
        }

        public IEnumerable<Fees> GetAll()
        {
            return _feesRepository.Table.ToList();
        }

        public Fees Save(Fees fees)
        {
            if (fees == null)
                throw new ArgumentNullException(nameof(fees));

            if (fees.Id == 0)
            {
                _feesRepository.Insert(fees);

                //event notification
                _eventPublisher.EntityInserted(fees);
            }
            else
            {
                _feesRepository.Update(fees);

                //event notification
                _eventPublisher.EntityUpdated(fees);
            }

            return fees;
        } 
        #endregion
    }
}
