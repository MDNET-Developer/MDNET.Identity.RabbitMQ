﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CreateFileMessage
    {
        public string? UserId { get; set; }
        public int FileId { get; set; }
    }
}
