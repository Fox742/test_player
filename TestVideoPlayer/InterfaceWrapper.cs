using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    /// <summary>
    /// Обёртка интерфейса, через которую выводит информацию движок
    /// </summary>
    internal static class InterfaceWrapper
    {
        static private BaseInterfaceWrapper _uiWrapper = null;
        static public void initInterface(BaseInterfaceWrapper uiWrapper)
        {
            _uiWrapper = uiWrapper;
        }

        /// <summary>
        /// Вывести сообщение на экран
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        static public void showMessage(string message, string caption = "")
        {
            if (_uiWrapper!=null)
            {
                _uiWrapper.PrintMessage(message,caption);
            }
        }
    }
}
