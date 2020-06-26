﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using DBContext;

namespace WebApiAntiContr.Controllers.UserControllers
{
    public class ShowRequestController : ApiController
    {
        public object Get(string id)
        {
            int idNum = Convert.ToInt32(id);

            DBDataContext db = new DBDataContext();
            var requests = (from req in db.Requests
                            where req.Id == idNum
                            select req)
                            .ToList();

            if (requests.Count == 0 || requests.Count > 1)
                return null;

            var request = requests[0];
            return new RecordComplainFullInfo
            {
                adress = request.Address,
                status = request.Status,
                textRequest = request.TextRequest,
                type = request.Type,
                unit = request.Unit
            };
        }
    }
}