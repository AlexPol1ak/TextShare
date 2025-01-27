using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TextShare.DAL.Converters
{
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() :
            base(v => v.ToDateTime(TimeOnly.MinValue), v => DateOnly.FromDateTime(v))
        { }
    }
}
