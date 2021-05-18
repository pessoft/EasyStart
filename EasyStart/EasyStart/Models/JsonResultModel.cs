using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class JsonResultModel
    {
        public static JsonResultModel CreateSuccess(object data) 
        {
            var self = new JsonResultModel();
            self.Data = data;
            self.Success = true;

            return self;
        }

        //TO DO
        //change on simple CreateSuccess(object data)
        public static JsonResultModel CreateSuccessWithDataURL(string url)
        {
            var self = new JsonResultModel();
            self.URL = url;
            self.Success = true;

            return self;
        }

        public static JsonResultModel CreateError(string errorMessage)
        {
            var self = new JsonResultModel();
            self.ErrorMessage = errorMessage;

            return self;
        }

        //TO DO
        // change to private
        public JsonResultModel() { }
        public object Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string URL { get; set; }

    }
}