using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.EscrowBaseWeb.EsignCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Web.Controllers
{
    public class EsignCompanyController : EscrowBaseWebControllerBase
    {
        public readonly IAbpSession _abpSession;
         
        private readonly IRepository<EsignCompanyMapping> _esignCompanyMapping;
        public EsignCompanyController(IRepository<EsignCompanyMapping> esignCompanyMapping, IAbpSession abpSession)
        {
            _esignCompanyMapping = esignCompanyMapping;
            _abpSession = abpSession;
        }


        public EsignCompanyMapping InsertOrUpdate(EsignCompanyMapping esignCompanyMapping)
        {
            return esignCompanyMapping;
        }


    }
}
