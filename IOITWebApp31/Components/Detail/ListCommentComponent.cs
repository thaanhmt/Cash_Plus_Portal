using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{

    [ViewComponent(Name = "ListComment")]
    public class ListCommentComponent : ViewComponent
    {
        public ListCommentComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CommentParentId, List<CommentDT> listData)
        {
            using (var db = new IOITDataContext())
            {
                //var data = listData.Where(c => c.CommentParentId == CommentParentId).ToList();
                ViewBag.CommentParentId = CommentParentId;
                return await Task.FromResult((IViewComponentResult)View("ListComment", listData));
            }
        }

    }
}
