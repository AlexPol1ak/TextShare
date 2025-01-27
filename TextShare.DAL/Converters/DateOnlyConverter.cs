using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TextShare.DAL.Converters
{
    /// <summary>
    /// Конвертер DateOnly - DateTime для столбцов базы данных.
    /// </summary>
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() :
            base(v => v.ToDateTime(TimeOnly.MinValue), v => DateOnly.FromDateTime(v))
        { }
    }
}
