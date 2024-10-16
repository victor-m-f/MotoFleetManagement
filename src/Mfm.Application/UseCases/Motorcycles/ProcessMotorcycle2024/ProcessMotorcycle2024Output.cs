﻿using Mfm.Application.UseCases.Base;
using System.Net;

namespace Mfm.Application.UseCases.Motorcycles.ProcessMotorcycle2024;

public sealed class ProcessMotorcycle2024Output : OutputBase
{
    public ProcessMotorcycle2024Output()
        : base(HttpStatusCode.Created)
    {
    }

    public ProcessMotorcycle2024Output(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }
}
