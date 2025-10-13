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
//    [ViewComponent(Name = "BlockPrice")]
//    public class BlockPriceComponent : ViewComponent
//    {
//        public BlockPriceComponent()
//        {
//        }

//        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<MaterialDT> MaterialItem)
//        {
//            using (var db = new IOITDataContext())
//            {
//                List<MaterialDT> data = new List<MaterialDT>();
//                if (MaterialItem != null)
//                {
//                    List<MaterialDT> dataItem = MaterialItem.Where(e => e.ProvinceId == 1).Take(6).ToList();
//                    foreach (var item in dataItem)
//                    {
//                        MaterialDT material = new MaterialDT();
//                        material.MaterialId = item.MaterialId;
//                        string name = "";
//                        var categoryMaterial = db.CategoryMaterial.Where(e => e.CategoryMaterialId == item.CategoryMaterialId).FirstOrDefault();
//                        if (categoryMaterial != null) name += categoryMaterial.Name + " ";
//                        var locationProvider = db.LocationProvider.Where(e => e.LocationProviderId == item.LocaltionProviderId).FirstOrDefault();
//                        if (locationProvider != null) name += locationProvider.Name;
//                        material.Name = name;
//                        var unitCalculate = db.UnitCalculate.Where(e => e.UnitCalculateId == item.UnitCalculateId).FirstOrDefault();
//                        if (unitCalculate != null) material.UnitCalculate = unitCalculate.Name;
//                        material.UnitPrice = item.UnitPrice;
//                        data.Add(material);
//                    }
//                }
//                return await Task.FromResult((IViewComponentResult)View("BlockPrice", data));
//            }
//        }
//    }
//}
