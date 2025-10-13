using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockLegalRelated")]
    public class BlockLegalRelatedComponent : ViewComponent
    {
        public BlockLegalRelatedComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number, int Id, string Language)
        {
            using (var db = new IOITDataContext())
            {
                ViewBag.Language = Language;
                IEnumerable<LegalDoc> data = (from n in db.LegalDoc
                                              where n.Status == (int)Const.Status.NORMAL
                                              && n.LegalDocId != Id
                                              && n.LanguageId == 1
                                              select n).OrderByDescending(e => e.DateIssue).ThenByDescending(e => e.CreatedAt).Take(Number).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockLegalRelated", data));
            }
        }

    }
}
