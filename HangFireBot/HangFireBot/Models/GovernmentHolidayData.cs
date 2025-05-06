public class GovernmentHolidayData
{
    public string Country { get; set; }
    public int Year { get; set; }
    public int TotalHolidays { get; set; }
    public List<Holiday> Holidays { get; set; }
}

public class Holiday
{
    public int SlNo { get; set; }
    public string Name { get; set; }
    public string Date { get; set; }
    public string Day { get; set; }
    public int NumOfHolidays { get; set; }
    public List<string> Days { get; set; }  // For holidays that span multiple days (like Eid)
}
