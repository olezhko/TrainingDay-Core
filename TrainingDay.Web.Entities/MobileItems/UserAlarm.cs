using System.ComponentModel.DataAnnotations;
using TrainingDay.Common;

namespace TrainingDay.Web.Entities.MobileItems
{
    public class UserAlarm : Alarm
    {
        public UserAlarm()
        {
            
        }
        public UserAlarm(Alarm alarm)
        {
            Days = alarm.Days;
            Name = alarm.Name;
            DatabaseId = alarm.Id;
            IsActive = alarm.IsActive;
            TimeOffset = alarm.TimeOffset;
            TrainingId = alarm.TrainingId;
        }

        [Key]
        public new int Id { get; set; }

        public Guid UserId { get; set; }
        public int DatabaseId { get; set; }

        public virtual User User { get; set; }
    }
}
