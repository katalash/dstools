using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public abstract class WrapperData
    {
        protected abstract void ReloadContents();
        protected abstract void ResaveContents();
        protected abstract void Init();

        protected WrapperData()
        {
            Init();
        }

        public void Reload()
        {
            ReloadContents();
            Init();
        }

        public void Resave()
        {
            ResaveContents();
        }
    }
}
