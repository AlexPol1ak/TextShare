namespace TextShare.Domain.Models
{
    /// <summary>
    /// Класс для размещения данных.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseData<T>
    {
        public T? Data { get; set; } = default;
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; } = null;

        public ResponseData() { }

        public ResponseData(T data, bool success = true, string? errorMessage = null) : this()
        {
            Data = data;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            string info = $"Response data type: {(Data == null ? "Null" : Data.GetType().ToString())}\n";
            info += $"Success: {Success}\n";
            if (ErrorMessage != null) info += $"Error message: {ErrorMessage}";
            return info;
        }
    }
}
