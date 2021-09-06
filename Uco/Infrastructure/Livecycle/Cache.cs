using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Uco.Infrastructure.Repositories;
using Uco.Models;
using Uco.Infrastructure.EntityExtensions;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Caching;
namespace Uco.Infrastructure.Livecycle
{
    public static partial class LS
    {
        public static System.Web.Caching.Cache Cache
        {
            get
            {
                return HttpRuntime.Cache;
            }
        }
        public static Random rnd = new Random();
        public static int GetRandom()
        {
            
            int month = rnd.Next(4500241, 187008221);
            return month;
        }
        public static List<T> GetRefList<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return CurrentEntityContext.Set<T>().Where(predicate).ToList();
        }
        public static List<T> GetAllRefList<T>() where T : class
        {
            return CurrentEntityContext.Set<T>().ToList();
        }
        public static T GetFirst<T>(Expression<Func<T,bool>> predicate) where T :class
        { 
            
        return CurrentEntityContext.Set<T>().FirstOrDefault(predicate); 
        }
        public static IList<T> GetForTest<T>(int ShopID, int limit = 20) where T :class
        {
            if (limit < 1) { limit = 1; }
            if (limit > 100) { limit = 100; }
            return CurrentEntityContext.Set<T>().AsQueryable().Take(limit).ToList();
        }

        public static string GetArea()
        {
            var context = new HttpContextWrapper(System.Web.HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(context);
            return GetAreaName(routeData);
        }
        private static string GetAreaName(RouteData routeData)
        {
            object area;

            if (routeData.DataTokens.TryGetValue("area", out area))
            {
                return area as string;
            }

            return GetAreaName(routeData.Route);
        }
        private static string GetAreaName(RouteBase route)
        {
            IRouteWithArea routeWithArea = route as IRouteWithArea;

            if (routeWithArea != null)
            {
                return routeWithArea.Area;
            }

            Route castRoute = route as Route;

            if (castRoute != null && castRoute.DataTokens != null)
            {
                return castRoute.DataTokens["area"] as string;
            }

            return null;
        }
        #region ModelAttribute 
        //Role depended settings
        private static Dictionary<string,ModelGeneralAttribute> _GeneralRole = new Dictionary<string,ModelGeneralAttribute>() 
        {
        {"Admin" , new ModelGeneralAttribute() },
        {"Member", new ModelGeneralAttribute(){ AjaxEdit=false,Create=false,CreateAjax=false,Delete=false,Edit=false,Show=false,DependedShow=false } }
        };

        public static ModelGeneralAttribute GetModelGeneral(Type type,string role)
        {
            var dnAttribute = type.GetCustomAttributes<ModelGeneralAttribute>(true).Where(x => x.Role == role || x.Role == null).OrderByDescending(x => x.Role).ToList().FirstOrDefault() as ModelGeneralAttribute;
            if (dnAttribute == null)
            {
                if (!string.IsNullOrEmpty(role) && _GeneralRole.ContainsKey(role))
                {
                    dnAttribute = _GeneralRole[role];
                }
                else
                {
                    dnAttribute = new ModelGeneralAttribute();
                }
            }
            return dnAttribute;
        }
        #endregion
        #region caching entity model
        public static List<T> GetCustom<T>(string key, Func<object> func) where T : class
        {
            return Get<T>(key,false, func);
        }
        #endregion
        #region SeoUrl
        /// <summary>
        /// Convert a name into a string that can be appended to a Uri.
        /// </summary>
        private static string EscapeName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = NormalizeString(name);

                // Replaces all non-alphanumeric character by a space
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < name.Length; i++)
                {
                    builder.Append(char.IsLetterOrDigit(name[i]) ? name[i] : ' ');
                }

                name = builder.ToString();

                // Replace multiple spaces into a single dash
                name = Regex.Replace(name, @"[ ]{1,}", @"-", RegexOptions.None);
            }

            return name;
        }

        /// <summary>
        /// Strips the value from any non english character by replacing thoses with their english equivalent.
        /// </summary>
        /// <param name="value">The string to normalize.</param>
        /// <returns>A string where all characters are part of the basic english ANSI encoding.</returns>
        /// <seealso cref="http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net"/>
        private static string NormalizeString(string value)
        {
            string normalizedFormD = value.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < normalizedFormD.Length; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(normalizedFormD[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(normalizedFormD[i]);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
        #endregion
        public static void SetSingle<Tent>(Tent o, Type t) where Tent : class
        {
            if (Cache[t.Name] != null)
            {
                var p = t.GetProperty("ID");
                if (p != null)
                {
                    foreach (var c in (Dictionary<string, List<Tent>>)Cache[t.Name])
                    {
                        var ID = p.GetValue(o);
                        ParameterExpression pe = Expression.Parameter(t, "p");
                        Expression left = Expression.Property(pe, "ID");
                        Expression right = Expression.Constant(ID);
                        Expression e1 = Expression.Equal(left, right);
                        var predicate = Expression.Lambda<Func<Tent, bool>>
                 (Expression.Equal(left, right),
                 new[] { pe }).Compile();
                        var fd = c.Value.FirstOrDefault(predicate);
                        if (fd != null)
                        {
                            var index = c.Value.IndexOf(fd);
                            if (index > -1)
                            {
                                c.Value[index] = o;
                                //  c.Value.Insert(index, o);

                            }
                        }
                    }
                }
                MethodInfo methodList = t.GetMethod("CacheItem", BindingFlags.Public | BindingFlags.Static);
                if (methodList != null)
                {
                    methodList.Invoke(null, new object[] { o,true,false,false });
                }
            }
           var property = t.GetProperties().FirstOrDefault(x => x.GetCustomAttribute<SeoUrlAttribute>() != null);
            if(property!= null)
            {
                var seoattribute = property.GetCustomAttribute<SeoUrlAttribute>();
                if(seoattribute!=null)
                {

                    string seoUrl = (string)property.GetValue(o);
                    if (string.IsNullOrEmpty(seoUrl))
                    {
                        //get new seo name
                        var nameprop = t.GetProperty(seoattribute.NameField);
                        if (nameprop == null)
                        {
                            nameprop = t.GetProperty(seoattribute.TitleField);
                        }
                        if (nameprop != null)
                        {

                            seoUrl = (string)nameprop.GetValue(o);
                            if(seoUrl != null)
                            {
                                seoUrl = EscapeName(NormalizeString(seoUrl));
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(seoUrl))
                    {
                          var p = t.GetProperty("ID");
                          if (p != null)
                          {
                              var IDint = p.GetValue(o);
                              if (IDint is int)
                              {
                                  //do insert update delete seo url record
                                  int ID = (int)IDint;
                                  seoUrl = seoUrl.ToLower();
                                  var exists = LS.Get<UrlRecord>().Where(x => x.Slug.StartsWith(seoUrl)).ToList();
                                   int nextNum=1;
                                  string nextPlus="-"+nextNum.ToString();
                                  string baseUrl = seoUrl;
                                  while(exists.Any(x=>x.Slug == seoUrl && (
                                      x.EntityID != ID || x.EntityName!= t.Name )  ))
                                  {
                                      seoUrl = baseUrl + nextPlus;
                                      nextNum++;
                                      nextPlus = "-" + nextNum.ToString();

                                  }
                                  //now seoUrl is clean and unique

                                  //update current
                                  //get from DB, must be real data, assigned to context
                                var thisUrlRecord = LS.CurrentEntityContext.UrlRecords
                                              .FirstOrDefault(x => x.EntityID == ID && x.EntityName == t.Name);
                                          if (thisUrlRecord != null)
                                          {
                                              if (thisUrlRecord.Slug != seoUrl) //if only text changed
                                      {
                                          
                                          
                                              thisUrlRecord.Slug = seoUrl;
                                              thisUrlRecord.IsActive = true;
                                          
                                              //don`t use save change there, make cycle and overflow
                                              //LS.CurrentEntityContext.SaveChanges();
                                              var list = new List<UrlRecord>() { thisUrlRecord };
                                              list.SqlUpdateById();
                                              list.Update();
                                                  
                                      }
                                  }
                                  else
                                  {
                                      //insert new to DB
                                      var newUrlRecord = new UrlRecord() { 
                                      EntityID=ID,
                                      EntityName = t.Name,
                                      IsActive=true,
                                      Slug = seoUrl
                                      };
                                      var list = new List<UrlRecord>() { newUrlRecord };
                                      list.SqlInsert();
                                      list.Insert();
                                     
                                      //don`t use save change there, make cycle and overflow
                                    //  LS.CurrentEntityContext.SaveChanges();

                                  }
                                  property.SetValue(o, seoUrl);
                              }
                          }

                    }
                }
                
            }
        }
        public static void RemoveSingle<Tent>(Tent o, Type t) where Tent : class
        {
            if (Cache[t.Name] != null)
            {
                var p = t.GetProperty("ID");
                if (p != null)
                {
                    foreach (var c in (Dictionary<string, List<Tent>>)Cache[t.Name])
                    {
                        var ID = p.GetValue(o);
                        ParameterExpression pe = Expression.Parameter(t, "p");
                        Expression left = Expression.Property(pe, "ID");
                        Expression right = Expression.Constant(ID);
                        Expression e1 = Expression.Equal(left, right);
                        var predicate = Expression.Lambda<Func<Tent, bool>>
                 (Expression.Equal(left, right),
                 new[] { pe }).Compile();
                        var fd = c.Value.FirstOrDefault(predicate);
                        if (fd != null)
                        {
                            var index = c.Value.IndexOf(fd);
                            if (index > -1)
                            {
                                c.Value.RemoveAt(index);// = o;
                                //  c.Value.Insert(index, o);

                            }
                        }
                    }
                }
                MethodInfo methodList = t.GetMethod("CacheItem", BindingFlags.Public | BindingFlags.Static);
                if (methodList != null)
                {
                    methodList.Invoke(null, new object[] { o, false, false, true });
                }
            }
        }
        public static void AddSingle<Tent>(Tent o, Type t) where Tent : class
        {
            if (Cache[t.Name] != null)
            {
                var p = t.GetProperty("ID");
                if (p != null)
                {
                    foreach (var c in (Dictionary<string, List<Tent>>)Cache[t.Name])
                    {
                       
                                c.Value.Add(o);
                                //  c.Value.Insert(index, o);

                    }
                }
                MethodInfo methodList = t.GetMethod("CacheItem", BindingFlags.Public | BindingFlags.Static);
                if (methodList != null)
                {
                    methodList.Invoke(null, new object[] { o, false, true, false });
                }
            }
        }
        public static void Update<T>(this IEnumerable<T> list)where T : class
        {
            foreach (var e in list)
            {
                var t = e.GetType();
               // var o = Activator.CreateInstance(t);
                MethodInfo method = typeof(LS).GetMethod("SetSingle", BindingFlags.Public | BindingFlags.Static);
                MethodInfo genericMethod = method.MakeGenericMethod(t);
                genericMethod.Invoke(null, new object[]{ e,t } );
               // SetSingle(e, t);
            }
        }
        public static void Insert<T>(this IEnumerable<T> list) where T : class
        {
            foreach (var e in list)
            {
                var t = e.GetType();
                // var o = Activator.CreateInstance(t);
                MethodInfo method = typeof(LS).GetMethod("AddSingle", BindingFlags.Public | BindingFlags.Static);
                MethodInfo genericMethod = method.MakeGenericMethod(t);
                genericMethod.Invoke(null, new object[] { e, t });
                // SetSingle(e, t);
            }
        }
        public static void Delete<T>(this IEnumerable<T> list) where T : class
        {
            foreach (var e in list)
            {
                var t = e.GetType();
                // var o = Activator.CreateInstance(t);
                MethodInfo method = typeof(LS).GetMethod("RemoveSingle", BindingFlags.Public | BindingFlags.Static);
                MethodInfo genericMethod = method.MakeGenericMethod(t);
                genericMethod.Invoke(null, new object[] { e, t });
                // SetSingle(e, t);
            }
        }
        public static List<SelectListItem> GetSelectList(Type type)
        {
            MethodInfo methodList = typeof(LS).GetMethod("GetSelectListGen", BindingFlags.Public | BindingFlags.Static);
            MethodInfo genericMethodList = methodList.MakeGenericMethod(type);

            var list = (List<SelectListItem>)genericMethodList.Invoke(null, new object[] { });
            return list;
        }
        public static List<SelectListItem> GetSelectListGen<T>() where T : class
        {
           var list = Get<T>();
           return list.Select(x => new SelectListItem() { 
           Text = typeof(T).GetProperty("Name").GetValue(x).ToString(),
           Value = typeof(T).GetProperty("ID").GetValue(x).ToString(),
           }).ToList();
        }
        public static void SetMeToCache<T>(this T o, string key)
        {
            lock (_lock)
            {
                  LS.Cache[key] = o;
            }
        }
        public static bool IsExistInCache(string key)
        {
            return LS.Cache[key] != null;
        } 
        public static void SetToCache<T>(T o, string key,int Minutes=0)
        {
            lock (_lock)
            {
                if (Minutes == 0)
                {
                    LS.Cache[key] = o;
                }else
                {
                    LS.Cache.Add(key, o, null, DateTime.Now.AddMinutes(Minutes), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
            }
        }
        public static void RemoveFromCache(string key)
        {
            LS.Cache.Remove(key);
        }
        public static T GetCachedFunc<T>(Func<T> o, string key, int Minutes = 0,bool replaceNew=false)
        {
            lock (_lock)
            {
                if(replaceNew)
                {
                    LS.Cache.Remove(key);
                }
                if (LS.Cache[key] == null)
                {
                    if (Minutes == 0)
                    {
                        LS.Cache[key] = o();
                    }
                    else
                    {
                        LS.Cache.Add(key, o(), null, DateTime.Now.AddMinutes(Minutes), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                }
            }
            return (T)LS.Cache[key];
        } 
        public static T GetFromCache<T>(string key)
        {
            if (LS.Cache[key] == null)
            {
                return default(T);
            }
            return (T)LS.Cache[key];
        }
        public static object _lock = new object();

      
        public static void Clear<T>() where T : class
        {
            var t = typeof(T);
            string key = t.Name;
            string master = key;
            if (LS.Cache[master] != null)
            {
                LS.Cache.Remove(master);
            }
            
        }
        public static List<T> CleanGet<T>() where T : class
        {
            Clear<T>();
            return Get<T>();
        }
        public static List<T> Get<T>(string AdditionalKey="",bool cacheByDomainAndLang=true,Func<object> func=null) where T : class
        {
            var t = typeof(T);
            int DomainID = 0;
            bool islang = false;
            string LanguageCode = "";
            string key = t.Name ;
            string master = key;
            if (cacheByDomainAndLang && t.GetProperty("LangCode") != null)
            {
                 LanguageCode = SF.GetLangCodeThreading();
                key += "_" + LanguageCode;
                islang = true;
            }
            if (cacheByDomainAndLang && t.GetProperty("DomainID") != null)
            {
                DomainID = RP.GetCurrentSettings().ID;
                key += "_" + DomainID.ToString();
            }
            var mainListKey = key;
            key += AdditionalKey;
            if (LS.Cache[master] == null)
            {
                LS.Cache[master] = new Dictionary<string, List<T>>();
            }
            lock (_lock)
            {
                if (!((Dictionary<string, List<T>>)LS.Cache[master]).ContainsKey(key))
                {
                    if (func != null)
                    {
                        var l = (List<T>)func();

                        ((Dictionary<string, List<T>>)LS.Cache[master]).Add(key, l);
                    }
                    else if (!string.IsNullOrEmpty(AdditionalKey) && ((Dictionary<string, List<T>>)LS.Cache[master]).ContainsKey(mainListKey))
                    {
                        var l = new List<T>();

                        ((Dictionary<string, List<T>>)LS.Cache[master]).Add(key, l);
                    }
                    else
                    {

                        var query = LS.CurrentEntityContext.Set<T>().AsQueryable();
                        if (DomainID > 0)
                        {
                            ParameterExpression pe = Expression.Parameter(t, "p");
                            Expression left = Expression.Property(pe, "DomainID");
                            Expression right = Expression.Constant(DomainID);
                            Expression e1 = Expression.Equal(left, right);
                            var predicate = Expression.Lambda<Func<T, bool>>
                     (Expression.Equal(left, right),
                     new[] { pe }).Compile();
                            query = query.Where(predicate).AsQueryable();
                        }
                        if (islang)
                        {
                            ParameterExpression pe = Expression.Parameter(t, "p"); // p=>p.field == value
                            Expression left = Expression.Property(pe, "LangCode");
                            Expression right = Expression.Constant(LanguageCode);
                            Expression e1 = Expression.Equal(left, right);
                            var predicate = Expression.Lambda<Func<T, bool>>
                     (Expression.Equal(left, right),
                     new[] { pe }).Compile();
                            query = query.Where(predicate).AsQueryable();
                        }
                        // Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T,bool>>();
                        var l = query.ToList();

                        ((Dictionary<string, List<T>>)LS.Cache[master]).Add(mainListKey, l);
                        if(mainListKey != key)
                        {
                            ((Dictionary<string, List<T>>)LS.Cache[master]).Add(key, new List<T>());
                        }
                        MethodInfo methodList = t.GetMethod("CacheList", BindingFlags.Public | BindingFlags.Static);
                        if (methodList != null)
                        {
                            methodList.Invoke(null, new object[] { l });
                        }
                    }
                    // LS.Cache[key] = l;
                }
            }
           return ((Dictionary<string, List<T>>)LS.Cache[master])[key];
          //  return new List<T>();
        }

    }
}