using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiCMS
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("translate", "translate");

        [HttpGet("{id}/{sl}/{tl}/{type}")]
        public async Task<IActionResult> TranslateById(int id, string sl = "vi", string tl = "en", int type = 1)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            //{
            //    def.meta = new Meta(222, "No permission");
            //    return Ok(def);
            //}
            try
            {
                using (var db = new IOITDataContext())
                {
                    if (type == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY)
                    {
                        Category data = await db.Category.FindAsync(id);

                        if (data == null)
                        {
                            def.meta = new Meta(404, "Not Found");
                            return Ok(def);
                        }

                        if (data.Name != null && data.Name != "")
                        {
                            data.Name = TranslateText(sl, tl, ReplaceTextByDictionary(data.Name, sl));
                            data.Url = Utils.NonUnicode(data.Name);
                        }
                        if (data.Description != null && data.Description != "")
                            data.Description = TranslateText(sl, tl, ReplaceTextByDictionary(data.Description, sl));
                        if (data.Contents != null && data.Contents != "")
                            data.Contents = Utils.TextToHtml(TranslateText(sl, tl, System.Net.WebUtility.HtmlDecode(ReplaceTextByDictionary(data.Contents, sl))));
                        if (data.MetaDescription != null && data.MetaDescription != "")
                            data.MetaDescription = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaDescription, sl));
                        if (data.MetaKeyword != null && data.MetaKeyword != "")
                            data.MetaKeyword = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaKeyword, sl));
                        if (data.MetaTitle != null && data.MetaTitle != "")
                            data.MetaTitle = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaTitle, sl));
                        def.data = data;
                    }
                    else if (type == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS)
                    {
                        News data = await db.News.FindAsync(id);

                        if (data == null)
                        {
                            def.meta = new Meta(404, "Not Found");
                            return Ok(def);
                        }

                        if (data.Title != null && data.Title != "")
                        {
                            data.Title = TranslateText(sl, tl, ReplaceTextByDictionary(data.Title, sl));
                            data.Url = Utils.NonUnicode(data.Title);
                        }
                        if (data.Description != null && data.Description != "")
                            data.Description = TranslateText(sl, tl, ReplaceTextByDictionary(data.Description, sl));
                        if (data.Contents != null && data.Contents != "")
                            data.Contents = Utils.TextToHtml(TranslateText(sl, tl, System.Net.WebUtility.HtmlDecode(ReplaceTextByDictionary(data.Contents, sl))));
                        if (data.Introduce != null && data.Introduce != "")
                            data.Introduce = TranslateText(sl, tl, ReplaceTextByDictionary(data.Introduce, sl));
                        if (data.SystemDiagram != null && data.SystemDiagram != "")
                            data.SystemDiagram = TranslateText(sl, tl, ReplaceTextByDictionary(data.SystemDiagram, sl));
                        if (data.MetaDescription != null && data.MetaDescription != "")
                            data.MetaDescription = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaDescription, sl));
                        if (data.MetaKeyword != null && data.MetaKeyword != "")
                            data.MetaKeyword = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaKeyword, sl));
                        if (data.MetaTitle != null && data.MetaTitle != "")
                            data.MetaTitle = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaTitle, sl));

                        ////
                        //if (data.Title != null && data.Title != "")
                        //{
                        //    data.Title = ReplaceTextByDictionary(TranslateText(data.Title, sl, tl), sl);
                        //    data.Url = Utils.NonUnicode(data.Title);
                        //}
                        //if (data.Description != null && data.Description != "")
                        //    data.Description = ReplaceTextByDictionary(TranslateText(data.Description, sl, tl), sl);
                        //if (data.Contents != null && data.Contents != "")
                        //    data.Contents = Utils.TextToHtml(ReplaceTextByDictionary(System.Net.WebUtility.HtmlDecode(TranslateText(data.Contents, sl, tl)), sl));
                        //if (data.Introduce != null && data.Introduce != "")
                        //    data.Introduce = ReplaceTextByDictionary(TranslateText(data.Introduce, sl, tl), sl);
                        //if (data.SystemDiagram != null && data.SystemDiagram != "")
                        //    data.SystemDiagram = ReplaceTextByDictionary(TranslateText(data.SystemDiagram, sl, tl), sl);
                        //if (data.MetaDescription != null && data.MetaDescription != "")
                        //    data.MetaDescription = ReplaceTextByDictionary(TranslateText(data.MetaDescription, sl, tl), sl);
                        //if (data.MetaKeyword != null && data.MetaKeyword != "")
                        //    data.MetaKeyword = ReplaceTextByDictionary(TranslateText(data.MetaKeyword, sl, tl), sl);
                        //if (data.MetaTitle != null && data.MetaTitle != "")
                        //    data.MetaTitle = ReplaceTextByDictionary(TranslateText(data.MetaTitle, sl, tl), sl);

                        def.data = data;
                    }
                    else if (type == (int)Const.TypeLanguageMapping.LANGUAGE_PRODUCT)
                    {
                        Product data = await db.Product.FindAsync(id);

                        if (data == null)
                        {
                            def.meta = new Meta(404, "Not Found");
                            return Ok(def);
                        }

                        if (data.Name != null && data.Name != "")
                        {
                            data.Name = TranslateText(sl, tl, ReplaceTextByDictionary(data.Name, sl));
                            data.Url = Utils.NonUnicode(data.Name);
                        }
                        if (data.Description != null && data.Description != "")
                            data.Description = Utils.TextToHtml(TranslateText(sl, tl, System.Net.WebUtility.HtmlDecode(ReplaceTextByDictionary(data.Description, sl))));
                        if (data.Contents != null && data.Contents != "")
                            data.Contents = Utils.TextToHtml(TranslateText(sl, tl, System.Net.WebUtility.HtmlDecode(ReplaceTextByDictionary(data.Contents, sl))));
                        if (data.Feature != null && data.Feature != "")
                            data.Feature = Utils.TextToHtml(TranslateText(sl, tl, System.Net.WebUtility.HtmlDecode(ReplaceTextByDictionary(data.Feature, sl))));
                        if (data.Configuration != null && data.Configuration != "")
                            data.Configuration = Utils.TextToHtml(TranslateText(sl, tl, System.Net.WebUtility.HtmlDecode(ReplaceTextByDictionary(data.Configuration, sl))));
                        if (data.NoteTech != null && data.NoteTech != "")
                            data.NoteTech = Utils.TextToHtml(TranslateText(sl, tl, System.Net.WebUtility.HtmlDecode(ReplaceTextByDictionary(data.NoteTech, sl))));
                        if (data.NotePromotion != null && data.NotePromotion != "")
                            data.NotePromotion = Utils.TextToHtml(TranslateText(sl, tl, System.Net.WebUtility.HtmlDecode(ReplaceTextByDictionary(data.NotePromotion, sl))));
                        if (data.MetaDescription != null && data.MetaDescription != "")
                            data.MetaDescription = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaDescription, sl));
                        if (data.MetaKeyword != null && data.MetaKeyword != "")
                            data.MetaKeyword = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaKeyword, sl));
                        if (data.MetaTitle != null && data.MetaTitle != "")
                            data.MetaTitle = TranslateText(sl, tl, ReplaceTextByDictionary(data.MetaTitle, sl));
                        def.data = data;
                    }
                    else if (type == (int)Const.TypeLanguageMapping.LANGUAGE_LEGALDOC)
                    {
                        LegalDoc data = await db.LegalDoc.FindAsync(id);

                        if (data == null)
                        {
                            def.meta = new Meta(404, "Not Found");
                            return Ok(def);
                        }

                        if (data.Name != null && data.Name != "")
                        {
                            data.Name = TranslateText(sl, tl, ReplaceTextByDictionary(data.Name, sl));
                            //data.Url = Utils.NonUnicode(data.Title);
                        }
                        if (data.Contents != null && data.Contents != "")
                            data.Contents = TranslateText(sl, tl, ReplaceTextByDictionary(data.Contents, sl));
                        if (data.Note != null && data.Note != "")
                            data.Note = TranslateText(sl, tl, ReplaceTextByDictionary(data.Note, sl));
                        if (data.TichYeu != null && data.TichYeu != "")
                            data.TichYeu = TranslateText(sl, tl, ReplaceTextByDictionary(data.TichYeu, sl));
                        def.data = data;
                    }
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpPost("translateByText")]
        public IActionResult TranslateByText([FromBody] TranslateDT data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            //{
            //    def.meta = new Meta(222, "No permission");
            //    return Ok(def);
            //}
            try
            {
                using (var db = new IOITDataContext())
                {
                    string result = TranslateText(data.sl, data.tl, System.Net.WebUtility.HtmlDecode(data.text));

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = Utils.TextToHtml(result);
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        public string TranslateText(string sl, string tl, string input)
        {
            // Translation Data
            string translation = "";
            if (input != null && input != "")
            {
                try
                {
                    // Set the language from/to in the url (or pass it into this function)
                    string url = String.Format
                    ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                     sl, tl, Uri.EscapeUriString(input));
                    HttpClient httpClient = new HttpClient();
                    string result = httpClient.GetStringAsync(url).Result;

                    // Get all json data
                    var jsonData = JsonConvert.DeserializeObject<List<dynamic>>(result);
                    // Extract just the first array element (This is the only data we are interested in)
                    var translationItems = jsonData[0];

                    //// Loop through the collection extracting the translated objects
                    foreach (object item in translationItems)
                    {
                        // Convert the item array to IEnumerable
                        IEnumerable translationLineObject = item as IEnumerable;

                        // Convert the IEnumerable translationLineObject to a IEnumerator
                        IEnumerator translationLineString = translationLineObject.GetEnumerator();

                        // Get first object in IEnumerator
                        translationLineString.MoveNext();

                        // Save its value (translated text)
                        translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                    }

                    // Remove first blank character
                    if (translation.Length > 1) { translation = translation.Substring(1); };
                }
                catch { }
            }
            // Return translation
            return translation;
        }

        public string ReplaceTextByDictionary(string textInput, string langCode)
        {
            if (textInput != null && textInput != "")
            {
                //Lấy danh sách từ điển
                using (var db = new IOITDataContext())
                {
                    var listDic = db.Dictionary.Where(e => e.Status != (int)Const.Status.DELETED).ToList();
                    foreach (var item in listDic)
                    {
                        if (langCode == "en")
                        {
                            textInput = textInput.Replace(item.StringEn, item.StringVn);
                            textInput = textInput.Replace(item.StringEn.Trim(), item.StringVn.Trim());
                            textInput = textInput.Replace(item.StringEn.Trim().ToLower(), item.StringVn.Trim().ToLower());
                        }
                        else
                        {
                            textInput = textInput.Replace(item.StringVn, item.StringEn);
                            textInput = textInput.Replace(item.StringVn.Trim(), item.StringEn.Trim());
                            textInput = textInput.Replace(item.StringVn.Trim().ToLower(), item.StringEn.Trim().ToLower());
                        }
                    }
                }
            }
            return textInput;
        }

    }
}