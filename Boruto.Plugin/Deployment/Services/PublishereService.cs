using Boruto.Extensions.FilterExpression;
using Boruto.Extensions.QueryExpression;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Services
{
    internal class PublishereService : ServiceAPI.IPublishereService
    {
        private readonly IOrganizationService orgService;
        private string _componentString;

        public PublishereService(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public string ComponentPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(_componentString))
                {
                    var sol = Boruto.Deployment.Models.Config.Instance?.Solution;
                    if (string.IsNullOrEmpty(sol))
                    {
                        throw new Exception("No solution found in configuration setup");
                    }

                    var solQuery = Entities.Solution.EntityLogicalName.ToQueryExpression();
                    solQuery.Criteria.Equal(Entities.Solution.Fields.UniqueName, sol);

                    var solution = this.orgService.RetrieveMultiple(solQuery).Entities.Select(r => new Entities.Solution(r)).FirstOrDefault();
                    if (solution == null)
                    {
                        throw new Exception($"Not solution with unique id { sol } was found");
                    }

                    var pubQuery = Entities.Publisher.EntityLogicalName.ToQueryExpression();
                    pubQuery.Criteria.Equal(Entities.Publisher.Fields.PublisherId, solution.PublisherId.Id);
                    var publish = this.orgService.RetrieveMultiple(pubQuery).Entities.Select(r => new Entities.Publisher(r)).Single();

                    this._componentString = publish.CustomizationPrefix;
                }
                return _componentString;
            }
        }

    }
}
