﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ResponseModels
{
    public class MPGUpdateResponse : IResponseModel
    {
        public double InstantMPG { get; set; }
        public double CurrentThrottlePercentage { get; set; }

        public bool Success
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }
    }
}