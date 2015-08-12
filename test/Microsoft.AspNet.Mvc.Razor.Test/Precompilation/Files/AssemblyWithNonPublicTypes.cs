using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc.Razor.Precompilation
{
    internal class InternalType
    {
    }

    public class PublicType
    {
        private class NestedPrivateType
        {
        }
    }

    public class ContainerType
    {
        public class NestedType
        {

        }
    }
    
    protected class ProtectedType
    {
    }
}
