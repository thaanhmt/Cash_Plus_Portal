using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockDS_HotData")]
    public class BlockDS_HotDataComponent : ViewComponent
    {
        public BlockDS_HotDataComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                try
                {
                    ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
                    ViewBag.access_token = "'" + HttpContext.Session.GetString("access_token") + "'";
                    //var dateStart = DateTime.Now.AddDays(-180);
                    //var dateEnd = DateTime.Now;
                    var data = await (from e in db.DataSet
                                      where e.Status == (int)Const.Status.NORMAL
                                            && e.IsHot == true
                                      select new DataSetDTO
                                      {
                                          DataSetId = e.DataSetId,
                                          Title = e.Title,
                                          Description = e.Description,
                                          Contents = e.Contents,
                                          Image = e.Image,
                                          Url = e.Url,
                                          LinkVideo = e.LinkVideo,
                                          AuthorName = e.AuthorName,
                                          AuthorEmail = e.AuthorEmail,
                                          AuthorPhone = e.AuthorPhone,
                                          Version = e.Version,
                                          Note = e.Note,
                                          DateStartActive = e.DateStartActive,
                                          DateStartOn = e.DateStartOn,
                                          DateEndOn = e.DateEndOn,
                                          DownNumber = e.DownNumber,
                                          ViewNumber = e.ViewNumber,
                                          Location = e.Location,
                                          IsHot = e.IsHot,
                                          Type = e.Type,
                                          ApplicationRangeId = e.ApplicationRangeId,
                                          ResearchAreaId = e.ResearchAreaId,
                                          IsPublish = e.IsPublish,
                                          ConfirmsPrivate = e.ConfirmsPrivate,
                                          ConfirmsPublish = e.ConfirmsPublish,
                                          MetaTitle = e.MetaTitle,
                                          MetaKeyword = e.MetaKeyword,
                                          MetaDescription = e.MetaDescription,
                                          LanguageId = e.LanguageId,
                                          WebsiteId = e.WebsiteId,
                                          CompanyId = e.CompanyId,
                                          UserCreatedId = e.UserCreatedId,
                                          CreatedAt = e.CreatedAt,
                                          UserEditedId = e.UserEditedId,
                                          EditedAt = e.EditedAt,
                                          UserApprovedId = e.UserApprovedId,
                                          ApprovingAt = e.ApprovingAt,
                                          ApprovedAt = e.ApprovedAt,
                                          UserPublishedId = e.UserPublishedId,
                                          PublishingAt = e.PublishingAt,
                                          PublishedAt = e.PublishedAt,
                                          UserId = e.UserId,
                                          UpdatedAt = e.UpdatedAt,
                                          Status = e.Status,
                                      }).OrderByDescending(c => c.DateStartActive).ToListAsync();
                    Random rng = new Random();
                    int n = data.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        var value = data[k];
                        data[k] = data[n];
                        data[n] = value;
                    }
                    data = data.Take(Number).ToList();
                    foreach (var itemD in data)
                    {
                        itemD.userCreated = db.Customer.Where(c => c.CustomerId == itemD.UserCreatedId).Select(c => new CustomerDT
                        {
                            UserId = c.CustomerId,
                            FullName = c.FullName,
                            UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                        }).FirstOrDefault();

                        itemD.listFiles = await db.Attactment.Where(c => c.TargetId == itemD.DataSetId
                                                && c.TargetType == (int)Const.TypeAttachment.FILE_DATASET
                                                && c.Status != (int)Const.Status.DELETED).Select(c => new AttactmentDTO
                                                {
                                                    AttactmentId = c.AttactmentId,
                                                    Name = c.Name,
                                                    TargetId = c.TargetId,
                                                    TargetType = c.TargetType,
                                                    Url = c.Url,
                                                    Thumb = c.Thumb,
                                                    Note = c.Note,
                                                    Extension = c.Extension,
                                                    ExtensionName = c.ExtensionName != null ? c.ExtensionName.Substring(1, c.ExtensionName.Length - 1).ToUpper() : "",
                                                    Storage = c.Storage,
                                                    CreatedId = c.CreatedId,
                                                    UpdatedId = c.UpdatedId,
                                                    CreatedAt = c.CreatedAt,
                                                    UpdatedAt = c.UpdatedAt,
                                                    Status = c.Status,
                                                }).ToListAsync();


                    }

                    return await Task.FromResult((IViewComponentResult)View("BlockDS_HotData", data));
                }
                catch (Exception ex)
                {
                    return await Task.FromResult((IViewComponentResult)View("BlockDS_HotData"));
                }
            }
        }

    }
}
