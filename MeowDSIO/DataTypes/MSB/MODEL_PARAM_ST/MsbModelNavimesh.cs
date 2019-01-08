using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST
{
    public class MsbModelNavimesh : MsbModelBase
    {
        protected override ModelParamSubtype GetSubtypeValue()
        {
            return ModelParamSubtype.Navimesh;
        }
    }
}
