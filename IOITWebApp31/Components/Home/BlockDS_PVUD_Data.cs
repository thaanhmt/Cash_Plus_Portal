using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockDS_PVUD_Data")]
    public class BlockDS_PVUD_DataComponent : ViewComponent
    {
        public BlockDS_PVUD_DataComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int LanguageId)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<CategoryAR> data = (from c in db.Category
                                                join dsm in db.DataSetMapping on c.CategoryId equals dsm.TargetId
                                                join ds in db.DataSet on dsm.DataSetId equals ds.DataSetId
                                                where c.Status == (int)Const.Status.NORMAL
                                                && c.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE
                                                //&& c.LanguageId == LanguageId
                                                group c by new
                                                {
                                                    c.CategoryId,
                                                    c.Name,
                                                    c.Url,
                                                    c.Image,
                                                    c.Location,
                                                } into g
                                                select new CategoryAR
                                                {
                                                    CategoryId = g.Key.CategoryId,
                                                    Name = g.Key.Name,
                                                    Url = g.Key.Url,
                                                    Image = g.Key.Image,
                                                    Location = g.Key.Location,
                                                    DataSetNumber = g.Count()
                                                }).OrderBy(e => e.Location).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockDS_PVUD_Data", data));
            }
        }

    }
}
