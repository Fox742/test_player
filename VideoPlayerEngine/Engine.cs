using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    /// <summary>
    /// Главный класс движка. К нему должен обращаться интерфейс из своих обработчиков
    /// </summary>
    public class Engine
    {

        VideoController videoController = null;

        public Engine( BaseInterfaceWrapper interfaceWrapper)
        {
            InterfaceWrapper.initInterface(interfaceWrapper);
            videoController = new VideoController();
        }

        /// <summary>
        /// Загрузить новое расписание
        /// </summary>
        /// <param name="sheduleFilePath"></param>
        public void LoadShedule(string sheduleFilePath)
        {
            
            try
            {
                Shedule shd = new Shedule(sheduleFilePath);
                InterfaceWrapper.refreshShedulePath(sheduleFilePath);
                videoController.LoadShedule(shd,sheduleFilePath);
            }
            catch (ShedOrderException soe)
            {
                InterfaceWrapper.showMessage(soe.Message, "Ошибка");
            }
            catch (ShedParseException spe)
            {
                InterfaceWrapper.showMessage(spe.Message,"Ошибка");
            }
            catch (ShedDataException sde)
            {
                InterfaceWrapper.showMessage(sde.Message, "Ошибка");
            }
            catch (System.Exception exc)
            {
                InterfaceWrapper.showMessage("Не удалось прочитать расписание из файла. Вот точная причина: "+exc.Message, "Ошибка");
            }
        }

    }
}
