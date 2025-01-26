namespace TextShare.Domain.Utils
{
    /// <summary>
    /// Класс для печати отладки в консоли.
    /// </summary>
    static public class DebugHelper
    {
        /// <summary>
        /// Количество символов разделителя
        /// </summary>
        static public int CountSplitter { get; set; } = 100;
        /// <summary>
        /// Символ разделитель.
        /// </summary>
        static public string ValueSplitter { get; set; } = "*";
        /// <summary>
        /// Флаг включения разделителя.
        /// </summary>
        static public bool EnabledSplitter { get; set; } = true;
        /// <summary>
        /// Цвет разделителя.
        /// </summary>
        static public ConsoleColor ColorSplitter { get; set; } = ConsoleColor.Green;
        /// <summary>
        /// Верхняя строка разделителя.
        /// </summary>
        public static string SplitterTop
        {
            get
            {
                return getSplitterString();
            }
        }
        /// <summary>
        /// Нижняя строка разделителя.
        /// </summary>
        public static string SplitterBottom
        {
            get
            {
                return getSplitterString(false);
            }
        }

        private static DateTime currentDateTime => DateTime.Now;
        private static string currentDateTimeString => $"[{currentDateTime.ToString("G")}]";
        /// <summary>
        /// Формирует строку разделителя.
        /// </summary>
        /// <param name="headerEnabled"></param>
        /// <returns></returns>
        private static string getSplitterString(bool headerEnabled = true)
        {
            string spl = string.Empty;
            string header = " DebugHepler ";

            if (headerEnabled)
            {
                for (int i = 0; i < CountSplitter; i++)
                {
                    spl += ValueSplitter;
                    if (i == CountSplitter / 2)
                        spl += header;
                }
            }
            else
            {
                for (int i = 0; i < CountSplitter + header.Length; i++)
                    spl += ValueSplitter;
            }
            return spl;
        }

        /// <summary>
        /// Печатает строку разделителя .
        /// </summary>
        private static void WriteLineSplitter(string splitter)
        {
            if (EnabledSplitter)
            {
                var originalForegroundConsoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ColorSplitter;

                Console.WriteLine(splitter);

                Console.ForegroundColor = originalForegroundConsoleColor;
            }

        }

        private static string getDataFromObj(object obj)
        {
            string? data = string.Empty;
            if (obj == null || obj is null)
            {
                data = "Передан null";
                return data;
            }

            data += "[Type] : ";
            try
            {
                data += obj.GetType().ToString(); ;
            }
            catch
            {
                data = "Не удалось определить";
            }

            data += "\n\t [ToString Value] : ";

            try
            {
                string? value = obj.ToString();
                if (value == null) data += "Null";
                else data += value;
            }
            catch
            {
                data += "Не удалось получить строковое представление. " +
                    "Возможно передан Null или отсутствует метод ToString.\n";
            }


            return data;
        }

        /// <summary>
        /// Печатает простые данные.
        /// </summary>
        /// <param name="objects"></param>
        public static void ShowData(params object[] objects)
        {


            WriteLineSplitter(SplitterTop);

            if (objects != null && objects.Length > 0)
            {
                int counter = 1;
                foreach (object obj in objects)
                {
                    string mess = getDataFromObj(obj);
                    Console.WriteLine($"Param {counter}: {mess}\n");
                    counter++;
                }
            }
            else
            {
                Console.WriteLine("Переданные параметры являются null");
            }

            WriteLineSplitter(SplitterBottom);
        }

        /// <summary>
        /// Печатает ошибку.
        /// </summary>
        /// <param name="obj">Данные</param>
        /// <param name="message">Сообщение</param>
        public static void ShowError(object obj, string? message = null)
        {
            WriteLineSplitter(SplitterTop);

            var ogirFore = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error!");
            Console.WriteLine(currentDateTimeString);
            Console.ForegroundColor = ogirFore;

            if (message != null) Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine(getDataFromObj(obj));

            WriteLineSplitter(SplitterBottom);
        }
    }
}
