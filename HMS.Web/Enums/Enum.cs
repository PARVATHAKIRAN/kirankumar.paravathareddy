namespace HMS.Web.Enums
{
    public class Enum
    {
        public enum Status : int
        {
            InActive = 0,
            Active = 1
        }
        public enum Roles
        {
            Admin,
            Doctor,
            Patient
        }

		public enum AppointmentStatus : int
		{
			Inactive = 0,
			Active = 1,
			Pending = 2
		}
	}
}
