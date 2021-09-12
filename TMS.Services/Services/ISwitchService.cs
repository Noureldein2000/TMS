using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface ISwitchService
    {
        string Connect<T>(T obj, SwitchEndPointDTO PSC, string BaseAddress, string TokenType);
    }
}
