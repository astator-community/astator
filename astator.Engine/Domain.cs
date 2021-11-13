using System.Runtime.Loader;

namespace astator.Engine
{
    public class Domain : AssemblyLoadContext
    {

        public Domain() : base(true)
        {
        }

        //protected override Assembly? Load(AssemblyName assemblyName)
        //{

        //}
    }
}
