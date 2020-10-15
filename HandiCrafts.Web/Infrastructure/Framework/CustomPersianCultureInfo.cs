using System.Globalization;

namespace HandiCrafts.Web.Infrastructure.Framework
{
    public class CustomPersianCultureInfo : CultureInfo
    {
        private readonly Calendar englishCalendar;

        public CustomPersianCultureInfo(string name) : base(name)
        {
            var en = new CultureInfo("en-US");
            englishCalendar = en.Calendar;
        }

        public override Calendar Calendar => englishCalendar;
    }
}
