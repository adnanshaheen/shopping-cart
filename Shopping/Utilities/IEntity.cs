﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Utilities
{
    public interface IEntity
    {
        void SetFeilds(DataRow dataRow);
    }
}
