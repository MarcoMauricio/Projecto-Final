using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.IO;
using FlowOptions.EggOn.ModuleCore;
using System.Configuration;
using System;

namespace FlowOptions.EggOn.WebApplication.Controllers
{
    public class EggOnController : Controller
    {
        public ActionResult GetDefaultPage(string catchall)
        {
            return View("~/Views/Default.cshtml");
        }

        // Gets a module asset. 
        // Usually only happens in DEBUG mode since in RELEASE mode all files are bundled together.
        public ActionResult GetModuleAsset(string path)
        {
            var modulesPath = Path.GetFullPath(Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings.Get("ModulesPath")));

            var filePath = Path.Combine(modulesPath, path);

            FileStream stream;

            try
            {
                stream = new FileStream(filePath, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                return new HttpNotFoundResult();
            }
            catch (Exception)
            {
                throw new HttpException("There was an error while getting the file.");
            }

            return new FileStreamResult(stream, MimeTypesHelper.GetMimeType(Path.GetExtension(filePath)));
        }
    }
}
