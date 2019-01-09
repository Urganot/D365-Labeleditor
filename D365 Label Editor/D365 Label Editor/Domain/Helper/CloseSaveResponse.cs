using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVA_LabelEditor.Domain.Helper
{
    public class CloseSaveResponse
    {
        public bool Save { get; set; } = false;
        public bool Close { get; set; } = true;
    }
}
