using System.Runtime.Loader;

namespace astator.Core.Engine
{
    internal class Domain : AssemblyLoadContext
    {

        public Domain() : base(true)
        {

        }
    }
}
