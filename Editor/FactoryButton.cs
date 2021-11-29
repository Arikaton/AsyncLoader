using System.Linq;
using UnityEngine.UIElements;

namespace LoadingModule.Editor
{
    public class FactoryButton : Button
    {
        private string _factoryName;
        
        public FactoryButton()
        {
        }

        public void BindData(string factoryName)
        {
            _factoryName = factoryName;
            text = _factoryName;
        }
    }
}