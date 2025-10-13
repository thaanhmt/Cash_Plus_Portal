//using IOITWebApp31.Models;
//using IOITWebApp31.Models.Data;
//using IOITWebApp31.Models.EF;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace IOITWebApp31.Components.Shared
//{
//    [ViewComponent(Name = "BlockTablePrice")]
//    public class BlockTablePriceComponent : ViewComponent
//    {
//        public BlockTablePriceComponent()
//        {
//        }

//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            using (var db = new IOITDataContext())
//            {
//                IEnumerable<MaterialDT> data = (from mi in db.Material
//                                                where (mi.ProvinceId == 1 || mi.ProvinceId == 2 || mi.ProvinceId == 3)
//                                                && mi.MaterialParentId == 0
//                                                && mi.Status != (int)Const.Status.DELETED
//                                                select new MaterialDT
//                                                {
//                                                    MaterialId = mi.MaterialId,
//                                                    MaterialParentId = mi.MaterialParentId,
//                                                    CategoryMaterialId = mi.CategoryMaterialId,
//                                                    LocaltionProviderId = mi.LocaltionProviderId,
//                                                    Location = mi.Location,
//                                                    UnitCalculateId = mi.UnitCalculateId,
//                                                    ProvinceId = mi.ProvinceId,
//                                                    UnitPrice = mi.UnitPrice,
//                                                    listMaterialChild = db.Material.Where(e => e.MaterialParentId == mi.MaterialId && e.Status != (int)Const.Status.DELETED)
//                                                    .Select(e => new MaterialDT
//                                                    {
//                                                        MaterialId = e.MaterialId,
//                                                        MaterialParentId = e.MaterialParentId,
//                                                        CategoryMaterialId = e.CategoryMaterialId,
//                                                        LocaltionProviderId = e.LocaltionProviderId,
//                                                        Location = e.Location,
//                                                        UnitCalculateId = e.UnitCalculateId,
//                                                        ProvinceId = e.ProvinceId,
//                                                        UnitPrice = e.UnitPrice,
//                                                    }).ToList(),
//                                                }).OrderByDescending(e => e.Location).ToList();

//                //Lấy 3 sp của hn
//                List<MaterialDT> dataHN = data.Where(e => e.ProvinceId == 1).Take(3).ToList();
//                if (dataHN.Count > 2)
//                {
//                    ViewData["dtHN1"] = dataHN.ToList()[0].listMaterialChild;
//                    int categoryMaterialId1 = (int)dataHN.ToList()[0].CategoryMaterialId;
//                    var categoryMaterial1 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId1).FirstOrDefault();
//                    if (categoryMaterial1 != null)
//                        ViewData["dataHN1"] = categoryMaterial1.Name;
//                    else
//                        ViewData["dataHN1"] = "";
//                    //
//                    ViewData["dtHN2"] = dataHN.ToList()[1].listMaterialChild;
//                    int categoryMaterialId2 = (int)dataHN.ToList()[1].CategoryMaterialId;
//                    var categoryMaterial2 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId2).FirstOrDefault();
//                    if (categoryMaterial2 != null)
//                        ViewData["dataHN2"] = categoryMaterial2.Name;
//                    else
//                        ViewData["dataHN2"] = "";
//                    //
//                    ViewData["dtHN3"] = dataHN.ToList()[2].listMaterialChild;
//                    int categoryMaterialId3 = (int)dataHN.ToList()[2].CategoryMaterialId;
//                    var categoryMaterial3 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId3).FirstOrDefault();
//                    if (categoryMaterial3 != null)
//                        ViewData["dataHN3"] = categoryMaterial3.Name;
//                    else
//                        ViewData["dataHN3"] = "";
//                }
//                //
//                //Lấy 3 sp của hcm
//                List<MaterialDT> dataHCM = data.Where(e => e.ProvinceId == 2).Take(3).ToList();
//                if (dataHCM.Count > 2)
//                {
//                    //ViewData["dtHCM1"] = dataHCM.ToList()[0].listMaterialChild;
//                    int categoryMaterialId4 = (int)dataHCM.ToList()[0].CategoryMaterialId;
//                    var categoryMaterial4 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId4).FirstOrDefault();
//                    if (categoryMaterial4 != null)
//                        ViewData["dataHCM1"] = categoryMaterial4.Name;
//                    else
//                        ViewData["dataHCM1"] = "";
//                    //
//                    //ViewData["dtHCM2"] = dataHCM.ToList()[1].listMaterialChild;
//                    int categoryMaterialId5 = (int)dataHCM.ToList()[1].CategoryMaterialId;
//                    var categoryMaterial5 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId5).FirstOrDefault();
//                    if (categoryMaterial5 != null)
//                        ViewData["dataHCM2"] = categoryMaterial5.Name;
//                    else
//                        ViewData["dataHCM2"] = "";
//                    //
//                    //ViewData["dtHCM3"] = dataHCM.ToList()[2].listMaterialChild;
//                    int categoryMaterialId6 = (int)dataHCM.ToList()[2].CategoryMaterialId;
//                    var categoryMaterial6 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId6).FirstOrDefault();
//                    if (categoryMaterial6 != null)
//                        ViewData["dataHCM3"] = categoryMaterial6.Name;
//                    else
//                        ViewData["dataHCM3"] = "";
//                }
//                //Lấy 3 sp của dn
//                List<MaterialDT> dataDN = data.Where(e => e.ProvinceId == 3).Take(3).ToList();
//                if (dataDN.Count > 2)
//                {
//                    //ViewData["dtDN1"] = dataDN.ToList()[0].listMaterialChild;
//                    int categoryMaterialId7 = (int)dataDN.ToList()[0].CategoryMaterialId;
//                    var categoryMaterial7 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId7).FirstOrDefault();
//                    if (categoryMaterial7 != null)
//                        ViewData["dataDN1"] = categoryMaterial7.Name;
//                    else
//                        ViewData["dataDN1"] = "";
//                    //
//                    //ViewData["dtDN2"] = dataDN.ToList()[1].listMaterialChild;
//                    int categoryMaterialId8 = (int)dataDN.ToList()[1].CategoryMaterialId;
//                    var categoryMaterial8 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId8).FirstOrDefault();
//                    if (categoryMaterial8 != null)
//                        ViewData["dataDN2"] = categoryMaterial8.Name;
//                    else
//                        ViewData["dataDN2"] = "";
//                    //
//                    //ViewData["dtDN3"] = dataDN.ToList()[2].listMaterialChild;
//                    int categoryMaterialId9 = (int)dataDN.ToList()[2].CategoryMaterialId;
//                    var categoryMaterial9 = db.CategoryMaterial.Where(e => e.CategoryMaterialId == categoryMaterialId9).FirstOrDefault();
//                    if (categoryMaterial9 != null)
//                        ViewData["dataDN3"] = categoryMaterial9.Name;
//                    else
//                        ViewData["dataDN3"] = "";
//                }
//                return await Task.FromResult((IViewComponentResult)View("BlockTablePrice", data));
//            }
//        }
//    }
//}
