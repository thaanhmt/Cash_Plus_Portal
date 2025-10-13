using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockDS_FAQ")]
    public class BlockDS_FAQComponent : ViewComponent
    {
        public BlockDS_FAQComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int LanguageId, int Number, int Skip, string Template)
        {
            ViewBag.Template = Template;
            using (var db = new IOITDataContext())
            {
                IEnumerable<LegalDoc> data = (from l in db.LegalDoc
                                              where l.Status == (int)Const.Status.NORMAL
                                              && l.LanguageId == LanguageId
                                              select l).OrderByDescending(e => e.CreatedAt).Skip(Skip).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockDS_FAQ", data));
            }
        }

    }
}
