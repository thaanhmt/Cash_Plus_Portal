using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockDS_TopUnit")]
    public class BlockDS_TopUnitComponent : ViewComponent
    {
        public BlockDS_TopUnitComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                var dateStart = DateTime.Now.AddDays(-180);
                var dateEnd = DateTime.Now;
                var data = (from u in db.Unit
                            join dsm in db.DataSetMapping on u.UnitId equals dsm.TargetId
                            join ds in db.DataSet on dsm.DataSetId equals ds.DataSetId
                            where u.Status == (int)Const.Status.NORMAL
                            //&& u.UnitParentId == 0
                            && dsm.TargetType == (int)Const.DataSetMapping.DATA_UNIT
                            && ds.Type == (int)Const.DataSetType.DATA_UNIT
                            && ds.Status == (int)Const.Status.NORMAL
                            && ds.PublishedAt >= dateStart && ds.PublishedAt <= dateEnd
                            group u by new
                            {
                                u.UnitId,
                                u.Name,
                                u.Image,
                            } into g
                            select new TopUnitHome
                            {
                                UnitId = g.Key.UnitId,
                                Name = g.Key.Name,
                                Image = g.Key.Image,
                                DataSetNumber = g.Count(),
                            }).OrderByDescending(c => c.DataSetNumber).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockDS_TopUnit", data));
            }
        }

    }
}
