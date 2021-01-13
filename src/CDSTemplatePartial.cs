using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NY.Dataverse.LINQPadDriver
{
    partial class CDSTemplate
    {
        public CDSTemplate(List<(EntityMetadata entityMetadata, List<(string attributeName, List<(string Label, int? Value)> options)> optionMetadata)> metadata) => Metadata = metadata;

        public List<(EntityMetadata entityMetadata, List<(string attributeName, List<(string Label, int? Value)> options)> optionMetadata)> Metadata { get; private set; }
    }
}
