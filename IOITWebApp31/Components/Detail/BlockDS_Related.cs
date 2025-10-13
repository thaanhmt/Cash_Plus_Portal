using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "BlockDS_Related")]
    public class BlockDS_RelatedComponent : ViewComponent
    {
        public BlockDS_RelatedComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(long DatasetId, int Number, List<CategoryDTL> listResearchArea)
        {
            using (var db = new IOITDataContext())
            {
                ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
                List<string> listResearchAreas = listResearchArea.Select(e => e.CategoryId + "").ToList();

                var data = await (from ds in db.DataSet
                                  join da in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                                  on ds.DataSetId equals da.DataSetId
                                  where ds.Status == (int)Const.Status.NORMAL
                                  && da.Status != (int)Const.Status.DELETED
                                  && da.DataSetId != DatasetId
                                  && listResearchAreas.Contains(da.TargetId.ToString())
                                  group ds by new
                                  {
                                      ds.DataSetId,
                                      ds.PublishedAt,

                                  } into e
                                  select new DataSetDTO
                                  {
                                      DataSetId = e.Key.DataSetId,
                                      PublishedAt = e.Key.PublishedAt
                                  }).OrderByDescending(e => e.PublishedAt).Take(Number).ToListAsync();

                foreach (var itemD in data)
                {
                    var itemData = await db.DataSet.Where(e => e.DataSetId == itemD.DataSetId).FirstOrDefaultAsync();
                    itemD.DataSetId = itemData.DataSetId;
                    itemD.Title = itemData.Title;
                    itemD.Description = itemData.Description;
                    //itemD.Contents = itemData.Contents;
                    itemD.Image = itemData.Image;
                    itemD.Url = itemData.Url;
                    itemD.LinkVideo = itemData.LinkVideo;
                    itemD.AuthorName = itemData.AuthorName;
                    itemD.AuthorEmail = itemData.AuthorEmail;
                    itemD.AuthorPhone = itemData.AuthorPhone;
                    itemD.Version = itemData.Version;
                    itemD.Note = itemData.Note;
                    itemD.DateStartActive = itemData.DateStartActive;
                    itemD.DateStartOn = itemData.DateStartOn;
                    itemD.DateEndOn = itemData.DateEndOn;
                    itemD.DownNumber = itemData.DownNumber;
                    itemD.ViewNumber = itemData.ViewNumber;
                    itemD.RateStar = itemData.RateStar;
                    itemD.Location = itemData.Location;
                    itemD.IsHot = itemData.IsHot;
                    itemD.Type = itemData.Type;
                    itemD.ApplicationRangeId = itemData.ApplicationRangeId;
                    itemD.ResearchAreaId = itemData.ResearchAreaId;
                    itemD.IsPublish = itemData.IsPublish;
                    itemD.ConfirmsPrivate = itemData.ConfirmsPrivate;
                    itemD.ConfirmsPublish = itemData.ConfirmsPublish;
                    itemD.MetaTitle = itemData.MetaTitle;
                    itemD.MetaKeyword = itemData.MetaKeyword;
                    itemD.MetaDescription = itemData.MetaDescription;
                    itemD.LanguageId = itemData.LanguageId;
                    itemD.WebsiteId = itemData.WebsiteId;
                    itemD.CompanyId = itemData.CompanyId;
                    itemD.UserCreatedId = itemData.UserCreatedId;
                    itemD.CreatedAt = itemData.CreatedAt;
                    itemD.UserEditedId = itemData.UserEditedId;
                    itemD.EditedAt = itemData.EditedAt;
                    itemD.UserApprovedId = itemData.UserApprovedId;
                    itemD.ApprovingAt = itemData.ApprovingAt;
                    itemD.ApprovedAt = itemData.ApprovedAt;
                    itemD.UserPublishedId = itemData.UserPublishedId;
                    itemD.PublishingAt = itemData.PublishingAt;
                    itemD.PublishedAt = itemData.PublishedAt;
                    itemD.UserId = itemData.UserId;
                    itemD.UpdatedAt = itemData.UpdatedAt;
                    itemD.Status = itemData.Status;
                    itemD.userCreated = db.Customer.Where(c => c.CustomerId == itemD.UserCreatedId).Select(c => new CustomerDT
                    {
                        UserId = c.CustomerId,
                        FullName = c.FullName,
                        UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                    }).FirstOrDefault();
                    itemD.listFiles = await db.Attactment.Where(c => c.TargetId == itemData.DataSetId
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

                return await Task.FromResult((IViewComponentResult)View("BlockDS_Related", data));
            }
        }
    }
}
