using System.Runtime.Loader;

namespace astator.Core.Engine
{
    public class Domain : AssemblyLoadContext
    {

        public Domain() : base(true)
        {

        }
    }
}
