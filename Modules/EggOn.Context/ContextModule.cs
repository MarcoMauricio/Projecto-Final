

using FlowOptions.EggOn.Logging;
using FlowOptions.EggOn.ModuleCore;
namespace FlowOptions.EggOn.Context
{
    public class ContextModule : IEggOnModule
    {
        public void Setup()
        {
            Logger.Debug("Application is registering the Context Area.");

            SetupDatabase();
        }

        internal void SetupDatabase()
        {
           // Nothing to do here
        }
    }
}
