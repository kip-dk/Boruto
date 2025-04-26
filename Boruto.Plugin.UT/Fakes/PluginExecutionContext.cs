using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Fakes
{
    public class PluginExecutionContext : Microsoft.Xrm.Sdk.IPluginExecutionContext
    {
        public PluginExecutionContext(int stage, bool async, string message, string logicalname, Guid? id)
        {
            this.Stage = stage;
            this.Mode = async ? 1 : 0;

            this.MessageName = message;
            this.PrimaryEntityName = logicalname;
            this.PrimaryEntityId = id ?? Guid.Empty;

            if (message == "Delete" && !string.IsNullOrEmpty(logicalname) && id != null)
            {
                this.InputParameters["Target"] = new Microsoft.Xrm.Sdk.EntityReference(logicalname, id.Value);
            }
        }

        public int Stage { get; }

        public IPluginExecutionContext ParentContext => null;

        public int Mode { get; }

        public int IsolationMode => 0;

        public int Depth => 1;

        public string MessageName { get; }

        public string PrimaryEntityName { get; }

        public Guid? RequestId => Guid.NewGuid();

        public string SecondaryEntityName => null;

        public ParameterCollection InputParameters { get; } = new ParameterCollection();

        public ParameterCollection OutputParameters { get; } = new ParameterCollection();

        public ParameterCollection SharedVariables { get; } = new ParameterCollection();

        public Guid UserId => new Guid();

        public Guid InitiatingUserId => new Guid();

        public Guid BusinessUnitId => new Guid();

        public Guid OrganizationId => Guid.NewGuid();

        public string OrganizationName => "FAKE";

        public Guid PrimaryEntityId { get; }

        public EntityImageCollection PreEntityImages => new EntityImageCollection();

        public EntityImageCollection PostEntityImages => new EntityImageCollection();

        public EntityReference OwningExtension => null;

        public Guid CorrelationId => Guid.NewGuid();

        public bool IsExecutingOffline => false;

        public bool IsOfflinePlayback => false;

        public bool IsInTransaction => true;

        public Guid OperationId => Guid.NewGuid();

        public DateTime OperationCreatedOn => System.DateTime.UtcNow;
    }
}
