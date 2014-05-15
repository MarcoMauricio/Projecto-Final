using AutoMapper;
using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.Base.ViewModels;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlowOptions.EggOn.Base.Controllers
{
    public class LayoutController : EggOnApiController
    {
        [Route("config/layout"), HttpGet]
        public LayoutDto GetLayout()
        {
            var layout = this.Database.FirstOrDefault<Layout>("");

            if (layout == null)
            {
                layout = new Layout()
                {
                    Id = Guid.Empty,
                    LogoId = null,
                    BarBackColor = "#3C3C3C",
                    BarButtonColor = "#222222",
                    BackColor = "#FFFFFF"
                };
            }

            return Mapper.Map<LayoutDto>(layout);
        }

        [Route("config/layout"), HttpPost]
        public LayoutDto UpdateLayout(LayoutDto data)
        {
            if (!this.CurrentUser.HasRole("Administrator"))
            {
                throw Forbidden("You need to be an administrator to see this resource.");
            }

            if (data == null)
            {
                throw BadRequest("The layout data is required.");
            }

            Layout layout;
            Layout oldLayout = this.Database.FirstOrDefault<Layout>("");

            if (oldLayout == null)
            {
                layout = Mapper.Map<Layout>(data);
                layout.Id = GuidComb.NewGuid();
                this.Database.Insert(layout);
            }
            else
            {
                layout = Mapper.Map<Layout>(data);
                layout.Id = oldLayout.Id;
                this.Database.Update(layout);
            }

            return Mapper.Map<LayoutDto>(layout);
        }
    }
}
