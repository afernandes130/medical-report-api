using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serverless.ApiProxy.Models
{
    public class Model3d
    {
        public string FileName { get; set; }
        public ModelConfig ModelConfig { get; set; }
        public ModelData ModelData { get; set; }
    }
}
