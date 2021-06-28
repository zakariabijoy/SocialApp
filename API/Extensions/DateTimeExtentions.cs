using System;

namespace API.Extensions
{
    public static class DateTimeExtentions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var today = DateTime.Today;    //28/6/2021
            var age = today.Year - dob.Year;   // 2021 - 1994 = 27
            if(dob.Date > today.AddYears(-age)) age --;  // 11/12/1994 >  28/6/1994 == 26 

            return age;
        }
    }
}