using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockDS_TopUser")]
    public class BlockDS_TopUserComponent : ViewComponent
    {
        public BlockDS_TopUserComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int LanguageId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                var dateStart = DateTime.Now.AddDays(-180);
                var dateEnd = DateTime.Now;
                var data = await (from d in db.DataSet
                                  join c in db.Customer on d.UserCreatedId equals c.CustomerId
                                  where d.Status == (int)Const.Status.NORMAL
                                  //&& c.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL
                                  //&& d.LanguageId == LanguageId
                                  && c.Status == (int)Const.Status.NORMAL
                                  && d.PublishedAt >= dateStart && d.PublishedAt <= dateEnd
                                  group c by new
                                  {
                                      c.CustomerId,
                                  } into g
                                  select new TopUserHome
                                  {
                                      CustomerId = g.Key.CustomerId,
                                      DataSetNumber = g.Count(),
                                  }).OrderByDescending(c => c.DataSetNumber).Take(Number).ToListAsync();

                foreach (var item in data)
                {
                    var cus = await db.Customer.Where(e => e.CustomerId == item.CustomerId).FirstOrDefaultAsync();
                    if (cus != null)
                    {
                        item.FullName = cus.FullName;
                        item.Avata = cus.Avata;
                    }
                    //var ds = await db.DataSet.Where(e => e.UserCreatedId == item.CustomerId).FirstOrDefaultAsync();
                    //if (ds != null)
                    //{
                    //    item.DataSetId = ds.DataSetId;
                    //    item.Title = ds.Title;
                    //    item.Description = ds.Description;
                    //    item.Url = ds.Url;
                    //}
                }

                return await Task.FromResult((IViewComponentResult)View("BlockDS_TopUser", data));
            }
        }

    }
}
