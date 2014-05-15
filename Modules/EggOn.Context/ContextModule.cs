using AutoMapper;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.Context.Models;
using FlowOptions.EggOn.Logging;
using FlowOptions.EggOn.ModuleCore;

namespace EggOn.Web.Service.Areas.Context
{
    public class ContextModule : IEggOnModule
    {
        public void Setup()
        {
            Logger.Debug("Application is registering the Context Area.");
        }
    }
}
