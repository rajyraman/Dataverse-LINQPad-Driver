using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NY.CommonDataService.LINQPadDriver
{
    partial class CDSTemplate
    {
        public CDSTemplate(List<EntityMetadata> metadata) => Metadata = metadata;

        public List<EntityMetadata> Metadata { get; private set; }
    }
}
