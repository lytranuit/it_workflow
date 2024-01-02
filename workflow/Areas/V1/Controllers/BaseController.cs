using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Vue.Data;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;
using static Vue.Data.ItContext;

namespace workflow.Areas.V1.Controllers
{
    [Area("V1")]
    [Authorize(Roles = "Administrator,User")]
    //[Authorize]
    public class BaseController : Controller
    {
        protected readonly ItContext _context;

        public BaseController(ItContext context)
        {
            _context = context;
            var listener = _context.GetService<DiagnosticSource>();
            (listener as DiagnosticListener).SubscribeWithAdapter(new CommandInterceptor());
        }
    }
}
