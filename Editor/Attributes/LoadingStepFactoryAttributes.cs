using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingModule.Editor.Entity
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    internal class LoadingStepFactoryIsHiddenAttribute : System.Attribute { }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    internal class LoadingStepFactoryNotUsableAttribute : System.Attribute { }
}
